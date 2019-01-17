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
    public class ColorPickerPreview : Selectable, IEventSystemHandler
    {
        private HMUI.Image _Image;

        private new void Awake()
        {
            base.Awake();
            _Image = gameObject.AddComponent<HMUI.Image>();
            _Image.material = Instantiate(Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").FirstOrDefault());
            Console.WriteLine("ColorPickerPreview awake done.");
        }

        public void AssignPreviewColor(Color color)
        {
            _Image.color = color;
        }
    }
}
