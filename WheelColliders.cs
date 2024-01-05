using UnityEngine;

namespace Shared.Object.Vehicle
{
    [System.Serializable]
    public struct WheelColliders
    {
        public WheelCollider FRWheel;
        public WheelCollider FLWheel;
        public WheelCollider RRWheel;
        public WheelCollider RLWheel;
    }
}