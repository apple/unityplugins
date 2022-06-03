using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Apple.PHASE.UnitTests
{
    public class PHASETests
    {
        static long InvalidId = -1;

        private string m_testResourcesDir = "TestAssets/";

        private Scene m_scene;

        static string[] m_testSources = { "Bells", "Voice", "Ambience", "StereoPopcorn", "Footsteps" };

        [UnitySetUp]
        public IEnumerator SetupPHASETestScene()
        {
            LoadSceneParameters parameters = new LoadSceneParameters(LoadSceneMode.Single);
            m_scene = SceneManager.LoadScene("TestScene", parameters);
            yield return new WaitForFixedUpdate();
            Assert.That(m_scene.IsValid(), "Failed to load TestScene");
        }

        [UnityTest]
        public IEnumerator PHASETestsCreatePlayDestroySource([ValueSource(nameof(m_testSources))] string testSourceName)
        {
            GameObject sourceObject = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + testSourceName));

            PHASESource source = sourceObject.GetComponent<PHASESource>();
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(source.GetInstanceID() != InvalidId, $"Failed to create {testSourceName}");

            source.Play();
            yield return new WaitForFixedUpdate();
            Assert.That(source.IsPlaying(), $"Failed to play PHASE Source {testSourceName}");

            yield return new WaitForSeconds(1f);
            source.Stop();
            yield return new WaitForSeconds(0.1f);
            Assert.That(!source.IsPlaying(), $"Failed to stop PHASE Source {testSourceName}");

            Object.Destroy(sourceObject);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(sourceObject, $"Failed to destroy {testSourceName}");

        }

        [UnityTest]
        public IEnumerator PHASETestsCreatePlaySwitchDestroySource()
        {
            GameObject sourceObject = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "Footsteps"));

            PHASESource source = sourceObject.GetComponent<PHASESource>();
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(source.GetInstanceID() != InvalidId, "Failed to create Footsteps");

            source.Play();
            yield return new WaitForSeconds(0.5f);

            source.Play();
            source.SetMetaParameterValue("Terrain", "Wood");
            yield return new WaitForSeconds(0.5f);

            source.Play();
            source.SetMetaParameterValue("Terrain", "Concrete");
            yield return new WaitForSeconds(0.5f);

            source.Stop();
            yield return new WaitForSeconds(0.2f);
            Assert.That(!source.IsPlaying(), "Failed to stop source");

            Object.Destroy(sourceObject);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(sourceObject, "Failed to destroy Footsteps");

        }

        [UnityTest]
        public IEnumerator PHASETestsDestroyLoopingSource()
        {
            GameObject sourceObject = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "Voice"));

            PHASESource source = sourceObject.GetComponent<PHASESource>();
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(source.GetInstanceID() != InvalidId, "Failed to create looping source");

            source.Play();
            yield return new WaitForSeconds(0.75f);
            Assert.That(source.IsPlaying(), "Failed to play looping source");

            try
            {
                Object.Destroy(sourceObject);
            }
            catch (KeyNotFoundException ex)
            {
                Debug.LogError(ex);
            }

            yield return new WaitForSeconds(0.5f);
            UnityEngine.Assertions.Assert.IsNull(sourceObject, "Failed to destroy Footsteps");

        }

        [UnityTest]
        public IEnumerator PHASETestsDisableAndEnableLoopingSource()
        {
            GameObject sourceObject = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "Voice"));

            PHASESource source = sourceObject.GetComponent<PHASESource>();
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(source.GetInstanceID() != InvalidId, "Failed to create looping source");

            source.Play();
            yield return new WaitForSeconds(0.75f);
            Assert.That(source.IsPlaying(), "Failed to play looping source");

            sourceObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            Assert.That(sourceObject.activeSelf == false, "Failed to play looping source");

            sourceObject.SetActive(true);
            yield return new WaitForSeconds(0.75f);
            Assert.That(sourceObject.activeSelf == true, "Failed to play looping source");

            source.Stop();
            yield return new WaitForSeconds(0.75f);
            Assert.That(!source.IsPlaying(), "Failed to stop looping source");

            try
            {
                Object.Destroy(sourceObject);
            }
            catch (KeyNotFoundException ex)
            {
                Debug.LogError(ex);
            }

            yield return new WaitForSeconds(0.5f);
            UnityEngine.Assertions.Assert.IsNull(sourceObject, "Failed to destroy Footsteps");

        }

        [UnityTest]
        public IEnumerator PHASETestOccluder()
        {
            GameObject sourceObject = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "Voice"));

            PHASESource source = sourceObject.GetComponent<PHASESource>();
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(source.GetInstanceID() != InvalidId, "Failed to create looping source");

            source.Play();
            yield return new WaitForSeconds(0.75f);
            Assert.That(source.IsPlaying(), "Failed to play looping source");

            GameObject occluderObject = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "PHASEOccluder"));
            PHASEOccluder occluder = occluderObject.GetComponent<PHASEOccluder>();
            yield return new WaitForSeconds(0.75f);
            Assert.That(occluder.GetOccluderId() != InvalidId, "Failed to instantiate PHASE Occluder");

            Object.Destroy(occluderObject);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(occluderObject, "Failed to destroy PHASE Occluder");

            source.Stop();
            yield return new WaitForSeconds(0.1f);
            Assert.That(!source.IsPlaying(), "Failed to stop source");

            Object.Destroy(sourceObject);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(sourceObject, "Failed to destroy PHASE Source");
        }

        [UnityTest]
        public IEnumerator PHASETestOccluderWithSubmeshesAndParentScaling()
        {
            GameObject sourceObject = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "Voice"));

            PHASESource source = sourceObject.GetComponent<PHASESource>();
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(source.GetInstanceID() != InvalidId, "Failed to create looping source");

            source.Play();
            yield return new WaitForSeconds(0.75f);
            Assert.That(source.IsPlaying(), "Failed to play looping source");

            GameObject occluderObject = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "PHASEOccluderMultipleSubmeshesScaledParent"));
            PHASEOccluder occluder = occluderObject.GetComponent<PHASEOccluder>();
            yield return new WaitForSeconds(0.75f);
            Assert.That(occluder.GetOccluderId() != InvalidId, "Failed to instantiate PHASE Occluder with submesh and scaled parent");

            Object.Destroy(occluderObject);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(occluderObject, "Failed to destroy PHASE Occluder");

            source.Stop();
            yield return new WaitForSeconds(0.1f);
            Assert.That(!source.IsPlaying(), "Failed to stop source");

            Object.Destroy(sourceObject);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(sourceObject, "Failed to destroy PHASE Source");
        }

        [UnityTest]
        public IEnumerator PHASETestPlaySoundEventMultipleTimes()
        {
            GameObject sourceObject = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "Voice"));

            PHASESource source = sourceObject.GetComponent<PHASESource>();
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(source.GetInstanceID() != InvalidId, "Failed to create looping source");

            source.Play();
            yield return new WaitForSeconds(0.05f);
            source.Play();
            yield return new WaitForSeconds(0.1f);
            source.Play();
            yield return new WaitForSeconds(0.75f);
            Assert.That(source.IsPlaying(), "Failed to play looping source");

            source.Stop();
            yield return new WaitForSeconds(0.1f);
            Assert.That(!source.IsPlaying(), "Failed to stop source");

            Object.Destroy(sourceObject);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(sourceObject, "Failed to destroy PHASE Source");
        }

        [UnityTest]
        public IEnumerator PHASETestOccluderDisableAndEnable()
        {
            GameObject sourceObject = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "Voice"));

            PHASESource source = sourceObject.GetComponent<PHASESource>();
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(source.GetInstanceID() != InvalidId, "Failed to create looping source");

            source.Play();
            yield return new WaitForSeconds(0.75f);
            Assert.That(source.IsPlaying(), "Failed to play looping source");

            GameObject occluderObject = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "PHASEOccluder"));
            PHASEOccluder occluder = occluderObject.GetComponent<PHASEOccluder>();
            yield return new WaitForSeconds(0.75f);
            Assert.That(occluder.GetOccluderId() != InvalidId, "Failed to instantiate PHASE Occluder");

            occluderObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            Assert.That(occluderObject.activeSelf == false);

            occluderObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            Assert.That(occluderObject.activeSelf == true);

            Object.Destroy(occluderObject);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(occluderObject, "Failed to destroy PHASE Occluder");

            source.Stop();
            yield return new WaitForSeconds(0.1f);
            Assert.That(!source.IsPlaying(), "Failed to stop source");

            Object.Destroy(sourceObject);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(sourceObject, "Failed to destroy PHASE Source");
        }

        [UnityTest]
        public IEnumerator PHASETestDisableAndEnableListener()
        {
            GameObject listenerObject = GameObject.Find("PHASEListener");
            GameObject sourceObject = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "Voice"));

            PHASESource source = sourceObject.GetComponent<PHASESource>();
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(source.GetInstanceID() != InvalidId, "Failed to create looping source");

            source.Play();
            yield return new WaitForSeconds(0.75f);
            Assert.That(source.IsPlaying(), "Failed to play looping source");

            listenerObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            Assert.That(listenerObject.activeSelf == false, "Failed to disable listener.");

            listenerObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            Assert.That(listenerObject.activeSelf == true, "Failed to enable listener.");

            source.Stop();
            yield return new WaitForSeconds(0.1f);
            Assert.That(!source.IsPlaying(), "Failed to stop source");

            Object.Destroy(sourceObject);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(sourceObject, "Failed to destroy PHASE Source");
        }

        [UnityTest]
        public IEnumerator PHASETestChangeReverbOnListener()
        {
            PHASEListener listener = GameObject.Find("PHASEListener").GetComponent<PHASEListener>();
            GameObject sourceObject = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "Voice"));

            PHASESource source = sourceObject.GetComponent<PHASESource>();
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(source.GetInstanceID() != InvalidId, "Failed to create looping source");

            source.Play();
            yield return new WaitForSeconds(0.5f);
            Assert.That(source.IsPlaying(), "Failed to play looping source");

            listener.SetReverbPreset(Helpers.ReverbPresets.None);
            yield return new WaitForSeconds(0.2f);

            listener.SetReverbPreset(Helpers.ReverbPresets.Cathedral);
            yield return new WaitForSeconds(0.2f);

            listener.SetReverbPreset(Helpers.ReverbPresets.MediumRoom);
            yield return new WaitForSeconds(0.2f);

            Object.Destroy(sourceObject);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(sourceObject, "Failed to destroy PHASE Source");
        }
    }
}
