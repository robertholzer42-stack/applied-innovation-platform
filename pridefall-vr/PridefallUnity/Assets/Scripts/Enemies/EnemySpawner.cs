using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pridefall.Core;
using Pridefall.Player;

namespace Pridefall.Enemies
{
    /// <summary>
    /// Encounter trigger volume. When the player walks in, spawns serialized
    /// waves through the ObjectPool at the spawn points, raises combat
    /// intensity for the music director, and drops it back to zero once every
    /// spawned enemy is dead (tracked via GameEvents.EnemyKilled, matched
    /// against this spawner's own instances).
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class EnemySpawner : MonoBehaviour
    {
        [System.Serializable]
        public class Wave
        {
            public GameObject Prefab;
            [Min(1)] public int Count = 1;
            [Tooltip("Seconds after the previous wave (or encounter start) before this wave spawns.")]
            public float Delay;
        }

        [SerializeField] private Wave[] _waves;
        [Tooltip("Spawn locations used round-robin. Falls back to this transform when empty.")]
        [SerializeField] private Transform[] _spawnPoints;
        [Tooltip("Music director hint while the encounter is live. 0 calm, 1 boss.")]
        [Range(0f, 1f)]
        [SerializeField] private float _combatIntensity = 0.6f;
        [Tooltip("If false the encounter re-arms after it is cleared.")]
        [SerializeField] private bool _oneShot = true;

        private readonly HashSet<GameObject> _alive = new();
        private bool _running;
        private bool _spent;
        private bool _allWavesSpawned;
        private int _nextSpawnPoint;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnEnable() => GameEvents.EnemyKilled += OnEnemyKilled;
        private void OnDisable() => GameEvents.EnemyKilled -= OnEnemyKilled;

        private void OnTriggerEnter(Collider other)
        {
            if (_running || _spent) return;
            if (_waves == null || _waves.Length == 0) return;
            if (other.GetComponentInParent<PlayerRig>() == null) return;

            StartCoroutine(RunEncounter());
        }

        private IEnumerator RunEncounter()
        {
            _running = true;
            _allWavesSpawned = false;
            _alive.Clear();
            GameEvents.RaiseCombatIntensityChanged(_combatIntensity);

            for (int w = 0; w < _waves.Length; w++)
            {
                Wave wave = _waves[w];
                if (wave == null || wave.Prefab == null) continue;
                if (wave.Delay > 0f) yield return new WaitForSeconds(wave.Delay);
                for (int i = 0; i < wave.Count; i++) SpawnOne(wave.Prefab);
            }

            _allWavesSpawned = true;
            if (_alive.Count == 0) EndEncounter(); // everything died before the last wave landed
        }

        private void SpawnOne(GameObject prefab)
        {
            Transform point = transform;
            if (_spawnPoints != null && _spawnPoints.Length > 0)
            {
                Transform candidate = _spawnPoints[_nextSpawnPoint % _spawnPoints.Length];
                _nextSpawnPoint++;
                if (candidate != null) point = candidate;
            }

            GameObject instance = ObjectPool.Instance != null
                ? ObjectPool.Instance.Spawn(prefab, point.position, point.rotation)
                : Instantiate(prefab, point.position, point.rotation);

            if (instance != null) _alive.Add(instance);
        }

        private void OnEnemyKilled(GameObject enemy, int scrap)
        {
            if (!_running) return;
            if (!_alive.Remove(enemy)) return; // someone else's enemy

            if (_allWavesSpawned && _alive.Count == 0) EndEncounter();
        }

        private void EndEncounter()
        {
            _running = false;
            _alive.Clear();
            GameEvents.RaiseCombatIntensityChanged(0f);
            if (_oneShot) _spent = true;
        }
    }
}
