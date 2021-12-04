using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KAP.Extension
{
    public static class MathExtension
    {
        public static int Sign(this int value) => ((float)value).Sign();
        public static int Sign(this float value) => value < 0 ? -1 : (value > 0 ? 1 : 0);
    }
}