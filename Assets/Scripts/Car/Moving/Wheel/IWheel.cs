using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodMeridiane.Car.Moving.Wheel
{
    public interface IWheel 
    {
        public float RPM { get; }

        public float Speed { get; }
    }
}
