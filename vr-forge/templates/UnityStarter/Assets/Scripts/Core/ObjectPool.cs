using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRForge.Core
{
    /// <summary>
    /// Scene-level prefab pool. Projectiles, impact effects, and spawned
    /// enemies must go through this; Instantiate/Destroy during play blows
    /// the standalone Quest frame budget. One instance lives in the scene,
    /// accessed via <see cref="Instance"/>.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }

        private readonly Dictionary<GameObject, Queue<GameObject>> _pools = new();
        private readonly Dictionary<GameObject, GameObject> _instanceToPrefab = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>Fetch a pooled instance of <paramref name="prefab"/>, creating one if the pool is dry.</summary>
        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
            {
                Debug.LogWarning("[ObjectPool] Spawn called with a null prefab.", this);
                return null;
            }

            if (!_pools.TryGetValue(prefab, out var queue))
            {
                queue = new Queue<GameObject>();
                _pools[prefab] = queue;
            }

            GameObject instance = DequeueLive(queue);
            if (instance == null)
            {
                instance = Instantiate(prefab, position, rotation, transform);
                _instanceToPrefab[instance] = prefab;
            }
            else
            {
                instance.transform.SetPositionAndRotation(position, rotation);
            }

            instance.SetActive(true);
            return instance;
        }

        /// <summary>Return an instance to its pool. Instances not spawned here are destroyed as a fail-safe.</summary>
        public void Despawn(GameObject instance)
        {
            if (instance == null) return;

            if (_instanceToPrefab.TryGetValue(instance, out var prefab))
            {
                instance.SetActive(false);
                _pools[prefab].Enqueue(instance);
            }
            else
            {
                Destroy(instance); // not pooled here; fail safe
            }
        }

        /// <summary>Return an instance to its pool after <paramref name="delay"/> seconds.</summary>
        public void Despawn(GameObject instance, float delay)
        {
            if (instance == null) return;

            if (delay <= 0f || !isActiveAndEnabled)
            {
                Despawn(instance);
                return;
            }
            StartCoroutine(DespawnAfter(instance, delay));
        }

        /// <summary>
        /// Dequeue the first live entry, skipping instances that a scene
        /// unload destroyed. Static and public so edit mode tests can cover
        /// the skip logic without a running pool.
        /// </summary>
        public static GameObject DequeueLive(Queue<GameObject> queue)
        {
            if (queue == null) return null;

            while (queue.Count > 0)
            {
                var candidate = queue.Dequeue();
                if (candidate != null) return candidate;
            }
            return null;
        }

        private IEnumerator DespawnAfter(GameObject instance, float delay)
        {
            yield return new WaitForSeconds(delay);
            Despawn(instance);
        }
    }
}
