using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Returns a random value in between the vector2 min and max values.
        /// </summary>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static float RandomValue(this Vector2 vector2)
        {
            return Random.Range(vector2.x, vector2.y);
        }
    }
}

