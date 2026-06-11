using UnityEngine;
using Pridefall.Core;
using Pridefall.Player;

namespace Pridefall.Weapons
{
    /// <summary>
    /// Salvaged Tender forge: the crafting bench of PRIDEFALL's loop.
    /// Converts scrap into energy cells and medgel, and bio-resin into spike
    /// bundles, dropping physical pickups onto the output tray for the player
    /// to grab. Operating requires standing inside the station's trigger
    /// volume; every outcome toasts to the wrist HUD via
    /// GameEvents.ObjectiveUpdated.
    /// </summary>
    public class FabricatorStation : MonoBehaviour
    {
        [Header("Output")]
        [Tooltip("Pickups spawn just above this transform with a small scatter so stacked orders don't interpenetrate.")]
        [SerializeField] private Transform _outputTray;
        [SerializeField] private float _trayScatterRadius = 0.06f;

        [Header("Recipes")]
        [SerializeField] private GameObject _energyCellPrefab;
        [SerializeField] private int _cellScrapCost = 5;
        [SerializeField] private GameObject _medgelPrefab;
        [SerializeField] private int _medgelScrapCost = 8;
        [SerializeField] private GameObject _spikeBundlePrefab;
        [SerializeField] private int _spikeBundleResinCost = 3;

        public bool PlayerInRange => _playerInRange != null;

        private PlayerRig _playerInRange;

        public void FabricateCell() => Fabricate(_energyCellPrefab, _cellScrapCost, 0, "energy cell");

        public void FabricateMedgel() => Fabricate(_medgelPrefab, _medgelScrapCost, 0, "medgel");

        public void FabricateSpikes() => Fabricate(_spikeBundlePrefab, 0, _spikeBundleResinCost, "spike bundle");

        private void Fabricate(GameObject prefab, int scrapCost, int resinCost, string label)
        {
            if (!PlayerInRange)
            {
                GameEvents.RaiseObjectiveUpdated("Fabricator: no operator at the forge");
                return;
            }
            if (prefab == null || _outputTray == null)
            {
                Debug.LogWarning($"[FabricatorStation] Recipe '{label}' is missing a prefab or output tray.", this);
                return;
            }
            var manager = GameManager.Instance;
            if (manager == null) return;

            if (scrapCost > 0 && !manager.SpendScrap(scrapCost))
            {
                GameEvents.RaiseObjectiveUpdated("Fabricator: insufficient scrap");
                return;
            }
            if (resinCost > 0 && !manager.SpendBioResin(resinCost))
            {
                GameEvents.RaiseObjectiveUpdated("Fabricator: insufficient bio-resin");
                return;
            }

            Vector2 scatter = Random.insideUnitCircle * _trayScatterRadius;
            Vector3 position = _outputTray.position + _outputTray.up * 0.02f + new Vector3(scatter.x, 0f, scatter.y);
            Instantiate(prefab, position, _outputTray.rotation); // stateful pickups are not pooled
            GameEvents.RaiseObjectiveUpdated($"Fabricator: {label} ready");
        }

        private void OnTriggerEnter(Collider other)
        {
            var rig = other.GetComponentInParent<PlayerRig>();
            if (rig != null) _playerInRange = rig;
        }

        private void OnTriggerExit(Collider other)
        {
            var rig = other.GetComponentInParent<PlayerRig>();
            if (rig != null && rig == _playerInRange) _playerInRange = null;
        }
    }
}
