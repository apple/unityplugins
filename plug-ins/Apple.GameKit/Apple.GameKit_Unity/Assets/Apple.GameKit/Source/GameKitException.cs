using System;
using Apple.Core.Runtime;

namespace Apple.GameKit
{
    public class GameKitException : Exception
    {
        public NSError NSError { get; set; }

        public GameKitException(IntPtr nsErrorPtr)
        {
            NSError = new NSError(nsErrorPtr);
        }

        public GameKitException(NSError nsError)
        {
            NSError = nsError;
        }

        public override string Message => $"Code={Code} Domain={Domain} Description={LocalizedDescription}";

        public long Code => NSError?.Code ?? default;
        public string Domain => NSError?.Domain ?? default;

        public bool IsGKErrorDomain => Domain == GKErrorDomain.Name;
        public GKErrorCode GKErrorCode => IsGKErrorDomain ? (GKErrorCode)Code : default;

        public string LocalizedDescription
        {
            get
            {
                if (IsGKErrorDomain)
                {
                    return $"{NSError?.LocalizedDescription ?? default} ({GKErrorCode})";
                }
                else
                {
                    return NSError?.LocalizedDescription ?? default;
                }
                
            }
        } 
        public NSDictionary<NSString, NSObject> UserInfo => NSError?.UserInfo ?? default;
    }
}
