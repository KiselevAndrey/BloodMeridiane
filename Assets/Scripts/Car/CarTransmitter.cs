using BloodMeridiane.Car.Moving;
using UnityEngine;

namespace BloodMeridiane.Car
{
    public class CarTransmitter : MonoBehaviour, IDisplayedSpeed
    {
        [SerializeField] private CarMoveController _moveController;

        public float CalculatedSpeed => _moveController.CalculatedSpeed;
        public float CalculatedPoweredWheelSpeed => _moveController.CalculatedPoweredWheelSpeed;
        public int ControlWheelSpeed => _moveController.ControlWheelSpeed;
    }
}