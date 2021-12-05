using BloodMeridiane.Car.Moving.Wheel;
using KAP.Extension;
using UnityEngine;

namespace BloodMeridiane.Car.Moving
{
    [RequireComponent(typeof(GearBox))]
    [RequireComponent(typeof(Lights))]
    public class CarMoveController : SimpleCarMoveController,
        ICarMoveController
    {
        [SerializeField] public Wheels Wheels;
        [Space(), SerializeField] public Motor Motor;
        [HideInInspector] public GearBox GearBox;

        [Space(), Header("Parameters")]
        [SerializeField, Min(1)] private float _velocityMultiplier = 3.6f;

        private ControlWheel _controlWheel;
        private Lights _lights;

        public float _verticalAxis, _steerAxis, _breakAxis;
        private bool _isBreaking;

        #region Properties
        public float CalculatedSpeed { get; private set; }
        public float CalculatedPoweredWheelSpeed { get; private set; }
        public int ControlWheelSpeed { get; private set; }
        #endregion

        #region Unity
        protected override void Awake()
        {
            base.Awake();

            GearBox = GetComponent<GearBox>();
            _controlWheel = GetComponentInChildren<ControlWheel>();
            _lights = GetComponent<Lights>();

            Wheels.InitWheels();
            GearBox.InitGearBox(Motor.MaxRPM);
        }

        private void Start()
        {
            Motor.CreateMotorCurves();
        }

        private void FixedUpdate()
        {
            Wheels.CalculateWheelsParameters();
            CalculateSpeed();

            GearUpdate();
            EngineUpdate();
            WheelsUpdate();
            LightsUpdate();
        }
        #endregion

        #region Update
        private void CalculateSpeed()
        {
            CalculatedSpeed = Rigidbody.velocity.magnitude * _velocityMultiplier;
            CalculatedPoweredWheelSpeed = Wheels.AveragePoweredSpeed * _velocityMultiplier;
            ControlWheelSpeed = (int)(_controlWheel.Speed * _velocityMultiplier);
        }

        private void GearUpdate()
        {
            GearBox.CheckGears(_verticalAxis, ControlWheelSpeed, Motor.RPM);
        }

        private void EngineUpdate()
        {
            if (GearBox.GearName == nameof(GearNames.N)) Motor.CalculateRPM();
            else Motor.CalculateRPM(Mathf.Abs(ControlWheelSpeed) * Motor.MaxRPM / GearBox.Gear.MaxSpeed);
        }

        private void WheelsUpdate()
        {
            Wheels.ApplySteer(_steerAxis, ControlWheelSpeed, GearBox.MaxSpeed);

            if (_verticalAxis != 0)
            {
                Wheels.ApplyForce(_verticalAxis, Motor.Torq * GearBox.Gear.Ratio * GearBox.GearMultiplier);
            }
            else Wheels.ApplyForce(0, 0);

            Wheels.ApplyBreak(_isBreaking.ToInt());

            Wheels.UpdateWheelColiders();
        }

        private void LightsUpdate()
        {
            _lights.EnableBreakLights(Wheels.IsBreaking);
            _lights.EnableReverceLigths(GearBox.GearName == GearNames.R.ToString());
        }
        #endregion

        #region Inputs
        public override void ApplyForce(float verticalAxis)
        {
            if (_breakAxis != 0)
            {
                _verticalAxis = 0f;
                return;
            }

            _verticalAxis = Mathf.Clamp(verticalAxis, -1f, 1f);

            var axisSign = _verticalAxis.Sign();
            var speedSign = ControlWheelSpeed.Sign();

            // если знаки противоположные
            if (axisSign != speedSign && Mathf.Abs(axisSign) == Mathf.Abs(speedSign))
            {
                _isBreaking = true;
            }
            else
            {
                _isBreaking = false;
            }
        }
        public override void ApplySteer(float steerAxis) => _steerAxis = Mathf.Clamp(steerAxis, -1f, 1f);
        public override void ApplyBreak(float breakAxis)
        {
            _breakAxis = Mathf.Clamp01(breakAxis);
            _isBreaking = _isBreaking || _breakAxis > 0;
        }
        #endregion
    }
}