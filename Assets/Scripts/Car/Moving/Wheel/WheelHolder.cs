using UnityEngine;

namespace BloodMeridiane.Car.Moving.Wheels
{
    public class WheelHolder : MonoBehaviour
    {
        [SerializeField] private Transform _wheelHolder;

        public void InitWheel(GameObject wheelObject)
        {
            if (_wheelHolder.childCount > 0)
                Destroy(_wheelHolder.GetChild(0));

            Instantiate(wheelObject, _wheelHolder);
        }
    }
}