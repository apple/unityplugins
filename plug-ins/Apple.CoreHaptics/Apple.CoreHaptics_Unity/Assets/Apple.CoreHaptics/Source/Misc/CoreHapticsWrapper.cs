using System;

namespace Apple.CoreHaptics
{
    #region Callback Type Definitions
    internal delegate void SuccessCallback();

    internal delegate void SuccessWithPointerCallback(IntPtr pointer);

    internal delegate void GenericCallback<T1>(T1 value);

    internal delegate void GenericCallback<T1, T2>(T1 value1, T2 value2);
    internal delegate void ErrorCallback(CHError error);

    internal delegate void ErrorWithPointerCallback(IntPtr pointer, CHError error);

    #endregion
}