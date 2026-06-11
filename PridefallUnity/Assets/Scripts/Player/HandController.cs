using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Pridefall.Interaction;

namespace Pridefall.Player
{
    public enum Handedness { Left, Right }

    /// <summary>
    /// One per tracked hand. Polls grip/trigger from the XR device, tracks
    /// hand velocity (for swim strokes and thrown objects), and owns the
    /// grab loop: nearest Grabbable or ClimbHold within reach on grip press.
    /// </summary>
    public class HandController : MonoBehaviour
    {
        [SerializeField] private Handedness _handedness;
        [SerializeField] private float _grabRadius = 0.12f;
        [SerializeField] private LayerMask _grabMask = ~0;

        public Handedness Hand => _handedness;
        public bool GripHeld { get; private set; }
        public bool TriggerHeld { get; private set; }
        public float TriggerValue { get; private set; }
        public Vector3 Velocity { get; private set; }
        public Grabbable HeldGrabbable { get; private set; }
        public ClimbHold HeldClimbHold { get; private set; }
        public bool IsClimbing => HeldClimbHold != null;

        public event System.Action<HandController, ClimbHold> ClimbGripStarted;
        public event System.Action<HandController, ClimbHold> ClimbGripEnded;

        private Vector3 _lastPosition;
        private readonly Collider[] _overlapResults = new Collider[8];

        private void Start()
        {
            _lastPosition = transform.position;
        }

        private void Update()
        {
            PollDevice();
        }

        private void FixedUpdate()
        {
            Velocity = (transform.position - _lastPosition) / Time.fixedDeltaTime;
            _lastPosition = transform.position;
        }

        private void PollDevice()
        {
            var characteristics = InputDeviceCharacteristics.Controller |
                (_handedness == Handedness.Left ? InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right);

            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(characteristics, devices);

            bool grip = false;
            bool trigger = false;
            float triggerValue = 0f;
            foreach (var device in devices)
            {
                if (device.TryGetFeatureValue(CommonUsages.gripButton, out bool g)) grip |= g;
                if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool t)) trigger |= t;
                if (device.TryGetFeatureValue(CommonUsages.trigger, out float tv)) triggerValue = Mathf.Max(triggerValue, tv);
            }

            if (grip && !GripHeld) TryGrab();
            if (!grip && GripHeld) Release();

            GripHeld = grip;
            TriggerHeld = trigger;
            TriggerValue = triggerValue;
        }

        private void TryGrab()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, _grabRadius, _overlapResults, _grabMask, QueryTriggerInteraction.Collide);

            ClimbHold bestHold = null;
            Grabbable bestGrabbable = null;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                float distance = Vector3.Distance(transform.position, _overlapResults[i].ClosestPoint(transform.position));
                if (distance >= bestDistance) continue;

                var hold = _overlapResults[i].GetComponentInParent<ClimbHold>();
                if (hold != null)
                {
                    bestHold = hold;
                    bestGrabbable = null;
                    bestDistance = distance;
                    continue;
                }

                var grabbable = _overlapResults[i].GetComponentInParent<Grabbable>();
                if (grabbable != null && grabbable.CanBeGrabbed)
                {
                    bestGrabbable = grabbable;
                    bestHold = null;
                    bestDistance = distance;
                }
            }

            if (bestHold != null)
            {
                HeldClimbHold = bestHold;
                ClimbGripStarted?.Invoke(this, bestHold);
            }
            else if (bestGrabbable != null)
            {
                HeldGrabbable = bestGrabbable;
                bestGrabbable.OnGrabbed(this);
            }
        }

        private void Release()
        {
            if (HeldClimbHold != null)
            {
                var hold = HeldClimbHold;
                HeldClimbHold = null;
                ClimbGripEnded?.Invoke(this, hold);
            }

            if (HeldGrabbable != null)
            {
                HeldGrabbable.OnReleased(this, Velocity);
                HeldGrabbable = null;
            }
        }

        /// <summary>Force-drop whatever this hand holds (death, cutscene, eel grab).</summary>
        public void ForceRelease() => Release();
    }
}
