using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BloodMeridiane.Car.Moving
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

        private List<WheelController> _wheels = new List<WheelController>();

        private float _verticalAxis, _torqForce, _steerAxis, _steerAngle;   // параметры для Update

        public void InitWheels()
        {
            _wheels = _wheelsCollidersTransform.GetComponentsInChildren<WheelController>().ToList();

            foreach (var wheel in _wheels)
            {
                wheel.Init(_TCSStrength);
            }
        }

        #region Update
        public void UpdateWheel()
        {
            foreach (var wheel in _wheels)
            {
                wheel.UpdateParameters();

                wheel.ApplyTorqForce(_verticalAxis, _torqForce);
                wheel.ApplySteer(_steerAxis, _steerAngle);

                UpdateWheelVisual(wheel);
            }
        }

        private void UpdateWheelVisual(WheelController wheel) => wheel.UpdateVisual();

        public float CalculateWheelSpeed()
        {
            int poweredWheel = 0;
            float averageSpeedWheel = 0;

            foreach (var wheel in _wheels)
            {
                if (wheel.CanPower)
                {
                    poweredWheel++;
                    averageSpeedWheel += wheel.Speed;
                }
                averageSpeedWheel += wheel.Speed;
            }

            return averageSpeedWheel / _wheels.Count;
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
            _steerAngle = Mathf.Lerp(_maxSteerAngle, _highSpeedSteerAngle, Mathf.Clamp01(speed / maxSpeed / 2)); ;
        }
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