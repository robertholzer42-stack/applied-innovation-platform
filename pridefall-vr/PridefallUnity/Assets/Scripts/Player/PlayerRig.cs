using UnityEngine;
using Pridefall.Input;

namespace Pridefall.Player
{
    /// <summary>
    /// Root of the player hierarchy. Wires together the play space, head,
    /// hands, and the locomotion provider stack, preferring real Omni One
    /// hardware and falling back to the simulator in the editor.
    ///
    /// Hierarchy:
    ///   PlayerRig (CharacterController, PlayerLocomotionController, PlayerHealth)
    ///     PlaySpace (XR origin)
    ///       Head (Camera)
    ///       LeftHand / RightHand (HandController)
    ///     BodyRoot (rotated to Omni ring yaw; holsters parent here)
    /// </summary>
    public class PlayerRig : MonoBehaviour
    {
        [Header("Anchors")]
        [SerializeField] private Transform _playSpace;
        [SerializeField] private Transform _head;
        [SerializeField] private HandController _leftHand;
        [SerializeField] private HandController _rightHand;
        [Tooltip("Rotated each frame to the Omni ring yaw. Holsters and the wrist HUD parent here.")]
        [SerializeField] private Transform _bodyRoot;

        [Header("Locomotion Providers (priority order)")]
        [SerializeField] private OmniOneLocomotionProvider _omniProvider;
        [SerializeField] private SimulatedLocomotionProvider _simulatedProvider;

        public Transform PlaySpace => _playSpace;
        public Transform Head => _head;
        public HandController LeftHand => _leftHand;
        public HandController RightHand => _rightHand;
        public Transform BodyRoot => _bodyRoot;

        public ILocomotionProvider ActiveLocomotionProvider { get; private set; }

        private void Awake()
        {
            ActiveLocomotionProvider = SelectProvider();
            ActiveLocomotionProvider.Calibrate();
        }

        private ILocomotionProvider SelectProvider()
        {
            if (_omniProvider != null && _omniProvider.IsActive)
            {
                Debug.Log("[PlayerRig] Omni One treadmill detected, using hardware locomotion.");
                return _omniProvider;
            }

            Debug.Log("[PlayerRig] No Omni One detected, using simulated locomotion (editor/desktop).");
            return _simulatedProvider;
        }

        private void LateUpdate()
        {
            if (_bodyRoot != null && ActiveLocomotionProvider != null)
            {
                float worldBodyYaw = _playSpace.eulerAngles.y + ActiveLocomotionProvider.BodyYawDegrees;
                _bodyRoot.rotation = Quaternion.Euler(0f, worldBodyYaw, 0f);

                // Keep holsters under the player even as the HMD moves in the play space.
                Vector3 headPlanar = _head.position;
                headPlanar.y = transform.position.y;
                _bodyRoot.position = headPlanar;
            }
        }
    }
}
