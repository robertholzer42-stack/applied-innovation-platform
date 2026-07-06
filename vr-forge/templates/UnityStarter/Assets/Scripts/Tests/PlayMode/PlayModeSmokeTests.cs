using System.Collections;
using NUnit.Framework;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VRForge.Core;

namespace VRForge.Tests
{
    /// <summary>
    /// Play mode smoke suite: the starter scene loads, exactly one XR rig
    /// exists, nothing logs an error in the first two seconds, and the
    /// object pool round-trips instances. Scene-dependent tests self-ignore
    /// (with instructions) until a scene is added to Build Settings, so the
    /// suite is green on a freshly scaffolded project.
    /// </summary>
    public class PlayModeSmokeTests
    {
        private static bool HasBuildScene()
        {
            return SceneManager.sceneCountInBuildSettings > 0;
        }

        [UnityTest]
        public IEnumerator FirstBuildSceneLoads()
        {
            if (!HasBuildScene())
            {
                Assert.Ignore("No scenes in Build Settings yet. Add the starter scene (see README-starter.md).");
            }

            yield return SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);

            Assert.IsTrue(SceneManager.GetActiveScene().isLoaded, "Scene at build index 0 failed to load.");
        }

        [UnityTest]
        public IEnumerator ExactlyOneXrRigInStarterScene()
        {
            if (!HasBuildScene())
            {
                Assert.Ignore("No scenes in Build Settings yet. Add the starter scene (see README-starter.md).");
            }

            yield return SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);

            XROrigin[] rigs = Object.FindObjectsByType<XROrigin>(FindObjectsSortMode.None);
            Assert.AreEqual(1, rigs.Length, $"Expected exactly one XROrigin in the starter scene, found {rigs.Length}.");
        }

        [UnityTest]
        public IEnumerator NoConsoleErrorsInFirstTwoSeconds()
        {
            if (HasBuildScene())
            {
                yield return SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
            }

            float elapsed = 0f;
            while (elapsed < 2f)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator ObjectPoolRoundTripReusesInstances()
        {
            var poolObject = new GameObject("TestObjectPool", typeof(ObjectPool));
            var prefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prefab.name = "TestPoolPrefab";
            prefab.SetActive(false);
            yield return null; // let Awake assign the singleton

            ObjectPool pool = ObjectPool.Instance;
            Assert.IsNotNull(pool, "ObjectPool.Instance was not assigned after Awake.");

            GameObject first = pool.Spawn(prefab, Vector3.one, Quaternion.identity);
            Assert.IsNotNull(first, "Spawn returned null for a valid prefab.");
            Assert.IsTrue(first.activeSelf, "Spawned instance should be active.");
            Assert.AreEqual(Vector3.one, first.transform.position);

            pool.Despawn(first);
            Assert.IsFalse(first.activeSelf, "Despawned instance should be inactive.");

            GameObject second = pool.Spawn(prefab, Vector3.zero, Quaternion.identity);
            Assert.AreSame(first, second, "Pool should reuse the despawned instance instead of instantiating.");
            Assert.AreEqual(Vector3.zero, second.transform.position);

            Object.Destroy(poolObject); // pooled instances are children, destroyed with it
            Object.Destroy(prefab);
            yield return null;
        }
    }
}
