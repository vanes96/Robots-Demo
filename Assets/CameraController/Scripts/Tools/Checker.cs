using ССP.Tools.States;
using System;
using UnityEngine;
using ССP.Tools.Loggers;
using ССP.Tools.Constants;

namespace ССP.Tools
{
    // All methods return bool values
    public static class Checker
    {
        public static bool IsFunctionEnabled(float functionValue, bool smooth = false)
        {
            if (smooth)
            {
                return IsValuePositive(functionValue - Limits.MinSmoothing, true);
            }
            else
            {
                return functionValue > Limits.MinSpeed;
            }
        }

        public static bool IsDeltaEnough(float delta, CameraState cameraState)
        {
            bool isDeltaEnough = false;

            switch (cameraState)
            {
                case CameraState.Rotating:
                    if (Mathf.Abs(delta) > Errors.DeltaRotationError)
                        isDeltaEnough = true;
                    break;
                case CameraState.Lifting:
                    if (Mathf.Abs(delta) > Errors.DeltaHeightError)
                        isDeltaEnough = true;
                    break;
                case CameraState.Zooming:
                    if (Mathf.Abs(delta) > Errors.DeltaDistanceError)
                        isDeltaEnough = true;
                    break;
                case CameraState.Following:
                    if (Mathf.Abs(delta) > Errors.DeltaPositionError)
                        isDeltaEnough = true;
                    break;
            }

            return isDeltaEnough;
        }

        public static bool IsValuePositive(float value, bool toAbsolute = false)
        {
            value = toAbsolute ? Mathf.Abs(value) : value;

            return value > Limits.MinSliderValue;
        }

        public static bool IsValuePositive(int value, bool toAbsolute = false)
        {
            value = toAbsolute ? Mathf.Abs(value) : value;

            return value > Limits.MinSliderValue;
        }
    }
}
