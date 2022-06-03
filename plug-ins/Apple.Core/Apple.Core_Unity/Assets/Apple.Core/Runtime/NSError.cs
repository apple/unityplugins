using System;
using System.Runtime.InteropServices;

namespace Apple.Core.Runtime
{
    public class NSError : Exception, IDisposable
    {
        #region Init & Dispose
        [DllImport(InteropUtility.DLLName)]
        private static extern void NSError_Free(IntPtr pointer);
        protected IntPtr Pointer { get; set; }
        
        public NSError(IntPtr pointer)
        {
            Pointer = pointer;
        }

        ~NSError()
        {
            Dispose(false);
        }
        #endregion

        #region IDisposable

        private bool _isDisposed;
        
        public void Dispose()
        {
            if (!_isDisposed)
            {
                Dispose(true);
                _isDisposed = true;
                
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                NSError_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }
        #endregion
        
        #region Code    
        [DllImport(InteropUtility.DLLName)]
        private static extern long NSError_GetCode(IntPtr pointer);
        
        public long Code
        {
            get => NSError_GetCode(Pointer);
        }
        #endregion
        
        #region Domain    
        [DllImport(InteropUtility.DLLName)]
        private static extern string NSError_GetDomain(IntPtr pointer);
        
        public string Domain
        {
            get => NSError_GetDomain(Pointer);
        }
        #endregion
        
        #region LocalizedDescription    
        [DllImport(InteropUtility.DLLName)]
        private static extern string NSError_GetLocalizedDescription(IntPtr pointer);
        
        public string LocalizedDescription
        {
            get => NSError_GetLocalizedDescription(Pointer);
        }
        #endregion
        
        #region UserInfo
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr NSError_GetUserInfo(IntPtr pointer);

        private NSDictionary _userInfo;

        public NSDictionary UserInfo
        {
            get
            {
                if (_userInfo == null)
                {
                    var pointer = NSError_GetUserInfo(Pointer);
                    
                    if(pointer != IntPtr.Zero)
                        _userInfo = new NSDictionary(pointer);
                }

                return _userInfo;
            }
        }
        #endregion
    }
}
