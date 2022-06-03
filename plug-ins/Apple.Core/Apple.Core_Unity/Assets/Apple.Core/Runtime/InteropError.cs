using System.Runtime.InteropServices;

namespace Apple.Core.Runtime
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InteropError
    {
        public int Code;
        public string LocalizedDescription;
        public string TaskId;
    }
}
