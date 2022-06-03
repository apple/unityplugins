using UnityEngine;
using NUnit.Framework.Interfaces;
using UnityEngine.TestRunner;
using Apple.Core.Tests;

[assembly: TestRunCallback(typeof(PHASETestRunner))]
public class PHASETestRunner : AppleTestRunner
{
    public override void RunFinished(ITestResult testResults)
    {
        Debug.Log("Calling AppleTestRunner.RunFinished");
        base.RunFinished(testResults);
    }
}
