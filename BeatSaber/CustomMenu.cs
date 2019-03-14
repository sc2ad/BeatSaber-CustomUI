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
using Image = UnityEngine.UI.Image;

namespace CustomUI.BeatSaber
{
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
