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
        public HMUI.Image Image;

        private new void Awake()
        {
            base.Awake();
            Image = gameObject.AddComponent<HMUI.Image>();
            if (Image != null)
            {
                Image.material = Instantiate(Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").FirstOrDefault());
                Image.sprite = UIUtilities.RoundedRectangle;
            }
            else
                Console.WriteLine("[BeatSaberCustomUI.ColorPickerPreview]: The '_Image' instance was null.");
            Console.WriteLine("[BeatSaberCustomUI.ColorPickerPreview]: ColorPickerPreview awake done.");
        }

        /// <summary>
        /// Assign to the <see cref="HMUI.Image"/> instance the given <see cref="Color"/>
        /// </summary>
        /// <param name="color">A color</param>
        public void AssignPreviewColor(Color color)
        {
            if (Image != null)
                Image.color = color;
        }
    }
}
