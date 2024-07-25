using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Apple.GameController.Controller
{
    #region Delegate Type Definitions
    public delegate void ControllerConnectionStateChangedCallback(GCControllerHandle handle);
    #endregion

    public static partial class GCControllerService
    {
        private static Dictionary<string, GCController> _controllers = new Dictionary<string, GCController>();

        public static event EventHandler<ControllerConnectedEventArgs> ControllerConnected;
        public static event EventHandler<ControllerConnectedEventArgs> ControllerDisconnected;

        #region Initialize
        public static void Initialize()
        {
            SetConnectionHandlers(OnControllerConnected, OnControllerDisconnected);
        }

        [MonoPInvokeCallback(typeof(ControllerConnectionStateChangedCallback))]
        private static void OnControllerConnected(GCControllerHandle handle)
        {
            TryConnectController(handle);
        }

        private static void TryConnectController(GCControllerHandle handle)
        {
            // Add controller if not already tracked...
            if (!_controllers.ContainsKey(handle.UniqueId))
            {
                _controllers[handle.UniqueId] = new GCController(handle);

                // Update the handle with latest info...
                _controllers[handle.UniqueId].Handle = handle;
                _controllers[handle.UniqueId].NotifiyConnectedStateChanged(new ConnectionStateChangeEventArgs(true));

                // Notify connected...
                ControllerConnected?.Invoke(null, new ControllerConnectedEventArgs(_controllers[handle.UniqueId]));
            }
        }

        [MonoPInvokeCallback(typeof(ControllerConnectionStateChangedCallback))]
        private static void OnControllerDisconnected(GCControllerHandle handle)
        {
            TryDisconnectController(handle);
        }

        private static void TryDisconnectController(GCControllerHandle handle)
        {
            if (_controllers.ContainsKey(handle.UniqueId))
            {
                _controllers[handle.UniqueId].Handle = handle;
                _controllers[handle.UniqueId].NotifiyConnectedStateChanged(new ConnectionStateChangeEventArgs(false));

                // Notify disconnected...
                ControllerDisconnected?.Invoke(null, new ControllerConnectedEventArgs(_controllers[handle.UniqueId]));

                // Remove the tracked controller...
                _controllers.Remove(handle.UniqueId);
            }
        }
        #endregion

        #region Poll All Controllers
        public static void PollAllControllers()
        {
            foreach(var pair in _controllers)
            {
                if(pair.Value.IsConnected)
                {
                    pair.Value.Poll();
                }
            }
        }
        #endregion

        #region Get Connected Controllers
        public static IEnumerable<GCController> GetConnectedControllers()
        {
            // Ensure existing controllers are "connected"...
            foreach(var handle in GetConnectedControllerHandles())
            {
                TryConnectController(handle);
            }

            return _controllers.Values;
        }
        #endregion
        #region Start Wireless Discovery
        #if !UNITY_EDITOR_WIN
        [DllImport(InteropUtility.DLLName)]
        private static extern void GameControllerWrapper_StartWirelessDiscovery(SuccessCallback onComplete);
        #endif
        private static TaskCompletionSource<bool> _startWirelessDiscoveryTCS;

        public static Task StartWirelessDiscovery()
        {
            if (_startWirelessDiscoveryTCS != null)
                throw new InvalidOperationException("An operation is already in progress.");

            _startWirelessDiscoveryTCS = new TaskCompletionSource<bool>();

            #if !UNITY_EDITOR_WIN
            GameControllerWrapper_StartWirelessDiscovery(OnWirelessDiscoveryComplete);
            return _startWirelessDiscoveryTCS.Task;
            #else
            var returnValue = _startWirelessDiscoveryTCS.Task;
            OnWirelessDiscoveryComplete();
            return returnValue;
            #endif
        }

        [MonoPInvokeCallback(typeof(SuccessCallback))]
        private static void OnWirelessDiscoveryComplete()
        {
            _startWirelessDiscoveryTCS?.SetResult(true);
            _startWirelessDiscoveryTCS = null;
        }
        #endregion

        #region Stop Wireless Discovery
        #if !UNITY_EDITOR_WIN
        [DllImport(InteropUtility.DLLName)]
        private static extern void GameControllerWrapper_StopWirelessDiscovery();
        #endif

        public static void StopWirelessDiscovery()
        {
            #if !UNITY_EDITOR_WIN
            GameControllerWrapper_StopWirelessDiscovery();
            #endif
        }
        #endregion
        #region Set Connection Handlers
        #if !UNITY_EDITOR_WIN
        [DllImport(InteropUtility.DLLName)]
        private static extern void GameControllerWrapper_SetConnectionHandlers(ControllerConnectionStateChangedCallback onConnected, ControllerConnectionStateChangedCallback onDisconnected);
#endif

        private static void SetConnectionHandlers(ControllerConnectionStateChangedCallback onConnected, ControllerConnectionStateChangedCallback onDisconnected)
        {
            #if !UNITY_EDITOR_WIN
            GameControllerWrapper_SetConnectionHandlers(onConnected, onDisconnected);
            #endif
        }
        #endregion

        #region Get Connected Controller Handles
        #if !UNITY_EDITOR_WIN
        [DllImport(InteropUtility.DLLName)]
        private static extern GCGetConnectedControllersResponse GameControllerWrapper_GetConnectedControllers();
        #endif

        public static GCControllerHandle[] GetConnectedControllerHandles()
        {
            // TODO: Instead of returning the handle, return a GCController based on the unique id...
            #if !UNITY_EDITOR_WIN
            var response = GameControllerWrapper_GetConnectedControllers();
            return response.GetControllers();
            #else
            return new GCControllerHandle[] {};
            #endif
        }
        #endregion

        #region Poll Controller
        #if !UNITY_EDITOR_WIN
        [DllImport(InteropUtility.DLLName)]
        private static extern GCControllerInputState GameControllerWrapper_PollController(string uniqueId);
        #endif
        public static GCControllerInputState PollController(GCControllerHandle controllerHandle)
        {
            #if !UNITY_EDITOR_WIN
            return GameControllerWrapper_PollController(controllerHandle.UniqueId);
            #else
            return GCControllerInputState.None;
            #endif
        }
        #endregion

        #region Controller Light Color
        #if !UNITY_EDITOR_WIN
        [DllImport(InteropUtility.DLLName)]
        private static extern void GameControllerWrapper_SetControllerLightColor(string uniqueId, float red, float green, float blue);
        #endif

        public static void SetControllerLightColor(GCControllerHandle controllerHandle, float red, float green, float blue)
        {
            #if !UNITY_EDITOR_WIN
            GameControllerWrapper_SetControllerLightColor(controllerHandle.UniqueId, red, green, blue);
            #endif
            Debug.Log($"Setting controller {controllerHandle.UniqueId} to [{red}, {green}, {blue}]");
        }
        #endregion
        
        #region Get Symbol for Input Name
        #if !UNITY_EDITOR_WIN
        [DllImport(InteropUtility.DLLName)]
        private static extern GCGetSymbolForInputNameResponse GameControllerWrapper_GetSymbolForInputName(string uniqueId, GCControllerInputName inputName, GCControllerSymbolScale symbolScale, GCControllerRenderingMode renderingMode);
        #else
        private static Texture2D tempSymbol = null;
        #endif

        public static Texture2D GetSymbolForInputName(GCControllerHandle controllerHandle, GCControllerInputName inputName, GCControllerSymbolScale symbolScale, GCControllerRenderingMode renderingMode)
        {
            #if !UNITY_EDITOR_WIN
            var response = GameControllerWrapper_GetSymbolForInputName(controllerHandle.UniqueId, inputName, symbolScale, renderingMode);
            return response.GetTexture();
            #else
            if( tempSymbol == null ) tempSymbol = new Texture2D(256, 256, TextureFormat.RGB24, false);
            return tempSymbol;
            #endif
        }
        #endregion
    }
}