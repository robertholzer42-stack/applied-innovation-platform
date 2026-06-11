using System.Collections.Generic;
using UnityEngine;

namespace Pridefall.Core
{
    /// <summary>
    /// Scene-level prefab pool. Projectiles, impact effects, and enemy spawns
    /// must go through this; Instantiate/Destroy in combat blows the
    /// standalone (XR2) frame budget.
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

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!_pools.TryGetValue(prefab, out var queue))
            {
                queue = new Queue<GameObject>();
                _pools[prefab] = queue;
            }

            GameObject instance = null;
            while (queue.Count > 0 && instance == null)
            {
                instance = queue.Dequeue(); // may have been destroyed on scene unload
            }

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

        public void Despawn(GameObject instance, float delay)
        {
            StartCoroutine(DespawnAfter(instance, delay));
        }

        private System.Collections.IEnumerator DespawnAfter(GameObject instance, float delay)
        {
            yield return new WaitForSeconds(delay);
            Despawn(instance);
        }
    }
}
