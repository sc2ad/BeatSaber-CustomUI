﻿using CustomUI.Utilities;
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
    public class ColorPickerPreview : Selectable, IEventSystemHandler
    {
        public HMUI.Image ImagePreview;

        private new void Awake()
        {
            base.Awake();
            ImagePreview = gameObject.AddComponent<HMUI.Image>();
            if (ImagePreview != null)
            {
                ImagePreview.material = Instantiate(Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").FirstOrDefault());
                ImagePreview.sprite = UIUtilities.RoundedRectangle;
            } else
                Console.WriteLine("[BeatSaberCustomUI.ColorPickerPreview]: The '_Image' instance was null.");
            //Console.WriteLine("[BeatSaberCustomUI.ColorPickerPreview]: ColorPickerPreview awake done.");
        }
    }
}
