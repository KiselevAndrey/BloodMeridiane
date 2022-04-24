using UnityEngine;

namespace BloodMeridiane.Gun.Show
{
    public class ShowPointTarget : MonoBehaviour
    {
        [SerializeField] private GameObject _pointPrefab;
        [SerializeField] private Material _pointMaterial;

        private GameObject _point;

        private void Start()
        {
            _point = Instantiate(_pointPrefab.gameObject);
            _point.GetComponent<Renderer>().material = _pointMaterial;
            _point.SetActive(false);
        }

        private void FixedUpdate()
        {
            Ray ray = new Ray(transform.position, transform.forward);

            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                _point.transform.position = hit.point;
                _point.SetActive(true);
                //print(hit.collider.name);
            }
            else
            {
                _point.SetActive(false);
            }
        }
    }
}