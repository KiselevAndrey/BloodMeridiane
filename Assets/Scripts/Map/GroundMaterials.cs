using UnityEngine;

namespace BloodMeridiane.Map
{
	[CreateAssetMenu(menuName = nameof(Map) + "/" + nameof(GroundMaterials))]
	[System.Serializable]
	public class GroundMaterials : ScriptableObject
	{
		#region singleton
		private static GroundMaterials _instance;
		public static GroundMaterials Instance
		{
			get
			{
				if (_instance == null)
					_instance = Resources.Load("SO/MAP/GroundMaterials") as GroundMaterials;
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
}