using NUnit.Framework.Interfaces;
using UnityEngine.TestRunner;
using Apple.Core.Tests;

/*
 * View test results by:
 * 1. Installing the output app on-device
 * 2. Open the app
 * 3. View resulting test logs with:
 * log show --predicate 'process=="CoreHaptics-Unity" AND message CONTAINS "UNITY TEST LOG"'
 * 
 * OR, after unit-test app runs, run:
 *
 * dvdo cat `conman path -i com.appleinternal.CoreHaptics-Unity -c appData`/Documents/testresults.xml
 */

[assembly:TestRunCallback(typeof(CHResultSerializer))]
public class CHResultSerializer : AppleTestRunner
{
    public override void TestStarted(ITest test)
    {
        base.TestStarted(test);
        CHTestLogger.LogToiOS($"=== Now running test {test.Name} ===");
    }

    public override void TestFinished(ITestResult result)
    {
        base.TestFinished(result);
        // Only log results for leaf nodes (actual tests)
        if (!result.Test.HasChildren)
        {
            CHTestLogger.LogToiOS(result.Message);
            CHTestLogger.LogTestResult(result);
        }
    }

    public override void RunFinished(ITestResult testResults)
    {
        CHTestLogger.LogTestRunSummary(testResults);
        base.RunFinished(testResults);
    }
}
