using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomUI.UIElements
{
    public class SliderProperties : MonoBehaviour
    {
        public float FromValue;
        public float ToValue;
        public float CurrentValue;
        public float PercentageValue;
        public bool IntValues = false;

        public void SetCurrentValueFromPercentage(float percentage)
        {
            PercentageValue = percentage;
            CurrentValue = GetValueFromPercentage(percentage);
            if (IntValues)
                CurrentValue = (float)Math.Floor(CurrentValue);
        }

        public float GetValueFromPercentage(float percentage)
        {
            return ((ToValue - FromValue) * percentage + FromValue);
        }

        public float GetPercentageFromValue(float value)
        {
            return ((value - FromValue) / (ToValue - FromValue));
        }

        public float GetPercentageFromCurrentValue()
        {
            return GetPercentageFromValue(CurrentValue);
        }
    }
}
