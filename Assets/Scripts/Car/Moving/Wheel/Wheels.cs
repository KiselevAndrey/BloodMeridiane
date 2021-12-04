using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BloodMeridiane.Car.Moving.Wheel
{
    [System.Serializable]
    public class Wheels
    {
        [Header("References")]
        [SerializeField] private Transform _wheelsCollidersTransform;

        [Header("Parameters")]
        [SerializeField, Range(1, 45)] private float _maxSteerAngle = 40f;
        [SerializeField, Range(1, 5)] private float _highSpeedSteerAngle = 5f;
        [SerializeField, Range(.05f, 1f)] private float _TCSStrength = 1f;
        [SerializeField, Min(0)] private float _breakForce = 1000f;

        private List<WheelController> _wheels = new List<WheelController>();

        private float _verticalAxis, _torqForce, _steerAxis, _steerAngle, _breakAxis;   // параметры для Update

        public int AverageSpeed { get; private set; }
        public float AveragePoweredSpeed { get; private set; }

        public void InitWheels()
        {
            _wheels = _wheelsCollidersTransform.GetComponentsInChildren<WheelController>().ToList();

            foreach (var wheel in _wheels)
            {
                wheel.Init(_TCSStrength, _breakForce);
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

        public void CalculateWheelSpeed()
        {
            int poweredWheel = 0;
            float averageSpeedWheel = 0;
            float averagePoweredSpeedWheel = 0;

            foreach (var wheel in _wheels)
            {
                wheel.CalulateSpeed();

                if (wheel.CanPower)
                {
                    poweredWheel++;
                    averagePoweredSpeedWheel += wheel.Speed;
                }
                averageSpeedWheel += wheel.Speed;
            }

            AverageSpeed = (int)(averageSpeedWheel / _wheels.Count);
            AveragePoweredSpeed = averagePoweredSpeedWheel / poweredWheel;
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