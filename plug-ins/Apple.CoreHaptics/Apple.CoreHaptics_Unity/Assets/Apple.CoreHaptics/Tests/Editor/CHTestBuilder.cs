using Apple.CoreHaptics.UnitTests;
using Apple.Core.Tests.Editor;

using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using UnityEditor.TestTools;

/*
 * Build the UnitTest player with:
 * /path/to/Unity.app/Contents/MacOS/Unity -runTests -batchmode -projectPath /path/to/CoreHaptics-Unity -testPlatform iOS -assemblyNames Tests
 */


[assembly:TestPlayerBuildModifier(typeof(CHTestBuilder))]
[assembly:PostBuildCleanup(typeof(CHTestBuilder))]

namespace Apple.CoreHaptics.UnitTests
{
    public class CHTestBuilder : AppleTestBuilder
    {
        private const string _testScene = "Assets/Apple.CoreHaptics/Tests/TestScene.unity";

        public override BuildPlayerOptions ModifyOptions(BuildPlayerOptions playerOptions)
        {
            Debug.Log("com.apple.unityplugin.corehaptics: Modifying test build player options.");
            if (!playerOptions.scenes.ToArray().Contains(_testScene))
            {
                Debug.Log($"Adding test scene {_testScene} to test build.");
                playerOptions.scenes = playerOptions.scenes.Append(_testScene).ToArray();
            }

            Debug.Log("com.apple.unityplugin.corehaptics: Calling base ModifyOptions.");
            playerOptions = base.ModifyOptions(playerOptions);

            return playerOptions;
        }

        public override void Cleanup()
        {
            Debug.Log("com.apple.unityplugin.corehaptics: Calling base cleanup.");
            base.Cleanup();
        }
    }
}
