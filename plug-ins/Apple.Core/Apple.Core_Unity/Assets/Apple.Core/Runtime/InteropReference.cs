using System;

namespace Apple.Core.Runtime
{
    public class InteropReference : IDisposable
    {
        public IntPtr Pointer { get; set; }
        
        public InteropReference(IntPtr pointer)
        {
            Pointer = pointer;
        }

        ~InteropReference()
        {
            Dispose(false);
        }
        
        #region Equality

        public static bool operator ==(InteropReference a, InteropReference b)
        {
            if (a is null)
            {
                return b is null;
            }

            return a.Equals(b);
        }

        public static bool operator !=(InteropReference a, InteropReference b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null && Pointer == IntPtr.Zero)
                return true;
            
            if(obj is InteropReference reference)
            {
                return Pointer.Equals(reference.Pointer);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Pointer.GetHashCode();
        }

        #endregion
        
        #region IDisposable

        private bool _isDisposed;
        private void Dispose(bool isDisposing)
        {
            if (_isDisposed)
                return;
            
            OnDispose(isDisposing);
            _isDisposed = true;
        }

        protected virtual void OnDispose(bool isDisposing)
        {
            
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        
        #region PointerCast

        /// <summary>
        /// Casts the pointer to the specified type, or returns
        /// null if the pointer == IntPtr.Zero.
        /// </summary>
        /// <param name="pointer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T PointerCast<T>(IntPtr pointer) where T : InteropReference
        {
            return ReflectionUtility.CreateInstanceOrDefault<T>(pointer);
        }

        /// <summary>
        /// Non-generic version of PointerCast that takes a Type argument.
        /// Throws if the provided type is not a subclass of InteropReference.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pointer"></param>
        /// <returns>InteropReference to the created subclass type.
        /// The caller is responsible for downcasting to the desired subclass.</returns>
        public static InteropReference PointerCast(Type type, IntPtr pointer)
        {
            return ReflectionUtility.CreateInstanceOrDefault(type, pointer);
        }

        #endregion
    }
}
