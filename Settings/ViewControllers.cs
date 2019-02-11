using BeatSaberCustomUI.UIElements;
using CustomUI.BeatSaber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomUI.Settings
{
    public class BoolViewController : SwitchSettingsController
    {
        public delegate bool GetBool();
        public event GetBool GetValue;

        public delegate void SetBool(bool value);
        public event SetBool SetValue;

        public string EnabledText = "ON";
        public string DisabledText = "OFF";

        protected override bool GetInitValue()
        {
            bool value = false;
            if (GetValue != null)
            {
                value = GetValue();
            }
            return value;
        }

        protected override void ApplyValue(bool value)
        {
            if (SetValue != null)
            {
                SetValue(value);
            }
        }

        protected override string TextForValue(bool value)
        {
            return (value) ? EnabledText : DisabledText;
        }
    }

    public abstract class IntSettingsController : IncDecSettingsController
    {
        private int _value;
        protected int _min;
        protected int _max;
        protected int _increment;

        protected abstract int GetInitValue();
        protected abstract void ApplyValue(int value);
        protected abstract string TextForValue(int value);


        public override void Init()
        {
            _value = this.GetInitValue();
            this.RefreshUI();
        }
        public override void ApplySettings()
        {
            this.ApplyValue(this._value);
        }
        private void RefreshUI()
        {
            this.text = this.TextForValue(this._value);
            this.enableDec = _value > _min;
            this.enableInc = _value < _max;
        }
        public override void IncButtonPressed()
        {
            this._value += _increment;
            if (this._value > _max) this._value = _max;
            this.RefreshUI();
        }
        public override void DecButtonPressed()
        {
            this._value -= _increment;
            if (this._value < _min) this._value = _min;
            this.RefreshUI();
        }
    }

    public class IntViewController : IntSettingsController
    {
        public delegate int GetInt();
        public event GetInt GetValue;

        public delegate void SetInt(int value);
        public event SetInt SetValue;

        public void SetValues(int min, int max, int increment)
        {
            _min = min;
            _max = max;
            _increment = increment;
        }

        public void UpdateIncrement(int increment)
        {
            _increment = increment;
        }

        private int FixValue(int value)
        {
            if (value % _increment != 0)
            {
                value -= (value % _increment);
            }
            if (value > _max) value = _max;
            if (value < _min) value = _min;
            return value;
        }

        protected override int GetInitValue()
        {
            int value = 0;
            if (GetValue != null)
            {
                value = FixValue(GetValue());
            }
            return value;
        }

        protected override void ApplyValue(int value)
        {
            if (SetValue != null)
            {
                SetValue(FixValue(value));
            }
        }

        protected override string TextForValue(int value)
        {
            return value.ToString();
        }
    }
    
    public class StringViewController : ListSettingsController
    {
        public Func<string> GetValue = () => String.Empty;
        public Action<string> SetValue = (_) => { };
        public string value = String.Empty;

        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            numberOfElements = 2;
            value = GetValue();
            idx = 0;
        }

        protected override void ApplyValue(int idx)
        {
            SetValue(value);
        }

        protected override string TextForValue(int idx)
        {
            if (value != String.Empty)
                return value;
            else
                return "<color=#ffffff66>Empty</color>";
        }

        public override void IncButtonPressed()
        {
            BeatSaberUI.DisplayKeyboard("Enter Text Below", value, (text) => { }, (text) => { value = text; base.IncButtonPressed(); base.DecButtonPressed(); });
        }

        public override void DecButtonPressed()
        {
        }
    }

    public class ListViewController : ListSettingsController
    {
        public Func<float> GetValue = () => 0f;
        public Action<float> SetValue = (_) => { };
        public Func<float, string> GetTextForValue = (_) => "?";

        public delegate string StringForValue(float value);
        public event StringForValue FormatValue;

        public List<float> values = new List<float>();
        public bool applyImmediately = false;

        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            numberOfElements = values.Count();
            var value = GetValue();
            idx = values.FindIndex(v => v == value);
            if (idx == -1)
                idx = 0;
        }

        protected override void ApplyValue(int idx)
        {
            SetValue(values[idx]);
        }

        protected override string TextForValue(int idx)
        {
            if (FormatValue != null)
                return FormatValue(values[idx]);

            return GetTextForValue(values[idx]);
        }

        public override void IncButtonPressed()
        {
            base.IncButtonPressed();
            if (applyImmediately)
                ApplySettings();
        }

        public override void DecButtonPressed()
        {
            base.DecButtonPressed();
            if (applyImmediately)
                ApplySettings();
        }
    }

    public class TupleViewController<T> : ListSettingsController
    {
        public Func<T> GetValue = () => default(T);
        public Action<T> SetValue = (_) => { };
        public Func<T, string> GetTextForValue = (_) => "?";

        public List<T> values;

        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            numberOfElements = values.Count;
            var value = GetValue();

            numberOfElements = values.Count();
            idx = values.FindIndex(v => v.Equals(value));
        }

        protected override void ApplyValue(int idx)
        {
            SetValue(values[idx]);
        }

        protected override string TextForValue(int idx)
        {
            return GetTextForValue(values[idx]);
        }
    }

    public class SliderViewController : IncDecSettingsController
    {
        public delegate float GetFloat();
        public event GetFloat GetValue;

        public delegate void SetFloat(float value);
        public event SetFloat SetValue;

        private float _min;
        private float _max;
        private bool _intValues;

        private HMUI.Scrollbar _sliderInst;
        private SliderProperties _sliderPropertiesInst;
        private TMPro.TextMeshProUGUI _textInst;

        public override void Init()
        {
            _sliderInst = transform.GetComponentInChildren<HMUI.Scrollbar>();
            _sliderPropertiesInst = _sliderInst.gameObject.GetComponent<SliderProperties>();
            _sliderPropertiesInst.CurrentValue = GetInitValue();
            _textInst = _sliderInst.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            _sliderInst.value = _sliderPropertiesInst.GetPercentageFromValue(_sliderPropertiesInst.CurrentValue);
            _sliderInst.onValueChanged.AddListener(delegate (float value) {
                _sliderPropertiesInst.SetCurrentValueFromPercentage(value);
                RefreshUI();
            });
            RefreshUI();
        }

        public override void ApplySettings()
        {
            ApplyValue(_sliderPropertiesInst.CurrentValue);
        }

        private void RefreshUI()
        {
            _textInst.text = TextForValue(_sliderPropertiesInst.CurrentValue);
        }

        public override void IncButtonPressed()
        {

        }

        public override void DecButtonPressed()
        {

        }

        public void SetValues(float min, float max, bool intValues)
        {
            _min = min;
            _max = max;
            _intValues = intValues;
        }

        protected float GetInitValue()
        {
            float value = 0;
            if (GetValue == null)
                value = _min;
            else
                value = GetValue();
            return value;
        }

        protected void ApplyValue(float value)
        {
            if (SetValue != null)
                SetValue((_intValues) ? ((float)Math.Floor(value)) : (value));
        }

        protected string TextForValue(float value)
        {
            if (_intValues)
                return Math.Floor(value).ToString("N0");
            return value.ToString("N1");
        }
    }

    public class ColorPickerViewController : SimpleSettingsController
    {
        public delegate Color GetColor();
        public event GetColor GetValue;

        public delegate void SetColor(Color value);
        public event SetColor SetValue;

        private ColorPickerPreviewClickable _ColorPickerPreviewClickableInst;

        public override void Init()
        {
            _ColorPickerPreviewClickableInst = transform.GetComponentInChildren<ColorPickerPreviewClickable>();
            _ColorPickerPreviewClickableInst.ImagePreview.color = GetInitValue();
        }

        protected Color GetInitValue()
        {
            Color color = new Color(1, 1, 1, 1);
            if (GetValue != null)
                color = GetValue();
            return color;
        }

        public override void ApplySettings()
        {
            ApplyValue(_ColorPickerPreviewClickableInst.ImagePreview.color);
        }

        public override void CancelSettings()
        {
            
        }

        public void SetValues(Color color)
        {
            _ColorPickerPreviewClickableInst.ImagePreview.color = color;
        }

        protected void ApplyValue(Color color)
        {
            if (SetValue != null)
            {
                SetValue(color);
            }
        }

        public Color ValueFromText(string text)
        {
            Color c;
            if (ColorUtility.TryParseHtmlString(text, out c))
                return c;
            return new Color(1, 1, 1, 1);
        }

        public string TextForValue(Color value)
        {
            return ("#" + ColorUtility.ToHtmlStringRGBA(value));
        }
    }
}
