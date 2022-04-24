using BloodMeridiane.Camera;
using UnityEngine;

namespace BloodMeridiane.Gun
{
    public class TurretRotateHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CameraViewPoint _cameraViewPoint;
        [SerializeField] private Transform _base;
        [SerializeField] private Transform _barrel;
        [SerializeField] private Transform _firePoint;

        [Header("Parameters")]
        [SerializeField, Min(0)] private float _horizontalRotationSpeed;
        [SerializeField, Min(0)] private float _verticalrotationSpeed;
        [SerializeField] private Vector2 _verticalAngles;

        private Vector3 _directionToTarget;

        private Vector3 _firePosition => _firePoint.position;

        private void Update()
        {
            _directionToTarget = _cameraViewPoint.PointPosition - _barrel.transform.position;

        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_firePosition, _firePosition + _firePoint.forward);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(_barrel.position, _barrel.position + _directionToTarget);
        }
    }
}