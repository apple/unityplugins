using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine;

namespace Apple.PHASE.UnitTests
{
    public class PHASETests
    {
        static long InvalidId = -1;

        static double InvalidGain = -1.0f;

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
        public IEnumerator PHASETestSetInvalidGainOnSource()
        {
            GameObject sourceObject = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "Voice"));

            PHASESource source = sourceObject.GetComponent<PHASESource>();
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(source.GetInstanceID() != InvalidId, "Failed to create looping source");

            source.Play();
            yield return new WaitForSeconds(0.75f);
            Assert.That(source.IsPlaying(), "Failed to play looping source");

            // Try to set the source gain to a value outside the valid range
            source.SetGain(InvalidGain);
            yield return new WaitForSeconds(0.2f);
            Assert.IsTrue(source.GetGain() != InvalidGain, "Gain of source was set to value out of range of [0,1]");

            source.Stop();
            yield return new WaitForSeconds(0.1f);
            Assert.That(!source.IsPlaying(), "Failed to stop source");

            Object.Destroy(sourceObject);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(sourceObject, "Failed to destroy PHASE Source");
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

        [UnityTest]
        public IEnumerator PHASETestSetInvalidGainOnListener()
        {
            PHASEListener listener = GameObject.Find("PHASEListener").GetComponent<PHASEListener>();

            GameObject sourceObject = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "Voice"));

            PHASESource source = sourceObject.GetComponent<PHASESource>();
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(source.GetInstanceID() != InvalidId, "Failed to create looping source");

            source.Play();
            yield return new WaitForSeconds(0.75f);
            Assert.That(source.IsPlaying(), "Failed to play looping source");

            // Try to set the listener gain to a value outside the valid range
            listener.SetGain(InvalidGain);
            yield return new WaitForSeconds(0.2f);
            Assert.IsTrue(listener.GetGain() != InvalidGain, "Gain of listener was set to value out of range of [0,1]");

            source.Stop();
            yield return new WaitForSeconds(0.1f);
            Assert.That(!source.IsPlaying(), "Failed to stop source");

            Object.Destroy(sourceObject);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(sourceObject, "Failed to destroy PHASE Source");
        }

        // Spatializer Tests
        [UnityTest]
        public IEnumerator PHASETestSpatializer()
        {
            GameObject spatializedSource =
                Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "SpatializedSource"));

            Assert.That(spatializedSource, "Failed to instantiate Spatialized Source.");

            AudioSource source = spatializedSource.GetComponent<AudioSource>();
            yield return new WaitForFixedUpdate();
            source.Play();
            yield return new WaitForSeconds(1f);
            Assert.That(source.isPlaying, "Spatialized source failed to play.");
            source.Stop();
            yield return new WaitForSeconds(0.01f);
            Assert.That(!source.isPlaying, "Failed to stop spatialized AudioSource.");

            Object.Destroy(source);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(source, "Failed to destroy PHASE Source");
        }

        [UnityTest]
        public IEnumerator PHASETestSpatializerRestart()
        {
            GameObject spatializedSource =
                Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "SpatializedSource"));

            Assert.That(spatializedSource, "Failed to instantiate Spatialized Source.");

            AudioSource source = spatializedSource.GetComponent<AudioSource>();
            yield return new WaitForFixedUpdate();
            source.Play();
            yield return new WaitForSeconds(1f);
            Assert.That(source.isPlaying, "Spatialized source failed to play.");
            source.Stop();
            yield return new WaitForSeconds(0.01f);
            Assert.That(!source.isPlaying, "Failed to stop spatialized AudioSource.");
            source.Play();
            yield return new WaitForSeconds(1f);
            Assert.That(source.isPlaying, "Spatialized source failed to play 2nd time.");
            source.Stop();
            yield return new WaitForSeconds(0.01f);
            Assert.That(!source.isPlaying, "Failed to stop spatialized AudioSource.");

            Object.Destroy(source);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(source, "Failed to destroy PHASE Source");
        }

        [UnityTest]
        public IEnumerator PHASETestTwoSpatializedSources()
        {
            GameObject spatializedSource1 =
                Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "SpatializedSource"));
            GameObject spatializedSource2 =
                Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "SpatializedSource"));

            Assert.That(spatializedSource1, "Failed to instantiate Spatialized Source.");
            Assert.That(spatializedSource2, "Failed to instantiate Spatialized Source.");

            AudioSource source1 = spatializedSource1.GetComponent<AudioSource>();
            AudioSource source2 = spatializedSource2.GetComponent<AudioSource>();
            yield return new WaitForFixedUpdate();
            source1.Play();
            yield return new WaitForSeconds(1f);
            Assert.That(source1.isPlaying, "Spatialized source failed to play.");
            source2.Play();
            yield return new WaitForSeconds(1f);
            Assert.That(source2.isPlaying, "Spatialized source failed to play.");
            source1.Stop();
            yield return new WaitForSeconds(0.01f);
            Assert.That(!source1.isPlaying, "Failed to stop spatialized AudioSource.");
            source2.Stop();
            yield return new WaitForSeconds(0.01f);
            Assert.That(!source2.isPlaying, "Failed to stop spatialized AudioSource.");

            Object.Destroy(source1);
            yield return new WaitForFixedUpdate();

            Object.Destroy(source2);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(source2, "Failed to destroy PHASE Source");
        }

        [UnityTest]
        public IEnumerator PHASETestSourceSpatializeWhilePlaying()
        {
            GameObject spatializedSource =
                Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "SpatializedSource"));

            Assert.That(spatializedSource, "Failed to instantiate Spatialized Source.");

            AudioSource source = spatializedSource.GetComponent<AudioSource>();
            source.spatialize = false;
            yield return new WaitForFixedUpdate();
            source.Play();
            yield return new WaitForSeconds(1f);
            source.spatialize = true;
            yield return new WaitForSeconds(1f);
            Assert.That(source.isPlaying, "Spatialized source failed to play.");
            source.Stop();
            yield return new WaitForSeconds(0.01f);
            Assert.That(!source.isPlaying, "Failed to stop spatialized AudioSource.");

            Object.Destroy(source);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(source, "Failed to destroy PHASE Source");
        }

        [UnityTest]
        public IEnumerator PHASETestSpatializedSourceDestroyAndReload()
        {
            GameObject spatializedSource =
                Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "SpatializedSource"));

            Assert.That(spatializedSource, "Failed to instantiate Spatialized Source.");

            AudioSource source = spatializedSource.GetComponent<AudioSource>();
            yield return new WaitForFixedUpdate();
            source.Play();
            yield return new WaitForSeconds(1f);
            Assert.That(source.isPlaying, "Spatialized source failed to play.");
            source.Stop();
            yield return new WaitForSeconds(0.01f);
            Assert.That(!source.isPlaying, "Failed to stop spatialized AudioSource.");

            Object.Destroy(source);
            yield return new WaitForFixedUpdate();

            spatializedSource = Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "SpatializedSource"));
            Assert.That(spatializedSource, "Failed to re-instantiate Spatialized Source.");

            source = spatializedSource.GetComponent<AudioSource>();
            yield return new WaitForFixedUpdate();
            source.Play();
            yield return new WaitForSeconds(1f);
            Assert.That(source.isPlaying, "Spatialized source failed to play.");
            source.Stop();
            yield return new WaitForSeconds(0.01f);
            Assert.That(!source.isPlaying, "Failed to stop spatialized AudioSource.");

            Object.Destroy(source);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(source, "Failed to destroy AudioSource");
        }

        [UnityTest]
        public IEnumerator PHASETestSpatializerOneShots()
        {
            GameObject spatializedSource =
                Object.Instantiate(Resources.Load<GameObject>(m_testResourcesDir + "SpatializedSource"));

            Assert.That(spatializedSource, "Failed to instantiate Spatialized Source.");

            AudioSource source = spatializedSource.GetComponent<AudioSource>();
            yield return new WaitForFixedUpdate();
            AudioClip testClip = Resources.Load<AudioClip>(m_testResourcesDir + "Wood_creak_loop");
            Assert.That(testClip, "Failed to load test audio file.");
            source.PlayOneShot(testClip);
            yield return new WaitForFixedUpdate();
            Assert.That(source.isPlaying, "Failed to play OneShot spatialized AudioSource.");
            yield return new WaitForSeconds(1f);

            source.spatialize = false;
            source.PlayOneShot(testClip);
            yield return new WaitForFixedUpdate();
            Assert.That(source.isPlaying, "Failed to play OneShot AudioSource.");
            yield return new WaitForSeconds(1f);

            Object.Destroy(source);
            yield return new WaitForFixedUpdate();
            UnityEngine.Assertions.Assert.IsNull(source, "Failed to destroy PHASE Source");
        }
    }
}
