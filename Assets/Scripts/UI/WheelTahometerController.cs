using BloodMeridiane.Car.Moving;
using UnityEngine;


namespace BloodMeridiane.UI
{
    public class WheelTahometerController : TahometerController
    {
        [SerializeField] private int _index;
        [SerializeField] private float _maxWheelTaho = 1f;

        protected override void CalculateNeedleRotation(CarMoveController car)
        {
            float wheelTaho = car.Wheels.GetRPM()[_index];
            float rotation = (MaxRPMNeedleRotation * wheelTaho) / _maxWheelTaho;
            Needle.rotation = Quaternion.Euler(0, 0, rotation);
            _maxWheelTaho = Mathf.Max(_maxWheelTaho, wheelTaho);
            //if (wheelTaho < _maxWheelTaho * .9f)
            //    _maxWheelTaho *= .9f;
        }

        protected override void UpdateTexts(CarMoveController car)
        {
            SpeedText.text = ((int)_maxWheelTaho).ToString();
            GearText.text = ((int)(car.Wheels.GetRPM()[_index] * Mathf.PI / 30)).ToString();
        }
    }
}
