using FishNet.Object.Prediction;
using UnityEngine;

namespace Shared.Object.Vehicle.Movement
{
    [System.Serializable]
    public struct WheelData
    {
        public float MotorTorque;
        public float BrakeTorque;
        public float SteeringAngle;

        public WheelData(float motorTorque, float brakeTorque, float steeringAngle)
        {
            MotorTorque = motorTorque;
            BrakeTorque = brakeTorque;
            SteeringAngle = steeringAngle;
        }

        public void ApplyToCollider(WheelCollider wheelCollider)
        {
            wheelCollider.motorTorque = MotorTorque;
            wheelCollider.brakeTorque = BrakeTorque;
            wheelCollider.steerAngle = SteeringAngle;
        }

        public static WheelData FromCollider(WheelCollider wheelCollider)
        {
            return new WheelData(wheelCollider.motorTorque, wheelCollider.brakeTorque,
                wheelCollider.steerAngle);
        }
    }

    public struct ReconcileData : IReconcileData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;

        public WheelData[] WheelsData;

        public ReconcileData(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity,
            WheelData[] wheelsData)
        {
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
            AngularVelocity = angularVelocity;

            WheelsData = wheelsData;

            _tick = 0;
        }

        private uint _tick;

        public void Dispose()
        {
        }

        public uint GetTick() => _tick;
        public void SetTick(uint value) => _tick = value;
    }
}