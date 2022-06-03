using NUnit.Framework.Interfaces;
using System;
using UnityEngine;

public class CHTestLogger : MonoBehaviour
{
	public void Start() {
        _ = new Apple.Core.AppleLogger();
    }

	public static void LogToiOS(string msg)
	{
#if UNITY_IPHONE
		Debug.Log(Application.platform == RuntimePlatform.IPhonePlayer ? $"\n===UNITY TEST LOG===\n{msg}" : msg);
#endif
	}

    public static void LogTestResult(ITestResult result)
    {
        var resultMessage = string.Join(
            Environment.NewLine,
            "<test_result>",
            "{ \"Test_result\":",
                "\t{", // Begin result

                    $"\t\t\"Test_name\": \"{result.Name}\",",
                    $"\t\t\"Result\": \"{result.ResultState}\",",
                    $"\t\t\"Raw_output\": \"{result.Output}\"",

                "\t}", // End result
            "}", // End json
            "</test_result>"
            );

        LogToiOS(resultMessage);
    }

    public static void LogTestRunSummary(ITestResult testResults)
    {
        var resultMessage = string.Join(
            Environment.NewLine,
            "<test_suite>",
            "{ \"Test_suite\":",
                "\t{", // Begin suite

                    $"\t\t\"Passing\": \"{testResults.PassCount}\",",
                    $"\t\t\"Failing\": \"{testResults.FailCount}\",",
                    $"\t\t\"Skipped\": \"{testResults.SkipCount}\",",
                    $"\t\t\"Inconclusive\": \"{testResults.InconclusiveCount}\"",

                "\t}", // End suite
            "}", // End json
            "</test_suite>"
            );

        LogToiOS(resultMessage);
    }
}
