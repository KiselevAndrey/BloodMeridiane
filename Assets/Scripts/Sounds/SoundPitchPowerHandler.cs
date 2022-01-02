using UnityEngine;

namespace BloodMeridiane.Sounds
{
    public class SoundPitchPowerHandler : MonoBehaviour, Utility.IPowered
    {
        [SerializeField, Min(0)] private float _minPitch;
        [SerializeField, Min(0)] private float _maxPitch;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void SetPower(float lerpPower)
        {
            _audioSource.pitch = Mathf.Lerp(_minPitch, _maxPitch, lerpPower);
        }

    }
}