using System.Runtime.InteropServices;

namespace Apple.GameController
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GCError
    {
        public int Code;
        public string LocalizedDescription;
    }
}