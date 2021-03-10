using ССP.Tools.Constants;
using ССP.Tools.Loggers;
using System;
using UnityEngine;

namespace ССP.Tools
{
    // All methods return float/int value
    public static class Converter
    {
        // Conversion of value (dragging) according to aspect ratio of game window
        public static float ConvertByAspectRatio(float value)
        {
            return value * Screen.height / Screen.width;
        }

        // Conversion of int slider value to float field
        //public static void ConvertSliderValueToField(ref float currentFieldValue, int currentSliderValue, float maxFieldValue, float minFieldValue = Consts.MinSliderValue,
        //                                             float minSliderValue = Consts.MinSliderValue, float maxSliderValue = Consts.MaxSliderValue)
        //{
        //    currentFieldValue = minFieldValue + (maxFieldValue - minFieldValue) * (currentSliderValue - minSliderValue) / (maxSliderValue - minSliderValue);
        //}

        // Conversion of float slider value to float field
        public static float ConvertSliderValueToField(float currentSliderValue, float maxFieldValue, float minFieldValue = Limits.MinSliderValue,
                                                     float minSliderValue = Limits.MinSliderValue, float maxSliderValue = Limits.MaxSliderValue)
        {           
            return minFieldValue + (maxFieldValue - minFieldValue) * (currentSliderValue - minSliderValue) / (maxSliderValue - minSliderValue);
        }

        // Conversion of initial float field to int slider value
        //public static void ConvertFieldToSliderValue(ref int currentSliderValue, float currentFieldValue, float maxFieldValue, float minFieldValue = Consts.MinSliderValue,
        //                                             float minSliderValue = Consts.MinSliderValue, float maxSliderValue = Consts.MaxSliderValue)
        //{
        //    currentSliderValue = Mathf.RoundToInt(minSliderValue + (maxSliderValue - minSliderValue) * (currentFieldValue - minFieldValue) / (maxFieldValue - minFieldValue));
        //}

        // Conversion of initial float field to float slider value
        public static float ConvertFieldToSliderValue(float currentFieldValue, float maxFieldValue, float minFieldValue = Limits.MinSliderValue,
                                                     float minSliderValue = Limits.MinSliderValue, float maxSliderValue = Limits.MaxSliderValue)
        {
            return (float)Math.Round(minSliderValue + (maxSliderValue - minSliderValue) * (currentFieldValue - minFieldValue) / (maxFieldValue - minFieldValue), 2);
        }

        public static float ConvertInitialDeltaToResult(float initialDelta, float controllingSpeed, float? deltaTime = null)
        {
            if (deltaTime == null)
            {
                deltaTime = 1;
            }

            return initialDelta * controllingSpeed * deltaTime.Value;
        }
    }
}
