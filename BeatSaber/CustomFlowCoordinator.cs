using CustomUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUI;

namespace CustomUI.BeatSaber
{
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
}
