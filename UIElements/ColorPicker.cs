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

namespace BeatSaberCustomUI.UIElements
{
    public class ColorPicker : Selectable, IEventSystemHandler
    {
        public ColorPickerPreview ColorPickerPreview;
        public ColorPickerHue ColorPickerHue;
        public ColorPickerCore ColorPickerCore;

        private AssetBundle _ColorPickerBundle;

        private new void Awake()
        {
            base.Awake();
            _ColorPickerBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("BeatSaberCustomUI.Resources.ColorPicker.assetbundle"));
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> ColorPickerBundle is null ?: " + (_ColorPickerBundle == null));

            ColorPickerPreview = new GameObject("ColorPickerPreview").AddComponent<ColorPickerPreview>();
            ColorPickerPreview.transform.SetParent(transform, false);
            (ColorPickerPreview.transform as RectTransform).sizeDelta = new Vector2(15, 10);
            ColorPickerPreview.transform.Translate(-45f, 40, 0);

            ColorPickerCore = new GameObject("ColorPickerCore").AddComponent<ColorPickerCore>();
            //ColorPickerCore = GameObject.CreatePrimitive(PrimitiveType.Quad).AddComponent<ColorPickerCore>();
            ColorPickerCore.ColorPickerPreview = ColorPickerPreview;
            ColorPickerCore.ColorPickerBundle = _ColorPickerBundle;
            ColorPickerCore.Initialize();
            ColorPickerCore.transform.SetParent(transform, false);
            (ColorPickerCore.transform as RectTransform).sizeDelta = new Vector2(50, 50);

            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> Creating slider");
            HMUI.Scrollbar slider = BeatSaberUI.CreateUISlider(transform as RectTransform, 0f, 1f, false);
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> Adding component to the slider");
            ColorPickerHue = slider.gameObject.AddComponent<ColorPickerHue>();
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> Is my instance of the component created null ?: " + (ColorPickerHue == null));
            //ColorPickerHue = new GameObject("ColorPickerHue").AddComponent<ColorPickerHue>();
            ColorPickerHue.ColorPickerBundle = _ColorPickerBundle;
            ColorPickerHue.Initialize();
            (ColorPickerHue.transform as RectTransform).sizeDelta = new Vector2(50, 7.5f);
            ColorPickerHue.transform.Translate(0, 35, 0);
            Console.WriteLine("ColorPicker awake done.");
        }

        /// <summary>
        /// Get the color of a sprite contained in an <see cref="HMUI.Image"/> on pointer click
        /// </summary>
        /// <param name="pointerData">The <see cref="PointerEventData"/> given by OnPointerDown</param>
        /// <param name="image">The <see cref="HMUI.Image"/> isntance</param>
        public static Color GetSelectedColorFromImage(PointerEventData pointerData, HMUI.Image image)
        {
            RectTransform rectTransform = image.transform as RectTransform;
            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pointerData.position, pointerData.pressEventCamera, out localCursor))
                return (new Color(0, 0, 0, 0));
            localCursor.x += Math.Abs(rectTransform.rect.x);
            localCursor.y += Math.Abs(rectTransform.rect.y);
            Console.WriteLine("sprite is null: " + (image.sprite == null));
            localCursor.x *= (image.sprite.texture.width / rectTransform.rect.width);
            localCursor.y *= (image.sprite.texture.height / rectTransform.rect.height);
            if (localCursor.x < 0 || localCursor.y < 0)
                return (new Color(0, 0, 0, 0));
            Console.WriteLine("localCursor: [" + (localCursor.x) + ", " + (localCursor.y) + "]");
            Console.WriteLine("rectTransform.rect x/y: [" + (rectTransform.rect.x) + ", " + (rectTransform.rect.y) + "]");
            Console.WriteLine("rectTransform.rect width/height: [" + (rectTransform.rect.width) + ", " + (rectTransform.rect.height) + "]");
            Color c = image.sprite.texture.GetPixel((int)(localCursor.x), (int)(localCursor.y));

            Console.WriteLine("Color at position is: [" + c.r + ", " + c.g + ", " + c.b + ", " + c.a + "]");
            return c;
        }
    }
}