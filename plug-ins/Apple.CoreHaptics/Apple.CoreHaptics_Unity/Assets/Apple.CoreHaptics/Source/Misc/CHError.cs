using System.Runtime.InteropServices;

namespace Apple.CoreHaptics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CHError
    {
        public readonly int Code;
        public readonly string LocalizedDescription;
    }
}