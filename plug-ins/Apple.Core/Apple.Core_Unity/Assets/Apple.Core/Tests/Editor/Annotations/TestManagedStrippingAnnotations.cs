using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Apple.Core.Runtime;
using NUnit.Framework;
using UnityEngine.Scripting;

namespace Apple.Core.Tests.Editor
{
    /// <summary>
    /// Wrapper objects (NSData, GKMatch, Product, ...) are created through reflection by <see cref="InteropReference.PointerCast{T}(IntPtr)"/>.
    /// Nothing else calls their (IntPtr) constructors, so managed code stripping removes them unless every type that can reach
    /// <c>PointerCast</c> is passed through a parameter annotated with <c>[DynamicallyAccessedMembers(WrapperMembers.Constructors)]</c>.
    ///
    /// These tests fail when that chain is broken. They read the attributes from metadata, because the linker removes them from built
    /// assemblies, so they can only run in the Editor.
    /// </summary>
    public class TestManagedStrippingAnnotations
    {
        private const string AnnotationTypeName = "System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute";

        // DynamicallyAccessedMemberTypes.PublicConstructors (0x3) | NonPublicConstructors (0x4)
        private const int ConstructorMemberTypes = 0x7;

        private const BindingFlags AllDeclared =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        // Generic parameters constrained to InteropReference (or on a type derived from it) that never reach PointerCast, so they
        // don't need the annotation. Add to this list only after checking that the parameter isn't passed to reflection-based
        // creation, directly or through another generic.
        private static readonly HashSet<string> NotReflectionCreated = new HashSet<string>
        {
            // Only compares type names.
            "Apple.Core.Runtime.NSObject.Is",
            // Builds its payload with direct constructor calls.
            "Apple.StoreKit.VerificationResult`1",
        };

        [Test]
        public void ReflectionEntryPointsAreAnnotated()
        {
            var core = typeof(InteropReference).Assembly;

            var missing = new List<string>();

            // Every generic parameter and System.Type parameter of these methods must be annotated.
            var methodsByType = new (Type Type, string[] Names)[]
            {
                (typeof(InteropReference), new[] { "PointerCast" }),
                (core.GetType("Apple.Core.Runtime.ReflectionUtility", throwOnError: true), new[] { "GetConstructor", "CreateInstanceOrDefault" }),
                (typeof(NSObject), new[] { "As" }),
            };

            foreach (var (type, names) in methodsByType)
            {
                foreach (var name in names)
                {
                    var overloads = type.GetMethods(AllDeclared).Where(method => method.Name == name).ToList();
                    Assert.That(overloads, Is.Not.Empty, $"{type.FullName}.{name} was renamed or removed; update this test.");

                    foreach (var method in overloads)
                    {
                        if (method.IsGenericMethodDefinition)
                        {
                            foreach (var genericParameter in method.GetGenericArguments())
                            {
                                if (!HasConstructorAnnotation(genericParameter))
                                {
                                    missing.Add($"{type.FullName}.{name}<{genericParameter.Name}>");
                                }
                            }
                        }

                        foreach (var parameter in method.GetParameters().Where(p => p.ParameterType == typeof(Type)))
                        {
                            if (!HasConstructorAnnotation(parameter))
                            {
                                missing.Add($"{type.FullName}.{name}({parameter.Name})");
                            }
                        }
                    }
                }
            }

            // Every generic parameter of these generic types must be annotated.
            var genericTypes = new[]
            {
                typeof(NSArray<>), typeof(NSMutableArray<>), typeof(NSDictionary<,>), typeof(NSMutableDictionary<,>), typeof(SwiftArray<>),
                typeof(InteropBoxer.BoxerNSObjectSubclass<>),
            };

            foreach (var type in genericTypes)
            {
                foreach (var genericParameter in type.GetGenericArguments().Where(p => !HasConstructorAnnotation(p)))
                {
                    missing.Add($"{type.FullName}<{genericParameter.Name}>");
                }
            }

            Assert.That(missing, Is.Empty, MissingMessage(missing));
        }

        [Test]
        public void NewGenericWrapperParametersAreReviewed()
        {
            var missing = new List<string>();

            foreach (var type in AppleRuntimeTypes())
            {
                if (type.IsGenericTypeDefinition && typeof(InteropReference).IsAssignableFrom(type))
                {
                    foreach (var genericParameter in type.GetGenericArguments())
                    {
                        Check(genericParameter, type.FullName, missing);
                    }
                }

                foreach (var method in type.GetMethods(AllDeclared).Where(method => method.IsGenericMethodDefinition))
                {
                    foreach (var genericParameter in method.GetGenericArguments().Where(IsConstrainedToInteropReference))
                    {
                        Check(genericParameter, $"{type.FullName}.{method.Name}", missing);
                    }
                }
            }

            Assert.That(missing, Is.Empty, MissingMessage(missing) +
                $" If the parameter never reaches PointerCast, add it to {nameof(NotReflectionCreated)} instead.");

            void Check(Type genericParameter, string owner, List<string> failures)
            {
                if (!NotReflectionCreated.Contains(owner) && !HasConstructorAnnotation(genericParameter))
                {
                    failures.Add($"{owner}<{genericParameter.Name}>");
                }
            }
        }

        [Test]
        public void InteropTypeNamePropertiesArePreserved()
        {
            // NSObject.GetInteropTypeName reads InteropTypeName through reflection, so stripping removes it unless it is preserved.
            var missing = AppleRuntimeTypes()
                .Select(type => type.GetProperty("InteropTypeName", AllDeclared & ~BindingFlags.Instance))
                .Where(property => property != null && !property.IsDefined(typeof(PreserveAttribute), inherit: false))
                .Select(property => $"{property.DeclaringType.FullName}.{property.Name}")
                .ToList();

            Assert.That(missing, Is.Empty,
                $"Add [Preserve] to: {string.Join(", ", missing)}. NSObject.GetInteropTypeName only finds it through reflection.");
        }

        private static string MissingMessage(IEnumerable<string> missing) =>
            $"Add [DynamicallyAccessedMembers(WrapperMembers.Constructors)] to: {string.Join(", ", missing)}.";

        private static IEnumerable<Type> AppleRuntimeTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly =>
                {
                    var name = assembly.GetName().Name;
                    return name.StartsWith("Apple.", StringComparison.Ordinal) &&
                           !name.EndsWith(".Editor", StringComparison.Ordinal) &&
                           !name.Contains(".Tests");
                })
                .SelectMany(assembly => assembly.GetTypes());
        }

        private static bool IsConstrainedToInteropReference(Type genericParameter)
        {
            return genericParameter.GetGenericParameterConstraints().Any(constraint => typeof(InteropReference).IsAssignableFrom(constraint));
        }

        private static bool HasConstructorAnnotation(MemberInfo member) => HasConstructorAnnotation(member.GetCustomAttributesData());

        private static bool HasConstructorAnnotation(ParameterInfo parameter) => HasConstructorAnnotation(parameter.GetCustomAttributesData());

        private static bool HasConstructorAnnotation(IEnumerable<CustomAttributeData> attributes)
        {
            return attributes.Any(attribute =>
                attribute.AttributeType.FullName == AnnotationTypeName &&
                ((int)attribute.ConstructorArguments[0].Value & ConstructorMemberTypes) == ConstructorMemberTypes);
        }
    }
}
