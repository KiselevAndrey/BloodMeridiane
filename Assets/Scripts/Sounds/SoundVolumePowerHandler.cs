using UnityEngine;

namespace BloodMeridiane.Sounds
{
    public class SoundVolumePowerHandler : MonoBehaviour, Utility.IPowered
    {
        [SerializeField, Min(0)] private float _minVolume;
        [SerializeField, Min(0)] private float _maxVolume;
        [SerializeField, Range(0, 0.1f)] private float _maxVolumBoost = 0.1f;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void SetPower(float lerpPower)
        {
            var volume = Mathf.Lerp(_minVolume, _maxVolume, lerpPower);
            if(volume - _maxVolumBoost > _audioSource.volume)
            {
                volume = _audioSource.volume + _maxVolumBoost;
            }

            _audioSource.volume = volume;
        }

    }
}