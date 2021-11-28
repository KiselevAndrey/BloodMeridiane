using BloodMeridiane.Car.Moving;
using UnityEngine;
using UnityEngine.UI;

namespace BloodMeridiane.UI
{
    public class TahometerController : MonoBehaviour
    {
        [SerializeField] protected Text SpeedText;
        [SerializeField] protected Text GearText;

        [SerializeField] protected RectTransform Needle;
        [SerializeField] protected float MaxRPMNeedleRotation = -270f;

        public void UpdateTaho(CarMoveController car)
        {
            CalculateNeedleRotation(car);
            UpdateTexts(car);            
        }

        protected virtual void CalculateNeedleRotation(CarMoveController car)
        {
            float rotation = (MaxRPMNeedleRotation * car.Motor.RPM) / car.Motor.MaxRPM;
            Needle.rotation = Quaternion.Euler(0, 0, rotation);
        }

        protected virtual void UpdateTexts(CarMoveController car)
        {
            SpeedText.text = ((int)car.ControlWheelSpeed).ToString();
            GearText.text = car.GearBox.GearName;
        }
    }
}