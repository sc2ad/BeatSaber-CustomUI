using CustomUI.Utilities;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.BeatSaber
{
    public class CustomListViewController : CustomViewController, TableView.IDataSource
    {
        public bool includePageButtons = true;
        public Button _pageUpButton;
        public Button _pageDownButton;
        public TableView _customListTableView;
        public List<CustomCellInfo> Data = new List<CustomCellInfo>();
        public Action<TableView, int> DidSelectRowEvent;
        private LevelListTableCell _songListTableCellInstance;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            try
            {
                if (firstActivation)
                {
                    _songListTableCellInstance = Resources.FindObjectsOfTypeAll<LevelListTableCell>().First(x => (x.name == "LevelListTableCell"));

                    RectTransform container = new GameObject("CustomListContainer", typeof(RectTransform)).transform as RectTransform;
                    container.SetParent(rectTransform, false);
                    container.sizeDelta = new Vector2(60f, 0f);

                    _customListTableView = new GameObject("CustomListTableView").AddComponent<TableView>();
                    _customListTableView.gameObject.AddComponent<RectMask2D>();
                    _customListTableView.transform.SetParent(container, false);

                    (_customListTableView.transform as RectTransform).anchorMin = new Vector2(0f, 0f);
                    (_customListTableView.transform as RectTransform).anchorMax = new Vector2(1f, 1f);
                    (_customListTableView.transform as RectTransform).sizeDelta = new Vector2(0f, 60f);
                    (_customListTableView.transform as RectTransform).anchoredPosition = new Vector3(0f, 0f);
                    
                    _customListTableView.SetPrivateField("_preallocatedCells", new TableView.CellsGroup[0]);
                    _customListTableView.SetPrivateField("_isInitialized", false);
                    _customListTableView.dataSource = this;

                    _customListTableView.didSelectCellWithIdxEvent += _customListTableView_didSelectRowEvent;

                    if (includePageButtons)
                    {
                        if (_pageUpButton == null)
                        {
                            _pageUpButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageUpButton")), container, false);
                            (_pageUpButton.transform as RectTransform).anchoredPosition = new Vector2(0f, 30f);
                            _pageUpButton.interactable = true;
                            _pageUpButton.onClick.AddListener(delegate ()
                            {
                                _customListTableView.PageScrollUp();
                            });
                        }

                        if (_pageDownButton == null)
                        {
                            _pageDownButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageDownButton")), container, false);
                            (_pageDownButton.transform as RectTransform).anchoredPosition = new Vector2(0f, -30f);
                            _pageDownButton.interactable = true;
                            _pageDownButton.onClick.AddListener(delegate ()
                            {
                                _customListTableView.PageScrollDown();
                            });
                        }
                    }
                }
                base.DidActivate(firstActivation, type);
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION IN CustomListViewController.DidActivate: " + e);
            }
        }

        protected override void DidDeactivate(DeactivationType type)
        {
            base.DidDeactivate(type);
        }

        private void _customListTableView_didSelectRowEvent(TableView arg1, int arg2)
        {
            DidSelectRowEvent?.Invoke(arg1, arg2);
        }

        public virtual float CellSize()
        {
            return 10f;
        }

        public virtual int NumberOfCells()
        {
            return Data.Count;
        }

        public virtual TableCell CellForIdx(int idx)
        {
            LevelListTableCell _tableCell = Instantiate(_songListTableCellInstance);
            _tableCell.SetPrivateField("_songNameText", Data[idx].text);
            _tableCell.SetPrivateField("_authorText", Data[idx].subtext);
            _tableCell.SetPrivateField("_coverImage", Data[idx].icon == null ? UIUtilities.BlankSprite : Data[idx].icon);
            _tableCell.reuseIdentifier = "CustomListCell";
            return _tableCell;
        }
    }
}
