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
                        backButtonPressed?.Invoke();
                    });
                }
            }

            DidActivateEvent?.Invoke(firstActivation, type);
        }
        
        protected override void DidDeactivate(DeactivationType type)
        {
            DidDeactivateEvent?.Invoke(type);
        }

        public void ClearBackButtonCallbacks()
        {
            backButtonPressed = null;
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

                _customListTableView.didSelectRowEvent += _customListTableView_didSelectRowEvent;

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

        private void _customListTableView_didSelectRowEvent(TableView arg1, int arg2)
        {
            DidSelectRowEvent?.Invoke(arg1, arg2);
        }
        
        public virtual float RowHeight()
        {
            return 10f;
        }

        public virtual int NumberOfRows()
        {
            return Data.Count;
        }

        public virtual TableCell CellForRow(int row)
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
        public FlowCoordinator parentFlowCoordinator;
        public CustomMenu customPanel;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (customPanel.mainViewController)
                customPanel.mainViewController.backButtonPressed += Dismiss;

            if (customPanel.leftViewController)
                customPanel.leftViewController.backButtonPressed += Dismiss;

            if (customPanel.rightViewController)
                customPanel.rightViewController.backButtonPressed += Dismiss;

            title = customPanel.title;

            if (activationType == FlowCoordinator.ActivationType.AddedToHierarchy)
            {
                ProvideInitialViewControllers(customPanel.mainViewController, customPanel.leftViewController, customPanel.rightViewController);
            }
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
        }

        public void Dismiss(bool immediately)
        {
            parentFlowCoordinator.InvokePrivateMethod("DismissFlowCoordinator", new object[] { this, null, immediately });
        }

        public void Dismiss()
        {
            Dismiss(false);
        }
    }

    public class CustomMenu : MonoBehaviour
    {
        public string title;
        public CustomFlowCoordinator customFlowCoordinator
        {
            get { return _masterFlowCoordinator as CustomFlowCoordinator; }
            private set { _masterFlowCoordinator = value; }
        }
        public CustomViewController mainViewController = null;
        public CustomViewController leftViewController = null;
        public CustomViewController rightViewController = null;

        private FlowCoordinator _masterFlowCoordinator;
        private Action<bool> _dismissCustom = null;
        private List<VRUIViewController> _rightViewControllerStack = new List<VRUIViewController>();
        private List<VRUIViewController> _leftViewControllerStack = new List<VRUIViewController>();


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
        
        public bool Present(bool immediately = false)
        {
            var _activeFlowCoordinator = GetActiveFlowCoordinator();
            if (_activeFlowCoordinator == null || _activeFlowCoordinator == customFlowCoordinator) return false;

            if (mainViewController != null)
            {
                if (customFlowCoordinator == null)
                {
                    customFlowCoordinator = new GameObject("CustomFlowCoordinator").AddComponent<CustomFlowCoordinator>();
                    DontDestroyOnLoad(customFlowCoordinator.gameObject);
                    customFlowCoordinator.customPanel = this;
                    _dismissCustom = customFlowCoordinator.Dismiss;
                }
                customFlowCoordinator.parentFlowCoordinator = _activeFlowCoordinator;
                ReflectionUtil.InvokePrivateMethod(_activeFlowCoordinator, "PresentFlowCoordinator", new object[] { customFlowCoordinator, null, immediately, false });
            }
            else
            {
                _dismissCustom = null;
                if (leftViewController)
                    SetScreen(_activeFlowCoordinator, leftViewController, _activeFlowCoordinator.leftScreenViewController, false, immediately);

                if (rightViewController)
                    SetScreen(_activeFlowCoordinator, rightViewController, _activeFlowCoordinator.rightScreenViewController, true, immediately);
                _masterFlowCoordinator = _activeFlowCoordinator;
            }
            return true;
        }
        public void Present()
        {
            Present(false);
        }

        public void Dismiss(bool immediately = false)
        {
            _dismissCustom?.Invoke(immediately);
        }
        public void Dismiss()
        {
            Dismiss(false);
        }

        private FlowCoordinator GetActiveFlowCoordinator()
        {
            FlowCoordinator[] flowCoordinators = Resources.FindObjectsOfTypeAll<FlowCoordinator>();
            foreach (FlowCoordinator f in flowCoordinators)
            {
                if (f.isActivated)
                    return f;
            }
            return null;
        }

        private VRUIViewController PopViewControllerStack(bool right)
        {
            if (right)
            {
                var viewController = _rightViewControllerStack.Last();
                _rightViewControllerStack.Remove(viewController);
                return viewController;
            }
            else
            {
                var viewController = _leftViewControllerStack.Last();
                _leftViewControllerStack.Remove(viewController);
                return viewController;
            }   
        }

        private void SetScreen(FlowCoordinator _activeFlowCoordinator, CustomViewController newViewController, VRUIViewController origViewController, bool right, bool immediately)
        {
            string method = right ? "SetRightScreenViewController" : "SetLeftScreenViewController";
            Action<bool> backAction = (immediate) => { _activeFlowCoordinator.InvokePrivateMethod(method, new object[] { PopViewControllerStack(right), immediate }); };
            _dismissCustom += backAction;  // custom back button behavior
            if (!newViewController.isActivated)
            {
                if (right)
                    _rightViewControllerStack.Add(origViewController);
                else
                    _leftViewControllerStack.Add(origViewController);

                if (newViewController.includeBackButton)
                {
                    newViewController.ClearBackButtonCallbacks();
                    newViewController.backButtonPressed += () => { backAction.Invoke(false); }; // default back button behavior
                }
                _activeFlowCoordinator.InvokePrivateMethod(method, new object[] { newViewController, immediately });
            }
        }
    }
}
