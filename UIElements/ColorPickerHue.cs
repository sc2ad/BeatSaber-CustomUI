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
    public class ColorPickerHue : Selectable, IEventSystemHandler
    {
        public AssetBundle ColorPickerBundle;

        private HMUI.Image _Image;
        private PointerEventData _PointerData;

        private new void Awake()
        {
            base.Awake();
            Console.WriteLine("ColorPickerHue awake done.");
        }

        public void Initialize()
        {
            _Image = gameObject.AddComponent<HMUI.Image>();
            _Image.material = Instantiate(Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").FirstOrDefault());
            _Image.material.shader = ColorPickerBundle.LoadAsset<Shader>("HueSlider");
            Console.WriteLine("ColorPickerHue initialized.");
        }

        private void Update()
        {
            //if (_PointerData != null)
            //    ColorPicker.GetSelectedColorFromImage(GetComponent<RectTransform>(), _PointerData, _Image);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            _PointerData = eventData;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            _PointerData = null;
        }
    }
}
