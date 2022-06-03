using System;
using UnityEngine;

namespace Apple.GameController
{
    #region Callback Type Definitions
    internal delegate void SuccessCallback();
    internal delegate void ErrorCallback(GCError error);
    #endregion
}