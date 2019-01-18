using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using CustomUI.BeatSaber;
using CustomUI.Utilities;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using IllusionPlugin;
using System.Linq;
using VRUI;

namespace CustomUI.MenuButton
{
    public class MenuButtonUI : MonoBehaviour
    {
        private static readonly WaitUntil _bottomPanelExists = new WaitUntil(() => GameObject.Find("MainMenuViewController/BottomPanel"));
        const int ButtonsPerRow = 4;
        const float RowSeparator = 9f;

        private RectTransform bottomPanel;
        private RectTransform menuButtonsOriginal;
        
        private RectTransform currentRow;
        private List<RectTransform> rows;
        private int buttonsInCurrentRow = ButtonsPerRow;

        private List<MenuButton> buttonData;
        private List<String> pinnedButtons;
        private MenuButtonListViewController _menuButtonListViewController;
        private CustomMenu _menuButtonListMenu;

        private static MenuButtonUI _instance = null;
        public static MenuButtonUI Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("MenuButtonUI").AddComponent<MenuButtonUI>();
                    _instance.Init();
                }
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
                {
                    Instance.StopAllCoroutines();
                    Destroy(Instance.gameObject);
                }
                Instance = null;
            }
        }

        private void Init()
        {
            buttonData = new List<MenuButton>();
            rows = new List<RectTransform>();

            pinnedButtons = ModPrefs.GetString("CustomUI", "PinnedMenuButtons", "", true).Split(',').ToList();
            
            StartCoroutine(AddMenuButtonListButton());
        }
        
        private static IEnumerator AddMenuButtonListButton()
        {
            yield return _bottomPanelExists;

            lock (Instance)
            {
                var bottomPanel = GameObject.Find("MainMenuViewController/BottomPanel").transform as RectTransform;
                var buttonRow = bottomPanel.Find("Buttons") as RectTransform;
                
                Button newButton = BeatSaberUI.CreateUIButton(buttonRow as RectTransform, "QuitButton", () => Instance.PresentList(), "Mods", null);
                buttonRow.Find("QuitButton").SetAsLastSibling();
            }
        }

        private void PresentList()
        {
            if (_menuButtonListViewController == null)
            {
                _menuButtonListViewController = BeatSaberUI.CreateViewController<MenuButtonListViewController>();
                _menuButtonListViewController.pinButtonPushed += PinButtonWasPushed;
                _menuButtonListViewController.backButtonPressed += ListBackPressed;
            }
            _menuButtonListViewController.SetData(buttonData);

            // Using MainFlowCoordinator instead of CustomMenu for compatibility with mods
            FlowCoordinator flowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            flowCoordinator.InvokePrivateMethod("PresentViewController", new object[] { _menuButtonListViewController, null, false });
            flowCoordinator.SetProperty("title", "Mods");
        }

        private void ListBackPressed()
        {
            FlowCoordinator flowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            flowCoordinator.InvokePrivateMethod("DismissViewController", new object[] { _menuButtonListViewController, null, false });
            flowCoordinator.SetProperty("title", string.Empty);
        }

        private static IEnumerator AddButtonToMainMenuCoroutine(MenuButton button)
        {
            yield return _bottomPanelExists;

            if (Instance.bottomPanel == null) Instance.bottomPanel = GameObject.Find("MainMenuViewController/BottomPanel").transform as RectTransform;
            if (Instance.menuButtonsOriginal == null) Instance.menuButtonsOriginal = Instance.bottomPanel.Find("Buttons") as RectTransform;

            lock (Instance)
            {
                Instance.AddButtonToMainMenu(button);
            }
        }

        private void AddButtonToMainMenu(MenuButton button)
        {
            if (buttonsInCurrentRow >= ButtonsPerRow)
            {
                AddRow();
            }

            Button newButton = BeatSaberUI.CreateUIButton(currentRow, "QuitButton", button.onClick, button.text, button.icon);
            newButton.GetComponentInChildren<HorizontalLayoutGroup>().padding = new RectOffset(6, 6, 0, 0);
            newButton.name = button.text;
            buttonsInCurrentRow++;
        }

        public static void AddButton(string buttonText, UnityAction onClick, Sprite icon = null)
        {
            bool pin = Instance.pinnedButtons.Contains(buttonText);

            MenuButton menuButton = new MenuButton(buttonText, onClick, icon, pin);
            Instance.buttonData.Add(menuButton);
            if (menuButton.pinned)
            {
                Instance.StartCoroutine(AddButtonToMainMenuCoroutine(menuButton));
            }
        }

        void PinButton(MenuButton menuButton)
        {
            if (menuButton.pinned) return;
            menuButton.pinned = true;
            if (!pinnedButtons.Contains(menuButton.text)) pinnedButtons.Add(menuButton.text);
            ModPrefs.SetString("CustomUI", "PinnedMenuButtons", string.Join(",", pinnedButtons));
            AddButtonToMainMenu(menuButton);
        }

        void UnpinButton(MenuButton menuButton)
        {
            if (!menuButton.pinned) return;
            menuButton.pinned = false;
            if(pinnedButtons.Contains(menuButton.text)) pinnedButtons.Remove(menuButton.text);
            ModPrefs.SetString("CustomUI", "PinnedMenuButtons", string.Join(",", pinnedButtons));
            Rebuild();
        }

        void PinButtonWasPushed(MenuButton button)
        {
            if (button.pinned)
            {
                UnpinButton(button);
            } else
            {
                // todo: max pins
                PinButton(button);
            }
        }

        private void AddRow()
        {
            RectTransform newRow = Instantiate(menuButtonsOriginal, bottomPanel);
            rows.Add(newRow);
            currentRow = newRow;
            buttonsInCurrentRow = 0;
            newRow.name = "CustomMenuButtonsRow" + rows.Count.ToString();

            foreach (Transform child in newRow)
            {
                child.name = String.Empty;
                Destroy(child.gameObject);
            }
            newRow.anchoredPosition += Vector2.up * RowSeparator * rows.Count;
        }

        private void Rebuild()
        {
            Console.WriteLine(rows.Count);
            while(rows.Count > 0)
            {
                Destroy(rows.First().gameObject);
                rows.Remove(rows.First());
            }
            Console.WriteLine(rows.Count);
            currentRow = null;
            buttonsInCurrentRow = ButtonsPerRow;
            rows.Clear();

            foreach (MenuButton button in buttonData)
            {
                if (button.pinned)
                {
                    AddButtonToMainMenu(button);
                }
            }
        }
    }
}
