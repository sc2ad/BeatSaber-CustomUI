//using CustomUI.BeatSaber;
//using HMUI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.UI;

//namespace BeatSaberCustomUI.UIElements
//{
//    class FavoriteColorsViewController : CustomListViewController
//    {
//        protected override void DidActivate(bool firstActivation, ActivationType type)
//        {
//            try
//            {
//                if (firstActivation)
//                {
                    
//                }
//                base.DidActivate(firstActivation, type);
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine("EXCEPTION IN CustomListViewController.DidActivate: " + e);
//            }
//        }

//        protected override void DidDeactivate(DeactivationType type)
//        {
//            base.DidDeactivate(type);
//        }
        

//        public override int NumberOfRows()
//        {
//            return Data.Count;
//        }

//        public override TableCell CellForRow(int row)
//        {
//            Vector2 cellSize = new Vector2(30, 0);
//            TableCell _tableCell = Instantiate(_settingsTableCellInstance);

//            ColorPickerPreviewClickable cppc = new GameObject("ColorPickerPreviewClickable").AddComponent<ColorPickerPreviewClickable>();
//            cppc.transform.SetParent(newSettingsObject.transform.Find("Value"), false);
//            cppc.ImagePreview.color = color;
//            (cppc.transform as RectTransform).localScale = new Vector2(0.06f, 0.06f);
//            (cppc.transform as RectTransform).localPosition += new Vector3(3f, 0.1f);
//            clickablePreview = cppc;

//            RectTransform container = new GameObject("container", typeof(RectTransform)).GetComponent<RectTransform>();
//            container.SetParent(_tableCell.transform);
//            container.sizeDelta = cellSize;

//            (_submenuOptions[row].transform as RectTransform).anchoredPosition = new Vector2(_settingsViewControllerWidth / 2, _rowHeight / 2);
//            (_submenuOptions[row].transform as RectTransform).sizeDelta = cellSize;
//            _submenuOptions[row].transform.SetParent(container, false);

//            _tableCell.reuseIdentifier = "CustomUISettingsTableCell";

//            return _tableCell;
//        }
//    }


//}
