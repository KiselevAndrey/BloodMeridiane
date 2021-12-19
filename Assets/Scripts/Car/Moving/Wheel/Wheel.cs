using UnityEngine;

namespace BloodMeridiane.Car.Moving.Wheels
{
    [RequireComponent(typeof(WheelCollider))]
    public class Wheel : MonoBehaviour, IWheel
    {
        protected WheelCollider Collider;

        public float RPM => Collider.rpm;

        public virtual float Speed => RPM * Collider.radius * Mathf.PI / 30;

        protected virtual void Awake()
        {
            Collider = GetComponent<WheelCollider>();
        }
    }
}