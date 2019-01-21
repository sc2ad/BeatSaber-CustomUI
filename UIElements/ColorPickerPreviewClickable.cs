using CustomUI.BeatSaber;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BeatSaberCustomUI.UIElements
{
    public class ColorPickerPreviewClickable : ColorPickerPreview, IEventSystemHandler
    {
        private CustomMenu _CustomMenu;
        private CustomViewController _CustomViewController;

        private ColorPicker _ColorPickerSettings;

        private new void Start()
        {
            base.Start();
            _CustomMenu = BeatSaberUI.CreateCustomMenu<CustomMenu>("Pick a color..");
            _CustomViewController = BeatSaberUI.CreateViewController<CustomViewController>();
        }

        private IEnumerator _WaitForPresenting()
        {
            yield return new WaitUntil(() => { return _CustomMenu.Present(false); });
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (_ColorPickerSettings != null)
                _ColorPickerSettings.ColorPickerPreview.ImagePreview.color = ImagePreview.color;
            _CustomMenu.SetMainViewController(_CustomViewController, true, (firstActivation, type) =>
            {
                if (firstActivation && type == VRUI.VRUIViewController.ActivationType.AddedToHierarchy)
                {
                    _ColorPickerSettings = _CustomViewController.CreateColorPicker(new Vector2(0, -10), new Vector2(0.7f, 0.7f));
                    if (_ColorPickerSettings != null)
                        _ColorPickerSettings.ColorPickerPreview.ImagePreview.color = ImagePreview.color;
                    (_ColorPickerSettings.ColorPickerHueSlider.transform as RectTransform).anchoredPosition = new Vector2(0, 35f);
                }
            });
            _CustomViewController.didDeactivateEvent += _UpdatingPreviewClickableColor;
            StartCoroutine(_WaitForPresenting());
        }

        private void _UpdatingPreviewClickableColor(VRUI.VRUIViewController.DeactivationType deactivationType)
        {
            ImagePreview.color = _ColorPickerSettings.ColorPickerPreview.ImagePreview.color;
        }
    }
}
