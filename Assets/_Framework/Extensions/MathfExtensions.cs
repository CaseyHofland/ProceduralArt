using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
    public static class MathfEx
    {
        /// <summary>
        /// Returns the interpolation value of a float in between 2 other floats. Returns 1 if value1 and value2 are equal.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="valueInBetween"></param>
        /// <returns></returns>
        public static float InterpolationValue(float value1, float value2, float valueInBetween)
        {
            if (value1 == value2)
                return 1f;

            return ((valueInBetween - value1) / (value2 - value1));
        }
    }
}

