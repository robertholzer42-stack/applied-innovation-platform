using UnityEngine;
using Pridefall.Core;
using Pridefall.Interaction;
using Pridefall.Player;

namespace Pridefall.Weapons
{
    /// <summary>
    /// Base for all Pathfinder ranged weapons. Owns the shared firing loop:
    /// poll the holding hand's trigger, gate by fire rate (semi-auto requires
    /// a trigger re-press, detected as an edge on TriggerHeld), spend cell
    /// charge, kick recoil, flash the muzzle. Reloads are physical: Eject()
    /// pops the loaded cell as a Grabbable, and pushing a fresh cell into the
    /// weapon's port trigger volume seats it. Subclasses only decide what
    /// leaves the barrel via Fire().
    /// </summary>
    public abstract class WeaponBase : Grabbable, IHolsterable
    {
        [Header("Firing")]
        [SerializeField] private float _shotsPerSecond = 4f;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private GameObject _muzzleFlashPrefab;
        [SerializeField] private float _muzzleFlashDuration = 0.06f;

        [Header("Recoil")]
        [Tooltip("Upward pitch kick per shot, degrees of local rotation. Recovers while held.")]
        [SerializeField] private float _recoilKickDegrees = 4f;
        [SerializeField] private float _recoilRecoverDegreesPerSecond = 40f;
        [SerializeField] private float _maxRecoilDegrees = 20f;

        [Header("Cell Port")]
        [Tooltip("Seat point for energy cells. The weapon needs a small trigger collider around this point; cells pushed inside it are inserted. Leave null for weapons that do not feed from cells.")]
        [SerializeField] private Transform _cellPort;
        [Tooltip("Max distance from the port for a cell touching the weapon's trigger volume to seat.")]
        [SerializeField] private float _cellPortRadius = 0.08f;
        [SerializeField] private float _cellEjectImpulse = 1.4f;
        [Tooltip("Optional cell already seated at the port on spawn (assign a scene/prefab child instance).")]
        [SerializeField] private EnergyCell _preloadedCell;

        public EnergyCell LoadedCell { get; private set; }
        public Transform Muzzle => _muzzle != null ? _muzzle : transform;

        /// <summary>Which HolsterSlot type stows this weapon.</summary>
        public abstract HolsterItemType HolsterType { get; }

        protected virtual bool IsAutomatic => false;
        protected virtual float RecoilMultiplier => 1f;
        protected virtual bool HasAmmo => LoadedCell != null && !LoadedCell.IsEmpty;

        private float _nextFireTime;
        private bool _triggerWasHeld;
        private float _recoilPitch;
        private Quaternion _heldBaseLocalRotation = Quaternion.identity;
        private float _lastEjectTime = -10f;

        protected virtual void Start()
        {
            if (_preloadedCell != null && LoadedCell == null && _cellPort != null && !_preloadedCell.IsEmpty)
            {
                LoadedCell = _preloadedCell;
                _preloadedCell.OnInserted(_cellPort);
            }
        }

        public override void OnGrabbed(HandController hand)
        {
            base.OnGrabbed(hand);
            _heldBaseLocalRotation = transform.localRotation;
            _recoilPitch = 0f;
            _triggerWasHeld = hand.TriggerHeld; // grabbing mid-squeeze must not fire semi-autos
        }

        private void Update()
        {
            if (!IsHeld || Holder == null) return;

            bool trigger = Holder.TriggerHeld;
            bool wantsFire = trigger && (IsAutomatic || !_triggerWasHeld);
            _triggerWasHeld = trigger;

            if (wantsFire && Time.time >= _nextFireTime)
            {
                if (HasAmmo)
                {
                    _nextFireTime = Time.time + 1f / Mathf.Max(0.01f, _shotsPerSecond);
                    ConsumeAmmo();
                    Fire();
                    _recoilPitch = Mathf.Min(_recoilPitch + _recoilKickDegrees * RecoilMultiplier, _maxRecoilDegrees);
                    SpawnMuzzleFlash();
                }
                else
                {
                    _nextFireTime = Time.time + 0.25f; // dry-fire debounce
                    OnDryFire();
                }
            }

            if (_recoilPitch > 0f)
            {
                _recoilPitch = Mathf.MoveTowards(_recoilPitch, 0f, _recoilRecoverDegreesPerSecond * Time.deltaTime);
            }
            transform.localRotation = _heldBaseLocalRotation * Quaternion.Euler(-_recoilPitch, 0f, 0f);
        }

