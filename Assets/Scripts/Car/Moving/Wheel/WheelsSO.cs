using UnityEngine;

namespace BloodMeridiane.Car.Moving.Wheels
{
    [CreateAssetMenu(menuName = nameof(Car) + "/" + nameof(Moving) + "/" + nameof(WheelsSO))]
    public class WheelsSO : ScriptableObject
    {
        [SerializeField] private GameObject _wheelObject;
        [SerializeField, Range(10, 1000)] private int _mass;
        [SerializeField, Range(0.2f, 1f)] private float _radius;
        [SerializeField, Range(.0f, 1f)] private float _TCSStrength = 1f;
        [SerializeField, Min(0)] private float _breakForce = 1000f;

        [SerializeField, Space] private Map.GroundMaterials _groundMaterials;

        public GameObject WheelObject => _wheelObject;
        public int Mass => _mass;
        public float Radius => _radius;
        public float TCSStrength => _TCSStrength;
        public float BreakForce => _breakForce;
        public Map.GroundMaterials GroundMaterials => _groundMaterials;
    }
}