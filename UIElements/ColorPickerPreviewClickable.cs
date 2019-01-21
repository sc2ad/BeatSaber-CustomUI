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

        private new void Start()
        {
            base.Start();
            _CustomMenu = BeatSaberUI.CreateCustomMenu<CustomMenu>("Pick a color..");
            _CustomViewController = BeatSaberUI.CreateViewController<CustomViewController>();
        }

        //private new void Start()
        //{
        //    base.Start();
        //    (transform as UnityEngine.RectTransform).anchoredPosition = new UnityEngine.Vector2(-50f, 0);
        //}

        //private void Update()
        //{
        //        (transform as UnityEngine.RectTransform).anchoredPosition = new UnityEngine.Vector2(-50f, 0);

        //    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> update cppc.transform.localPosition x|y|z: " + transform.localPosition.x + " | " + transform.localPosition.y + " | " + transform.localPosition.z);
        //}

        private IEnumerator _WaitForPresenting()
        {
            yield return new WaitUntil(() => { return _CustomMenu.Present(false); });
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _CustomMenu.SetMainViewController(_CustomViewController, true, (firstActivation, type) =>
            {
                if (firstActivation && type == VRUI.VRUIViewController.ActivationType.AddedToHierarchy)
                {
                    ColorPicker cp = _CustomViewController.CreateColorPicker(new Vector2(0, -10), new Vector2(0.7f, 0.7f));
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>> anchorMin: " + (cp.ColorPickerHueSlider.transform as RectTransform).anchorMin);
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>> anchorMax: " + (cp.ColorPickerHueSlider.transform as RectTransform).anchorMax);
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>> before anchoredPosition.x: " + (cp.ColorPickerHueSlider.transform as RectTransform).anchoredPosition.x);
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>> before anchoredPosition.y: " + (cp.ColorPickerHueSlider.transform as RectTransform).anchoredPosition.y);
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>> sizeDelta.x: " + (cp.ColorPickerHueSlider.transform as RectTransform).sizeDelta.x);
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>> sizeDelta.y: " + (cp.ColorPickerHueSlider.transform as RectTransform).sizeDelta.y);
                    (cp.ColorPickerHueSlider.transform as RectTransform).anchoredPosition = new Vector2(0, 35f);
                }
            });
            StartCoroutine(_WaitForPresenting());
        }
    }
}
