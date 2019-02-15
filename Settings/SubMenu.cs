using CustomUI.BeatSaber;
using CustomUI.UIElements;
using CustomUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace CustomUI.Settings
{
    public class SubMenu
    {
        public Transform transform;
        public CustomSettingsListViewController viewController;

        public SubMenu(CustomSettingsListViewController viewController)
        {
            this.viewController = viewController;
            this.transform = viewController.transform;
        }

        public SubMenu(Transform transform)
        {
            this.transform = transform;
        }

        public BoolViewController AddBool(string name)
        {
            return AddBool(name, "");
        }
        public BoolViewController AddBool(string name, string hintText)
        {
            return AddToggleSetting<BoolViewController>(name, hintText);
        }

        public IntViewController AddInt(string name, int min, int max, int increment)
        {
            return AddInt(name, "", min, max, increment);
        }
        public IntViewController AddInt(string name, string hintText, int min, int max, int increment)
        {
            var view = AddIntSetting<IntViewController>(name, hintText);
            view.SetValues(min, max, increment);
            return view;
        }

        public ListViewController AddList(string name, float[] values)
        {
            return AddList(name, values, "");
        }
        public ListViewController AddList(string name, float[] values, string hintText)
        {
            var view = AddListSetting<ListViewController>(name, hintText);
            view.values = values.ToList();
            return view;
        }

        public StringViewController AddString(string name, string hintText = null)
        {
            var view = AddStringSetting<StringViewController>(name, hintText);
            return view;
        }

        public SliderViewController AddSlider(string name, string hintText, float min, float max, float increment, bool intValues)
        {
            var view = AddSliderSetting<SliderViewController>(name, hintText, min, max, increment, intValues);
            view.SetValues(min, max, intValues);
            return view;
        }

        public ColorPickerViewController AddColorPicker(string name, string hintText, Color color)
        {
            var view = AddColorPickerSetting<ColorPickerViewController>(name, hintText, color, out var clickablePreview);
            view.SetPreviewInstance(clickablePreview);
            view.SetValues(color);
            return view;
        }

        public T AddListSetting<T>(string name) where T : ListSettingsController
        {
            return AddListSetting<T>(name, "");
        }
        public T AddListSetting<T>(string name, string hintText) where T : ListSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<VolumeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);
            newSettingsObject.name = name;

            var incBg = newSettingsObject.transform.Find("Value").Find("IncButton").Find("BG").gameObject.GetComponent<Image>();
            (incBg.transform as RectTransform).localScale *= new Vector2(0.8f, 0.8f);
            var decBg = newSettingsObject.transform.Find("Value").Find("DecButton").Find("BG").gameObject.GetComponent<Image>();
            (decBg.transform as RectTransform).localScale *= new Vector2(0.8f, 0.8f);

            VolumeSettingsController volume = newSettingsObject.GetComponent<VolumeSettingsController>();
            T newListSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(ListSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            var tmpText = newSettingsObject.GetComponentInChildren<TMP_Text>();
            tmpText.text = name;
            if (hintText != String.Empty)
                BeatSaberUI.AddHintText(tmpText.rectTransform, hintText);

            viewController?.AddSubmenuOption(newSettingsObject);
            return newListSettingsController;
        }

        public T AddStringSetting<T>(string name) where T : ListSettingsController
        {
            return AddStringSetting<T>(name, "");
        }
        public T AddStringSetting<T>(string name, string hintText) where T : ListSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<VolumeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);
            newSettingsObject.name = name;
            newSettingsObject.transform.Find("Value").gameObject.GetComponent<HorizontalLayoutGroup>().spacing += 2;
            newSettingsObject.transform.Find("Value").Find("DecButton").gameObject.SetActive(false);
            var bgIcon = newSettingsObject.transform.Find("Value").Find("IncButton").Find("BG").gameObject.GetComponent<Image>();
            (bgIcon.transform as RectTransform).localScale *= new Vector2(0.8f, 0.8f);
            var arrowIcon = newSettingsObject.transform.Find("Value").Find("IncButton").Find("Arrow").gameObject.GetComponent<Image>();
            arrowIcon.sprite = UIUtilities.EditIcon;
            var valueText = newSettingsObject.transform.Find("Value").Find("ValueText").gameObject.GetComponent<TextMeshProUGUI>();
            valueText.alignment = TextAlignmentOptions.MidlineRight;
            valueText.enableWordWrapping = false;
            BeatSaberUI.AddHintText(valueText.rectTransform, hintText);

            VolumeSettingsController volume = newSettingsObject.GetComponent<VolumeSettingsController>();
            T newListSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(ListSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            var tmpText = newSettingsObject.GetComponentInChildren<TMP_Text>();
            tmpText.text = name;
            if (hintText != String.Empty)
                BeatSaberUI.AddHintText(tmpText.rectTransform, hintText);

            viewController?.AddSubmenuOption(newSettingsObject);
            return newListSettingsController;
        }

        public T AddToggleSetting<T>(string name) where T : SwitchSettingsController
        {
            return AddToggleSetting<T>(name, "");
        }
        public T AddToggleSetting<T>(string name, string hintText) where T : SwitchSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<WindowModeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);
            newSettingsObject.name = name;

            var incBg = newSettingsObject.transform.Find("Value").Find("IncButton").Find("BG").gameObject.GetComponent<Image>();
            (incBg.transform as RectTransform).localScale *= new Vector2(0.8f, 0.8f);
            var decBg = newSettingsObject.transform.Find("Value").Find("DecButton").Find("BG").gameObject.GetComponent<Image>();
            (decBg.transform as RectTransform).localScale *= new Vector2(0.8f, 0.8f);

            WindowModeSettingsController volume = newSettingsObject.GetComponent<WindowModeSettingsController>();
            T newToggleSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(SwitchSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            var tmpText = newSettingsObject.GetComponentInChildren<TMP_Text>();
            tmpText.text = name;
            if (hintText != String.Empty)
                BeatSaberUI.AddHintText(tmpText.rectTransform, hintText);

            viewController?.AddSubmenuOption(newSettingsObject);
            return newToggleSettingsController;
        }

        public T AddIntSetting<T>(string name) where T : IntSettingsController
        {
            return AddIntSetting<T>(name, "");
        }
        public T AddIntSetting<T>(string name, string hintText) where T : IntSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<WindowModeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);
            newSettingsObject.name = name;

            var incBg = newSettingsObject.transform.Find("Value").Find("IncButton").Find("BG").gameObject.GetComponent<Image>();
            (incBg.transform as RectTransform).localScale *= new Vector2(0.8f, 0.8f);
            var decBg = newSettingsObject.transform.Find("Value").Find("DecButton").Find("BG").gameObject.GetComponent<Image>();
            (decBg.transform as RectTransform).localScale *= new Vector2(0.8f, 0.8f);

            WindowModeSettingsController volume = newSettingsObject.GetComponent<WindowModeSettingsController>();
            T newToggleSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(IncDecSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            var tmpText = newSettingsObject.GetComponentInChildren<TMP_Text>();
            tmpText.text = name;
            if (hintText != String.Empty)
                BeatSaberUI.AddHintText(tmpText.rectTransform, hintText);

            viewController?.AddSubmenuOption(newSettingsObject);
            return newToggleSettingsController;
        }

        public T AddSliderSetting<T>(string name, string hintText, float min, float max, float increment, bool intValues) where T : IncDecSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<WindowModeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);
            newSettingsObject.name = name;

            WindowModeSettingsController volume = newSettingsObject.GetComponent<WindowModeSettingsController>();
            T newSliderSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(IncDecSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            GameObject.Destroy(newSettingsObject.transform.Find("Value").Find("DecButton").gameObject);
            GameObject.Destroy(newSettingsObject.transform.Find("Value").Find("ValueText").gameObject);
            GameObject.Destroy(newSettingsObject.transform.Find("Value").Find("IncButton").gameObject);

            CustomSlider slider = newSliderSettingsController.gameObject.AddComponent<CustomSlider>();
            slider.Scrollbar = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<HMUI.Scrollbar>().First(s => s.name != "CustomUISlider"), newSettingsObject.transform.Find("Value"), false);
            slider.Scrollbar.name = "CustomUISlider";
            slider.Scrollbar.GetComponentInChildren<TextMeshProUGUI>().enableWordWrapping = false;
            (slider.Scrollbar.transform as RectTransform).sizeDelta = new Vector2(39.5f, 4.5f);
            (slider.Scrollbar.transform as RectTransform).anchorMin = new Vector2(0, 0.5f);


            slider.Scrollbar.numberOfSteps = (int)((max - min) / increment) + 1;
            slider.MinValue = min;
            slider.MaxValue = max;
            slider.IsIntValue = intValues;

            var tmpText = newSettingsObject.GetComponentInChildren<TMP_Text>();
            tmpText.text = name;
            if (hintText != String.Empty)
                BeatSaberUI.AddHintText(tmpText.rectTransform, hintText);

            viewController?.AddSubmenuOption(newSettingsObject);
            return newSliderSettingsController;
        }

        public T AddColorPickerSetting<T>(string name, Color color, out ColorPickerPreviewClickable clickablePreview) where T : SimpleSettingsController
        {
            return AddColorPickerSetting<T>(name, "", color, out clickablePreview);
        }
        public T AddColorPickerSetting<T>(string name, string hintText, Color color, out ColorPickerPreviewClickable clickablePreview) where T : SimpleSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<WindowModeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);
            newSettingsObject.name = name;

            WindowModeSettingsController volume = newSettingsObject.GetComponent<WindowModeSettingsController>();
            T newColorPickerSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(SimpleSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            GameObject.Destroy(newSettingsObject.GetComponentInChildren<HorizontalLayoutGroup>());
            GameObject.Destroy(newSettingsObject.transform.Find("Value").Find("DecButton").gameObject);
            GameObject.Destroy(newSettingsObject.transform.Find("Value").Find("ValueText").gameObject);
            GameObject.Destroy(newSettingsObject.transform.Find("Value").Find("IncButton").gameObject);

            ColorPickerPreviewClickable cppc = new GameObject("ColorPickerPreviewClickable").AddComponent<ColorPickerPreviewClickable>();
            cppc.transform.SetParent(newSettingsObject.transform.Find("Value"), false);
            cppc.ImagePreview.color = color;
            (cppc.transform as RectTransform).localScale = new Vector2(0.06f, 0.06f);
            (cppc.transform as RectTransform).localPosition += new Vector3(3f, 0.1f);
            clickablePreview = cppc;

            var tmpText = newSettingsObject.GetComponentInChildren<TMP_Text>();
            tmpText.text = name;
            if (hintText != String.Empty)
                BeatSaberUI.AddHintText(tmpText.rectTransform, hintText);

            viewController?.AddSubmenuOption(newSettingsObject);
            return newColorPickerSettingsController;
        }
    }
}
