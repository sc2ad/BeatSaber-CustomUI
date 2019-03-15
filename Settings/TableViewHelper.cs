using CustomUI.Utilities;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.Settings
{
    public class TableViewHelper : MonoBehaviour
    {
        TableView table;
        RectTransform viewport;

        public Button _pageUpButton = null;
        public Button _pageDownButton = null;

        RectTransform _scrollRectTransform
        {
            get { return table.GetPrivateField<RectTransform>("_scrollRectTransform"); }
            set { table.SetPrivateField("_scrollRectTransform", value); }
        }

        int _numberOfCells
        {
            get { return table.GetPrivateField<int>("_numberOfCells"); }
            set { table.SetPrivateField("_numberOfCells", value); }
        }

        float _cellSize
        {
            get { return table.GetPrivateField<float>("_cellSize"); }
            set { table.SetPrivateField("_cellSize", value); }
        }

        float _targetPosition
        {
            get { return table.GetPrivateField<float>("_targetPosition"); }
            set { table.SetPrivateField("_targetPosition", value); }
        }


        RectTransform _contentTransform
        {
            get { return table.GetPrivateField<RectTransform>("_contentTransform"); }
            set { table.SetPrivateField("_contentTransform", value); }
        }

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            table = GetComponent<TableView>();
            viewport = GetComponentsInChildren<RectTransform>().First(x => x.name == "Viewport");
            ScrollToTop();
        }

        public void ScrollToTop()
        {
            _targetPosition = 1f;
            table.enabled = true;
            RefreshScrollButtons();
        }

        public void PageScrollUp()
        {
            _targetPosition = _contentTransform.anchoredPosition.y - Mathf.Max(1f, GetNumberOfVisibleCells() - 1f) * _cellSize;
            if (_targetPosition < 0f)
            {
                _targetPosition = 0f;
            }
            table.enabled = true;
            RefreshScrollButtons();
        }

        public void PageScrollDown()
        {
            float num = _scrollRectTransform.rect.height;
            float num2 = (float)_numberOfCells * _cellSize - num;
            _targetPosition = _contentTransform.anchoredPosition.y + Mathf.Max(1f, GetNumberOfVisibleCells() - 1f) * this._cellSize;
            if (_targetPosition > num2)
            {
                _targetPosition = num2;
            }
            table.enabled = true;
            RefreshScrollButtons();
            //_scrollRectTransform.sizeDelta = new Vector2(-20f, -10f);
        }

        public virtual void RefreshScrollButtons()
        {
            table.RefreshScrollButtons();
            if (_pageDownButton)
            {
                _pageDownButton.interactable = !Mathf.Approximately(_targetPosition, 0f);
            }
            if (_pageUpButton)
            {
                _pageUpButton.interactable = !Mathf.Approximately(_targetPosition, 1f);
            }
        }

        private float GetNumberOfVisibleCells()
        {
            return 6.0f;
        }

        public virtual float GetScrollStep()
        {
            float height = viewport.rect.height;
            float num = _numberOfCells * _cellSize - height;
            int num2 = Mathf.CeilToInt(num / _cellSize);
            return 1f / num2;
        }
    }
}