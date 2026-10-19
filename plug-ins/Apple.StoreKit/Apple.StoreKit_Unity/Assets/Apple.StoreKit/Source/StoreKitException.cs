using System;
using Apple.Core.Runtime;

namespace Apple.StoreKit
{
    public class StoreKitException : Exception
    {
        public NSError NSError { get; set; }

        public StoreKitException(IntPtr nsErrorPtr)
        {
            NSError = new NSError(nsErrorPtr);
        }

        public StoreKitException(NSError nsError)
        {
            NSError = nsError;
        }

        public override string Message => $"Code={Code} Domain={Domain} Description={LocalizedDescription}";

        public long Code => NSError?.Code ?? default;
        public string Domain => NSError?.Domain ?? default;
        public string LocalizedDescription => NSError?.LocalizedDescription ?? default;
        public NSDictionary<NSString, NSObject> UserInfo => NSError?.UserInfo ?? default;
    }
}
