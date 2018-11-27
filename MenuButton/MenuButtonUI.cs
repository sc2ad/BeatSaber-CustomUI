using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using CustomUI.BeatSaber;
using System.Collections;

namespace CustomUI.MenuButton
{
    public class MenuButtonUI : MonoBehaviour
    {
        const int ButtonsPerRow = 4;
        const float RowSeparator = 9f;
        
        private static RectTransform bottomPanel;
        private static RectTransform menuButtonsOriginal;

        private static int rowCount = 0;
        private static RectTransform currentRow;
        private static int buttonsInCurrentRow;

        private static MenuButtonUI _instance = null;
        public static MenuButtonUI Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("MenuButtonUI").AddComponent<MenuButtonUI>();
                    Init();
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
        }

        private static void Init()
        {
            // Find Menu buttons
            bottomPanel = GameObject.Find("BottomPanel").transform as RectTransform;
            menuButtonsOriginal = bottomPanel.Find("Buttons") as RectTransform;
            buttonsInCurrentRow = ButtonsPerRow;
        }
        
        private static RectTransform AddRow()
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
                if (buttonsInCurrentRow >= ButtonsPerRow)
                {
                    currentRow = AddRow();
                    buttonsInCurrentRow = 0;
                }
                Button newButton = BeatSaberUI.CreateUIButton(currentRow as RectTransform, "QuitButton", onClick, text, icon);
                buttonsInCurrentRow++;
            }
        }

        public static void AddButton(string buttonText, UnityAction onClick, Sprite icon = null)
        {
            SharedCoroutineStarter.instance.StartCoroutine(AddButtonDelayed(buttonText, onClick, icon));
        }
    }
}
