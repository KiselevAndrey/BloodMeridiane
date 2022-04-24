namespace BloodMeridiane.Car.Moving
{
    public interface IDisplayedSpeed
    {
        public float CalculatedSpeed { get; }
        public float CalculatedPoweredWheelSpeed { get; }
        public int ControlWheelSpeed { get; }
    }
}