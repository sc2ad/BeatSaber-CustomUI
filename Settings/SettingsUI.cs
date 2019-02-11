using BeatSaberCustomUI.UIElements;
using CustomUI.BeatSaber;
using CustomUI.Utilities;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRUI;
using Image = UnityEngine.UI.Image;

namespace CustomUI.Settings
{
    public class SettingsUI : MonoBehaviour
    {
        private MainMenuViewController _mainMenuViewController = null;
        private SettingsNavigationController settingsMenu = null;
        private MainSettingsMenuViewController mainSettingsMenu = null;
        private MainSettingsTableView _mainSettingsTableView = null;
        private TableView subMenuTableView = null;
        private TableViewHelper subMenuTableViewHelper = null;
        private MainSettingsTableCell tableCell = null;
        private Transform othersSubmenu = null;
        private SimpleDialogPromptViewController prompt = null;

        private Button _pageUpButton = null;
        private Button _pageDownButton = null;
        private Vector2 buttonOffset = new Vector2(24, 0);
        private bool initialized = false;

        private static SettingsUI _instance = null;
        public static SettingsUI Instance
        {
            get
            {
                if (!_instance)
                    _instance = new GameObject("SettingsUI").AddComponent<SettingsUI>();
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        
        public void SceneManagerOnActiveSceneChanged(Scene from, Scene to)
        {
            if (to.name == "EmptyTransition")
            {
                if (Instance)
                    Destroy(Instance.gameObject);
                initialized = false;
            }
        }

        private void SetupUI()
        {
            if (initialized) return;

            try
            {
                var _menuMasterViewController = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
                prompt = ReflectionUtil.GetPrivateField<SimpleDialogPromptViewController>(_menuMasterViewController, "_simpleDialogPromptViewController");

                _mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
                settingsMenu = Resources.FindObjectsOfTypeAll<SettingsNavigationController>().FirstOrDefault();
                mainSettingsMenu = Resources.FindObjectsOfTypeAll<MainSettingsMenuViewController>().FirstOrDefault();
                _mainSettingsTableView = mainSettingsMenu.GetPrivateField<MainSettingsTableView>("_mainSettingsTableView");
                subMenuTableView = _mainSettingsTableView.GetComponentInChildren<TableView>();
                subMenuTableViewHelper = subMenuTableView.gameObject.AddComponent<TableViewHelper>();
                othersSubmenu = settingsMenu.transform.Find("OtherSettings");

                AddPageButtons();

                if (tableCell == null)
                {
                    tableCell = Resources.FindObjectsOfTypeAll<MainSettingsTableCell>().FirstOrDefault();
                    // Get a refence to the Settings Table cell text in case we want to change font size, etc
                    var text = tableCell.GetPrivateField<TextMeshProUGUI>("_settingsSubMenuText");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SettingsUI] Crash when trying to setup UI! Exception: {ex.ToString()}");
            }
        }

        private void AddPageButtons()
        {
            try
            {
                RectTransform viewport = _mainSettingsTableView.GetComponentsInChildren<RectTransform>().First(x => x.name == "Viewport");
                viewport.anchorMin = new Vector2(0f, 0.5f);
                viewport.anchorMax = new Vector2(1f, 0.5f);
                viewport.sizeDelta = new Vector2(0f, 48f);
                viewport.anchoredPosition = new Vector2(0f, 0f);

                RectTransform container = (RectTransform)_mainSettingsTableView.transform;

                if (_pageUpButton == null)
                {
                    _pageUpButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageUpButton")), container);

                    _pageUpButton.transform.SetParent(container.parent);
                    _pageUpButton.transform.localScale = Vector3.one;
                    _pageUpButton.transform.localPosition -= new Vector3(0, 4.5f);
                    _pageUpButton.interactable = false;
                    _pageUpButton.onClick.AddListener(delegate ()
                    {
                        subMenuTableViewHelper.PageScrollUp();
                    });
                }

                if (_pageDownButton == null)
                {
                    _pageDownButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageDownButton")), container);

                    _pageDownButton.transform.SetParent(container.parent);
                    _pageDownButton.transform.localScale = Vector3.one;
                    _pageDownButton.transform.localPosition -= new Vector3(0, 6.5f);
                    _pageDownButton.interactable = false;
                    _pageDownButton.onClick.AddListener(delegate ()
                    {
                        subMenuTableViewHelper.PageScrollDown();
                    });
                }

                subMenuTableViewHelper._pageUpButton = _pageUpButton;
                subMenuTableViewHelper._pageDownButton = _pageDownButton;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SettingsUI] Crash when trying to add page buttons! Exception: {ex.ToString()}");
            }
        }

        public static SubMenu CreateSubMenu(string name)
        {
            lock(Instance) {
                Instance.SetupUI();

                var subMenuGameObject = Instantiate(Instance.othersSubmenu.gameObject, Instance.othersSubmenu.transform.parent);
                subMenuGameObject.name = name.Replace(" ", "");
                Transform mainContainer = CleanScreen(subMenuGameObject.transform);

                var newSubMenuInfo = new SettingsSubMenuInfo();
                newSubMenuInfo.SetPrivateField("_menuName", name);
                newSubMenuInfo.SetPrivateField("_viewController", subMenuGameObject.GetComponent<VRUIViewController>());

                var subMenuInfos = Instance.mainSettingsMenu.GetPrivateField<SettingsSubMenuInfo[]>("_settingsSubMenuInfos").ToList();
                subMenuInfos.Add(newSubMenuInfo);
                Instance.mainSettingsMenu.SetPrivateField("_settingsSubMenuInfos", subMenuInfos.ToArray());

                 SubMenu menu = new SubMenu(mainContainer);
                return menu;
            }
        }


        static Transform CleanScreen(Transform screen)
        {
            var container = screen.Find("Content").Find("SettingsContainer");
            var tempList = container.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                DestroyImmediate(child.gameObject);
            }
            return container;
        }
    }
    
