using System;

namespace Apple.Core
{
    /// <summary>
    /// Attribute for manually specifying a class's corresponding Objective-C type name. This is required for classes
    /// that do not share their C# type name with their Objective-C counterpart, such as nested classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InteropTypeNameAttribute : Attribute
    {
        public string TypeName { get; private set; }

        public InteropTypeNameAttribute(string customTypeName)
        {
            TypeName = customTypeName;
        }
    }
}
