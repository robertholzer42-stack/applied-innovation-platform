using System;
using NUnit.Framework;
using Pridefall.Core;
using UnityEngine;

namespace Pridefall.Tests
{
    /// <summary>
    /// Edit mode unit tests for PRIDEFALL's pure-logic core: DamageInfo
    /// field mapping, the DamageType enum contract, ComfortSettings
    /// defaults, and the GameEvents static bus round-trip. No scene, no
    /// play mode, so these run in seconds via scripts/run-tests.sh.
    /// </summary>
    public class EditModeTests
    {
        private int _healthEventCount;
        private string _lastObjective;
        private Action<int, int, float> _healthHandler;
        private Action<string> _objectiveHandler;

        [SetUp]
        public void SetUp()
        {
            _healthEventCount = 0;
            _lastObjective = null;
            _healthHandler = OnHealthChanged;
            _objectiveHandler = OnObjectiveUpdated;
        }

        /// <summary>
        /// GameEvents is a static bus shared across the whole test session;
        /// always unsubscribe so a failed test cannot leak handlers into
        /// later tests or into play mode runs.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            GameEvents.PlayerHealthChanged -= _healthHandler;
            GameEvents.ObjectiveUpdated -= _objectiveHandler;
        }

        private void OnHealthChanged(int current, int max, float fill) => _healthEventCount++;

        private void OnObjectiveUpdated(string text) => _lastObjective = text;

        [Test]
        public void DamageInfo_StoresConstructorValues()
        {
            var source = new GameObject("DamageSource");
            try
            {
                var info = new DamageInfo(25f, DamageType.Thermal, Vector3.up, Vector3.forward, source);

                Assert.AreEqual(25f, info.Amount);
                Assert.AreEqual(DamageType.Thermal, info.Type);
                Assert.AreEqual(Vector3.up, info.Point);
                Assert.AreEqual(Vector3.forward, info.Direction);
                Assert.AreSame(source, info.Source);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(source);
            }
        }

        [Test]
        public void DamageInfo_AllowsNullSource()
        {
            var info = new DamageInfo(10f, DamageType.Kinetic, Vector3.zero, Vector3.zero, null);

            Assert.IsNull(info.Source);
            Assert.AreEqual(DamageType.Kinetic, info.Type);
        }

        [Test]
        public void DamageType_CoversExactlyTheFourGameDamageKinds()
        {
            var names = Enum.GetNames(typeof(DamageType));

            Assert.AreEqual(4, names.Length, "DamageType gained or lost a member; update weapons, enemies, and this test together.");
            CollectionAssert.AreEquivalent(
                new[] { "Kinetic", "Energy", "Thermal", "Corrosive" },
                names);
        }

        [Test]
        public void ComfortSettings_DefaultsMatchOmniOneShippingConfig()
        {
            var settings = new ComfortSettings();

            Assert.AreEqual(VignetteStrength.Light, settings.Vignette, "Vignette should default to Light.");
            Assert.IsFalse(settings.ThumbstickLocomotionFallback, "Thumbstick fallback should be off by default on Omni One.");
            Assert.IsFalse(settings.SnapTurnFallback, "Snap turn fallback should be off by default on Omni One.");
            Assert.AreEqual(1.25f, settings.MovementGain, 0.0001f, "MovementGain should default to 1.25.");
        }

        [Test]
        public void GameEvents_SubscribedHandlerReceivesRaisedValues()
        {
            GameEvents.ObjectiveUpdated += _objectiveHandler;

            GameEvents.RaiseObjectiveUpdated("Reach the fabricator");

            Assert.AreEqual("Reach the fabricator", _lastObjective);
        }

        [Test]
        public void GameEvents_HealthHandlerFiresOncePerRaise()
        {
            GameEvents.PlayerHealthChanged += _healthHandler;

            GameEvents.RaisePlayerHealthChanged(2, 3, 0.5f);
            GameEvents.RaisePlayerHealthChanged(1, 3, 1f);

            Assert.AreEqual(2, _healthEventCount);
        }

        [Test]
        public void GameEvents_UnsubscribedHandlerStopsReceiving()
        {
            GameEvents.PlayerHealthChanged += _healthHandler;
            GameEvents.RaisePlayerHealthChanged(2, 3, 1f);
            Assert.AreEqual(1, _healthEventCount, "Sanity: handler should fire while subscribed.");

            GameEvents.PlayerHealthChanged -= _healthHandler;
            GameEvents.RaisePlayerHealthChanged(0, 3, 0f);

            Assert.AreEqual(1, _healthEventCount, "Handler fired after unsubscribe; the bus leaked a reference.");
        }

        [Test]
        public void GameEvents_RaiseWithNoSubscribersDoesNotThrow()
        {
            Assert.DoesNotThrow(() => GameEvents.RaiseCombatIntensityChanged(0.75f));
            Assert.DoesNotThrow(() => GameEvents.RaisePlayerSubmergedChanged(true));
        }
    }
}
