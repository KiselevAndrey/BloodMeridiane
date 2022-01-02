using UnityEngine;

namespace BloodMeridiane.Utility
{
    public class PowersHandler : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float _power = 0.1f;
        
        private IPowered[] _powerHandlers;

        private void Awake()
        {
            _powerHandlers = GetComponentsInChildren<IPowered>();
        }

        private void Start()
        {
            SetPower(0f);
        }

        public void SetPower(float power)
        {
            power = Mathf.Clamp01(power);

            if (_power == power) return;

            _power = power;

            foreach (var particle in _powerHandlers)
            {
                particle.SetPower(power);
            }
        }
    }
}
