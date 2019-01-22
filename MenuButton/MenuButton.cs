using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CustomUI.MenuButton
{
    public class MenuButton
    {
        public string text = "";
        public string hintText = "";
        public UnityAction onClick = null;
        public Sprite icon;
        public bool pinned = false;
        private bool _interactable = true;
        public bool interactable
        {
            get
            {
                return _interactable;
            }
            set
            {
                _interactable = value;
                for (int i=buttons.Count - 1; i >= 0; i--)
                {
                    if (buttons[i])
                        buttons[i].interactable = _interactable;
                    else
                        buttons.RemoveAt(i);
                }
            }
        }
        public List<Button> buttons = new List<Button>();

        public MenuButton(string text, string hintText, UnityAction onClick, Sprite icon, bool pinned)
        {
            this.text = text;
            this.onClick = onClick;
            this.icon = icon;
            this.pinned = pinned;
            this.hintText = hintText;
        }
    }
}
