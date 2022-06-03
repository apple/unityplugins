using System;
using Apple.Core.Runtime;

namespace Apple.GameKit
{
    public class GameKitException : NSError
    {
        public GameKitException(IntPtr pointer) : base(pointer)
        {
        }
    }
}