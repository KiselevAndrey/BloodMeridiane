using System.Collections;
using UnityEngine;

namespace BloodMeridiane.Guns
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] private Ammo _ammoPrefab;
        [SerializeField] private GameObject _firePrefab;
        [SerializeField] private Transform[] _firePoints;
        [SerializeField, Min(0)] private float _shootRate;
        [SerializeField, Min(0)] private float _reloadTime;
        [SerializeField, Min(0)] private int _clipCapacity;
        [SerializeField, Min(0)] private float _ammoSpeed;
        [SerializeField, Tooltip("Одновременный выстрел из всех стволов")]
        private bool _isShootingInOneVolley;
        [SerializeField] private bool _waitShotEndBeforeReload;

        [Header("Show Parameters")]
        [SerializeField] private int _ammoInClip;
        [SerializeField] private bool _isReloading;
        [SerializeField] private bool _isShooting;
        [SerializeField] private bool _isPermissionToShoot;

        private Coroutine _reloading;

        private int _firePointIndex;

        private bool CanShoot =>
            _isReloading == false &&
            _isShooting == false &&
            _isPermissionToShoot == true;

        private void Awake()
        {
            _ammoInClip = _clipCapacity;
        }

        private void Update()
        {
            if (CanShoot == false)
                return;

            Shooting();
        }

        private void Shooting()
        {
            _isShooting = true;

            if (_isShootingInOneVolley)
            {
                foreach (var point in _firePoints)
                {
                    SpawnBullet(point);
                }
            }
            else
            {
                var firePoint = _firePoints[_firePointIndex++];
                _firePointIndex %= _firePoints.Length;
                SpawnBullet(firePoint);
            }

            _isPermissionToShoot = false;

            StartCoroutine(ShootingRate());
        }

        private void SpawnBullet(Transform firePoint)
        {
            if (_ammoInClip > 0)
            {
                var ammo = KAP.Pool.Pool.Spawn(_ammoPrefab, firePoint.position, firePoint.rotation);
                ammo.SetSpeed(_ammoSpeed);
                print("Spawn ammo");
            }

            if(--_ammoInClip <= 0)
                Reload(_waitShotEndBeforeReload);
        }

        private IEnumerator ShootingRate()
        {
            yield return new WaitForSeconds(_shootRate);

            _isShooting = false;
        }

        public void Fire()
        {
            _isPermissionToShoot = true && _isReloading == false;
        }

        public void Reload(bool waitEndShoot = true)
        {
            if (_reloading == null)
                _reloading = StartCoroutine(Reloading(waitEndShoot));
        }

        private IEnumerator Reloading(bool waitEndShoot)
        {
            _isReloading = true;
            _isPermissionToShoot = false;

            // ожидание окончания выстрела
            while (_isShooting && waitEndShoot)
            {
                yield return null;
            }


            yield return new WaitForSeconds(_reloadTime);

            _isReloading = false;
            _ammoInClip = _clipCapacity;
            _reloading = null;
        }
    }
}