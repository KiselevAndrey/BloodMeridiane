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
        [SerializeField, Range(.02f, .4f), Tooltip("Engine inertia. Engine reacts faster on lower values.\n������� ���������. ��������� ������� ��������� �� ����� ������ ��������.")] private float _engineInertia = .15f;

        [Header("TorqueCurve Parameters")]
        [SerializeField, Range(0.1f, 0.5f)] private float _forcePercentAtMinRpm = 0.2f;
        [SerializeField, Range(0.1f, 0.95f)] private float _rpmPercentAtMaxForce = 0.7f;
        [SerializeField, Range(0.5f, 1f)] private float _forcePercentAtMaxRpm= 0.7f;

        [Header("References")]
        [SerializeField] private Utility.PowersHandler _soundPowerHandler;
        [SerializeField] private Utility.PowersHandler _exhaustPowerHandler;

        private AnimationCurve _engineTorqueCurve;
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
            //_engineTorqueCurve.AddKey(_maxRpm, _motorForce * _forcePercentAtMaxRpm / 3);
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

            // ���������� ������ ������ � �������� ���� � �������� �������.
            // ������ ������������ ��������� ��������-���������� ��������, ������� ������� �� ���������.
            // ���� ����� ������������ ��� ����������� �������� ������.
            RPM = Mathf.SmoothDamp(RPM, targetRpm, ref velocity, _engineInertia);
        }

        public void Update(float verticalAxis)
        {
            float rpmParts = _currentRPM / _maxRpm;
            float absVerticalAxis = Mathf.Abs(verticalAxis);

            UpdateSounds(rpmParts, absVerticalAxis);
            UpdateExhaust(rpmParts, absVerticalAxis);
        }

        private void UpdateSounds(float rpmParts, float absVerticalAxis)
        {
            _soundPowerHandler.SetPower(rpmParts * 0.7f + absVerticalAxis * 0.3f);
        }

        private void UpdateExhaust(float rpmParts, float absVerticalAxis)
        {
            _exhaustPowerHandler.SetPower(rpmParts * absVerticalAxis);
        }
    }
}