using CustomUI.BeatSaber;
using CustomUI.Utilities;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace CustomUI.Settings
{
    public class CustomSettingsListViewController : CustomListViewController
    {
        private readonly int _maxOptionsPerPage = 6;
        private List<GameObject> _submenuOptions = new List<GameObject>();

        private TableCell _settingsTableCellInstance;
        protected override void DidActivate(bool firstActivation, ActivationType type)
        { 
            try
            {
                if (firstActivation)
                {
                    _submenuOptions.ForEach(s => s.transform.SetParent(null, false));
                    if (_settingsTableCellInstance == null)
                    {
                        var settingsListItem = new GameObject("SettingsTableCell");

                        var listItemRectTransform = settingsListItem.AddComponent<RectTransform>();
                        listItemRectTransform.anchorMin = new Vector2(0f, 0f);
                        listItemRectTransform.anchorMax = new Vector2(1f, 1f);

                        var horiz = settingsListItem.AddComponent<HorizontalLayoutGroup>();
                        horiz.spacing = 2f;
                        horiz.childControlHeight = true;
                        horiz.childControlWidth = false;
                        horiz.childForceExpandWidth = false;
                        horiz.childAlignment = TextAnchor.MiddleLeft;
                        (horiz.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
                        horiz.padding = new RectOffset(0, 0, 4, 4);

                        _settingsTableCellInstance = settingsListItem.gameObject.AddComponent<TableCell>();
                        _settingsTableCellInstance.reuseIdentifier = "CustomUISettingsTableCell";
                    }
                }

                base.DidActivate(firstActivation, type);

                if (firstActivation)
                {
                    int numOptions = _submenuOptions.Count() > _maxOptionsPerPage ? _maxOptionsPerPage : _submenuOptions.Count();
                    float listHeight = numOptions * RowHeight();
                    float listTableOffset = 5f;

                    // Adjust the page button sizes
                    _pageUpButton.transform.localScale /= 1.4f;
                    _pageDownButton.transform.localScale /= 1.4f;
                    // Move the page buttons depending on how many options there are
                    (_pageUpButton.transform as RectTransform).anchoredPosition = new Vector2(0f, listHeight / 2 + 1.35f + listTableOffset);
                    (_pageDownButton.transform as RectTransform).anchoredPosition = new Vector2(0f, -listHeight / 2 - 1.35f + listTableOffset);

                    // Set the size of the listTableView (this is the area where list items are displayed)
                    (_customListTableView.transform as RectTransform).sizeDelta = new Vector2(0f, listHeight);
                    (_customListTableView.transform as RectTransform).localPosition += new Vector3(0f, listTableOffset);

                    var content = (rectTransform.Find("Content") as RectTransform);
                    DestroyImmediate(content.gameObject.GetComponent<ContentSizeFitter>());
                    content.GetComponents<Image>().ToList().ForEach(i => 
                    {
                        i.rectTransform.sizeDelta = new Vector2(i.rectTransform.sizeDelta.x, listHeight - 77);
                        i.rectTransform.localPosition = new Vector2(i.rectTransform.localPosition.x, 5);
                        //var newImage = (Image)ReflectionUtil.CopyComponent(i, typeof(Image), typeof(Image), content.gameObject);
                        //newImage.rectTransform.sizeDelta = new Vector2(100, 100);
                        //newImage.material = UIUtilities.NoGlowMaterial;
                        //DestroyImmediate(i);


                    });

                    // Fit the tableview to our window
                    (_customListTableView.transform.parent as RectTransform).sizeDelta = new Vector2(100f, 0);
                }

                if (_submenuOptions.Count <= 6)
                {
                    _pageDownButton.gameObject.SetActive(false);
                    _pageUpButton.gameObject.SetActive(false);
                }
                else
                {
                    _pageDownButton.gameObject.SetActive(true);
                    _pageUpButton.gameObject.SetActive(true);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION IN CustomSettingsListViewController.DidActivate: " + e);
            }
        }

        public void AddSubmenuOption(GameObject option)
        {
            option.transform.SetParent(transform.Find("Content").Find("SettingsContainer"));
            _submenuOptions.Add(option);
        }

        protected override void DidDeactivate(DeactivationType type)
        {
            base.DidDeactivate(type);
        }

        public override float RowHeight()
        {
            return 8f;
        }

        public override int NumberOfRows()
        {
            return _submenuOptions.Count();
        }

        public override TableCell CellForRow(int row)
        {
            Vector2 cellSize = new Vector2(90, 8);
            
            TableCell _tableCell = Instantiate(_settingsTableCellInstance);
            
            RectTransform container = new GameObject("container", typeof(RectTransform)).GetComponent<RectTransform>();
            container.SetParent(_tableCell.transform);
            container.sizeDelta = cellSize;
            
            (_submenuOptions[row].transform as RectTransform).anchoredPosition = new Vector2(52, 3.9f);
            (_submenuOptions[row].transform as RectTransform).sizeDelta = cellSize;
            _submenuOptions[row].transform.SetParent(container, false);

            _tableCell.reuseIdentifier = "CustomUISettingsTableCell";
            
            return _tableCell;
        }
    }
}
