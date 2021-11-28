using UnityEngine;

namespace BloodMeridiane.Camera
{
    public class CameraFollowerEasy : MonoBehaviour
    {
        [SerializeField] private Vector3 _offset;
        [SerializeField] private float _translationSpeed = 10f;
        [SerializeField] private float _rotationSpeed = 5f;

        private CameraTarget _target;

        private void Awake()
        {
            var targets = FindObjectsOfType<CameraTarget>();

            for (int i = 0; i < targets.Length; i++)
            {
                if(targets[i].gameObject.activeSelf)
                {
                    _target = targets[i];
                    break;
                }
            }
        }

        private void LateUpdate()
        {
            HandleTranslation();
            HandleRotation();
        }

        private void HandleRotation()
        {
            Vector3 targetPosition = _target.transform.TransformPoint(_offset);
            transform.position = Vector3.Lerp(transform.position, targetPosition, _translationSpeed * Time.deltaTime);
        }

        private void HandleTranslation()
        {
            Vector3 direction = _target.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
        }
    }
}