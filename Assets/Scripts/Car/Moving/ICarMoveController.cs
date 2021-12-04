using UnityEngine;

namespace BloodMeridiane.Car.Moving
{
    public interface ICarMoveController
    {
        public Transform Transform { get; }

        public void ApplyForce(float verticalAxis);
        public void ApplySteer(float horizontalAxis);
        public void ApplyBreak(float breakAxis);

        public void RestCar();
    }
}