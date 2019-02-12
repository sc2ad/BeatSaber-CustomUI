using CustomUI.Utilities;
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
    public class ColorPickerCore : Selectable, IEventSystemHandler
    {
        public ColorPickerPreview ColorPickerPreview;

        private HMUI.Image _Image;
        private PointerEventData _PointerData;
        private float _HueValue = 0f;

        /// <summary>
        /// Initialize the <see cref="ColorPickerCore"/> (should be called after assigning the <see cref="ColorPickerPreview"/> variable)
        /// </summary>
        public void Initialize()
        {
            _Image = gameObject.AddComponent<HMUI.Image>();
            if (_Image != null)
            {
                _Image.material = new Material(UIUtilities.NoGlowMaterial);
                _Image.sprite = UIUtilities.ColorPickerBase;
                _Image.sprite.texture.wrapMode = TextureWrapMode.Clamp;
                _Image.material.SetTexture("_MainTex", _Image.sprite.texture);
            } else
                Console.WriteLine("[BeatSaberCustomUI.ColorPickerCore]: The '_Image' instance was null.");
            //Console.WriteLine("[BeatSaberCustomUI.ColorPickerCore]: ColorPickerCore initialized.");
        }

        private void Update()
        {
            if (_PointerData != null)
                ColorPickerPreview.GetComponent<HMUI.Image>().color = ColorPicker.GetSelectedColorFromImage(_PointerData, _Image);
        }

        /// <summary>
        /// Get new color by applying an hue value to it
        /// </summary>
        /// <param name="color">The <see cref="Color"/></param>
        /// <param name="hueValue">A value between 0 and 1 corresponding to the hue</param>
        /// <returns>A new <see cref="Color"/> with the hue applied</returns>
        public Color GetCorrectColorFromHue(Color color, float hueValue)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            h = hueValue;
            return (Color.HSVToRGB(h, s, v));
        }

        /// <summary>
        /// Change the hue value located in the Shader
        /// </summary>
        /// <param name="hueValue">A value between 0 and 1 corresponding to the hue</param>
        public void ChangeColorPickerHue(float hueValue)
        {
            if (_Image != null)
            {
                _HueValue = hueValue;
                _Image.material.SetFloat("_Hue", hueValue);
            }
        }

        /// <summary>
        /// Method called when the pointer is clicked inside the <see cref="ColorPickerCore"/>
        /// </summary>
        /// <param name="eventData">Some informations about the pointer</param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _PointerData = eventData;
        }

        /// <summary>
        /// Method called when the pointer is released inside the <see cref="ColorPickerCore"/>
        /// </summary>
        /// <param name="eventData">Some informations about the pointer</param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            _PointerData = null;
        }

        /// <summary>
        /// Method called when the pointer is exiting the <see cref="ColorPickerCore"/>
        /// </summary>
        /// <param name="eventData">Some informations about the pointer</param>
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _PointerData = null;
        }
    }
}
