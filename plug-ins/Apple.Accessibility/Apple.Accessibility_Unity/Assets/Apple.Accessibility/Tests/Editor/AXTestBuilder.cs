using Apple.Accessibility.UnitTests;
using Apple.Core.Tests.Editor;

using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using UnityEditor.TestTools;


[assembly:TestPlayerBuildModifier(typeof(AXTestBuilder))]
[assembly:PostBuildCleanup(typeof(AXTestBuilder))]

namespace Apple.Accessibility.UnitTests
{
    public class AXTestBuilder : AppleTestBuilder
    {
        private string _testScene = "Assets/Apple.Accessibility/Tests/TestScene.unity";

        public override BuildPlayerOptions ModifyOptions(BuildPlayerOptions playerOptions)
        {
            Debug.Log("com.apple.unityplugin.accessibility: Modifying test build player options.");
            if (!playerOptions.scenes.ToArray().Contains(_testScene))
            {
                Debug.Log($"Adding test scene {_testScene} to test build.");
                playerOptions.scenes = playerOptions.scenes.Append(_testScene).ToArray();
            }

            Debug.Log("com.apple.unityplugin.accessibility: Calling base ModifyOptions.");
            playerOptions = base.ModifyOptions(playerOptions);

            return playerOptions;
        }

        public override void Cleanup()
        {
            Debug.Log("com.apple.unityplugin.accessibility: Calling base cleanup.");
            base.Cleanup();
        }
    }
}
