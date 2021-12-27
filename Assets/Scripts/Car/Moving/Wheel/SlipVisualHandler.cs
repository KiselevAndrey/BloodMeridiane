using BloodMeridiane.ParticleSystems;
using UnityEngine;

namespace BloodMeridiane.Car.Moving.Wheels
{
    public class SlipVisualHandler : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float _power;
        
        private ParticlePowerHandler[] _particles;

        private void Awake()
        {
            _particles = GetComponentsInChildren<ParticlePowerHandler>();
        }

        public void SetPower(float power)
        {
            power = Mathf.Clamp01(power);
            _power = power;

            foreach (var particle in _particles)
            {
                particle.SetPower(power);
            }
        }
    }
}
