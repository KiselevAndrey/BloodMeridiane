using System.Collections;
using UnityEngine;

namespace BloodMeridiane.Guns
{
    [RequireComponent(typeof(Rigidbody))]
    public class Ammo : MonoBehaviour
    {
        //public GameObject hitPrefab;
        //public GameObject muzzlePrefab;
        [SerializeField, Min(0)] private float _lifeTime;
        
        private Rigidbody _rb;
        private Coroutine _lifeCoroutine;

        private void Awake()
        {
            TryGetComponent(out _rb);
        }

        private void OnEnable()
        {
            if (_lifeCoroutine != null)
                StopCoroutine(_lifeCoroutine);

            _lifeCoroutine = StartCoroutine(LivingTime());
        }

        private IEnumerator LivingTime()
        {
            yield return new WaitForSeconds(_lifeTime);

            KAP.Pool.Pool.Despawn(gameObject);
            print("Despawn ammo life time");
        }

        private void OnTriggerEnter(Collider other)
        {
            //var hitEffect = Instantiate(hitPrefab, other.GetContact(0).point, Quaternion.identity);
            //Destroy(hitEffect, 5f);
            print(other.gameObject);
            KAP.Pool.Pool.Despawn(gameObject);
            print("Despawn ammo Trigger");
        }

        public void SetSpeed(float speed)
        {
            speed = Mathf.Abs(speed);
            _rb.velocity = transform.forward * speed;
        }
    }
}