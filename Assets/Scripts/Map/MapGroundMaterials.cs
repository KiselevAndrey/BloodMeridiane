using UnityEngine;

namespace BloodMeridiane.Map
{
	[CreateAssetMenu(menuName = nameof(Map) + "/" + nameof(MapGroundMaterials))]
	[System.Serializable]
	public class MapGroundMaterials : ScriptableObject
	{
		#region singleton
		private static MapGroundMaterials _instance;
		public static MapGroundMaterials Instance
		{
			get
			{
				if (_instance == null)
					_instance = Resources.Load("SO/MAP/GroundMaterials") as MapGroundMaterials;
				return _instance;
			}
		}
		#endregion

		[System.Serializable]
		public class GroundMaterialFriction
		{
			public PhysicMaterial PhysicsMaterial;
			public float ForwardStiffness = 1f;
			public float SidewaysStiffness = 1f;
			public float Slip = .25f;
			public float Damp = 1f;
			[Range(0f, 1f)] public float Volume = 0.5f;
			public GameObject Particles;
			public AudioClip Sound;
			//public RCC_Skidmarks skidmark; // след от машины
		}

        public GroundMaterialFriction[] Frictions;

        [System.Serializable]
        public class TerrainFriction
        {
            public PhysicMaterial PhysicsMaterial;

            [System.Serializable]
            public class SplatmapIndexes
            {
                public int Index = 0;
                public PhysicMaterial Material;
            }

            public SplatmapIndexes[] SplatmapInds;
        }

        public TerrainFriction[] TerrainFrictions;
    }

	[System.Serializable]
	public class GroundMaterial
    {
		[SerializeField] private PhysicMaterial _physicsMaterial;
		[SerializeField] private float _forwardStiffness = 1f;
		[SerializeField] private float _sidewaysStiffness = 1f;
        [SerializeField] private float _forwardSlip = .25f;
        [SerializeField] private float _sidewaysSlip = .25f;
        //[SerializeField] private float _damp = 1f;
        [Range(0f, 1f)] [SerializeField] private float _volume = 0.5f;
		[SerializeField] private GameObject _slipParticles;
		[SerializeField] private AudioClip _slipSound;
		[SerializeField] private GameObject _moveParticles;
		[SerializeField] private AudioClip _moveSound;

		public PhysicMaterial PhysicsMaterial => _physicsMaterial;
		public float ForwardStiffness => _forwardStiffness;
		public float SidewaysStiffness => _sidewaysStiffness;
        public float ForwardSlip => _forwardSlip;
        public float SidewaysSlip => _sidewaysSlip;
		//public float Damp = 1f;
		public float Volume => _volume;
		public GameObject SlipParticles => _slipParticles;
		public AudioClip SlipSound => _slipSound;
		public GameObject Particles => _moveParticles;
		public AudioClip Sound => _moveSound;
	}


	[System.Serializable]
	public class GroundMaterials
	{
		[SerializeField] private GroundMaterial _asphalt;

		public GroundMaterial Asphalt => _asphalt;

		public GroundMaterial GetMaterial(PhysicMaterial physicMaterial)
		{
			if (physicMaterial == Asphalt.PhysicsMaterial) return Asphalt;

			Debug.Log($"{physicMaterial} is not found in {nameof(GroundMaterials)}");
			return Asphalt;
        }
	}
}