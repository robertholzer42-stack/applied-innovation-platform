using System;
using System.Collections.Generic;
using UnityEngine;
using Pridefall.Core;
using Pridefall.Player;

namespace Pridefall.Environment
{
    /// <summary>
    /// Periodic thermal vent. Cycles idle -> telegraph (rumble window; bind
    /// VFX/SFX to the events) -> eruption. While erupting it deals Thermal
    /// damage-per-second to anything damageable inside a capsule above the
    /// vent and lifts the player straight up, so geysers double as elevators,
    /// the Hubris-style traversal toy: time the ride, eat a little heat,
    /// gain a ledge.
    /// </summary>
    public class GeyserHazard : MonoBehaviour
    {
        private enum Phase { Idle, Telegraph, Erupting }

        [Header("Cycle")]
        [SerializeField] private float _idleSeconds = 6f;
        [Tooltip("Random extra idle per cycle so neighboring geysers desynchronize.")]
        [SerializeField] private float _idleJitterSeconds = 1.5f;
        [Tooltip("Rumble window before the blast; long enough to read and react.")]
        [SerializeField] private float _telegraphSeconds = 2f;
        [SerializeField] private float _eruptSeconds = 3f;

        [Header("Column")]
        [SerializeField] private float _columnHeight = 8f;
        [SerializeField] private float _columnRadius = 1.2f;
        [SerializeField] private LayerMask _affectedLayers = ~0;

        [Header("Damage")]
        [Tooltip("Thermal DPS inside the column. Kept survivable so a short elevator ride costs health, not a life.")]
        [SerializeField] private float _damagePerSecond = 12f;

        [Header("Launch")]
        [Tooltip("Upward speed applied to the player in the column. Tune against gravity (-8.83 * GravityScale) for elevator height.")]
        [SerializeField] private float _launchSpeed = 9f;

        [Header("Hooks")]
        [Tooltip("Enabled during the telegraph window (rumble VFX, steam wisps).")]
        [SerializeField] private GameObject _telegraphObject;
        [Tooltip("Enabled during the eruption (steam column VFX).")]
        [SerializeField] private GameObject _eruptionObject;

        /// <summary>Rumble window opened; bind SFX/haptics/particle bursts here.</summary>
        public event Action TelegraphStarted;
        /// <summary>Eruption began.</summary>
        public event Action EruptionStarted;
        /// <summary>Eruption finished; the vent goes quiet.</summary>
        public event Action EruptionEnded;

        public bool IsErupting => _phase == Phase.Erupting;

        private Phase _phase = Phase.Idle;
        private float _phaseTimer;
        private float _currentIdleSeconds;
        private PlayerLocomotionController _liftedPlayer;

        private readonly Collider[] _overlapBuffer = new Collider[16];
        private readonly HashSet<IDamageable> _damagedThisTick = new();

        private void OnEnable()
        {
            _phase = Phase.Idle;
            _phaseTimer = 0f;
            _currentIdleSeconds = _idleSeconds + UnityEngine.Random.Range(0f, _idleJitterSeconds);
            SetActiveSafe(_telegraphObject, false);
            SetActiveSafe(_eruptionObject, false);
        }

        private void OnDisable()
        {
            ReleaseLiftedPlayer();
            SetActiveSafe(_telegraphObject, false);
            SetActiveSafe(_eruptionObject, false);
        }

        private void Update()
        {
            _phaseTimer += Time.deltaTime;

            switch (_phase)
            {
                case Phase.Idle:
                    if (_phaseTimer >= _currentIdleSeconds) EnterTelegraph();
                    break;

                case Phase.Telegraph:
                    if (_phaseTimer >= _telegraphSeconds) EnterEruption();
                    break;

                case Phase.Erupting:
                    TickEruption();
                    if (_phaseTimer >= _eruptSeconds) EnterIdle();
                    break;
            }
        }

        private void EnterTelegraph()
        {
            _phase = Phase.Telegraph;
            _phaseTimer = 0f;
            SetActiveSafe(_telegraphObject, true);
            TelegraphStarted?.Invoke();
        }

        private void EnterEruption()
        {
            _phase = Phase.Erupting;
            _phaseTimer = 0f;
            SetActiveSafe(_telegraphObject, false);
            SetActiveSafe(_eruptionObject, true);
            EruptionStarted?.Invoke();
        }

        private void EnterIdle()
        {
            ReleaseLiftedPlayer();
            _phase = Phase.Idle;
            _phaseTimer = 0f;
            _currentIdleSeconds = _idleSeconds + UnityEngine.Random.Range(0f, _idleJitterSeconds);
            SetActiveSafe(_eruptionObject, false);
            EruptionEnded?.Invoke();
        }

        private void TickEruption()
        {
            Vector3 basePoint = transform.position + Vector3.up * _columnRadius;
            Vector3 topPoint = transform.position + Vector3.up * Mathf.Max(_columnHeight - _columnRadius, _columnRadius);
            int hits = Physics.OverlapCapsuleNonAlloc(basePoint, topPoint, _columnRadius,
                _overlapBuffer, _affectedLayers, QueryTriggerInteraction.Ignore);

            _damagedThisTick.Clear();
            bool playerInColumn = false;

            for (int i = 0; i < hits; i++)
            {
                Collider hit = _overlapBuffer[i];
                if (hit == null) continue;

                var damageable = hit.GetComponentInParent<IDamageable>();
                if (damageable != null && damageable.IsAlive && _damagedThisTick.Add(damageable))
                {
                    damageable.TakeDamage(new DamageInfo(_damagePerSecond * Time.deltaTime, DamageType.Thermal,
                        hit.transform.position, Vector3.up, gameObject));
                }

                var locomotion = hit.GetComponentInParent<PlayerLocomotionController>();
                if (locomotion != null)
                {
                    playerInColumn = true;
                    _liftedPlayer = locomotion;
                    // External control moves the capsule up immediately and clears
                    // isGrounded; the ballistic handoff happens on release.
                    locomotion.SetExternalVelocity(Vector3.up * _launchSpeed, MovementState.Airborne);
                }
            }

            if (!playerInColumn) ReleaseLiftedPlayer();
        }

        private void ReleaseLiftedPlayer()
        {
            if (_liftedPlayer == null) return;
            // Hand the body back with the full upward velocity inherited; this
            // ballistic exit is what turns the vent into an elevator.
            _liftedPlayer.ReleaseExternalControl(Vector3.up * _launchSpeed);
            _liftedPlayer = null;
        }

        private static void SetActiveSafe(GameObject target, bool active)
        {
            if (target != null && target.activeSelf != active) target.SetActive(active);
        }
    }
}
