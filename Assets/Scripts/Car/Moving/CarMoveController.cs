using BloodMeridiane.Car.Moving.Wheel;
using System.Collections;
using UnityEngine;

namespace BloodMeridiane.Car.Moving
{
    [RequireComponent(typeof(GearBox))]
    public class CarMoveController : SimpleCarMoveController,
        ICarMoveController
    {
        [SerializeField] public Wheels Wheels;
        [Space(), SerializeField] public Motor Motor;
        [HideInInspector] public GearBox GearBox;

        [Space(), Header("Parameters")]
        [SerializeField, Min(1)] private float _velocityMultiplier = 3.6f;
        [SerializeField, Range(0, 1)] private float _wheelVelocityMultiplier = 0.374f;

        private float _verticalAxis, _steerAxis;
        private float _calculatedWheelSpeedMultiplier;
        [Tooltip("Колеса буксуют")] private bool _isWheelsSpun;

        #region Properties
        public float CalculatedSpeed { get; private set; }
        public float CalculatedWheelSpeed { get; private set; }
        #endregion

        #region Unity
        protected override void Awake()
        {
            base.Awake();

            GearBox = GetComponent<GearBox>();

            Wheels.InitWheels();
            GearBox.InitGearBox(Motor.MaxRPM);
        }

        private void Start()
        {
            Motor.CreateMotorCurves();
        }

        private void FixedUpdate()
        {
            CalculateSpeed();

            GearUpdate();
            EngineUpdate();
            WheelsUpdate();
        }
        #endregion

        #region Update
        private void CalculateSpeed()
        {
            CalculatedSpeed = Rigidbody.velocity.magnitude * _velocityMultiplier;
            CalculatedWheelSpeed = Wheels.CalculateWheelSpeed() * _velocityMultiplier;

            //_isWheelsSpun = Mathf.Abs(Mathf.Abs(CalculatedWheelSpeed) - CalculatedSpeed) > CalculatedSpeed * .1f;
            float absCalculatedWheelSpeed = Mathf.Abs(CalculatedWheelSpeed);
            if (absCalculatedWheelSpeed > CalculatedSpeed)
            {
                _calculatedWheelSpeedMultiplier = (absCalculatedWheelSpeed - CalculatedSpeed) / CalculatedSpeed;
                _calculatedWheelSpeedMultiplier = Mathf.Clamp(_calculatedWheelSpeedMultiplier, 0, 0.9f);
            }
        }

        private void GearUpdate()
        {
            GearBox.CheckGears(_verticalAxis, CalculatedWheelSpeed, Motor.RPM);
        }

        private void EngineUpdate()
        {
            if (GearBox.GearName == nameof(GearNames.N)) Motor.CalculateRPM();
            else Motor.CalculateRPM(Mathf.Abs(CalculatedSpeed) * Motor.MaxRPM / GearBox.Gear.MaxSpeed);
        }

        private void WheelsUpdate()
        {
            Wheels.ApplySteer(_steerAxis, CalculatedSpeed, GearBox.MaxSpeed);

            if (_verticalAxis != 0 && _isWheelsSpun == false)
            {
                Wheels.ApplyForce(_verticalAxis, Motor.Torq * GearBox.Gear.Ratio * GearBox.GearMultiplier);// * (1 - _calculatedWheelSpeedMultiplier));
            }
            else Wheels.ApplyForce(0, 0);

            Wheels.UpdateWheel();
        }
        #endregion

        #region Inputs
        public override void ApplyForce(float verticalAxis) => _verticalAxis = Mathf.Clamp(verticalAxis, -1f, 1f);

        public override void ApplySteer(float steerAxis) => _steerAxis = Mathf.Clamp(steerAxis, -1f, 1f);
        #endregion
    }
}