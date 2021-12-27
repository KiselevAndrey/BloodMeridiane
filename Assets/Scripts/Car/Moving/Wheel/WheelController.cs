using BloodMeridiane.Map;
using KAP.Extension;
using UnityEngine;

namespace BloodMeridiane.Car.Moving.Wheels
{
    public class WheelController : Wheel
    {
        [SerializeField] private WheelHolder _wheelHolder;

        [Space()]
        [SerializeField] private bool _canPower;
        [SerializeField, Range(-1, 1)] private float _powerMultiplier = 1f;
        [Space()]
        [SerializeField] private bool _canSteer;
        [SerializeField, Range(-1, 1)] private float _steerMultiplier = 1f;
        [Space()]
        [SerializeField] private bool _canBrake;
        [SerializeField, Range(0, 1)] private float _brakeMultiplier = 1f;

        private ControlWheel _controlWheel;
        private WheelHit _wheelHit;
        private WheelsSO _wheelsSO;
        private GroundMaterial _groundMaterial;
        private SlipVisualHandler _slipHandler;

        private float _calculatedSpeed;

        #region Properties
        public bool IsGrounded { get; private set; }
        public bool IsBraking { get; private set; }
        public bool CanPower => _canPower;
        public bool CanBreak=> _canBrake;

        public override float Speed => _calculatedSpeed;

        //private MapGroundMaterials GroundMaterials => MapGroundMaterials.Instance;
        //private MapGroundMaterials.GroundMaterialFriction[] _physicsFrictions => GroundMaterials.Frictions;
        #endregion

        protected override void Awake()
        {
            base.Awake();

            _controlWheel = GetComponentInParent<ControlWheel>();
        }

        public void Init(WheelsSO wheelsSO)
        {
            _wheelsSO = wheelsSO;

            if (Collider != null)
            {
                Collider.radius = _wheelsSO.Radius;
                Collider.mass = _wheelsSO.Mass;
            }

            _wheelHolder.InitWheel(_wheelsSO.WheelObject);
        }

        public void CalulateSpeed() 
        {
            _calculatedSpeed = base.Speed;
        }

        #region Update
        public void UpdateParameters()
        {
            IsGrounded = Collider.GetGroundHit(out _wheelHit);

            if (IsGrounded)
            {
                var groundMaterial = GetGroundMaterial();
                if (_groundMaterial != groundMaterial)
                    SetNewGroundMaterial(groundMaterial);
            }
        }

        public void UpdateVisual()
        {
            Collider.GetWorldPose(out Vector3 pos, out Quaternion quat);
            _wheelHolder.transform.position = pos;
            _wheelHolder.transform.rotation = quat;

            UpdateSlipVisual();
            UpdateMoveVisual();
        }

        private void UpdateSlipVisual()
        {
            _slipHandler.transform.position = _wheelHit.point;
            _slipHandler.SetPower(Mathf.Max(Mathf.Abs(_wheelHit.forwardSlip) - _groundMaterial.ForwardSlip, Mathf.Abs(_wheelHit.sidewaysSlip) - _groundMaterial.SidewaysSlip) * IsGrounded.ToInt());
        }

        private void UpdateMoveVisual()
        {
        }
        #endregion

        #region Apply
        public void ApplySteer(float steerInput, float maxAngle)
        {
            if (_canSteer)
                Collider.steerAngle = steerInput * _steerMultiplier * maxAngle;
        }

        public void ApplyTorqForce(float verticalAxis, float torqForce)
        {
            if (_canPower == false) return;

            //Collider.motorTorque = verticalAxis * torqForce * _powerMultiplier;

            if (IsGrounded)
            {
                //if (Mathf.Abs(RPM) - Mathf.Abs(_controlWheel.RPM) > 100)
                //    Collider.motorTorque = 0f;
                //else
                //{
                    if (Mathf.Abs(RPM) >= 100 && _groundMaterial != null)
                    {
                        if (_wheelHit.forwardSlip > _groundMaterial.ForwardSlip)
                        {
                            torqForce -= Mathf.Clamp(torqForce * _wheelHit.forwardSlip * _wheelsSO.TCSStrength, 0f, Mathf.Infinity);
                        }
                        else
                        {
                            torqForce += Mathf.Clamp(torqForce * _wheelHit.forwardSlip * _wheelsSO.TCSStrength, -Mathf.Infinity, 0f);
                        }
                    }
                    Collider.motorTorque = verticalAxis * torqForce * _powerMultiplier;
                //}
            }
            else
            {
                Collider.motorTorque = verticalAxis* torqForce * _powerMultiplier;
            }
        }

        public void ApplyBreakForce(float breakMultiplier)
        {
            if (CanBreak == false) return;

            Collider.brakeTorque = _wheelsSO.BreakForce * _brakeMultiplier * breakMultiplier;

            IsBraking = breakMultiplier > 0;
        }
        #endregion

        #region GroundMaterial
        private void SetNewGroundMaterial(GroundMaterial newGroundMaterial)
        {
            _groundMaterial = newGroundMaterial;

            var forwardFriction = Collider.forwardFriction;
            forwardFriction.stiffness = _groundMaterial.ForwardStiffness;
            Collider.forwardFriction = forwardFriction;

            var sidewaysFriction = Collider.sidewaysFriction;
            sidewaysFriction.stiffness = _groundMaterial.SidewaysStiffness;
            Collider.sidewaysFriction = sidewaysFriction;

            if (_slipHandler != null)
                Destroy(_slipHandler.gameObject);

            if (_groundMaterial.SlipParticles)
            {
                _slipHandler = Instantiate(_groundMaterial.SlipParticles, transform).GetComponent<SlipVisualHandler>();
                _slipHandler.SetPower(0);
            }
        }

        private GroundMaterial GetGroundMaterial()
        {
            if (_wheelHit.point == Vector3.zero)
                return null;

            return _wheelsSO.GroundMaterials.GetMaterial(_wheelHit.collider.sharedMaterial);
        }
        #endregion
    }
}