using ССP.Tools.Constants;
using System;
using UnityEngine;

namespace ССP.Tools
{
    public static class Fixer
    {
        // Correction of not valid value
        public static float FixValue(float initialValue, float minValue, float maxValue)
        {
            float fixedValue = initialValue;

            if (initialValue > maxValue)
            {
                fixedValue = maxValue;
            }
            else
            if (initialValue < minValue)
            {
                fixedValue = minValue;
            }

            return fixedValue;
        }

        public static void FixAngle(ref float angle)
        {
            if (angle > Angles.Turnover)
            {
                angle -= Angles.Turnover;
            }
            else 
            if (angle < 0)
            {
                angle += Angles.Turnover;
            }
        }

        // Fix of value that bigger than valid maximum limit
        public static float NormalizeValue(float value, float maxValue)
        {
            if (Mathf.Abs(value) > maxValue)
            {
                value = maxValue * Mathf.Sign(value);
            }

            return value;
        }
       
    }
}
