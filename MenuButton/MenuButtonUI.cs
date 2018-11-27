using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using CustomUI.BeatSaber;
using System.Collections;
using UnityEngine.SceneManagement;

namespace CustomUI.MenuButton
{
    public class MenuButtonUI : MonoBehaviour
    {
        const int ButtonsPerRow = 4;
        const float RowSeparator = 9f;
        
        private RectTransform bottomPanel;
        private RectTransform menuButtonsOriginal;

        private int rowCount = 0;
        private RectTransform currentRow;
        private int buttonsInCurrentRow;

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
                    Destroy(Instance.gameObject);
                Instance = null;
            }
        }

        private void Init()
        {
            // Find Menu buttons
            bottomPanel = GameObject.Find("BottomPanel").transform as RectTransform;
            menuButtonsOriginal = bottomPanel.Find("Buttons") as RectTransform;
            buttonsInCurrentRow = ButtonsPerRow;
        }
        
        private RectTransform AddRow()
        {
            RectTransform newRow = RectTransform.Instantiate(menuButtonsOriginal, bottomPanel);
            foreach (Transform child in newRow)
            {
                GameObject.Destroy(child.gameObject);
            }
            rowCount++;
            newRow.anchoredPosition += Vector2.up * RowSeparator * rowCount;
            return newRow;
        }

        private static IEnumerator AddButtonDelayed(string text, UnityAction onClick, Sprite icon)
        {
            yield return new WaitForSeconds(0.1f);
            lock (Instance)
            {
                if (Instance.buttonsInCurrentRow >= ButtonsPerRow)
                {
                    Instance.currentRow = Instance.AddRow();
                    Instance.buttonsInCurrentRow = 0;
                }
                Button newButton = BeatSaberUI.CreateUIButton(Instance.currentRow as RectTransform, "QuitButton", onClick, text, icon);
                Instance.buttonsInCurrentRow++;
            }
        }

        public static void AddButton(string buttonText, UnityAction onClick, Sprite icon = null)
        {
            SharedCoroutineStarter.instance.StartCoroutine(AddButtonDelayed(buttonText, onClick, icon));
        }
    }
}
