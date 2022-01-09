using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BloodMeridiane.Car.Moving.Wheels
{
    [System.Serializable]
    public class Wheels
    {
        [Header("References")]
        [SerializeField] private Transform _wheelsCollidersTransform;
        [SerializeField] private WheelsSO _wheelsSO;

        [Header("Parameters")]
        [SerializeField, Range(1, 45)] private float _maxSteerAngle = 40f;
        [SerializeField, Range(1, 45)] private float _highSpeedSteerAngle = 5f;

        private List<WheelController> _wheels = new List<WheelController>();

        private float _verticalAxis, _torqForce, _steerAxis, _steerAngle, _breakAxis;   // параметры для Update

        public int AverageSpeed { get; private set; }
        public float AveragePoweredSpeed { get; private set; }
        public bool IsBreaking { get; private set; }

        public void InitWheels()
        {
            _wheels = _wheelsCollidersTransform.GetComponentsInChildren<WheelController>().ToList();

            foreach (var wheel in _wheels)
            {
                wheel.Init(_wheelsSO);
            }
        }

        #region Update
        public void UpdateWheelColiders()
        {
            foreach (var wheel in _wheels)
            {
                wheel.UpdateParameters();

                wheel.ApplyTorqForce(_verticalAxis, _torqForce);
                wheel.ApplyBreakForce(_breakAxis);
                wheel.ApplySteer(_steerAxis, _steerAngle);
                
                wheel.UpdateVisual();
            }
        }

        public void CalculateWheelsParameters()
        {
            int poweredWheels = 0;
            float averageWheelSpeed = 0;
            float averagePoweredWheelSpeed = 0;
            int breakingWheels = 0;
            int allBreakingWheels = 0;

            foreach (var wheel in _wheels)
            {
                wheel.CalulateSpeed();

                averageWheelSpeed += wheel.Speed;

                if (wheel.CanPower)
                {
                    poweredWheels++;
                    averagePoweredWheelSpeed += wheel.Speed;
                }

                if (wheel.CanBreak)
                {
                    allBreakingWheels++;
                    if (wheel.IsBraking)
                        breakingWheels++;
                }
            }

            AverageSpeed = (int)(averageWheelSpeed / _wheels.Count);
            AveragePoweredSpeed = averagePoweredWheelSpeed / poweredWheels;
            IsBreaking = breakingWheels > allBreakingWheels / 2;
        }
        #endregion

        #region Axis
        public void ApplyForce(float verticalAxis, float torqForce)
        {
            _verticalAxis = verticalAxis;
            _torqForce = torqForce;
        }

        public void ApplySteer(float steerAxis, float speed, float maxSpeed)
        {
            _steerAxis = steerAxis;
            _steerAngle = Mathf.Lerp(_maxSteerAngle, _highSpeedSteerAngle / 2, Mathf.Clamp01(speed / maxSpeed)); ;
        }

        public void ApplyBreak(float breakAxis) => _breakAxis = breakAxis;
        #endregion

        public List<float> GetRPM()
        {
            List<float> temp = new List<float>();

            foreach (var wheel in _wheels)
            {
                temp.Add(wheel.RPM);
            }

            return temp;
        }
    }
}