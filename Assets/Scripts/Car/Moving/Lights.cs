using UnityEngine;

namespace BloodMeridiane.Car.Moving
{
    public class Lights : MonoBehaviour
    {
        [SerializeField] private GameObject _breakLigths;
        [SerializeField] private GameObject _reverceLights;

        private void Start()
        {
            EnableBreakLights(false);
            EnableReverceLigths(false);
        }

        public void EnableBreakLights(bool value) => _breakLigths.SetActive(value);
        public void EnableReverceLigths(bool value) => _reverceLights.SetActive(value);
    }
}