using CustomUI.UIElements;
using CustomUI.Utilities;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRUI;
using Image = UnityEngine.UI.Image;

namespace CustomUI.BeatSaber
{
    public class BeatSaberUI : MonoBehaviour
    {
        private static Button _backButtonInstance;
        private static GameObject _loadingIndicatorInstance;
        private static CustomMenu _keyboardMenu = null;
        private static CustomUIKeyboard _keyboard = null;
        private static Action<string> _textChangedEvent;
        private static Action<string> _textEntrySuccessEvent = null;
        private static Action _textEntryCancelledEvent = null;
        private static bool _isKeyboardOpen = false;
        private static string _initialValue;
        private static TextMeshProUGUI _inputText;

        public static bool DisplayKeyboard(string title, string initialValue, Action<string> TextChangedEvent = null, Action<string> TextEntrySuccessEvent = null, Action TextEntryCancelledEvent = null)
        {
            if (_isKeyboardOpen) return false;

            if (_keyboardMenu == null)
            {
                _keyboardMenu = CreateCustomMenu<CustomMenu>(title);
                var mainViewController = CreateViewController<CustomViewController>();
                _keyboardMenu.SetMainViewController(mainViewController, false, (firstActivation, type) =>
                {
                    if (firstActivation)
                    {
                        var _customKeyboardGO = Instantiate(Resources.FindObjectsOfTypeAll<UIKeyboard>().First(x => x.name != "CustomUIKeyboard"), mainViewController.rectTransform, false).gameObject;
                        Destroy(_customKeyboardGO.GetComponent<UIKeyboard>());
                        _keyboard = _customKeyboardGO.AddComponent<CustomUIKeyboard>();

                        _inputText = CreateText(mainViewController.rectTransform, String.Empty, new Vector2(0f, 22f));
                        _inputText.alignment = TextAlignmentOptions.Center;
                        _inputText.fontSize = 6f;

                        _keyboard.okButtonWasPressedEvent += () =>
                        {
                            _textEntrySuccessEvent?.Invoke(_inputText.text);
                            _inputText.text = String.Empty;
                            _keyboard.OkButtonInteractivity = false;
                            _keyboardMenu.Dismiss();
                            _isKeyboardOpen = false;
                        };
                        _keyboard.cancelButtonWasPressedEvent += () =>
                        {
                            _textEntryCancelledEvent?.Invoke();
                            _inputText.text = String.Empty;
                            _keyboard.OkButtonInteractivity = false;
                            _keyboardMenu.Dismiss();
                            _isKeyboardOpen = false;
                        };
                        _keyboard.textKeyWasPressedEvent += (key) =>
                        {
                            _inputText.text += key;
                            _textChangedEvent?.Invoke(_inputText.text);

                            if (_inputText.text.Length > 0)
                                _keyboard.OkButtonInteractivity = true;
                        };
                        _keyboard.deleteButtonWasPressedEvent += () =>
                        {
                            if (_inputText.text.Length > 0)
                            {
                                _inputText.text = _inputText.text.Substring(0, _inputText.text.Length - 1);
                                _textChangedEvent?.Invoke(_inputText.text);
                            }
                            if (_inputText.text.Length == 0)
                                _keyboard.OkButtonInteractivity = false;
                        };
                    }
                    _inputText.text = _initialValue;
                    _keyboard.OkButtonInteractivity = _inputText.text.Length > 0;
                });
            }
            _keyboardMenu.title = title == null ? String.Empty : title;
            _initialValue = initialValue == null ? String.Empty : initialValue;
            _textChangedEvent = TextChangedEvent;
            _textEntrySuccessEvent = TextEntrySuccessEvent;
            _textEntryCancelledEvent = TextEntryCancelledEvent;
            _keyboardMenu.Present();
            _isKeyboardOpen = true;
            return true;
        }


