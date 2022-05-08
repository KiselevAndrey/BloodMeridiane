using UnityEngine;

namespace BloodMeridiane.Guns
{
    public class MountingPoint : MonoBehaviour
    {
        [SerializeField, Range(0, 360f)] private float _angleLimit = 90f;
        [SerializeField, Range(0, 20f)] private float _aimTolerance = 1f;
        [SerializeField, Min(0)] private float _turnSpeed = 90f;

        private Transform _turret;

        public float AngleLimit => _angleLimit;

        private void Awake()
        {
            _turret = transform.GetChild(0);
        }

        public bool Aim(Vector3 targetPoint)
        {
            var los = targetPoint - transform.position;
            var halfAngle = _angleLimit / 2;
            var losOnPlane = Vector3.ProjectOnPlane(los, transform.up);
            var deltaAngle = Vector3.SignedAngle(transform.forward, losOnPlane, transform.up);

            var reachAngleLimit = false;
            if (Mathf.Abs(deltaAngle) > halfAngle)
            {
                reachAngleLimit = true;
                losOnPlane = transform.rotation * Quaternion.Euler(0, Mathf.Clamp(deltaAngle, -halfAngle, halfAngle), 0) * Vector3.forward;
            }

            var targetRotation = Quaternion.LookRotation(losOnPlane, transform.up);
            var aimed = reachAngleLimit == false && Quaternion.Angle(_turret.rotation, targetRotation) < _aimTolerance;
            _turret.rotation = Quaternion.RotateTowards(_turret.rotation, targetRotation, _turnSpeed * Time.deltaTime);

            return aimed;
        }

        public void SetAngle(float angle)
        {
            angle = Mathf.Clamp(angle, 0f, 360f);
            _angleLimit = angle;
        }

        public void SetAimTolerance(float tolerance)
        {
            tolerance = Mathf.Clamp(tolerance, 0, 20f);
            _aimTolerance = tolerance;
        }

        public void SetTurnSpeed(float speed)
        {
            speed = Mathf.Abs(speed);
            _turnSpeed = speed;
        }
    }
}