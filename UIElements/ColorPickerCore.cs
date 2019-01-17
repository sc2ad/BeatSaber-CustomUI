using CustomUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BeatSaberCustomUI.UIElements
{
    public class ColorPickerCore : Selectable, IEventSystemHandler
    {
        public AssetBundle ColorPickerBundle;
        public ColorPickerPreview ColorPickerPreview;

        private HMUI.Image _Image;
        private PointerEventData _PointerData;

        private new void Awake()
        {
            base.Awake();
            _Image = gameObject.AddComponent<HMUI.Image>();
            Console.WriteLine("ColorPickerCore awake done.");
        }

        public void Initialize()
        {
            //_Image.material = Instantiate(Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").FirstOrDefault());
            //_Image.sprite = CustomUI.Utilities.UIUtilities.RadialColorPicker;
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> ColorPickerBundle is null ?: " + (ColorPickerBundle == null));
            _Image.material = new Material(ColorPickerBundle.LoadAsset<Shader>("HueShift"));
            _Image.material.SetFloat("_Hue", 0.5f);
            _Image.sprite = UIUtilities.ColorPickerBase;
            _Image.material.SetTexture("_MainTex", _Image.sprite.texture);
            Console.WriteLine("ColorPickerCore initialized.");
        }

        private void Update()
        {
            if (_PointerData != null)
            {
                Color c = ColorPicker.GetSelectedColorFromImage(_PointerData, _Image);
                if (c.r != 0 && c.g != 0 && c.b != 0 && c.a != 0)
                {
                    float H, S, V;
                    Color.RGBToHSV(c, out H, out S, out V);
                    H = 0.5f;
                    Color output = Color.HSVToRGB(H, S, V);
                    ColorPickerPreview.GetComponent<HMUI.Image>().color = output;
                    Console.WriteLine("Color applied to the preview.");
                }
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _PointerData = eventData;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            _PointerData = null;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _PointerData = null;
        }
    }
}
