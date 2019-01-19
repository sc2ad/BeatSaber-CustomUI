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
            if (_Image != null)
            {
                _Image.material = Instantiate(Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").FirstOrDefault());
                _Image.sprite = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "CreditsButton"))
                                                                                    .transform.Find("Wrapper/BG")
                                                                                    .GetComponent<HMUI.Image>().sprite);
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
            if (_Image != null)
                _Image.color = color;
        }
    }
}
