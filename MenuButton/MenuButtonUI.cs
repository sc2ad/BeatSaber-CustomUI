using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using CustomUI.BeatSaber;

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
                    _instance = new GameObject("MenuButtonUI").AddComponent<MenuButtonUI>();
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

        public static void AddButton(string buttonText, UnityAction onClick, Sprite icon = null)
        {
            lock (Instance)
            {
                if (buttonsInCurrentRow >= ButtonsPerRow)
                {
                    currentRow = AddRow();
                    buttonsInCurrentRow = 0;
                }
                Button newButton = BeatSaberUI.CreateUIButton(currentRow as RectTransform, "QuitButton", onClick, buttonText, icon);
                buttonsInCurrentRow++;
            }
        }
    }
}
