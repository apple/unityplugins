using System;
using Apple.Core.Runtime;

namespace Apple.GameKit
{
    public class GameKitException : Exception, IDisposable
    {
        public NSError NSError { get; set; }

        #region Init & Dispose
        public GameKitException(IntPtr nsErrorPtr)
        {
            NSError = new NSError(nsErrorPtr);
        }

        private bool _isDisposed;

        public void Dispose()
        {
            if (!_isDisposed)
            {
                NSError?.Dispose();
                NSError = null;

                _isDisposed = true;

                GC.SuppressFinalize(this);
            }
        }
        #endregion

        public override string Message => $"Code={Code} Domain={Domain} Description={LocalizedDescription}";

        #region NSError properties
        public long Code => NSError?.Code ?? default;
        public string Domain => NSError?.Domain ?? default;
        public string LocalizedDescription => NSError?.LocalizedDescription ?? default;
        public NSDictionary<NSString, NSObject> UserInfo => NSError?.UserInfo ?? default;
        #endregion
    }
}