        /// <summary>Spawn whatever this weapon shoots. Ammo, rate, recoil, and flash are already handled.</summary>
        protected abstract void Fire();

        /// <summary>Trigger pulled with no ammo. Default is a silent click.</summary>
        protected virtual void OnDryFire() { }

        protected virtual void ConsumeAmmo()
        {
            if (LoadedCell != null) LoadedCell.Consume();
        }

        /// <summary>Seats a cell in the port. Force-releases it from a hand if needed.</summary>
        public virtual bool TryInsertCell(EnergyCell cell)
        {
            if (cell == null || LoadedCell != null || _cellPort == null) return false;
            if (cell.IsEmpty || cell.IsInserted) return false;
            if (Time.time - _lastEjectTime < 0.4f) return false; // don't swallow the cell we just ejected

            if (cell.IsHeld) cell.Holder.ForceRelease();
            LoadedCell = cell;
            cell.OnInserted(_cellPort);
            return true;
        }

        /// <summary>Pops the loaded cell out of the port as a free physics Grabbable.</summary>
        public virtual void Eject()
        {
            if (LoadedCell == null) return;

            var cell = LoadedCell;
            LoadedCell = null;
            _lastEjectTime = Time.time;
            Transform port = _cellPort != null ? _cellPort : transform;
            cell.OnEjected(port.forward * _cellEjectImpulse);
        }

        private void OnTriggerStay(Collider other)
        {
            if (LoadedCell != null || _cellPort == null) return;

            var cell = other.GetComponentInParent<EnergyCell>();
            if (cell == null) return;

            // The weapon's rigidbody may carry other trigger volumes; gate inserts by port proximity.
            if ((other.ClosestPoint(_cellPort.position) - _cellPort.position).sqrMagnitude > _cellPortRadius * _cellPortRadius) return;

            TryInsertCell(cell);
        }

        /// <summary>Pool-spawns a projectile at the muzzle. Direction must be normalized.</summary>
        protected Projectile SpawnProjectile(Projectile prefab, Vector3 direction, float speed, float damage, DamageType type, GameObject impactPrefab)
        {
            if (prefab == null || ObjectPool.Instance == null) return null;

            var instance = ObjectPool.Instance.Spawn(prefab.gameObject, Muzzle.position, Quaternion.LookRotation(direction));
            var projectile = instance.GetComponent<Projectile>();
            if (projectile == null)
            {
                ObjectPool.Instance.Despawn(instance);
                return null;
            }

            // Source is the whole wielder hierarchy so shots ignore the player's own body and hands.
            GameObject source = Holder != null ? Holder.transform.root.gameObject : gameObject;
            projectile.Initialize(direction * speed, damage, type, source, impactPrefab);
            return projectile;
        }

        /// <summary>Perturbs a forward direction inside a cone of the given half-angle.</summary>
        protected Vector3 ApplySpread(Vector3 forward, float spreadDegrees)
        {
            if (spreadDegrees <= 0f) return forward;

            Vector2 jitter = Random.insideUnitCircle * spreadDegrees;
            return Quaternion.AngleAxis(jitter.x, Muzzle.up) * Quaternion.AngleAxis(jitter.y, Muzzle.right) * forward;
        }

        private void SpawnMuzzleFlash()
        {
            if (_muzzleFlashPrefab == null || ObjectPool.Instance == null) return;

            var flash = ObjectPool.Instance.Spawn(_muzzleFlashPrefab, Muzzle.position, Muzzle.rotation);
            ObjectPool.Instance.Despawn(flash, _muzzleFlashDuration);
        }
    }
}
