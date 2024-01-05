using FishNet.Object.Prediction;

namespace Shared.Object.Vehicle.Movement
{
    public struct MoveData : IReplicateData
    {
        public float SteeringInput;
        public float GasInput;

        public MoveData(float steeringInput, float gasInput)
        {
            SteeringInput = steeringInput;
            GasInput = gasInput;

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