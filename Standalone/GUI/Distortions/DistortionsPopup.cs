﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class DistortionsPopup : IDisposable
    {
        private Layout layout;
        private Widget mainWidget;
        private PopupContainer popupContainer;

        private GUIManager guiManager;

        private List<Button> buttons = new List<Button>();

        public DistortionsPopup(StateWizardController stateWizardController, GUIManager guiManager)
        {
            layout = LayoutManager.Instance.loadLayout("Medical.GUI.Distortions.DistortionsPopup.layout");
            mainWidget = layout.getWidget(0);
            mainWidget.Visible = false;
            popupContainer = new PopupContainer(mainWidget);

            this.guiManager = guiManager;

            Widget anatomyDistortionPanel = mainWidget.findWidget("AnatomyDistortionPanel");
            Widget examDistortionPanel = mainWidget.findWidget("ExamDistortionPanel");

            int anatomyPosition = 3;
            int examPosition = 3;
            foreach (StateWizard wizard in stateWizardController.WizardEnum)
            {
                String caption = wizard.TextLine1;
                if (wizard.TextLine2 != null)
                {
                    caption += "\n" + wizard.TextLine2;
                }
                Button wizardButton;
                if (wizard.WizardType == WizardType.Anatomy)
                {
                    wizardButton = anatomyDistortionPanel.createWidgetT("Button", "RibbonButton", anatomyPosition, 2, 78, 68, Align.Default, wizard.Name) as Button;
                }
                else
                {
                    wizardButton = examDistortionPanel.createWidgetT("Button", "RibbonButton", examPosition, 2, 78, 68, Align.Default, wizard.Name) as Button;
                }
                wizardButton.Caption = caption;
                int buttonWidth = (int)wizardButton.getTextSize().Width + 10;
                if (buttonWidth < 38)
                {
                    buttonWidth = 38;
                }
                wizardButton.setSize(buttonWidth, wizardButton.Height);
                wizardButton.UserObject = wizard;
                wizardButton.StaticImage.setItemResource(wizard.ImageKey);
                wizardButton.MouseButtonClick += new MyGUIEvent(wizardButton_MouseButtonClick);
                if (wizard.WizardType == WizardType.Anatomy)
                {
                    anatomyPosition += buttonWidth + 3;
                }
                else
                {
                    examPosition += buttonWidth + 3;
                }
            }
            //anatomyPosition -= 3;
            //examPosition -= 3;
            anatomyDistortionPanel.setSize(anatomyPosition, anatomyDistortionPanel.Height);
            examDistortionPanel.setSize(examPosition, examDistortionPanel.Height);

            Size2 size = new Size2(mainWidget.Width, mainWidget.Height);
            if (examDistortionPanel.Right > anatomyDistortionPanel.Right)
            {
                size.Width = examDistortionPanel.Right;
            }
            else
            {
                size.Width = anatomyDistortionPanel.Right;
            }
            mainWidget.setSize((int)size.Width, (int)size.Height);
        }

        public void Dispose()
        {
            foreach (Button button in buttons)
            {
                Gui.Instance.destroyWidget(button);
            }
            buttons.Clear();
            LayoutManager.Instance.unloadLayout(layout);
        }

        public void show(int left, int top)
        {
            popupContainer.show(left, top);
        }

        void wizardButton_MouseButtonClick(Widget source, EventArgs e)
        {
            popupContainer.hide();
            guiManager.startWizard(source.UserObject as StateWizard);
        }
    }
}
