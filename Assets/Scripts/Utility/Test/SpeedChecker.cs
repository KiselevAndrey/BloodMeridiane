using BloodMeridiane.Car.Moving;
using UnityEngine;

namespace BloodMeridiane.Utility.Test
{
    public class SpeedChecker : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDisplayedSpeed displayedSpeed))
            {
                print($"CalculatedSpeed: {displayedSpeed.CalculatedSpeed} κμ/χ");
            }
        }
    }
}