using UnityEngine;

namespace BloodMeridiane.Car.CenterOfMass
{
    [RequireComponent(typeof(Rigidbody))]
    public class CenterOfMass : MonoBehaviour
    {
        [SerializeField] private Transform _centerOfMass;
        [SerializeField] private bool _drawGizmos;

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.centerOfMass = Vector3.Scale(_centerOfMass.localPosition, transform.localScale);
        }

        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmos) return;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_rb.worldCenterOfMass, 0.1f);
        }
    }
}