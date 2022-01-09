using UnityEngine;

namespace BloodMeridiane.Camera
{
    public class CameraTarget : MonoBehaviour 
    {
        [SerializeField, Min(0)] private float _cameraUpDistance = 1f;

        public float CameraUpDistance => _cameraUpDistance;
    }
}