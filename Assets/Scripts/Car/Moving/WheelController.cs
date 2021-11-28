using BloodMeridiane.Map;
using UnityEngine;

namespace BloodMeridiane.Car.Moving.Wheel
{
    public class WheelController : Wheel
    {
        [SerializeField] private Transform _wheelTransform;

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

        private int _groundIndex;
        private float _TCSStrength = 2f;

        #region Properties
        public bool IsGrounded { get; private set; }
        public bool CanPower => _canPower;

        private GroundMaterials GroundMaterials => GroundMaterials.Instance;
        private GroundMaterials.GroundMaterialFriction[] _physicsFrictions => GroundMaterials.Frictions;
        #endregion

        protected override void Awake()
        {
            base.Awake();

            _controlWheel = GetComponentInParent<ControlWheel>();
        }

        public void Init(float TCSStrength)
        {
            _TCSStrength = TCSStrength;
        }

        public void ApplySteer(float steerInput, float maxAngle)
        {
            if (_canSteer)
                Collider.steerAngle = steerInput * _steerMultiplier * maxAngle;
        }

        public void UpdateParameters()
        {
            IsGrounded = Collider.GetGroundHit(out _wheelHit);
            _groundIndex = GetGroundMaterialIndex();
        }

        public void UpdateVisual()
        {
            Collider.GetWorldPose(out Vector3 pos, out Quaternion quat);
            _wheelTransform.position = pos;
            _wheelTransform.rotation = quat;
        }

        public void ApplyTorqForce(float verticalAxis, float torqForce)
        {
            if (_canPower == false) return;

            if (Mathf.Abs(RPM) - Mathf.Abs(_controlWheel.RPM) > 100) 
                Collider.motorTorque = 0f;
            else
                Collider.motorTorque = verticalAxis * torqForce * _powerMultiplier;

            //print($"torqForce {torqForce}");

            //if (Mathf.Abs(RPM) >= 100)
            //{
            //    if (_wheelHit.forwardSlip > _physicsFrictions[_groundIndex].Slip)
            //    {
            //        torqForce -= Mathf.Clamp(torqForce * _wheelHit.forwardSlip * _TCSStrength, 0f, Mathf.Infinity);
            //        //print($"new -torqForce {torqForce}");
            //    }
            //    else
            //    {
            //        torqForce += Mathf.Clamp(torqForce * _wheelHit.forwardSlip * _TCSStrength, -Mathf.Infinity, 0f);
            //        //print($"new torqForce {torqForce}");
            //    }
            //}

        }


        /// <summary> Выдает индекс косаемого материала </summary>
        private int GetGroundMaterialIndex()
        {
            //Уже связывался с каким-либо физическим материалом из Configurable Ground Materials?
            //bool contacted = false;

            if (_wheelHit.point == Vector3.zero)
                return 0;

            int result = 0;

            for (int i = 0; i < _physicsFrictions.Length; i++)
            {
                if (_wheelHit.collider.sharedMaterial == _physicsFrictions[i].PhysicsMaterial)
                {
                    //contacted = true;
                    result = i;
                    break;
                }
            }

            #region Пока с Terrain data не работаю
            // Если physic material не является грунтовым, то надо проверить может быть мы на terrain collider
            //if (contacted == false)
            //{
            //    for (int i = 0; i < GroundMaterials.TerrainFrictions.Length; i++)
            //    {
            //        if (_wheelHit.collider.sharedMaterial == GroundMaterials.TerrainFrictions[i].PhysicsMaterial)
            //        {
            //            Vector3 playerPos = transform.position;
            //            Vector3 TerrainCord = ConvertToSplatMapCoordinate(playerPos);
            //            float comp = 0f;

            //            for (int k = 0; k < mNumTextures; k++)
            //            {

            //                if (comp < mSplatmapData[(int)TerrainCord.z, (int)TerrainCord.x, k])
            //                    result = k;

            //            }

            //            result = RCCGroundMaterialsInstance.terrainFrictions[i].splatmapIndexes[result].index;
            //            break;

            //        }

            //    }

            //}
            #endregion

            return result;
        }

        #region Обработка Terrain
        /// <summary>
        /// Converts to splat map coordinate.
        /// </summary>
        /// <returns>The to splat map coordinate.</returns>
        /// <param name="playerPos">Player position.</param>
        private Vector3 ConvertToSplatMapCoordinate(Vector3 playerPos)
        {
            Vector3 vecRet = new Vector3();
            Terrain ter = Terrain.activeTerrain;
            Vector3 terPosition = ter.transform.position;
            vecRet.x = ((playerPos.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
            vecRet.z = ((playerPos.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;
            return vecRet;
        }
        #endregion
    }
}