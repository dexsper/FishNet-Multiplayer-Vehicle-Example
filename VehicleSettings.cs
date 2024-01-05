using UnityEngine;

namespace Shared.Object.Vehicle
{
    [System.Serializable]
    public struct VehicleSettings
    {
        public AnimationCurve SteeringCurve;

        public float BrakePower;
        public float MotorPower;
    }
}