    public class SubMenu
    {
        public Transform transform;

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

        public SliderViewController AddSlider(string name, string hintText, float min, float max, bool intValues)
        {
            var view = AddSliderSetting<SliderViewController>(name, hintText, min, max, intValues);
            view.SetValues(min, max, intValues);
            return view;
        }

        public ColorPickerViewController AddColorPicker(string name, string hintText)
        {
            var view = AddColorPickerSetting<ColorPickerViewController>(name, hintText);
            return view;
        }
        public ColorPickerViewController AddColorPicker(string name, string hintText, Color color)
        {
            var view = AddColorPickerSetting<ColorPickerViewController>(name, hintText);
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
            
            VolumeSettingsController volume = newSettingsObject.GetComponent<VolumeSettingsController>();
            T newListSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(ListSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            var tmpText = newSettingsObject.GetComponentInChildren<TMP_Text>();
            tmpText.text = name;
            BeatSaberUI.AddHintText(tmpText.rectTransform, hintText);

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
            //var bgIcon = newSettingsObject.transform.Find("Value").Find("IncButton").Find("BG").gameObject.GetComponent<Image>();
            //(bgIcon.transform as RectTransform).localScale *= new Vector2(0.9f, 0.9f);
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
            BeatSaberUI.AddHintText(tmpText.rectTransform, hintText);

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

            WindowModeSettingsController volume = newSettingsObject.GetComponent<WindowModeSettingsController>();
            T newToggleSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(SwitchSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            var tmpText = newSettingsObject.GetComponentInChildren<TMP_Text>();
            tmpText.text = name;
            BeatSaberUI.AddHintText(tmpText.rectTransform, hintText);

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

            WindowModeSettingsController volume = newSettingsObject.GetComponent<WindowModeSettingsController>();
            T newToggleSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(IncDecSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            var tmpText = newSettingsObject.GetComponentInChildren<TMP_Text>();
            tmpText.text = name;
            BeatSaberUI.AddHintText(tmpText.rectTransform, hintText);

            return newToggleSettingsController;
        }
        
        public T AddSliderSetting<T>(string name, string hintText, float min, float max, bool intValues) where T : IncDecSettingsController
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

            HMUI.Scrollbar slider = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<HMUI.Scrollbar>().First(),
                                                           newSettingsObject.transform.Find("Value"), false);
            SliderProperties sliderProperties = slider.gameObject.AddComponent<SliderProperties>();

            sliderProperties.FromValue = min;
            sliderProperties.ToValue = max;
            sliderProperties.IntValues = intValues;
            slider.GetComponentInChildren<TextMeshProUGUI>().enableWordWrapping = false;
            (slider.transform as RectTransform).sizeDelta = new Vector2(39.5f, 7.5f);
            (slider.transform as RectTransform).anchorMin = new Vector2(0, 0.5f);

            var tmpText = newSettingsObject.GetComponentInChildren<TMP_Text>();
            tmpText.text = name;
            BeatSaberUI.AddHintText(tmpText.rectTransform, hintText);

            return newSliderSettingsController;
        }

        public T AddColorPickerSetting<T>(string name) where T : SimpleSettingsController
        {
            return AddColorPickerSetting<T>(name, "");
        }
        public T AddColorPickerSetting<T>(string name, string hintText) where T : SimpleSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<WindowModeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);
            newSettingsObject.name = name;

            WindowModeSettingsController volume = newSettingsObject.GetComponent<WindowModeSettingsController>();
            T newColorPickerSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(SimpleSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            GameObject.Destroy(newSettingsObject.transform.Find("Value").Find("DecButton").gameObject);
            GameObject.Destroy(newSettingsObject.transform.Find("Value").Find("ValueText").gameObject);
            GameObject.Destroy(newSettingsObject.transform.Find("Value").Find("IncButton").gameObject);

            ColorPickerPreviewClickable cppc = new GameObject("ColorPickerPreviewClickable").AddComponent<ColorPickerPreviewClickable>();
            cppc.ImagePreview.sprite = null;

            //cppc.transform.localScale = new Vector3(sizeDelta.x, sizeDelta.y, colorPicker.transform.localScale.z);
            cppc.transform.SetParent(newSettingsObject.transform.Find("Value"), false);

            //cppc.transform.localScale = new Vector3(0.2f, 0.15f);
            //(cppc.transform as RectTransform).anchorMin = new Vector2(0, 0.5f);
            //(cppc.transform as RectTransform).anchorMax = new Vector2(0, 0.5f);
            //(cppc.transform as RectTransform).anchoredPosition = new Vector2(-50, 0);
            (cppc.transform as RectTransform).sizeDelta = new Vector2(39.5f, 7f);

            var tmpText = newSettingsObject.GetComponentInChildren<TMP_Text>();
            tmpText.text = name;
            BeatSaberUI.AddHintText(tmpText.rectTransform, hintText);

            return newColorPickerSettingsController;
        }
    }
}
