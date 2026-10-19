using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using NUnit.Framework;

namespace Apple.BackgroundAssets.Tests {

	/// <summary>
	/// Layout guards for the structs and signatures that cross the managed/native boundary.
	///
	/// These bindings are written so that IL2CPP hands every struct to the native wrapper exactly as
	/// it is laid out in managed memory. That only holds while the structs stay blittable. The moment
	/// one field isn't, IL2CPP quietly switches the whole containing struct onto the marshalled path:
	/// it emits a second "_marshaled_pinvoke" version of the type plus functions that convert between
	/// the two a field at a time. Nothing warns about it, and for the LayoutKind.Explicit unions here
	/// that conversion is actively wrong — see TestExplicitLayoutStructsAreBlittable.
	///
	/// Blittability is invisible in source, so these tests read it back off the compiled assembly.
	/// </summary>
	public class TestInteropBlittability {

		/// <summary>The assembly whose interop surface is under test.</summary>
		static Assembly BackgroundAssets => typeof(Error).Assembly;

		#region Tests

		/// <summary>
		/// Every struct that appears in a native signature must be blittable.
		///
		/// "Native signature" means a [DllImport] declaration (a call into the wrapper) or the Invoke
		/// of a delegate the wrapper calls back (a reverse P/Invoke). Those are the only places the
		/// marshaller inspects a struct, and it inspects the whole field graph, so this walks the graph
		/// too rather than just the top-level type.
		/// </summary>
		[Test]
		public void TestStructsInNativeSignaturesAreBlittable() {
			Dictionary<Type, string> structs = StructsInNativeSignatures();

			// If discovery silently found nothing, the test below would pass while checking nothing.
			Assert.IsNotEmpty(structs, "Found no structs in any native signature. Discovery is broken, not the bindings.");

			List<string> failures = new List<string>();
			foreach (KeyValuePair<Type, string> entry in structs) {
				if (!IsBlittable(entry.Key, out string reason)) {
					failures.Add($"{NameOf(entry.Key)} (reached via {entry.Value}): {reason}");
				}
			}

			Assert.IsEmpty(failures, Report("These interop structs are not blittable, so IL2CPP will marshal them field by field", failures));
		}

		/// <summary>
		/// Every union must be blittable.
		///
		/// Marshalling converts a struct one field at a time, which is meaningless for a union: the
		/// marshaller has no notion of which member is live, so it writes each member in turn to the
		/// same bytes and the last one wins. baw_assetpackmanifest_res overlays the manifest with an
		/// error; when a non-blittable bool put it on the marshalled path, the error was copied over
		/// the manifest and destroyed its asset-pack array pointer, crashing
		/// AssetPackManifest.GetAssetPack() on iOS 26.
		///
		/// There is no way to marshal a union correctly, so the fix is to never be marshalled. This
		/// test is deliberately broader than the one above: it covers every explicit-layout struct in
		/// the assembly, including any not yet wired up to a native call.
		/// </summary>
		[Test]
		public void TestExplicitLayoutStructsAreBlittable() {
			List<Type> unions = new List<Type>();
			foreach (Type type in DeclaredTypes()) {
				if (type.IsValueType && !type.IsEnum && type.StructLayoutAttribute?.Value == LayoutKind.Explicit) {
					unions.Add(type);
				}
			}

			Assert.IsNotEmpty(unions, "Found no explicit-layout structs. Discovery is broken, not the bindings.");

			List<string> failures = new List<string>();
			foreach (Type union in unions) {
				if (!IsBlittable(union, out string reason)) {
					failures.Add($"{NameOf(union)}: {reason}");
				}
			}

			Assert.IsEmpty(failures, Report("These overlapping structs are not blittable, so marshalling them will corrupt whichever member is live", failures));
		}

		#endregion

		#region Blittability

		/// <summary>The types a P/Invoke can pass straight through, with no conversion step.</summary>
		static readonly HashSet<Type> BlittableTypes = new HashSet<Type> {
			typeof(sbyte), typeof(byte),
			typeof(short), typeof(ushort),
			typeof(int),   typeof(uint),
			typeof(long),  typeof(ulong),
			typeof(float), typeof(double),
			typeof(IntPtr), typeof(UIntPtr)
		};

