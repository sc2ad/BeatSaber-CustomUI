using CustomUI.BeatSaber;
using CustomUI.Utilities;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace CustomUI.BeatSaber
{
    public class CustomViewController : VRUIViewController
    {
        public Action backButtonPressed;
        public Button _backButton;
        public bool includeBackButton;
        public CustomFlowCoordinator customFlowCoordinator;

        public Action<bool, VRUIViewController.ActivationType> DidActivateEvent;
        public Action<VRUIViewController.DeactivationType> DidDeactivateEvent;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            if (firstActivation)
            {
                if (includeBackButton && _backButton == null)
                {
                    _backButton = BeatSaberUI.CreateBackButton(rectTransform as RectTransform);
                    _backButton.onClick.AddListener(delegate ()
                    {
                        if (backButtonPressed != null) backButtonPressed();
                    });
                }
            }

            if (DidActivateEvent != null)
                DidActivateEvent(firstActivation, type);
        }

        protected override void DidDeactivate(DeactivationType type)
        {
            if (DidDeactivateEvent != null)
                DidDeactivateEvent(type);
        }
    }

    public class CustomCellInfo
    {
        public string text;
        public string subtext;
        public Sprite icon;

        public CustomCellInfo(string text, string subtext, Sprite icon = null)
        {
            this.text = text;
            this.subtext = subtext;
            this.icon = icon;
        }
    };

    public class CustomListViewController : CustomViewController, TableView.IDataSource
    {
        public Button _pageUpButton;
        public Button _pageDownButton;
        public TableView _customListTableView;
        public List<CustomCellInfo> Data = new List<CustomCellInfo>();
        public Action<TableView, int> DidSelectRowEvent;

        private LevelListTableCell _songListTableCellInstance;

        protected override void DidActivate(bool firstActivation, ActivationType type)
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

                _customListTableView.didSelectRowEvent += _platformsTableView_didSelectRowEvent; ;

                _pageUpButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageUpButton")), container, false);
                (_pageUpButton.transform as RectTransform).anchoredPosition = new Vector2(0f, 30f);//-14
                _pageUpButton.interactable = true;
                _pageUpButton.onClick.AddListener(delegate ()
                {
                    _customListTableView.PageScrollUp();
                });

                _pageDownButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageDownButton")), container, false);
                (_pageDownButton.transform as RectTransform).anchoredPosition = new Vector2(0f, -30f);//8
                _pageDownButton.interactable = true;
                _pageDownButton.onClick.AddListener(delegate ()
                {
                    _customListTableView.PageScrollDown();
                });
            }

            base.DidActivate(firstActivation, type);
        }

        protected override void DidDeactivate(DeactivationType type)
        {
            base.DidDeactivate(type);
        }

        private void _platformsTableView_didSelectRowEvent(TableView arg1, int arg2)
        {
            if (DidSelectRowEvent != null)
                DidSelectRowEvent(arg1, arg2);
        }
        
        public float RowHeight()
        {
            return 10f;
        }

        public int NumberOfRows()
        {
            return Data.Count;
        }

        public TableCell CellForRow(int row)
        {
            LevelListTableCell _tableCell = Instantiate(_songListTableCellInstance);
            
            _tableCell.songName = Data[row].text;
            _tableCell.author = Data[row].subtext;
            _tableCell.coverImage = Data[row].icon == null ? UIUtilities.BlankSprite : Data[row].icon;
            _tableCell.reuseIdentifier = "CustomListCell";

            return _tableCell;
        }
    }

    public class CustomFlowCoordinator : FlowCoordinator
    {
        //public CustomViewController customViewController;
        public FlowCoordinator parentFlowCoordinator;
        public CustomMenu customPanel;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation)
            {
                title = customPanel.title;
            }

            if (customPanel.mainViewController == null)
            {
                customPanel.mainViewController = BeatSaberUI.CreateViewController<CustomViewController>();
                customPanel.mainViewController.includeBackButton = true;
            }

            if (customPanel.mainViewController != null)
            {
                customPanel.mainViewController.customFlowCoordinator = this;
                customPanel.mainViewController.backButtonPressed += Dismiss;
            }
            if(customPanel.leftViewController != null)
                customPanel.leftViewController.customFlowCoordinator = this;

            if(customPanel.rightViewController != null)
                customPanel.rightViewController.customFlowCoordinator = this;

            if (activationType == FlowCoordinator.ActivationType.AddedToHierarchy)
            {
                ProvideInitialViewControllers(customPanel.mainViewController, customPanel.leftViewController, customPanel.rightViewController);
            }

        }

        public void Dismiss()
        {
            parentFlowCoordinator.InvokePrivateMethod("DismissFlowCoordinator", new object[] { this, null, false });
        }

        protected override void DidDeactivate(DeactivationType type)
        {

        }
    }

    public class CustomMenu : MonoBehaviour
    {
        public CustomFlowCoordinator customFlowCoordinator;
        public CustomViewController mainViewController = null;
        public CustomViewController leftViewController = null;
        public CustomViewController rightViewController = null;
        public string title;

        public void SetMainViewController(CustomViewController viewController, bool includeBackButton, Action<bool, VRUIViewController.ActivationType> DidActivate = null, Action<VRUIViewController.DeactivationType> DidDeactivate = null)
        {
            mainViewController = viewController;
            mainViewController.includeBackButton = includeBackButton;
            if(DidActivate != null)
                mainViewController.DidActivateEvent += DidActivate;
            if(DidDeactivate != null)
                mainViewController.DidDeactivateEvent += DidDeactivate;
        }

        public void SetLeftViewController(CustomViewController viewController, bool includeBackButton, Action<bool, VRUIViewController.ActivationType> DidActivate = null, Action<VRUIViewController.DeactivationType> DidDeactivate = null)
        {
            leftViewController = viewController;
            leftViewController.includeBackButton = includeBackButton;
            if (DidActivate != null)
                leftViewController.DidActivateEvent += DidActivate;
            if (DidDeactivate != null)
                leftViewController.DidDeactivateEvent += DidDeactivate;
        }

        public void SetRightViewController(CustomViewController viewController, bool includeBackButton, Action<bool, VRUIViewController.ActivationType> DidActivate = null, Action<VRUIViewController.DeactivationType> DidDeactivate = null)
        {
            rightViewController = viewController;
            rightViewController.includeBackButton = includeBackButton;
            if (DidActivate != null)
                rightViewController.DidActivateEvent += DidActivate;
            if (DidDeactivate != null)
                rightViewController.DidDeactivateEvent += DidDeactivate;
        }
        
        private FlowCoordinator GetActiveFlowCoordinator()
        {
            FlowCoordinator[] flowCoordinators = Resources.FindObjectsOfTypeAll<FlowCoordinator>();
            foreach(FlowCoordinator f in flowCoordinators)
            {
                if (f.isActivated)
                    return f;
            }
            return null;
        }

        public void Present()
        {
            var _activeFlowCoordinator = GetActiveFlowCoordinator();
            if (_activeFlowCoordinator != null)
            {
                if (customFlowCoordinator == null)
                {
                    customFlowCoordinator = new GameObject("CustomFlowCoordinator").AddComponent<CustomFlowCoordinator>();
                    customFlowCoordinator.customPanel = this;
                }
                customFlowCoordinator.parentFlowCoordinator = _activeFlowCoordinator;
                
                ReflectionUtil.InvokePrivateMethod(_activeFlowCoordinator, "PresentFlowCoordinator", new object[] { customFlowCoordinator, null, false, false });
            }
        }

        public void Dismiss()
        {
            customFlowCoordinator?.Dismiss();
        }
    }
}
