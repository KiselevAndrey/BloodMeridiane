using UnityEngine;

namespace BloodMeridiane.Car.Moving
{
    [System.Serializable]
    public class Motor
    {
        [SerializeField] private float _motorForce = 50f;
        [SerializeField, Min(1000)] private int _maxRpm = 8000;
        [SerializeField, Range(50, 1000)] private int _startEngineRpm = 500;
        [SerializeField, Range(0, 10)] private float _maxFuelConsumption = 5f;
        [SerializeField, Range(.02f, .4f), Tooltip("Engine inertia. Engine reacts faster on lower values.\nИнерция двигателя. Двигатель быстрее реагирует на более низкие значения.")] private float _engineInertia = .15f;

        [Header("TorqueCurve Parameters")]
        [SerializeField, Range(0, 0.5f)] private float _forcePercentAtMinRpm = 0.2f;
        [SerializeField, Range(0, 1f)] private float _rpmPercentAtMaxForce = 0.7f;
        [SerializeField, Range(0.5f, 1f)] private float _forcePercentAtMaxRpm= 0.7f;


        [SerializeField] private AnimationCurve _engineTorqueCurve;
        private AnimationCurve _fuelConsumptionCurve;

        private float _currentRPM;

        #region Properties
        public float RPM { get => Mathf.Clamp(_currentRPM, _startEngineRpm / 2, _maxRpm); set => _currentRPM = Mathf.Min(value, _maxRpm); }
        public float Torq => _engineTorqueCurve.Evaluate(RPM);
        public int MaxRPM => _maxRpm;
        #endregion

        #region CreateCurve
        public void CreateMotorCurves()
        {
            CreateEngineCurve();
            CreateFuelConsumptionCurve();
        }

        /// <summary>  Creates the engine curve. </summary>
        private void CreateEngineCurve()
        {
            _engineTorqueCurve = new AnimationCurve();
            _engineTorqueCurve.AddKey(0f, _motorForce * _forcePercentAtMinRpm);          // First index of the curve.
            _engineTorqueCurve.AddKey(_maxRpm * _rpmPercentAtMaxForce, _motorForce);     // Second index of the curve at max.
            _engineTorqueCurve.AddKey(_maxRpm, _motorForce * _forcePercentAtMaxRpm);     // Last index of the curve at maximum RPM.
        }

        private void CreateFuelConsumptionCurve()
        {
            _fuelConsumptionCurve = new AnimationCurve();
            _fuelConsumptionCurve.AddKey(0f, 0f);
            _fuelConsumptionCurve.AddKey(_maxRpm, _maxFuelConsumption);
        }
        #endregion
    
        public void CalculateRPM(float targetRpm = -1)
        {
            if (targetRpm == -1) targetRpm = _startEngineRpm;

            float velocity = 0;

            // Постепенно меняет вектор к желаемой цели с течением времени.
            // Вектор сглаживается некоторой пружинно-демпферной функцией, которая никогда не проскочит.
            // Чаще всего используется для сглаживания следящей камеры.
            RPM = Mathf.SmoothDamp(RPM, targetRpm, ref velocity, _engineInertia);
        }
    }
}