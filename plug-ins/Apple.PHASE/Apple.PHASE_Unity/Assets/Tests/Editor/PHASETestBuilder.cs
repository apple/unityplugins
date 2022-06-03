using Apple.PHASE.UnitTests;
using Apple.Core.Tests.Editor;
using System.Collections.Generic;
using UnityEngine.TestTools;
using UnityEditor.TestTools;

/*
 * Build the UnitTest player with:
 * /path/to/Unity.app/Contents/MacOS/Unity -runTests -batchmode -projectPath /path/to/PHASEUnityProject -testPlatform iOS -assemblyNames Tests
 */

[assembly: TestPlayerBuildModifier(typeof(PHASETestBuilder))]
[assembly: PostBuildCleanup(typeof(PHASETestBuilder))]

namespace Apple.PHASE.UnitTests
{
    public class PHASETestBuilder : AppleTestBuilder
    {
        public List<string> _testScenePaths = new List<string> {
            "Assets/Tests/Resources/TestAssets/TestScene.unity"
        };
    }
}