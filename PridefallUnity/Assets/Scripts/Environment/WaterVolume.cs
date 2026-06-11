using UnityEngine;
using Pridefall.Player;

namespace Pridefall.Environment
{
    /// <summary>
    /// A swimmable body of water (the Mirror Sea, flooded Daedalus decks).
    /// Trigger collider; the top face is the swim surface.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class WaterVolume : MonoBehaviour
    {
        private BoxCollider _box;

        public float SurfaceY
        {
            get
            {
                if (_box == null) _box = GetComponent<BoxCollider>();
                return _box.bounds.max.y;
            }
        }

        private void Awake()
        {
            _box = GetComponent<BoxCollider>();
            _box.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var swimmer = other.GetComponentInParent<SwimmingSystem>();
            if (swimmer != null) swimmer.EnterWater(this);
        }

        private void OnTriggerExit(Collider other)
        {
            var swimmer = other.GetComponentInParent<SwimmingSystem>();
            if (swimmer != null) swimmer.ExitWater(this);
        }
    }
}
