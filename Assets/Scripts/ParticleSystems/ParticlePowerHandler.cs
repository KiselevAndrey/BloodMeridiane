using UnityEngine;

namespace BloodMeridiane.ParticleSystems
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticlePowerHandler : MonoBehaviour
    {
        [SerializeField, Min(0)] private int _minParticleEmission;
        [SerializeField, Min(0)] private int _maxParticleEmission;

        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        public void SetPower(float lerpPower)
        {
            var emission = _particleSystem.emission;
            emission.rateOverTime = Mathf.Lerp(_minParticleEmission, _maxParticleEmission, lerpPower);
        }
    }
}