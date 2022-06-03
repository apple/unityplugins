using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;

namespace Apple.CoreHaptics
{
    public class CHHapticPatternPlayer
    {
        protected static ConcurrentDictionary<IntPtr, CHHapticPatternPlayer> PointerToPlayers = new ConcurrentDictionary<IntPtr, CHHapticPatternPlayer>();
        private CHException _lastOperationException;
        
        public IntPtr PlayerId;
        private CHHapticEngine _engine;
        
        public bool Muted
        {
            get => PlayerId != IntPtr.Zero && CHHapticPatternPlayerInterface.GetIsMuted(PlayerId); 
            set
            {
                if(PlayerId != IntPtr.Zero) CHHapticPatternPlayerInterface.SetIsMuted(PlayerId, value); 
            }
        }
        
        public CHHapticPatternPlayer(IntPtr existingPlayerId, CHHapticEngine engine)
        {
            PlayerId = existingPlayerId;
            _engine = engine;
        }
        
        public void Destroy()
        {
            if (PlayerId == IntPtr.Zero)
                return;
            
            CHHapticPatternPlayerInterface.Destroy(PlayerId);
            PlayerId = IntPtr.Zero;
        }

        internal void Setup()
        {
            PointerToPlayers[PlayerId] = this;
        }

        public virtual void Start(float startTime = 0f)
        {
            if (PlayerId == IntPtr.Zero)
                return;
            
            ResetLastOperationException();
            CHHapticPatternPlayerInterface.Start(PlayerId, startTime, OnThrowError);
            CheckAndThrowLastOperationException();
        }

        public virtual void Stop(float stopTime = 0f)
        {
            if (PlayerId == IntPtr.Zero)
                return;
            
            ResetLastOperationException();
            CHHapticPatternPlayerInterface.Stop(PlayerId, stopTime, OnThrowError);
            CheckAndThrowLastOperationException();
        }
        
        public virtual void SendParameters(IEnumerable<CHHapticParameter> parameters, float atTime = 0f)
        {
            if (PlayerId == IntPtr.Zero)
                return;

            if (parameters is null || !parameters.Any())
                throw new ArgumentException("No CHHapticParameters were provided for sending.");

            // Build request...
                var sendParameters = parameters.Select(p => new CHSendParameter(p)).ToArray();
            var handle = GCHandle.Alloc(sendParameters, GCHandleType.Pinned);

            var request = new CHSendParametersRequest
            {
                Parameters = handle.AddrOfPinnedObject(),
                ParametersLength = sendParameters.Length,
                AtTime = atTime
            };

            // Send request...
            ResetLastOperationException();
            CHHapticPatternPlayerInterface.SendParameters(PlayerId, request, OnThrowError);
            handle.Free();
            CheckAndThrowLastOperationException();
        }

        public virtual void ScheduleParameterCurve(CHHapticParameterCurve curve, float atTime = 0f)
        {
            if (PlayerId == IntPtr.Zero)
                return;

            if (curve is null)
                throw new ArgumentException("No CHHapticParameterCurve was provided for scheduling.");

            var requestCurve = new CHHapticParameterCurveRequest(curve);

            // Send request...
            ResetLastOperationException();
            CHHapticPatternPlayerInterface.ScheduleParameterCurve(PlayerId, requestCurve, atTime, OnThrowError);
            requestCurve.FreeHandle();
            CheckAndThrowLastOperationException();
        }


        #region Static Callbacks
        [MonoPInvokeCallback(typeof(ErrorWithPointerCallback))]
        protected static void OnThrowError(IntPtr playerPointer, CHError error)
        {
            if (PointerToPlayers.TryGetValue(playerPointer, out var player))
            {
                player._lastOperationException = new CHException(error);
            }
        }
        #endregion
        
        #region Protected Utility

        protected void ResetLastOperationException()
        {
            _lastOperationException = null;
        }
        protected void CheckAndThrowLastOperationException()
        {
            // Throw exception from last native operation...
            if (!(_lastOperationException is null))
            {
                throw _lastOperationException;
            }
        }
        #endregion
    }
}