		/// <summary>
		/// Reports whether the marshaller can leave <paramref name="type"/> alone, and if not, why not.
		/// </summary>
		static bool IsBlittable(Type type, out string reason) {
			reason = null;

			// A pointer is an address; the marshaller never looks through it. An enum is always backed
			// by an integer type.
			if (type.IsPointer || type.IsEnum) {
				return true;
			}

			if (BlittableTypes.Contains(type)) {
				return true;
			}

			if (type == typeof(bool)) {
				reason = "bool is never blittable, and no [MarshalAs] annotation changes that. "
				       + "Use byte for a field holding a C '_Bool'";
				return false;
			}

			if (!type.IsValueType || type == typeof(char) || type == typeof(decimal)) {
				reason = $"{NameOf(type)} is not a blittable type";
				return false;
			}

			// A struct is blittable only if its layout is pinned down and every field is blittable too.
			StructLayoutAttribute layout = type.StructLayoutAttribute;
			if (layout == null || layout.Value == LayoutKind.Auto) {
				reason = $"{NameOf(type)} has automatic layout; it needs [StructLayout(LayoutKind.Sequential)] or [StructLayout(LayoutKind.Explicit)]";
				return false;
			}

			foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
				// [MarshalAs] on a field is a request to convert, which is exactly what must not happen.
				if (field.GetCustomAttribute<MarshalAsAttribute>() != null) {
					reason = $"field '{NameOf(type)}.{field.Name}' carries [MarshalAs], which puts the whole struct on the marshalled path";
					return false;
				}

				if (!IsBlittable(field.FieldType, out string fieldReason)) {
					reason = $"field '{NameOf(type)}.{field.Name}': {fieldReason}";
					return false;
				}
			}

			return true;
		}

		#endregion

		#region Discovery

		/// <summary>
		/// Every method the marshaller sees: the [DllImport] declarations that call into the native
		/// wrapper, and the Invoke of every delegate the wrapper calls back out through.
		/// </summary>
		static IEnumerable<MethodInfo> NativeSignatures() {
			foreach (Type type in DeclaredTypes()) {
				if (typeof(Delegate).IsAssignableFrom(type)) {
					MethodInfo invoke = type.GetMethod("Invoke");
					if (invoke != null) {
						yield return invoke;
					}
					continue;
				}

				const BindingFlags All = BindingFlags.Static | BindingFlags.Instance
				                       | BindingFlags.Public | BindingFlags.NonPublic
				                       | BindingFlags.DeclaredOnly;

				foreach (MethodInfo method in type.GetMethods(All)) {
					// [DllImport] is a pseudo-custom attribute: it lands in the method's metadata flags
					// rather than the custom attribute table, so test the flag.
					if ((method.Attributes & MethodAttributes.PinvokeImpl) != 0) {
						yield return method;
					}
				}
			}
		}

		/// <summary>
		/// Every struct reachable from a native signature, mapped to the signature that reaches it.
		/// </summary>
		static Dictionary<Type, string> StructsInNativeSignatures() {
			Dictionary<Type, string> found = new Dictionary<Type, string>();

			foreach (MethodInfo method in NativeSignatures()) {
				string site = DescribeSite(method);

				CollectStruct(method.ReturnType, site, found);
				foreach (ParameterInfo parameter in method.GetParameters()) {
					CollectStruct(parameter.ParameterType, site, found);
				}
			}

			return found;
		}

		/// <summary>
		/// Records <paramref name="type"/> if it is a struct, looking through by-ref, array and pointer
		/// wrappers first. Strings and other reference types are skipped: a reference type is free to be
		/// marshalled as a parameter, and unlike a struct field it drags nothing else onto that path.
		/// </summary>
		static void CollectStruct(Type type, string site, Dictionary<Type, string> found) {
			while (type.IsByRef || type.IsArray || type.IsPointer) {
				type = type.GetElementType();
			}

			if (type == typeof(void) || !type.IsValueType || type.IsEnum || type.IsPrimitive) {
				return;
			}

			if (!found.ContainsKey(type)) {
				found.Add(type, site);
			}
		}

		/// <summary>The assembly's own types, minus anything the compiler generated for us.</summary>
		static IEnumerable<Type> DeclaredTypes() {
			foreach (Type type in BackgroundAssets.GetTypes()) {
				// Async methods and iterators compile down to structs holding managed references. They
				// never reach native code, and they would otherwise dominate the failure list.
				if (type.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false)) {
					continue;
				}

				yield return type;
			}
		}

		#endregion

		#region Reporting

		/// <summary>A name that includes declaring types, e.g. "AssetPackManifest.baw_assetpackmanifest_res".</summary>
		static string NameOf(Type type) {
			string name = type.Name;
			for (Type declaring = type.DeclaringType; declaring != null; declaring = declaring.DeclaringType) {
				name = declaring.Name + "." + name;
			}

			return name;
		}

		static string DescribeSite(MethodInfo method) {
			Type declaring = method.DeclaringType;

			return typeof(Delegate).IsAssignableFrom(declaring)
				? $"callback {NameOf(declaring)}"
				: $"{NameOf(declaring)}.{method.Name}";
		}

		static string Report(string headline, List<string> failures) {
			return $"{headline}:{Environment.NewLine}  " + string.Join($"{Environment.NewLine}  ", failures);
		}

		#endregion

	}

}
