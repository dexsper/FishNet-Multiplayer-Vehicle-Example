using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using Shared.Object.Vehicle.Movement;
using UnityEngine;

namespace Shared.Object.Vehicle
{
    [RequireComponent(typeof(VehicleInput))]
    public class VehicleController : NetworkBehaviour
    {
        [SerializeField] private VehicleSettings _vehicleSettings;
        [SerializeField] private WheelColliders _wheelColliders;
        [SerializeField] private WheelMeshes _wheelMeshes;

        private MoveData? _lastMoveData;
        private float _slipAngle;
        private float _movingDirection;

        private VehicleInput _input;
        private Rigidbody _rigidbody;
        private Transform _cachedTransform;

        public float Speed { get; private set; }
        private bool CanControl => (IsOwner || (!Owner.IsValid && IsServerStarted));

        private void Awake()
        {
            _input = GetComponent<VehicleInput>();
            _rigidbody = GetComponent<Rigidbody>();
            _cachedTransform = GetComponent<Transform>();
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            TimeManager.OnTick += TimeManager_OnTick;
            TimeManager.OnPostTick += TimeManager_OnPostTick;

            _input.enabled = Owner.IsLocalClient;
        }

        public override void OnStopNetwork()
        {
            TimeManager.OnTick -= TimeManager_OnTick;
            TimeManager.OnPostTick -= TimeManager_OnPostTick;
        }


        private void TimeManager_OnTick()
        {
            Move(BuildMoveData());
        }

        private MoveData BuildMoveData()
        {
            if (!CanControl)
                return default;

            MoveData md = new MoveData(
                _input.SteeringInput,
                _input.GasInput
            );

            return md;
        }

        [ReplicateV2]
        private void Move(MoveData md, ReplicateState state = ReplicateState.Invalid,
            Channel channel = Channel.Unreliable)
        {
            if (!CanControl)
            {
                if (state is ReplicateState.UserCreated or ReplicateState.ReplayedUserCreated)
                {
                    _lastMoveData = md;
                }
                else
                {
                    if (_lastMoveData.HasValue)
                    {
                        uint tick = md.GetTick();
                        md = _lastMoveData.Value;
                        md.SetTick(tick);
                    }
                }
            }

            float brakeInput;
            Vector3 forward = _cachedTransform.forward;
            Vector3 velocity = _rigidbody.velocity;

            _slipAngle = Vector3.Angle(forward, velocity - forward);
            _movingDirection = Vector3.Dot(forward, velocity);

            if (!(_movingDirection < -0.5f) || !(md.GasInput > 0))
            {
                if (_movingDirection > 0.5f && md.GasInput < 0)
                {
                    brakeInput = Mathf.Abs(md.GasInput);
                }
                else
                {
                    brakeInput = 0;
                }
            }
            else
            {
                brakeInput = Mathf.Abs(md.GasInput);
            }

            Speed = velocity.magnitude;

            float steeringAngle = md.SteeringInput * _vehicleSettings.SteeringCurve.Evaluate(Speed);
            float motorTorque = _vehicleSettings.MotorPower * md.GasInput;
            float brakeTorque = _vehicleSettings.BrakePower * brakeInput;

            _wheelColliders.RRWheel.motorTorque = motorTorque;
            _wheelColliders.RLWheel.motorTorque = motorTorque;
            _wheelColliders.RRWheel.brakeTorque = brakeTorque * 0.3f;
            _wheelColliders.RLWheel.brakeTorque = brakeTorque * 0.3f;

            _wheelColliders.FRWheel.steerAngle = steeringAngle;
            _wheelColliders.FLWheel.steerAngle = steeringAngle;
            _wheelColliders.FRWheel.brakeTorque = brakeTorque * 0.7f;
            _wheelColliders.FLWheel.brakeTorque = brakeTorque * 0.7f;

            UpdateWheels();
        }

        private void TimeManager_OnPostTick()
        {
            if (!IsServerStarted)
                return;

            ReconcileData rd = new ReconcileData(
                _cachedTransform.position,
                _cachedTransform.rotation,
                _rigidbody.velocity,
                _rigidbody.angularVelocity,
                GetWheelsData()
            );

            Reconciliation(rd);
        }

        [ReconcileV2]
        private void Reconciliation(ReconcileData rd, Channel channel = Channel.Unreliable)
        {
            _cachedTransform.position = rd.Position;
            _cachedTransform.rotation = rd.Rotation;

            _rigidbody.velocity = rd.Velocity;
            _rigidbody.angularVelocity = rd.AngularVelocity;

            rd.WheelsData[0].ApplyToCollider(_wheelColliders.FLWheel);
            rd.WheelsData[1].ApplyToCollider(_wheelColliders.FRWheel);
            rd.WheelsData[2].ApplyToCollider(_wheelColliders.RLWheel);
            rd.WheelsData[3].ApplyToCollider(_wheelColliders.RRWheel);
        }

        private void UpdateWheels()
        {
            UpdateWheel(_wheelColliders.FLWheel, _wheelMeshes.FLWheel);
            UpdateWheel(_wheelColliders.FRWheel, _wheelMeshes.FRWheel);
            UpdateWheel(_wheelColliders.RLWheel, _wheelMeshes.RLWheel);
            UpdateWheel(_wheelColliders.RRWheel, _wheelMeshes.RRWheel);
        }

        private void UpdateWheel(WheelCollider wheelCollider, Transform wheel)
        {
            wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);

            wheel.position = pos;
            wheel.rotation = rot;
        }

        private WheelData[] GetWheelsData()
        {
            return new[]
            {
                WheelData.FromCollider(_wheelColliders.FLWheel),
                WheelData.FromCollider(_wheelColliders.FRWheel),
                WheelData.FromCollider(_wheelColliders.RLWheel),
                WheelData.FromCollider(_wheelColliders.RRWheel),
            };
        }
    }
}