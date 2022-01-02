using UnityEngine;

namespace BloodMeridiane.Camera
{
    public class CameraFollower : MonoBehaviour
    {
        [SerializeField] private float _smooth = 0.3f;
        [SerializeField, Min(0)] private float _minDistance = 1.0f;
        [SerializeField, Min(0)] private float _maxDistance = 10.0f;
        [SerializeField] private float _height = 1.0f;
        [SerializeField] private float _angle = 20;
        [SerializeField] private LayerMask _lineOfSightMask;

        private CameraTarget _target;

        private float _yVelocity = 0.0f;
        private float _distance;

        private void Awake()
        {
            var targets = FindObjectsOfType<CameraTarget>();

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].gameObject.activeSelf)
                {
                    _target = targets[i];
                    break;
                }
            }

            if (_target == null)
            {
                Debug.Log("Camera Target not found!");
                Destroy(this);
            }
        }

        private void Start()
        {
            _distance = Mathf.Lerp(_minDistance, _maxDistance, 0.5f);
        }

        private void LateUpdate()
        {

            // Damp angle from current y-angle towards target y-angle

            //float xAngle = Mathf.SmoothDampAngle(transform.eulerAngles.x, _target.transform.eulerAngles.x + Angle, ref _xVelocity, smooth);

            float yAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _target.transform.eulerAngles.y, ref _yVelocity, _smooth);

            // Look at the target
            transform.eulerAngles = new Vector3(_angle, yAngle, 0.0f);

            var direction = transform.rotation * -Vector3.forward;
            var targetDistance = AdjustLineOfSight(_target.transform.position + new Vector3(0, _height, 0), direction);


            transform.position = _target.transform.position + new Vector3(0, _height, 0) + direction * targetDistance;

        }

        private float AdjustLineOfSight(Vector3 target, Vector3 direction)
        {
            RaycastHit hit;

            if (Physics.Raycast(target, direction, out hit, _distance, _lineOfSightMask.value))
                return hit.distance;
            else
                return _distance;
        }
    }
}
