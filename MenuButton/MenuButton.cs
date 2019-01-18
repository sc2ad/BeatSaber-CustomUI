using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace CustomUI.MenuButton
{
    class MenuButton
    {
        public string text = "";
        public UnityAction onClick = null;
        public Sprite icon;
        public bool pinned = false;

        public MenuButton(string text, UnityAction onClick, Sprite icon, bool pinned)
        {
            this.text = text;
            this.onClick = onClick;
            this.icon = icon;
            this.pinned = pinned;
        }
    }
}
