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
        
        public static TaskCompletionSource<TResult> Create<TResult>(out long callbackId)
        {
            var tcs = new TaskCompletionSource<TResult>();
            
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
            TaskCompletionSource<TResult> tcs = null;
            
            lock (_taskLock)
            {
                if (!_taskCompletionSources.ContainsKey(callbackId))
                    return false;

                tcs = (TaskCompletionSource<TResult>)_taskCompletionSources[callbackId];
                _taskCompletionSources.Remove(callbackId);
            }
            
            return tcs.TrySetResult(result);
        }
        
        public static bool TrySetExceptionAndRemove<TResult>(long callbackId, Exception exception)
        {
            TaskCompletionSource<TResult> tcs = null;
            
            lock (_taskLock)
            {
                if (!_taskCompletionSources.ContainsKey(callbackId))
                    return false;

                tcs = (TaskCompletionSource<TResult>)_taskCompletionSources[callbackId];
                _taskCompletionSources.Remove(callbackId);
            }
            
            return tcs.TrySetException(exception);
        }

        public static bool TryGet<TResult>(long callbackId, out TaskCompletionSource<TResult> tcs)
        {
            tcs = null;

            lock (_taskLock)
            {
                if (!_taskCompletionSources.ContainsKey(callbackId))
                    return false;

                tcs = (TaskCompletionSource<TResult>)_taskCompletionSources[callbackId];
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
}
