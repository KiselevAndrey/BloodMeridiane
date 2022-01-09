using UnityEngine;

namespace BloodMeridiane.Car.Moving.Wheels
{
    [RequireComponent(typeof(WheelCollider))]
    public abstract class Wheel : MonoBehaviour, IWheel
    {
        protected WheelCollider Collider;

        public float RPM => Collider.rpm;
        public virtual float Speed => RPM * Collider.radius * Mathf.PI / 30;
        public bool IsGrounded => Collider.isGrounded;

        protected virtual void Awake()
        {
            Collider = GetComponent<WheelCollider>();
        }

        protected void Print(string str) => print($"{str}. {transform.name}");
    }
}