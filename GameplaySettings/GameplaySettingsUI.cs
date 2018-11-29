using CustomUI.BeatSaber;
using CustomUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CustomUI.GameplaySettings
{
    public class GameplaySettingsUI : MonoBehaviour
    {
        public bool initialized = false;
        private Button _pageDownButton = null;
        private Button _pageUpButton = null;
        private int _listIndex = 0;
        private GameplaySettingsPanels panel;
        private List<GameOption> customOptions = new List<GameOption>();
        private List<Transform> defaultSeparators = new List<Transform>();
        private List<GameOption> currentOptions = null;

        private static Dictionary<GameplaySettingsPanels, GameplaySettingsUI> _instances = new Dictionary<GameplaySettingsPanels, GameplaySettingsUI>();
        public static Dictionary<GameplaySettingsPanels, GameplaySettingsUI> Instance
        {
            get
            {
                for (int i = 0; i < 4; i++)
                    CreateInstance((GameplaySettingsPanels)i);

                return _instances;
            }
            private set
            {
                _instances = value;
            }
        }

        private static void CreateInstance(GameplaySettingsPanels panel)
        {
            if (_instances.ContainsKey(panel)) return;

            _instances[panel] = new GameObject("GameplaySettingsUI").AddComponent<GameplaySettingsUI>();
            _instances[panel].panel = panel;
        }
        
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        void OnDestroy()
        {
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(Scene from, Scene to)
        {
            if (to.name == "EmptyTransition")
            {
                Destroy(this.gameObject);
                initialized = false;
            }
            else if (to.name == "Menu")
            {
                Build();
            }
        }

        //Returns a list of options for the current page index
        private List<GameOption> GetOptionsForPage(int page)
        {
            //Default options
            if (page == 0) return null;

            page--; //If the page isn't 0, we should pick from the 0th pagination of our list
            
            //Get 4 custom options and return them
            return customOptions.Skip(4 * page).Take(4).ToList();
        }

        //Sets the active value for our game options depending on the active page
        private void ChangePage(int page, Transform container, params Transform[] defaults)
        {
            currentOptions = Instance[panel].GetOptionsForPage(Instance[panel]._listIndex);
            bool defaultsActive = currentOptions == null;
            defaults?.ToList().ForEach(x => x.gameObject.SetActive(defaultsActive));
            
            if (defaultsActive)
            {
                for (int i = 0; i < defaultSeparators.Count; i++)
                    defaultSeparators[i].gameObject.SetActive(true);

                customOptions.ForEach(o => o.separator.SetActive(false));
            }
            else
            {
                foreach (GameOption g in customOptions)
                    g.separator.SetActive(false);

                currentOptions[currentOptions.Count-1].separator.SetActive(false);
                for (int i = 0; i < currentOptions.Count-1; i++)
                    currentOptions[i].separator.SetActive(true);

                defaultSeparators.ForEach(s => s.gameObject.SetActive(false));
            }
            
            //Custom options
            Instance[panel].customOptions?.ToList().ForEach(x => x.gameObject.SetActive(false));
            if (!defaultsActive) currentOptions?.ToList().ForEach(x => x.gameObject.SetActive(true));
        }

        public static MultiSelectOption CreateListOption(GameplaySettingsPanels panel, string optionName, string hintText = "")
        {
            lock (Instance[panel])
            {
                MultiSelectOption ret = new MultiSelectOption(optionName, hintText);
                ret.SetPanel(panel);
                Instance[panel].customOptions.Add(ret);
                return ret;
            }
        }

        public static MultiSelectOption CreateListOption(string optionName, string hintText)
        {
            return CreateListOption(GameplaySettingsPanels.ModifiersRight, optionName, hintText);
        }

        public static MultiSelectOption CreateListOption(string optionName)
        {
            return CreateListOption(optionName, "");
        }

        public static ToggleOption CreateToggleOption(GameplaySettingsPanels panel, string optionName, string hintText = "", Sprite optionIcon = null, float multiplier = 0f)
        {
            lock (Instance[panel])
            {
                ToggleOption ret = new ToggleOption(optionName, hintText, optionIcon, multiplier);
                ret.SetPanel(panel);
                Instance[panel].customOptions.Add(ret);
                return ret;
            }
        }

        public static ToggleOption CreateToggleOption(string optionName, string hintText, Sprite optionIcon, float multiplier)
        {
            return CreateToggleOption(GameplaySettingsPanels.ModifiersRight, optionName, hintText, optionIcon, multiplier);
        }

        public static ToggleOption CreateToggleOption(string optionName, string hintText, Sprite optionIcon)
        {
            return CreateToggleOption(optionName, hintText, optionIcon, 0f);
        }

        public static ToggleOption CreateToggleOption(string optionName, string hintText)
        {
            return CreateToggleOption(optionName, hintText, null, 0f);
        }

        public static ToggleOption CreateToggleOption(string optionName)
        {
            return CreateToggleOption(optionName, "", null, 0f);
        }

        public void Build()
        {
            string pageName = String.Empty, panelName = String.Empty;
            GameOption.GetPanelNames(panel, ref pageName, ref panelName);

            //Grab necessary references
            SoloFreePlayFlowCoordinator sfpfc = Resources.FindObjectsOfTypeAll<SoloFreePlayFlowCoordinator>().First();
            GameplaySetupViewController gsvc = sfpfc.GetField<GameplaySetupViewController>("_gameplaySetupViewController");

            //Get reference to the switch container
            RectTransform page = (RectTransform)gsvc.transform.Find(pageName);
            Destroy(page.gameObject.GetComponent<HorizontalLayoutGroup>());
            Destroy(page.gameObject.GetComponent<ContentSizeFitter>());
            RectTransform container = (RectTransform)page.Find(panelName);

            if (!container.gameObject.GetComponent<ContentSizeFitter>())
            {
                var fitter = container.gameObject.AddComponent<ContentSizeFitter>();
                fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            
            if (!initialized)
            {
                //Get references to the original switches, so we can later duplicate then destroy them
                Transform option1 = null, option2 = null, option3 = null, option4 = null;
                GameOption.GetOptionTransforms(panel, container, ref option1, ref option2, ref option3, ref option4);
                
                foreach (Transform t in container)
                {
                    if (t.name.StartsWith("Separator"))
                    {
                        defaultSeparators.Add(t);
                    }
                }
                defaultSeparators.Reverse();
                

                //Create up button
                _pageUpButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageUpButton")), container);

                _pageUpButton.transform.SetParent(container.parent);
                _pageUpButton.transform.localScale = Vector3.one / 2;
                _pageUpButton.transform.localPosition = new Vector3(_pageUpButton.transform.localPosition.x,  -2.2f, _pageUpButton.transform.localPosition.z);
                _pageUpButton.interactable = true;
                //(_pageUpButton.transform as RectTransform).sizeDelta = new Vector2((_pageUpButton.transform.parent as RectTransform).sizeDelta.x, 3.5f);
                _pageUpButton.onClick.RemoveAllListeners();
                _pageUpButton.onClick.AddListener(delegate ()
                {
                    Instance[panel].ChangePage(--Instance[panel]._listIndex, container, option1, option2, option3, option4);

                    //Nice responsive scroll buttons
                    if (Instance[panel]._listIndex <= 0) _pageUpButton.gameObject.SetActive(false);
                    if (Instance[panel].customOptions.Count > 0) _pageDownButton.gameObject.SetActive(true);
                });

                //Create down button
                _pageDownButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageDownButton")), container);
                
                _pageDownButton.transform.SetParent(container.parent);
                _pageDownButton.transform.localScale = Vector3.one / 2;
                _pageDownButton.transform.localPosition = new Vector3(_pageDownButton.transform.localPosition.x, -34.3f, _pageDownButton.transform.localPosition.z);
                _pageDownButton.interactable = true;
                //(_pageDownButton.transform as RectTransform).sizeDelta = new Vector2((_pageDownButton.transform.parent as RectTransform).sizeDelta.x, (_pageDownButton.transform as RectTransform).sizeDelta.y);
                _pageDownButton.onClick.RemoveAllListeners();
                _pageDownButton.onClick.AddListener(delegate ()
                {
                    Instance[panel].ChangePage(++Instance[panel]._listIndex, container, option1, option2, option3, option4);

                    //Nice responsive scroll buttons
                    if (Instance[panel]._listIndex >= 0) _pageUpButton.gameObject.SetActive(true);
                    if (((Instance[panel].customOptions.Count + 4 - 1) / 4) - Instance[panel]._listIndex <= 0) _pageDownButton.gameObject.SetActive(false);
                });

                _pageUpButton.gameObject.SetActive(false);
                _pageDownButton.gameObject.SetActive(Instance[panel].customOptions.Count > 0);

                //Unfortunately, due to weird object creation for versioning, this doesn't always
                //happen when the scene changes
                Instance[panel]._listIndex = 0;

                initialized = true;
            }

            //Create custom options
            foreach (GameOption option in Instance[panel].customOptions)
            {
                //Due to possible "different" types (due to cross-plugin support), we need to do this through reflection
                option.Instantiate();
            }
        }
    }
}
