using System;

// UnityLinker honors the trimming annotation [DynamicallyAccessedMembers] but netstandard2.1 doesn't define it, and the linker matches
// it by name, so it is declared here. It is internal so that it can't clash with another package that declares the same type.
namespace System.Diagnostics.CodeAnalysis
{
    [Flags]
    internal enum DynamicallyAccessedMemberTypes
    {
        None = 0,
        PublicParameterlessConstructor = 0x0001,
        PublicConstructors = 0x0002 | PublicParameterlessConstructor,
        NonPublicConstructors = 0x0004,
    }

    /// <summary>
    /// Tells the linker which members of a type must survive stripping when that type is passed to, or used as, the annotated
    /// parameter, return value, or generic parameter.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Method |
        AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter |
        AttributeTargets.Field | AttributeTargets.Property,
        Inherited = false)]
    internal sealed class DynamicallyAccessedMembersAttribute : Attribute
    {
        public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes)
        {
            MemberTypes = memberTypes;
        }

        public DynamicallyAccessedMemberTypes MemberTypes { get; }
    }
}

namespace Apple.Core.Runtime
{
    /// <summary>
    /// Values for <see cref="System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute"/> used by this package.
    /// </summary>
    internal static class WrapperMembers
    {
        /// <summary>
        /// Wrapper objects are created by <see cref="InteropReference.PointerCast{T}(IntPtr)"/>, which finds the <c>(IntPtr)</c>
        /// constructor through reflection. Nothing else calls that constructor, so managed code stripping removes it unless the
        /// type is passed through a parameter annotated with this value.
        /// </summary>
        public const System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes Constructors =
            System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicConstructors |
            System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.NonPublicConstructors;
    }
}
