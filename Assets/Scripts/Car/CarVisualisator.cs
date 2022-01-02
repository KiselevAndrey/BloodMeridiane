using BloodMeridiane.Car.Moving;
using KAP.Extension;
using UnityEngine;

namespace BloodMeridiane.Car
{
    public class CarVisualisator : MonoBehaviour
    {
        [SerializeField] private float _calculatedSpeed;
        [SerializeField] private float _controlWheelSpeed;
        //[SerializeField] private float _speedDifference;
        [Header("Motor")]
        [SerializeField] private int _rpm;
        [SerializeField] private float _currentTorq;
        [Header("Gear Box")]
        [SerializeField] private string _gearName;
        [SerializeField] private int _gear;
        [SerializeField] private float _gearRatio;
        [SerializeField] private float _targetSpeed;
        [SerializeField] private float _gearBoxMultiplier;

        private CarMoveController _car;

        private void Awake()
        {
            _car = GetComponent<CarMoveController>();
        }

        private void Update()
        {
            _calculatedSpeed = _car.CalculatedSpeed;
            _controlWheelSpeed = _car.ControlWheelSpeed;
            _rpm = (int)_car.Motor.RPM;
            _currentTorq = _car.Motor.Torq;
            _gear = _car.GearBox.CurrentGear;
            _gearRatio = _car.GearBox.Gear.Ratio;
            _gearName = _car.GearBox.GearName;
            _targetSpeed = _car.GearBox.Gear.TargetSpeedForNextGear;
            //_speedDifference = Mathf.Clamp((Mathf.Abs(_wheelsSpeed) - _speed) / _speed, 0, 0.9f);

            _gearBoxMultiplier = _car.GearBox.GearMultiplier;
        }
    }
}