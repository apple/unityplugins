using Apple.Core.Tests.Editor;

using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using UnityEditor.TestTools;

/*
 * Build the UnitTest player with:
 * /path/to/Unity.app/Contents/MacOS/Unity -runTests -batchmode -projectPath /path/to/unity_project -testPlatform [iOS, macOS, tvOS] -assemblyNames [your_test_assembly]
 */


/*
 * If you need to include a test scene in your own test runs,
 * you will need to add a file to your project under Assets/Tests/Editor.
 * Call it something like `MyProjectTestBuilder.cs`
 * It should contain the following code:

using Apple.MyProject.UnitTests;
using Apple.Core.Tests.Editor;

using System.Collections.Generic;
using UnityEngine.TestTools;
using UnityEditor.TestTools;

[assembly:TestPlayerBuildModifier(typeof(MyTestBuilder))]
[assembly:PostBuildCleanup(typeof(MyTestBuilder))]

namespace Apple.MyProject.UnitTests
{
    public class MyTestBuilder : AppleTestBuilder
    {
        public List<string> _testScenePaths = new List<string> {
            "Assets/Apple.Accessibility/Tests/TestScene1.unity",
            "Assets/Apple.Accessibility/Tests/TestScene2.unity",
            "etc"
        };
    }
}
*/

[assembly: TestPlayerBuildModifier(typeof(AppleTestBuilder))]
[assembly: PostBuildCleanup(typeof(AppleTestBuilder))]

namespace Apple.Core.Tests.Editor
{
    public class AppleTestBuilder : ITestPlayerBuildModifier, IPostBuildCleanup
    {
        private static bool s_RunningPlayerTests;

        public virtual BuildPlayerOptions ModifyOptions(BuildPlayerOptions playerOptions)
        {
            Debug.Log("com.apple.unityplugin.core: Modifying player options in the test build");
            playerOptions.options &= ~(BuildOptions.AutoRunPlayer | BuildOptions.ConnectToHost);

            var buildLocation = Path.GetFullPath("TestPlayers");
            var fileName = Path.GetFileName(playerOptions.locationPathName);
            if (!string.IsNullOrEmpty(fileName))
            {
                buildLocation = Path.Combine(buildLocation, fileName);
            }

            playerOptions.locationPathName = buildLocation;

            s_RunningPlayerTests = true;

            return playerOptions;
        }

        public virtual void Cleanup()
        {
            Debug.Log("com.apple.unityplugin.core: Entered cleanup. Checking if commandline run...");
            if (s_RunningPlayerTests && IsRunningTestsFromCommandline())
            {
                Debug.Log("com.apple.unityplugin.core: Exiting Editor.");
                EditorApplication.update += () => { EditorApplication.Exit(0); };
            }
            Debug.Log("com.apple.unityplugin.core: Exiting Cleanup.");
        }

        internal static bool IsRunningTestsFromCommandline()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            return commandLineArgs.Any(value => value == "-runTests");
        }
    }
}