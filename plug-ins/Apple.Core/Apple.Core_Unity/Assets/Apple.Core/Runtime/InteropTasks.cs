using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Scripting;

namespace Apple.Core.Runtime
{
    [Preserve]
    public static class InteropTasks
    {
        private static readonly Dictionary<long, object> _taskCompletionSources = new Dictionary<long, object>();
        private static readonly object _taskLock = new object();
        private static long _callbackId = long.MinValue;
        
        public static CleanupTaskCompletionSource<TResult> Create<TResult>(out long callbackId)
        {
            var tcs = new CleanupTaskCompletionSource<TResult>();
            
            lock (_taskLock)
            {
                if (_callbackId == long.MaxValue)
                    _callbackId = long.MinValue;

                _callbackId++;
                callbackId = _callbackId;
                
                _taskCompletionSources.Add(callbackId, tcs);
            }

            return tcs;
        }

        public static CleanupTaskCompletionSource<TResult> Create<TResult>(Action cleanup, out long callbackId)
        {
            var tcs = new CleanupTaskCompletionSource<TResult>(cleanup);
            
            lock (_taskLock)
            {
                if (_callbackId == long.MaxValue)
                    _callbackId = long.MinValue;

                _callbackId++;
                callbackId = _callbackId;
                
                _taskCompletionSources.Add(callbackId, tcs);
            }

            return tcs;
        }

        public static bool TrySetResultAndRemove<TResult>(long callbackId, TResult result)
        {
            CleanupTaskCompletionSource<TResult> tcs = null;
            
            lock (_taskLock)
            {
                if (!_taskCompletionSources.ContainsKey(callbackId))
                    return false;

                tcs = (CleanupTaskCompletionSource<TResult>)_taskCompletionSources[callbackId];
                _taskCompletionSources.Remove(callbackId);
            }
            
            return tcs.TrySetResult(result);
        }
        
        public static bool TrySetExceptionAndRemove<TResult>(long callbackId, Exception exception)
        {
            CleanupTaskCompletionSource<TResult> tcs = null;
            
            lock (_taskLock)
            {
                if (!_taskCompletionSources.ContainsKey(callbackId))
                    return false;

                tcs = (CleanupTaskCompletionSource<TResult>)_taskCompletionSources[callbackId];
                _taskCompletionSources.Remove(callbackId);
            }
            
            return tcs.TrySetException(exception);
        }

        public static bool TryGet<TResult>(long callbackId, out CleanupTaskCompletionSource<TResult> tcs)
        {
            tcs = null;

            lock (_taskLock)
            {
                if (!_taskCompletionSources.ContainsKey(callbackId))
                    return false;

                tcs = (CleanupTaskCompletionSource<TResult>)_taskCompletionSources[callbackId];
            }

            return true;
        }

        public static void Remove(long callbackId)
        {
            lock (_taskLock)
            {
                _taskCompletionSources.Remove(callbackId);
            }
        }
    }

    public class CleanupTaskCompletionSource<TResult>
    {
        private readonly Action _cleanup;
        private readonly TaskCompletionSource<TResult> _tcs = new TaskCompletionSource<TResult>();
        public Task<TResult> Task => _tcs.Task;

        public CleanupTaskCompletionSource(Action cleanup = null)
        {
            _cleanup = cleanup;
        }

        public bool TrySetException(Exception exception)
        {
            _cleanup?.Invoke();
            return _tcs.TrySetException(exception);
        }

        public bool TrySetResult(TResult result)
        {
            _cleanup?.Invoke();
            return _tcs.TrySetResult(result);
        }

        public bool TrySetCanceled()
        {
            _cleanup?.Invoke();
            return _tcs.TrySetCanceled();
        }

        public bool TrySetCanceled(System.Threading.CancellationToken token)
        {
            _cleanup?.Invoke();
            return _tcs.TrySetCanceled(token);
        }
    }
}
