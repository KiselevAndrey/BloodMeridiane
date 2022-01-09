using UnityEngine;

namespace BloodMeridiane.ParticleSystems
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticlePowerHandler : MonoBehaviour, Utility.IPowered
    {
        [SerializeField] private int _minParticleEmission;
        [SerializeField] private int _maxParticleEmission;

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