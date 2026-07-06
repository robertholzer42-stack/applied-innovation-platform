using System;
using UnityEngine;
using VRForge.Core;

namespace VRForge.Player
{
    /// <summary>
    /// Player health with delayed regeneration. Damage lands through
    /// <see cref="IDamageable"/>; every change is published on
    /// <see cref="GameEvents.PlayerHealthChanged"/> so UI and audio never
    /// need a direct reference. Lives on the XR Origin.
    /// </summary>
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private float maxHealth = 100f;

        [SerializeField]
        [Tooltip("Seconds after the last hit before regeneration starts.")]
        private float regenDelay = 4f;

        [SerializeField]
        [Tooltip("Health restored per second once regeneration starts.")]
        private float regenPerSecond = 10f;

        private float _lastDamageTime = float.NegativeInfinity;

        /// <summary>Current health, 0..maxHealth.</summary>
        public float Current { get; private set; }

        /// <summary>Configured maximum health.</summary>
        public float Max => maxHealth;

        /// <summary>True once health has reached zero; regeneration stops.</summary>
        public bool IsDead => Current <= 0f;

        /// <summary>Raised once when health first reaches zero.</summary>
        public event Action Died;

        private void Awake()
        {
            maxHealth = Mathf.Max(1f, maxHealth);
            Current = maxHealth;
        }

        private void Start()
        {
            GameEvents.RaisePlayerHealthChanged(Current, maxHealth);
        }

        private void Update()
        {
            if (IsDead || Current >= maxHealth) return;
            if (Time.time - _lastDamageTime < regenDelay) return;

            Current = Mathf.Min(maxHealth, Current + regenPerSecond * Time.deltaTime);
            GameEvents.RaisePlayerHealthChanged(Current, maxHealth);
        }

        /// <inheritdoc />
        public void TakeDamage(in DamageInfo info)
        {
            if (IsDead) return;
            if (info.Amount <= 0f) return;

            _lastDamageTime = Time.time;
            Current = Mathf.Max(0f, Current - info.Amount);
            GameEvents.RaisePlayerHealthChanged(Current, maxHealth);

            if (IsDead)
            {
                Died?.Invoke();
            }
        }

        /// <summary>Restore health (pickups, checkpoints). Ignored while dead.</summary>
        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f) return;

            Current = Mathf.Min(maxHealth, Current + amount);
            GameEvents.RaisePlayerHealthChanged(Current, maxHealth);
        }
    }
}
