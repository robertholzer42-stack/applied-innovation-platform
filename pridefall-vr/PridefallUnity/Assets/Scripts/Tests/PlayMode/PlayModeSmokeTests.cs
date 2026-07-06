using System;
using System.Collections;
using NUnit.Framework;
using Pridefall.Core;
using Pridefall.Player;
using UnityEngine;
using UnityEngine.TestTools;

namespace Pridefall.Tests
{
    /// <summary>
    /// Play mode smoke suite for PRIDEFALL's runtime core. Everything is
    /// built from code at runtime (pool, health carrier), so the suite is
    /// scene-independent and needs no XR rig: it runs headless in batchmode
    /// via scripts/run-tests.sh.
    /// </summary>
    public class PlayModeSmokeTests
    {
        // PlayerHealth serialized defaults: 3 segments of 35 capacity each.
        private const float SegmentCapacity = 35f;

        private bool _playerDied;
        private Action<Vector3> _diedHandler;

        [SetUp]
        public void SetUp()
        {
            _playerDied = false;
            _diedHandler = OnPlayerDied;
        }

        /// <summary>
        /// GameEvents is a static bus; unsubscribe in teardown so a failed
        /// test never leaks handlers into later tests.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            GameEvents.PlayerDied -= _diedHandler;
        }

        private void OnPlayerDied(Vector3 position) => _playerDied = true;

        [UnityTest]
        public IEnumerator ObjectPool_SpawnDespawnRoundTrip_ReusesInstances()
        {
            var poolObject = new GameObject("TestObjectPool");
            var pool = poolObject.AddComponent<ObjectPool>();
            var prefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prefab.name = "TestPoolPrefab";
            prefab.SetActive(false);
            yield return null; // let Awake run and the singleton settle

            GameObject first = pool.Spawn(prefab, Vector3.one, Quaternion.identity);
            Assert.IsNotNull(first, "Spawn returned null for a valid prefab.");
            Assert.IsTrue(first.activeSelf, "Spawned instance should be active.");
            Assert.AreEqual(Vector3.one, first.transform.position);

            pool.Despawn(first);
            Assert.IsFalse(first.activeSelf, "Despawned instance should be inactive.");

            GameObject second = pool.Spawn(prefab, Vector3.zero, Quaternion.identity);
            Assert.AreSame(first, second, "Pool should reuse the despawned instance instead of instantiating.");
            Assert.AreEqual(Vector3.zero, second.transform.position);

            UnityEngine.Object.Destroy(poolObject); // pooled instances are children, destroyed with it
            UnityEngine.Object.Destroy(prefab);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ObjectPool_DespawnOfForeignObject_DestroysItAsFailSafe()
        {
            var poolObject = new GameObject("TestObjectPool");
            var pool = poolObject.AddComponent<ObjectPool>();
            var foreign = new GameObject("NotPooled");
            yield return null;

            pool.Despawn(foreign);
            yield return null; // Destroy takes effect at end of frame

            Assert.IsTrue(foreign == null, "Despawn of a non-pooled object should destroy it.");

            UnityEngine.Object.Destroy(poolObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayerHealth_ExactSegmentDamage_ConsumesOneFullSegment()
        {
            var carrier = new GameObject("TestPlayerHealth");
            var health = carrier.AddComponent<PlayerHealth>();
            yield return null; // Awake: 2 full segments + active fill 1.0

            Assert.IsTrue(health.IsAlive);
            Assert.AreEqual(2, health.FullSegments, "Default rig starts with maxSegments - 1 full segments.");
            Assert.AreEqual(1f, health.ActiveSegmentFill, 0.0001f);

            health.TakeDamage(new DamageInfo(SegmentCapacity, DamageType.Kinetic, Vector3.zero, Vector3.forward, null));

            Assert.IsTrue(health.IsAlive);
            Assert.AreEqual(1, health.FullSegments, "One full segment should be consumed by exactly one segment of damage.");
            Assert.AreEqual(1f, health.ActiveSegmentFill, 0.0001f, "Active fill should roll over to a fresh segment.");

            UnityEngine.Object.Destroy(carrier);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayerHealth_PartialDamage_OnlyDrainsActiveSegment()
        {
            var carrier = new GameObject("TestPlayerHealth");
            var health = carrier.AddComponent<PlayerHealth>();
            yield return null;

            health.TakeDamage(new DamageInfo(SegmentCapacity * 0.5f, DamageType.Energy, Vector3.zero, Vector3.forward, null));

            Assert.AreEqual(2, health.FullSegments, "Partial damage must not consume a full segment.");
            Assert.AreEqual(0.5f, health.ActiveSegmentFill, 0.01f);

            UnityEngine.Object.Destroy(carrier);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayerHealth_Medgel_RefillsActiveSegmentThenRefusesAtFull()
        {
            var carrier = new GameObject("TestPlayerHealth");
            var health = carrier.AddComponent<PlayerHealth>();
            yield return null;

            Assert.IsFalse(health.ApplyMedgel(), "Medgel at full health should be refused.");

            health.TakeDamage(new DamageInfo(SegmentCapacity * 0.5f, DamageType.Corrosive, Vector3.zero, Vector3.forward, null));
            Assert.IsTrue(health.ApplyMedgel(), "Medgel should apply when the active segment is drained.");
            Assert.AreEqual(1f, health.ActiveSegmentFill, 0.0001f, "Medgel should top the active segment back to full.");
            Assert.AreEqual(2, health.FullSegments);

            UnityEngine.Object.Destroy(carrier);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayerHealth_OverkillDamage_KillsAndRaisesPlayerDied()
        {
            GameEvents.PlayerDied += _diedHandler;
            var carrier = new GameObject("TestPlayerHealth");
            var health = carrier.AddComponent<PlayerHealth>();
            yield return null;

            // Total capacity is 3 segments * 35 = 105; 200 is unambiguous overkill.
            health.TakeDamage(new DamageInfo(200f, DamageType.Thermal, Vector3.zero, Vector3.forward, null));

            Assert.IsFalse(health.IsAlive, "Overkill damage should kill the player.");
            Assert.AreEqual(0, health.FullSegments);
            Assert.AreEqual(0f, health.ActiveSegmentFill, 0.0001f);
            Assert.IsTrue(_playerDied, "Death should raise GameEvents.PlayerDied.");

            UnityEngine.Object.Destroy(carrier);
            yield return null;
        }
    }
}
