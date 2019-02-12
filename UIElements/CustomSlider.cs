using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomUI.UIElements
{
    public class CustomSlider : MonoBehaviour
    {
        public HMUI.Scrollbar Scrollbar;
        public float MinValue;
        public float MaxValue;
        public float CurrentValue;
        public bool IsIntValue = false;

        public void SetCurrentValueFromPercentage(float percentage)
        {
            CurrentValue = GetValueFromPercentage(percentage);
            if (IsIntValue)
                CurrentValue = (int)CurrentValue;
        }

        public float GetValueFromPercentage(float percentage)
        {
            return ((MaxValue - MinValue) * percentage + MinValue);
        }

        public float GetPercentageFromValue(float value)
        {
            return ((value - MinValue) / (MaxValue - MinValue));
        }

        public float GetPercentageFromCurrentValue()
        {
            return GetPercentageFromValue(CurrentValue);
        }
    }
}
