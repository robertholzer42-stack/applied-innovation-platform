using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using VRForge.Core;

namespace VRForge.Tests
{
    /// <summary>
    /// Edit mode unit tests for the pure logic pieces of the starter core:
    /// DamageInfo math and the pool's live-dequeue behavior. No scene, no
    /// play mode, so these run in seconds via scripts/run-tests.sh.
    /// </summary>
    public class EditModeTests
    {
        [Test]
        public void DamageInfo_StoresConstructorValues()
        {
            var source = new GameObject("DamageSource");
            try
            {
                var info = new DamageInfo(DamageType.Fire, 25f, Vector3.up, Vector3.forward, source);

                Assert.AreEqual(DamageType.Fire, info.Type);
                Assert.AreEqual(25f, info.Amount);
                Assert.AreEqual(Vector3.up, info.Point);
                Assert.AreEqual(Vector3.forward, info.Direction);
                Assert.AreSame(source, info.Source);
            }
            finally
            {
                Object.DestroyImmediate(source);
            }
        }

        [Test]
        public void DamageInfo_ConvenienceConstructor_DefaultsPositionalFields()
        {
            var info = new DamageInfo(DamageType.Impact, 10f);

            Assert.AreEqual(Vector3.zero, info.Point);
            Assert.AreEqual(Vector3.zero, info.Direction);
            Assert.IsNull(info.Source);
        }

        [Test]
        public void AmountAfterResistance_ZeroResistance_ReturnsFullAmount()
        {
            var info = new DamageInfo(DamageType.Generic, 40f);

            Assert.AreEqual(40f, info.AmountAfterResistance(0f), 0.0001f);
        }

        [Test]
        public void AmountAfterResistance_HalfResistance_ReturnsHalf()
        {
            var info = new DamageInfo(DamageType.Generic, 40f);

            Assert.AreEqual(20f, info.AmountAfterResistance(0.5f), 0.0001f);
        }

        [Test]
        public void AmountAfterResistance_ResistanceAboveOne_ClampsToZeroDamage()
        {
            var info = new DamageInfo(DamageType.Generic, 40f);

            Assert.AreEqual(0f, info.AmountAfterResistance(3f), 0.0001f);
        }

        [Test]
        public void AmountAfterResistance_NegativeResistance_ClampsToFullAmount()
        {
            var info = new DamageInfo(DamageType.Generic, 40f);

            Assert.AreEqual(40f, info.AmountAfterResistance(-2f), 0.0001f);
        }

        [Test]
        public void Scaled_MultipliesAmountAndPreservesOtherFields()
        {
            var info = new DamageInfo(DamageType.Energy, 10f, Vector3.one, Vector3.back, null);

            var scaled = info.Scaled(2.5f);

            Assert.AreEqual(25f, scaled.Amount, 0.0001f);
            Assert.AreEqual(DamageType.Energy, scaled.Type);
            Assert.AreEqual(Vector3.one, scaled.Point);
            Assert.AreEqual(Vector3.back, scaled.Direction);
        }

        [Test]
        public void Scaled_NegativeMultiplier_ClampsAmountToZero()
        {
            var info = new DamageInfo(DamageType.Energy, 10f);

            Assert.AreEqual(0f, info.Scaled(-1f).Amount, 0.0001f);
        }

        [Test]
        public void DequeueLive_NullQueue_ReturnsNull()
        {
            Assert.IsNull(ObjectPool.DequeueLive(null));
        }

        [Test]
        public void DequeueLive_EmptyQueue_ReturnsNull()
        {
            Assert.IsNull(ObjectPool.DequeueLive(new Queue<GameObject>()));
        }

        [Test]
        public void DequeueLive_SkipsNullAndDestroyedEntries_ReturnsFirstLive()
        {
            var destroyed = new GameObject("Destroyed");
            Object.DestroyImmediate(destroyed);
            var live = new GameObject("Live");
            try
            {
                var queue = new Queue<GameObject>();
                queue.Enqueue(null);
                queue.Enqueue(destroyed);
                queue.Enqueue(live);

                Assert.AreSame(live, ObjectPool.DequeueLive(queue));
                Assert.AreEqual(0, queue.Count, "Dead entries should have been consumed.");
            }
            finally
            {
                Object.DestroyImmediate(live);
            }
        }
    }
}
