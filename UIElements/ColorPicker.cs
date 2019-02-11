using CustomUI.BeatSaber;
using CustomUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomUI.UIElements
{
    public class ColorPicker : Selectable, IEventSystemHandler
    {
        public ColorPickerPreview ColorPickerPreview;
        public HMUI.Image ColorPickerHueBG;
        public HMUI.Scrollbar ColorPickerHueSlider;
        public SliderProperties HueSliderProperties;
        public ColorPickerCore ColorPickerCore;

        private AssetBundle _ColorPickerBundle;

        private new void Awake()
        {
            base.Awake();
            _ColorPickerBundle = UIUtilities.ColorPickerBundle;
            if (_ColorPickerBundle == null)
            {
                Console.WriteLine("[BeatSaberCustomUI.ColorPicker]: The loading of the 'ColorPicker.assetbundle' resulted into a failure, stopping the ColorPicker creation.");
                return;
            }

            //ColorPickerPreview initialization
            ColorPickerPreview = new GameObject("ColorPickerPreview").AddComponent<ColorPickerPreview>();
            if (ColorPickerPreview != null)
            {
                ColorPickerPreview.transform.SetParent(transform, false);
                (ColorPickerPreview.transform as RectTransform).sizeDelta = new Vector2(8.5f, 8.5f);
                ColorPickerPreview.transform.Translate(-40f, 35.5f, 0);
            } else
                Console.WriteLine("[BeatSaberCustomUI.ColorPicker]: The 'ColorPickerPreview' instance was null.");
            //ColorPickerCore initialization
            ColorPickerCore = new GameObject("ColorPickerCore").AddComponent<ColorPickerCore>();
            if (ColorPickerCore != null)
            {
                ColorPickerCore.ColorPickerPreview = ColorPickerPreview;
                ColorPickerCore.ColorPickerBundle = _ColorPickerBundle;
                ColorPickerCore.Initialize();
                ColorPickerCore.transform.SetParent(transform, false);
                (ColorPickerCore.transform as RectTransform).sizeDelta = new Vector2(50, 50);
            } else
                Console.WriteLine("[BeatSaberCustomUI.ColorPicker]: The 'ColorPickerCore' instance was null.");

            //ColorPickerHue background initialization
            ColorPickerHueBG = new GameObject("ColorPickerHueBG").AddComponent<HMUI.Image>();
            if (ColorPickerHueBG != null)
            {
                ColorPickerHueBG.material = new Material(_ColorPickerBundle.LoadAsset<Shader>("HueSlider"));
                ColorPickerHueBG.material.renderQueue = 3001;
                ColorPickerHueBG.transform.SetParent(transform, false);
                (ColorPickerHueBG.transform as RectTransform).sizeDelta = new Vector2(50, 7.5f);
                ColorPickerHueBG.transform.Translate(0, 35, 0);
            } else
                Console.WriteLine("[BeatSaberCustomUI.ColorPicker]: The 'ColorPickerHueBG' instance was null.");

            //ColorPickerHue slider initialization
            ColorPickerHueSlider = BeatSaberUI.CreateUISlider(transform as RectTransform, 0f, 1f, false, (float value) => {
                if (HueSliderProperties != null)
                    HueSliderProperties.SetCurrentValueFromPercentage(value);
                ColorPickerCore.ChangeColorPickerHue(value);
            });
            if (ColorPickerHueSlider != null)
            {
                HueSliderProperties = ColorPickerHueSlider.GetComponent<SliderProperties>();
                ColorPickerHueSlider.value = 0f;
                ColorPickerHueSlider.gameObject.name = "ColorPickerHueSlider";
                ColorPickerHueSlider.transform.SetParent(transform, false);
                (ColorPickerHueSlider.transform as RectTransform).sizeDelta = new Vector2(54, 7.5f);
                (ColorPickerHueSlider.transform as RectTransform).anchoredPosition = new Vector2(0, -2f);
                ColorPickerHueSlider.transform.Translate(0, 37f, -0.00001f);
                ColorPickerHueSlider.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                ColorPickerHueSlider.transform.Find("SlidingArea/Handle").GetComponent<Image>().color = new Color(1, 1, 1, 1);
                ColorPickerHueSlider.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
            } else
                Console.WriteLine("[BeatSaberCustomUI.ColorPicker]: The 'ColorPickerHueSlider' instance was null.");

            Console.WriteLine("[BeatSaberCustomUI.ColorPicker]: ColorPicker awake done.");
        }

        /// <summary>
        /// Get the color of a sprite contained in an <see cref="HMUI.Image"/> on pointer click
        /// </summary>
        /// <param name="pointerData">The <see cref="PointerEventData"/> given by OnPointerDown</param>
        /// <param name="image">The <see cref="HMUI.Image"/> instance</param>
        public static Color GetSelectedColorFromImage(PointerEventData pointerData, HMUI.Image image)
        {
            RectTransform rectTransform = image.transform as RectTransform;
            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pointerData.position, pointerData.pressEventCamera, out localCursor))
                return (new Color(0, 0, 0, 0));
            localCursor.x += Math.Abs(rectTransform.rect.x);
            localCursor.y += Math.Abs(rectTransform.rect.y);
            localCursor.x *= (image.sprite.texture.width / rectTransform.rect.width);
            localCursor.y *= (image.sprite.texture.height / rectTransform.rect.height);
            if (localCursor.x < 0 || localCursor.y < 0)
                return (new Color(0, 0, 0, 0));

            return (image.sprite.texture.GetPixel((int)(localCursor.x), (int)(localCursor.y)));
        }
    }
}