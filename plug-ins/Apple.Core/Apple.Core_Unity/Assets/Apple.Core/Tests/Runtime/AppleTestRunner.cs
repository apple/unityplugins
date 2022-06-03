using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.TestRunner;
using System.IO;
using System.Xml;

using Apple.Core.Tests;

[assembly: TestRunCallback(typeof(AppleTestRunner))]

namespace Apple.Core.Tests
{
    public class AppleTestRunner : ITestRunCallback
    {
        public virtual void RunStarted(ITest testsToRun) { }

        public virtual void TestStarted(ITest test) { }

        public virtual void TestFinished(ITestResult result) { }

        public virtual void RunFinished(ITestResult testResults)
        {
            var path = Path.Combine(Application.persistentDataPath, "testresults.xml");
            using (var xw = XmlWriter.Create(path, new XmlWriterSettings { Indent = true }))
            {
                testResults.ToXml(true).WriteTo(xw);
            }

            Debug.Log($"Unity test results written to: {path}");

            Application.Quit(testResults.FailCount > 0 ? 1 : 0);
        }
    }
}