using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace CustomUI.BeatSaber
{
    public class CustomViewController : VRUIViewController
    {
        public Action backButtonPressed;
        public Button _backButton;
        public bool includeBackButton;

        public Action<bool, VRUIViewController.ActivationType> DidActivateEvent;
        public Action<VRUIViewController.DeactivationType> DidDeactivateEvent;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            if (firstActivation)
            {
                if (includeBackButton && _backButton == null)
                {
                    _backButton = BeatSaberUI.CreateBackButton(rectTransform as RectTransform);
                    _backButton.onClick.AddListener(delegate ()
                    {
                        backButtonPressed?.Invoke();
                    });
                }
            }

            DidActivateEvent?.Invoke(firstActivation, type);
        }

        protected override void DidDeactivate(DeactivationType type)
        {
            DidDeactivateEvent?.Invoke(type);
        }

        public void ClearBackButtonCallbacks()
        {
            backButtonPressed = null;
        }
    }

    public class CustomCellInfo
    {
        public string text;
        public string subtext;
        public Sprite icon;

        public CustomCellInfo(string text, string subtext, Sprite icon = null)
        {
            this.text = text;
            this.subtext = subtext;
            this.icon = icon;
        }
    };
}
