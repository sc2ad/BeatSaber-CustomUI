using CustomUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CustomUI.GameplaySettings
{
    public class GameplaySettingsUI : MonoBehaviour
    {
        public static bool initialized = false;
        private Button _pageDownButton = null;
        private Button _pageUpButton = null;
        private int _listIndex = 0;
        private List<GameOption> customOptions = new List<GameOption>();
        private List<Transform> defaultSeparators = new List<Transform>();

        private static GameplaySettingsUI _instance = null;
        public static GameplaySettingsUI Instance
        {
            get
            {
                if (!_instance)
                    _instance = new GameObject("GameplaySettingsUI").AddComponent<GameplaySettingsUI>();
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
                if (Instance)
                    Destroy(Instance.gameObject);
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
            var options = Instance.GetOptionsForPage(Instance._listIndex);
            bool defaultsActive = options == null;
            defaults?.ToList().ForEach(x => x.gameObject.SetActive(defaultsActive));


            if (defaultsActive)
            {
                for (int i = 0; i < defaultSeparators.Count; i++)
                    defaultSeparators[i].gameObject.SetActive(true);

                customOptions.ElementAt(0)?.separator.SetActive(false);
            }
            else
            {
                for (int i = 0; i < defaultSeparators.Count; i++)
                    defaultSeparators[i].gameObject.SetActive(false);

                foreach (GameOption g in customOptions)
                    g.separator.SetActive(false);

                options[options.Count-1].separator.SetActive(false);
                for (int i = 0; i < options.Count-1; i++)
                    options[i].separator.SetActive(true);
            }


            //Custom options
            Instance.customOptions?.ToList().ForEach(x => x.gameObject.SetActive(false));
            if (!defaultsActive) options?.ToList().ForEach(x => x.gameObject.SetActive(true));
        }

        public static MultiSelectOption CreateListOption(string optionName)
        {
            lock (Instance)
            {
                MultiSelectOption ret = new MultiSelectOption(optionName);
                Instance.customOptions.Add(ret);
                return ret;
            }
        }

        public static ToggleOption CreateToggleOption(string optionName, string hintText, Sprite optionIcon, float multiplier)
        {
            lock (Instance)
            {
                ToggleOption ret = new ToggleOption(optionName, hintText, optionIcon, multiplier);
                Instance.customOptions.Add(ret);
                return ret;
            }
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
            //Grab necessary references
            SoloFreePlayFlowCoordinator sfpfc = Resources.FindObjectsOfTypeAll<SoloFreePlayFlowCoordinator>().First();
            GameplaySetupViewController gsvc = sfpfc.GetField<GameplaySetupViewController>("_gameplaySetupViewController");

            //Get reference to the switch container
            RectTransform container = (RectTransform)gsvc.transform.Find("GameplayModifiers").Find("RightColumn");

            if (!initialized)
            {
                //container.sizeDelta = new Vector2(container.sizeDelta.x, container.sizeDelta.y + 7f); //Grow container so it aligns properly with text
                
                //Get references to the original switches, so we can later duplicate then destroy them
                Transform noFail = container.Find("NoFail");
                Transform noObstacles = container.Find("NoObstacles");
                Transform noBombs = container.Find("NoBombs");
                Transform slowerSong = container.Find("SlowerSong");

                //Get references to other UI elements we need to hide
                //Transform divider = (RectTransform)_govc.transform.Find("Switches").Find("Separator");
                //Transform defaults = (RectTransform)_govc.transform.Find("Switches").Find("DefaultsButton");

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
                _pageUpButton.transform.parent = container.parent;
                _pageUpButton.transform.localScale = Vector3.one/2;
                _pageUpButton.transform.localPosition -= new Vector3(0, 2f);
                _pageUpButton.interactable = true;
                //(_pageUpButton.transform as RectTransform).sizeDelta = new Vector2((_pageUpButton.transform.parent as RectTransform).sizeDelta.x, 3.5f);
                _pageUpButton.onClick.RemoveAllListeners();
                _pageUpButton.onClick.AddListener(delegate ()
                {
                    Instance.ChangePage(--Instance._listIndex, container, noFail, noObstacles, noBombs, slowerSong);

                    //Nice responsive scroll buttons
                    if (Instance._listIndex <= 0) _pageUpButton.gameObject.SetActive(false);
                    if (Instance.customOptions.Count > 0) _pageDownButton.gameObject.SetActive(true);
                });

                //Create down button
                _pageDownButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageDownButton")), container);
                _pageDownButton.transform.parent = container.parent;
                _pageDownButton.transform.localScale = Vector3.one/2;
                _pageDownButton.transform.localPosition -= new Vector3(0, 7f);
                _pageDownButton.interactable = true;
                //(_pageDownButton.transform as RectTransform).sizeDelta = new Vector2((_pageDownButton.transform.parent as RectTransform).sizeDelta.x, (_pageDownButton.transform as RectTransform).sizeDelta.y);
                _pageDownButton.onClick.RemoveAllListeners();
                _pageDownButton.onClick.AddListener(delegate ()
                {
                    Instance.ChangePage(++Instance._listIndex, container, noFail, noObstacles, noBombs, slowerSong);

                    //Nice responsive scroll buttons
                    if (Instance._listIndex >= 0) _pageUpButton.gameObject.SetActive(true);
                    if (((Instance.customOptions.Count + 4 - 1) / 4) - Instance._listIndex <= 0) _pageDownButton.gameObject.SetActive(false);
                });

                _pageUpButton.gameObject.SetActive(false);
                _pageDownButton.gameObject.SetActive(Instance.customOptions.Count > 0);

                //Unfortunately, due to weird object creation for versioning, this doesn't always
                //happen when the scene changes
                Instance._listIndex = 0;

                initialized = true;
            }

            //Create custom options
            foreach (object option in Instance.customOptions)
            {
                //Due to possible "different" types (due to cross-plugin support), we need to do this through reflection
                option.InvokeMethod("Instantiate");
            }
        }
    }
}
