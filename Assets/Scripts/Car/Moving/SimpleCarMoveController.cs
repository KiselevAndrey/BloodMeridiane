using System.Collections;
using UnityEngine;

namespace BloodMeridiane.Car.Moving
{
    public class SimpleCarMoveController : MonoBehaviour, ICarMoveController
    {
        protected Rigidbody Rigidbody;

        private bool _canRest = true;

        public Transform Transform => transform;

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        public virtual void ApplyForce(float verticalAxis) { }

        public virtual void ApplySteer(float horizontalAxis) { }

        #region Rest
        public void RestCar()
        {
            if (_canRest)
            {
                Rigidbody.AddForce(Vector3.up * Rigidbody.mass * 300);
                Rigidbody.MoveRotation(Quaternion.Euler(0, transform.eulerAngles.y, 0));
                StartCoroutine(RestCooldown());
            }
        }

        private IEnumerator RestCooldown()
        {
            _canRest = false;
            yield return new WaitForSeconds(2f);
            _canRest = true;
        }
        #endregion
    }
}