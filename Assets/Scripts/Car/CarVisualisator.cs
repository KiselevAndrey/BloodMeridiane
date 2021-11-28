using BloodMeridiane.Car.Moving;
using UnityEngine;

namespace BloodMeridiane.Car
{
    public class CarVisualisator : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _wheelsSpeed;
        [SerializeField] private float _speedDifference;
        [Header("Motor")]
        [SerializeField] private int _rpm;
        [SerializeField] private float _currentTorq;
        [Header("Gear Box")]
        [SerializeField] private string _gearName;
        [SerializeField] private int _gear;
        [SerializeField] private float _gearRatio;
        [SerializeField] private float _targetSpeed;

        private CarMoveController _car;

        private void Awake()
        {
            _car = GetComponent<CarMoveController>();
        }

        private void Update()
        {
            _speed = _car.CalculatedSpeed;
            _wheelsSpeed = _car.CalculatedWheelSpeed;
            _rpm = (int)_car.Motor.RPM;
            _currentTorq = _car.Motor.Torq;
            _gear = _car.GearBox.CurrentGear;
            _gearRatio = _car.GearBox.Gear.Ratio;
            _gearName = _car.GearBox.GearName;
            _targetSpeed = _car.GearBox.Gear.TargetSpeedForNextGear;
            _speedDifference = Mathf.Clamp((Mathf.Abs(_wheelsSpeed) - _speed) / _speed, 0, 0.9f);
        }
    }
}