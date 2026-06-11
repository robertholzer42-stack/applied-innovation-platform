using System;
using UnityEngine;
using Pridefall.Player;

namespace Pridefall.Environment
{
    /// <summary>
    /// Underwater air refill station: a sea-floor fissure streaming
    /// breathable bubbles. Staying in the stream tops up the air meter,
    /// throttled so the refill pulse (and its bubble-burst VFX) fires at a
    /// readable cadence rather than every physics tick.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class BubbleVent : MonoBehaviour
    {
        [Tooltip("Seconds between refill pulses while the player stays in the stream.")]
        [SerializeField] private float _refillInterval = 0.75f;

        /// <summary>A refill pulse landed; bind bubble-burst VFX/SFX here.</summary>
        public event Action AirDelivered;

        private float _nextRefillTime;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (Time.time < _nextRefillTime) return;

            var swimmer = other.GetComponentInParent<SwimmingSystem>();
            if (swimmer == null) return;

            _nextRefillTime = Time.time + _refillInterval;
            swimmer.RefillAir();
            AirDelivered?.Invoke();
        }
    }
}
