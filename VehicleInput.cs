using Shared.Input;
using UnityEngine;

namespace Shared.Object.Vehicle
{
    public class VehicleInput : MonoBehaviour
    {
        public float SteeringInput { get; private set; }
        public float GasInput { get; private set; }

        private void Update()
        {
            SteeringInput = UnityEngine.Input.GetAxis("Horizontal");
            GasInput = UnityEngine.Input.GetAxis("Vertical");
        }
    }
}