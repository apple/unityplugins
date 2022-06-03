using System;

namespace Apple.CoreHaptics
{
    public class CHException : Exception
    {
        public int Code { get; private set; }
        public string LocalizedDescription { get; private set; }

        public CHException(CHError error)
        {
            Code = error.Code;
            LocalizedDescription = error.LocalizedDescription;
        }
    }
}