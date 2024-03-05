using NUnit.Framework;

using Apple.Core.Runtime;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

// This is necessary because some of the tests throw before they have a chance to return.
#pragma warning disable CS0162 // Unreachable code detected

public class TestInteropPInvokeExceptionHandler
{
    [Test]
    public void TestCatchAndLog1WithException()
    {
        Assert.DoesNotThrow(() =>
        {
            // TResult CatchAndLog<TResult>(Func<TResult> func)
            var message = "TResult CatchAndLog<TResult>(Func<TResult> func)";
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Exception, $"Exception: {message}");
            Assert.AreEqual(default(int), InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                throw new Exception(message);
                return 9999;
            }));
        });
    }

    [Test]
    public void TestCatchAndLog1WithoutException()
    {
        Assert.DoesNotThrow(() =>
        {
            // TResult CatchAndLog<TResult>(Func<TResult> func)
            Assert.AreEqual(9999, InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                return 9999;
            }));
        });
    }

    [Test]
    public void TestCatchAndLog2WithException()
    {
        Assert.DoesNotThrow(() =>
        {
            // void CatchAndLog(Action action)
            var message = "void CatchAndLog(Action action)";
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Exception, $"Exception: {message}");
            InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                throw new Exception(message);
            });
        });
    }

    [Test]
    public void TestCatchAndLog2WithoutException()
    {
        Assert.DoesNotThrow(() =>
        {
            // void CatchAndLog(Action action)
            InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                return;
            });
        });
    }

    [Test]
    public void TestCatchAndLog3WithException()
    {
        Assert.DoesNotThrow(() =>
        {
            // Task CatchAndLog(Func<Task> func)
            var message = "Task CatchAndLog(Func<Task> func)";
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Exception, $"Exception: {message}");
            var task = InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                throw new Exception(message);
                return Task.CompletedTask;
            });

            Assert.Throws<AggregateException>(() =>
            {
                task.Wait(TimeSpan.FromSeconds(1));
            });

            Assert.IsTrue(task.IsFaulted);
        });
    }

    [Test]
    public void TestCatchAndLog3WithoutException()
    {
        Assert.DoesNotThrow(() =>
        {
            // Task CatchAndLog(Func<Task> func)
            var task = InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                return Task.CompletedTask;
            });

            Assert.DoesNotThrow(() =>
            {
                task.Wait(TimeSpan.FromSeconds(1));
            });

            Assert.IsTrue(task.IsCompleted);
        });
    }

    [Test]
    public void TestCatchAndLog4WithException()
    {
        Assert.DoesNotThrow(() =>
        {
            // Task<TResult> CatchAndLog<TResult>(Func<Task<TResult>> func)
            var message = "Task<TResult> CatchAndLog<TResult>(Func<Task<TResult>> func)";
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Exception, $"Exception: {message}");
            var task = InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                throw new Exception(message);
                return Task.Run(() => 9999);
            });

            Assert.Throws<AggregateException>(() =>
            {
                task.Wait(TimeSpan.FromSeconds(1));
            });

            Assert.IsTrue(task.IsFaulted);
        });
    }

    [Test]
    public void TestCatchAndLog4WithoutException()
    {
        Assert.DoesNotThrow(() =>
        {
            // Task<TResult> CatchAndLog<TResult>(Func<Task<TResult>> func)
            var task = InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                return Task.Run(() => 9999);
            });

            Assert.DoesNotThrow(() =>
            {
                task.Wait(TimeSpan.FromSeconds(1));
            });

            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(task.Result, 9999);
        });
    }

}
