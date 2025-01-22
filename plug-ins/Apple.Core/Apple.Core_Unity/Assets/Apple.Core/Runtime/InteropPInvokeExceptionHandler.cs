using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Apple.Core.Runtime
{
    // MonoPInvokeCallback methods called from native code can cause mysterious SIGABRT crashes 
    // if they throw exceptions.

    // Guidance:
    // There are two scenarios in the Unity plug-in code where p-invoke callbacks are used.
    //
    // 1. Event style callbacks that can be issued any time the native code needs to inform 
    //    the app/game that something has occurred. The CatchAndLog methods below log the
    //    exception but eat it to avoid causing a SIGABRT when execution passes back to the
    //    native side.
    //
    // 2. Async Task completion callbacks. Exceptions that occur in async Task completion
    //    callbacks are stowed and marshaled back to the main Unity C# script execution
    //    context via InteropTasks.TrySetExceptionAndRemove.

    public static class InteropPInvokeExceptionHandler
    {
        public static TResult CatchAndLog<TResult>(Func<TResult> func)
        {
            try
            {
                return func.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return default;
            }
        }

        public static void CatchAndLog(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public static Task CatchAndLog(Func<Task> func)
        {
            try
            {
                return func.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return Task.FromException(ex);
            }
        }

        public static Task<TResult> CatchAndLog<TResult>(Func<Task<TResult>> func)
        {
            try
            {
                return func.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return Task.FromException<TResult>(ex);
            }
        }
    }
}
