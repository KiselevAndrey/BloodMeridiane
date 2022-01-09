using BloodMeridiane.Utility;
using UnityEngine;

namespace BloodMeridiane.Camera
{
    public class CameraFollower : MonoBehaviour
    {
        [Header("Rotation Parameters")]
        [SerializeField, Min(0)] private float _rotateSpeed = 5f;
        [SerializeField, Range(-90, 0)] private float _minAngleX = -70;
        [SerializeField, Range(0, 90)] private float _maxAngleX = 80;
        [Header("Scroll Parameters")]
        [SerializeField, Range(0, 5)] private float _minDistance = 1.0f;
        [SerializeField, Range(5, 20)] private float _maxDistance = 10.0f;
        [SerializeField, Min(0)] private float _scrollSpeed = 5f;

        [Header("Anothers Parameters")]
        [SerializeField] private LayerMask _lineOfSightMask;

        private CameraTarget _target;
        private UnityEngine.Camera _camera;

        private float _oldCameraDistance;
        private float _xAngle;
        private float _yAngle;

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

            _camera = GetComponentInChildren<UnityEngine.Camera>();
        }

        private void Start()
        {
            _oldCameraDistance = Mathf.Lerp(_minDistance, _maxDistance, 0.5f);
            Cursor.lockState = CursorLockMode.Locked;

            _xAngle = transform.eulerAngles.x;
            _yAngle = transform.eulerAngles.y;
        }

        private void LateUpdate()
        {
            transform.position = _target.transform.position + new Vector3(0, _target.CameraUpDistance, 0);
            Rotate();
            ChangeCameraDistance();
        }

        private void Rotate()
        {
            float mouseX = Input.GetAxis(InputAxis.MouseX);
            float mouseY = Input.GetAxis(InputAxis.MouseY);

            if (mouseX != 0)
            {
                _xAngle = Mathf.Lerp(_xAngle, _xAngle - mouseY, _rotateSpeed * Time.deltaTime);
                _xAngle = Mathf.Clamp(_xAngle, _minAngleX, _maxAngleX);
            }

            if (mouseY != 0)
            {
                _yAngle = Mathf.Lerp(_yAngle, _yAngle + mouseX, _rotateSpeed * Time.deltaTime);
            }

            // Look at the target
            transform.eulerAngles = new Vector3(_xAngle, _yAngle, 0.0f);
        }

        private void ChangeCameraDistance()
        {
            float scroll = Input.GetAxis(InputAxis.MouseScrollWheel);

            if(scroll != 0)
            {
                _oldCameraDistance = Mathf.Lerp(_oldCameraDistance, _oldCameraDistance - scroll * _scrollSpeed, Time.deltaTime);
                _oldCameraDistance = Mathf.Clamp(_oldCameraDistance, _minDistance, _maxDistance);
            }

            var direction = transform.rotation * -Vector3.forward;
            var cameraDistance = AdjustLineOfSight(transform.position, direction);

            _camera.transform.position = transform.position + direction * cameraDistance;
        }

        private float AdjustLineOfSight(Vector3 target, Vector3 direction)
        {
            if (Physics.Raycast(target, direction, out RaycastHit hit, _oldCameraDistance, _lineOfSightMask.value))
                return hit.distance - 0.1f;
            else
                return _oldCameraDistance;
        }
    }
}
