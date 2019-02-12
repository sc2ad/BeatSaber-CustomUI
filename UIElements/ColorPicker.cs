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
        public ColorPickerCore ColorPickerCore;

        private new void Awake()
        {
            base.Awake();

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
                ColorPickerCore.Initialize();
                ColorPickerCore.transform.SetParent(transform, false);
                (ColorPickerCore.transform as RectTransform).sizeDelta = new Vector2(70, 70);
                (ColorPickerCore.transform as RectTransform).localPosition += new Vector3(0, 20);
            } else
                Console.WriteLine("[BeatSaberCustomUI.ColorPicker]: The 'ColorPickerCore' instance was null.");
            
            //Console.WriteLine("[BeatSaberCustomUI.ColorPicker]: ColorPicker awake done.");
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