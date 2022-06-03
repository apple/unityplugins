using System;
using System.Runtime.InteropServices;

namespace Apple.Core.Runtime
{
    public class NSDictionary : InteropReference
    {
        #region Init & Dispose
        internal NSDictionary(IntPtr pointer) : base(pointer) {}
        
        [DllImport(InteropUtility.DLLName)]
        private static extern void NSDictionary_Free(IntPtr pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                NSDictionary_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        #endregion

        #region GetValueForKey as? NSDictionary
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr NSDictionary_GetValueForKey_AsNSDictionary(IntPtr pointer, string key);

        public NSDictionary GetNSDictionary(string key)
        {
            var pointer = NSDictionary_GetValueForKey_AsNSDictionary(Pointer, key);
            
            if(pointer != IntPtr.Zero)
                return new NSDictionary(pointer);

            return null;
        }
        #endregion
        
        #region GetValueForKey as? NSError
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr NSDictionary_GetValueForKey_AsNSError(IntPtr pointer, string key);

        public NSError GetNSError(string key)
        {
            var pointer = NSDictionary_GetValueForKey_AsNSError(Pointer, key);
            
            if(pointer != IntPtr.Zero)
                return new NSError(pointer);

            return null;
        }
        #endregion
        
        #region GetValueForKey as? String
        [DllImport(InteropUtility.DLLName)]
        private static extern string NSDictionary_GetValueForKey_AsString(IntPtr pointer, string key);

        public string GetString(string key)
        {
            return NSDictionary_GetValueForKey_AsString(Pointer, key);
        }
        #endregion
        
        #region GetValueForKey as? Boolean
        [DllImport(InteropUtility.DLLName)]
        private static extern bool NSDictionary_GetValueForKey_AsBoolean(IntPtr pointer, string key);

        public bool GetBoolean(string key)
        {
            return NSDictionary_GetValueForKey_AsBoolean(Pointer, key);
        }
        #endregion
        
        #region GetValueForKey as? Int64
        [DllImport(InteropUtility.DLLName)]
        private static extern long NSDictionary_GetValueForKey_AsInt64(IntPtr pointer, string key);

        public long GetInt64(string key)
        {
            return NSDictionary_GetValueForKey_AsInt64(Pointer, key);
        }
        #endregion
        
        #region GetValueForKey as? Double
        [DllImport(InteropUtility.DLLName)]
        private static extern long NSDictionary_GetValueForKey_AsDouble(IntPtr pointer, string key);

        public double GetDouble(string key)
        {
            return NSDictionary_GetValueForKey_AsDouble(Pointer, key);
        }
        #endregion
    }
}
