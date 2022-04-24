using UnityEngine;

namespace BloodMeridiane.Camera
{
    public class CameraViewPoint : MonoBehaviour
    {
        [SerializeField] private bool _showPoint;
        [SerializeField] private GameObject _pointPrefab;
        [SerializeField] private Material _pointMaterial;
        [SerializeField, Min(0)] private float _distanceIfNoHit = 20;

        private GameObject _point;
        private CameraFollower _camera;

        public Vector3 PointPosition { get; private set; }

        private void Start()
        {
            _point = Instantiate(_pointPrefab.gameObject);
            _point.GetComponent<Renderer>().material = _pointMaterial;
            _point.SetActive(false);
            _camera = UnityEngine.Camera.main. GetComponentInParent<CameraFollower>();
        }

        private void Update()
        {
            Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (_showPoint)
                {
                    _point.transform.position = hit.point;
                    _point.SetActive(true);
                }

                PointPosition = hit.point;
                //print(hit.collider.name);
            }
            else
            {
                PointPosition = _camera.transform.position + _camera.transform.forward * _distanceIfNoHit;

                if(_showPoint)
                {
                    _point.transform.position = PointPosition;
                    _point.SetActive(true);
                }
                else
                    _point.SetActive(false);
            }
        }
    }
}