        public static Button CreateUIButton(RectTransform parent, string buttonTemplate, Vector2 anchoredPosition, Vector2 sizeDelta, UnityAction onClick = null, string buttonText = "BUTTON", Sprite icon = null)
        {
            Button btn = Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == buttonTemplate)), parent, false);
            btn.onClick = new Button.ButtonClickedEvent();
            if (onClick != null)
                btn.onClick.AddListener(onClick);
            btn.name = "CustomUIButton";

            (btn.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
            (btn.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
            (btn.transform as RectTransform).anchoredPosition = anchoredPosition;
            (btn.transform as RectTransform).sizeDelta = sizeDelta;

            btn.SetButtonText(buttonText);
            if (icon != null)
                btn.SetButtonIcon(icon);

            return btn;
        }

        public static Button CreateUIButton(RectTransform parent, string buttonTemplate, Vector2 anchoredPosition, UnityAction onClick = null, string buttonText = "BUTTON", Sprite icon = null)
        {
            Button btn = Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == buttonTemplate)), parent, false);
            btn.onClick = new Button.ButtonClickedEvent();
            if (onClick != null)
                btn.onClick.AddListener(onClick);
            btn.name = "CustomUIButton";

            (btn.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
            (btn.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
            (btn.transform as RectTransform).anchoredPosition = anchoredPosition;

            btn.SetButtonText(buttonText);
            if (icon != null)
                btn.SetButtonIcon(icon);

            return btn;
        }
        
        public static Button CreateUIButton(RectTransform parent, string buttonTemplate, UnityAction onClick = null, string buttonText = "BUTTON", Sprite icon = null)
        {
            Button btn = Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == buttonTemplate)), parent, false);
            btn.onClick = new Button.ButtonClickedEvent();
            if (onClick != null)
                btn.onClick.AddListener(onClick);
            btn.name = "CustomUIButton";

            (btn.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
            (btn.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
            btn.SetButtonText(buttonText);
            if (icon != null)
                btn.SetButtonIcon(icon);
            return btn;
        }

        public static Button CreateBackButton(RectTransform parent, UnityAction onClick = null)
        {
            if (_backButtonInstance == null)
            {
                try
                {
                    _backButtonInstance = Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "BackArrowButton"));
                }
                catch
                {
                    return null;
                }
            }
            
            Button btn = Instantiate(_backButtonInstance, parent, false);
            btn.onClick = new Button.ButtonClickedEvent();
            if (onClick != null)
                btn.onClick.AddListener(onClick);
            btn.name = "CustomUIButton";
            
            return btn;
        }

        public static T CreateViewController<T>() where T : VRUIViewController
        {
            T vc = new GameObject("CustomViewController").AddComponent<T>();
            DontDestroyOnLoad(vc.gameObject);

            vc.rectTransform.anchorMin = new Vector2(0f, 0f);
            vc.rectTransform.anchorMax = new Vector2(1f, 1f);
            vc.rectTransform.sizeDelta = new Vector2(0f, 0f);
            vc.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            
            return vc;
        }
        
        public static T CreateCustomMenu<T>(string title) where T: CustomMenu
        {
            T customMenu = new GameObject("CustomUIMenu").AddComponent<T>();
            customMenu.title = title;
            return customMenu;
        }

        public static GameObject CreateLoadingSpinner(Transform parent)
        {
            if (_loadingIndicatorInstance == null)
            {
                try
                {
                    _loadingIndicatorInstance = Resources.FindObjectsOfTypeAll<GameObject>().Where(x => x.name == "LoadingIndicator").First();
                }
                catch
                {
                    return null;
                }
            }

            GameObject loadingSpinner = Instantiate(_loadingIndicatorInstance, parent, false);
            loadingSpinner.name = "CustomUILoadingSpinner";

            return loadingSpinner;
        }

        public static TextMeshProUGUI CreateText(RectTransform parent, string text, Vector2 anchoredPosition)
        {
            TextMeshProUGUI textMesh = new GameObject("CustomUIText").AddComponent<TextMeshProUGUI>();
            textMesh.rectTransform.SetParent(parent, false);
            textMesh.text = text;
            textMesh.fontSize = 4;
            textMesh.color = Color.white;
            textMesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            textMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.sizeDelta = new Vector2(60f, 10f);
            textMesh.rectTransform.anchoredPosition = anchoredPosition;

            return textMesh;
        }

        public static TextMeshProUGUI CreateText(RectTransform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            TextMeshProUGUI textMesh = new GameObject("CustomUIText").AddComponent<TextMeshProUGUI>();
            textMesh.rectTransform.SetParent(parent, false);
            textMesh.text = text;
            textMesh.fontSize = 4;
            textMesh.color = Color.white;
            textMesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            textMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.sizeDelta = sizeDelta;
            textMesh.rectTransform.anchoredPosition = anchoredPosition;

            return textMesh;
        }

        public static HoverHint AddHintText(RectTransform parent, string text)
        {
            var hoverHint = parent.gameObject.AddComponent<HoverHint>();
            hoverHint.text = text;
            //hoverHint.name = "CustomHintText";
            HoverHintController hoverHintController = Resources.FindObjectsOfTypeAll<HoverHintController>().First();
            hoverHint.SetPrivateField("_hoverHintController", hoverHintController);
            return hoverHint;
        }

        public static CustomSlider CreateUISlider(Transform parent, float min, float max, float increment, bool intValues, UnityAction<float> onValueChanged = null)
        {
            CustomSlider slider = new GameObject("CustomUISlider").AddComponent<CustomSlider>();
            GameObject.DontDestroyOnLoad(slider.gameObject);
            slider.Scrollbar = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<HMUI.Scrollbar>().First(s => s.name != "CustomUISlider"), parent, false);
            slider.Scrollbar.name = "CustomUISlider";
            slider.Scrollbar.transform.SetParent(parent, false);

            slider.Scrollbar.GetComponentInChildren<TextMeshProUGUI>().enableWordWrapping = false;
            slider.Scrollbar.numberOfSteps = (int)((max - min) / increment) + 1;
            slider.MinValue = min;
            slider.MaxValue = max;
            slider.IsIntValue = intValues;
            slider.SetCurrentValueFromPercentage(slider.Scrollbar.value);
            slider.Scrollbar.GetComponentInChildren<TextMeshProUGUI>().text = slider.CurrentValue.ToString("N1");
            slider.Scrollbar.onValueChanged.RemoveAllListeners();
            slider.Scrollbar.onValueChanged.AddListener(delegate (float value) {
                TextMeshProUGUI valueLabel = slider.Scrollbar.GetComponentInChildren<TextMeshProUGUI>();
                valueLabel.enableWordWrapping = false;
                slider.SetCurrentValueFromPercentage(value);
                valueLabel.text = slider.CurrentValue.ToString("N1");
            });
            if (onValueChanged != null)
                slider.Scrollbar.onValueChanged.AddListener(onValueChanged);
            return slider;
        }

        public static ColorPicker CreateColorPicker(RectTransform parent, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            ColorPicker colorPicker = new GameObject("ColorPicker").AddComponent<ColorPicker>();

            colorPicker.transform.localScale = new Vector3(sizeDelta.x, sizeDelta.y, colorPicker.transform.localScale.z);
            colorPicker.transform.SetParent(parent, false);
            colorPicker.transform.localPosition = new Vector3(anchoredPosition.x, anchoredPosition.y);

            return colorPicker;
        }
    }
}
