﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Standalone;
using MyGUIPlugin;
using OgreWrapper;
using Engine.ObjectManagement;
using Medical.Controller;
using Logging;

namespace Medical.GUI
{
    class BasicGUI : IDisposable
    {
        private ScreenLayoutManager screenLayoutManager;
        private BasicRibbon basicRibbon;
        private MyGUILayoutContainer basicRibbonContainer;
        private StandaloneController standaloneController;
        private LeftPopoutLayoutContainer leftAnimatedContainer;
        private TopPopoutLayoutContainer topAnimatedContainer;
        private StateWizardPanelController stateWizardPanelController;
        private StateWizardController stateWizardController;
        private StateList stateList;
        private StateWizardRibbonTab wizardRibbonTab;

        public BasicGUI(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;

            MyGUIInterface myGui = standaloneController.MedicalController.PluginManager.getPlugin("MyGUIPlugin") as MyGUIInterface;

            Gui gui = Gui.Instance;
            gui.setVisiblePointer(false);

            OgreResourceGroupManager.getInstance().addResourceLocation("GUI/PiperJBO/Layouts", "EngineArchive", "MyGUI", true);
            OgreResourceGroupManager.getInstance().addResourceLocation("GUI/PiperJBO/Imagesets", "EngineArchive", "MyGUI", true);

            LanguageManager.Instance.loadUserTags("core_theme_black_orange_tag.xml");
            gui.load("core_skin.xml");
            gui.load("Imagesets.xml");

            screenLayoutManager = new ScreenLayoutManager(standaloneController.MedicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle);
            screenLayoutManager.Root.SuppressLayout = true;
            basicRibbon = new BasicRibbon(gui, standaloneController);
            basicRibbonContainer = new MyGUILayoutContainer(basicRibbon.RibbonRootWidget);
            topAnimatedContainer = new TopPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            screenLayoutManager.Root.Top = topAnimatedContainer;
            topAnimatedContainer.setInitialPanel(basicRibbonContainer);

            leftAnimatedContainer = new LeftPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            ScreenLayout.Root.Left = leftAnimatedContainer;
            screenLayoutManager.Root.SuppressLayout = false;

            stateWizardPanelController = new StateWizardPanelController(gui, standaloneController.MedicalController, standaloneController.MedicalStateController, standaloneController.NavigationController, standaloneController.LayerController, standaloneController.SceneViewController, standaloneController.TemporaryStateBlender, standaloneController.MovementSequenceController, standaloneController.ImageRenderer);
            stateWizardController = new StateWizardController(standaloneController.MedicalController.MainTimer, standaloneController.TemporaryStateBlender, standaloneController.NavigationController, this);
            stateWizardController.StateCreated += new MedicalStateCreated(stateWizardController_StateCreated);

            stateList = new StateList("StateList.layout", standaloneController.MedicalStateController);

            createWizardPanels();

            wizardRibbonTab = new StateWizardRibbonTab(gui, stateWizardController, this);
        }

        void stateWizardController_StateCreated(MedicalState state)
        {
            standaloneController.MedicalStateController.addState(state);
        }

        public void Dispose()
        {
            wizardRibbonTab.Dispose();
            stateWizardController.Dispose();
            stateWizardPanelController.Dispose();
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
            basicRibbon.Dispose();
        }

        public void changeTopPanel(LayoutContainer topContainer)
        {
            if (topContainer != null)
            {
                topContainer.Visible = true;
                topContainer.bringToFront();
            }
            topAnimatedContainer.changePanel(topContainer, 0.25f, animationCompleted);
        }

        public void resetTopPanel()
        {
            changeTopPanel(basicRibbonContainer);
        }

        public void changeLeftPanel(LayoutContainer leftContainer)
        {
            if (leftContainer != null)
            {
                leftContainer.Visible = true;
                leftContainer.bringToFront();
            }
            else if(standaloneController.MedicalStateController.getNumStates() > 0)
            {
                leftContainer = stateList.LayoutContainer;
                leftContainer.Visible = true;
                leftContainer.bringToFront();
            }
            leftAnimatedContainer.changePanel(leftContainer, 0.25f, animationCompleted);
        }

        private void animationCompleted(LayoutContainer oldChild)
        {
            if (oldChild != null)
            {
                oldChild.Visible = false;
            }
        }

        void standaloneController_SceneUnloading(SimScene scene)
        {
            basicRibbon.sceneUnloading(scene);
        }

        void standaloneController_SceneLoaded(SimScene scene)
        {
            basicRibbon.sceneLoaded(scene);
            stateWizardPanelController.sceneChanged(standaloneController.MedicalController, scene.getDefaultSubScene().getSimElementManager<SimulationScene>());
        }

        public ScreenLayoutManager ScreenLayout
        {
            get
            {
                return screenLayoutManager;
            }
        }

        public void startWizard(StateWizard wizard)
        {
            if (standaloneController.MedicalStateController.getNumStates() == 0)
            {
                standaloneController.MedicalStateController.createNormalStateFromScene();
            }

            stateWizardController.CurrentSceneView = standaloneController.SceneViewController.ActiveWindow;
            stateWizardPanelController.CurrentSceneView = standaloneController.SceneViewController.ActiveWindow;
            stateWizardController.startWizard(wizard);
        }

        private void createWizardPanels()
        {
            //Create single distortion wizards
            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DOPPLER))
            {
                //Doppler
                StateWizard dopplerWizard = new StateWizard("Doppler", stateWizardController);
                dopplerWizard.TextLine1 = "Doppler";
                //dopplerWizard.ImageLarge = Resources.DopplerWizardLarge;
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDopplerPanel));
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDopplerPanel));
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(dopplerWizard);
            }

            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DENTITION))
            {
                //Teeth
                StateWizard teethWizard = new StateWizard("Dentition", stateWizardController);
                teethWizard.TextLine1 = "Dentition";
                //teethWizard.ImageLarge = Resources.TeethWizardIcon;
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(teethWizard);
            }

            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_CEPHALOMETRIC))
            {
                //Profile
                StateWizard profileWizard = new StateWizard("Cephalometric", stateWizardController);
                profileWizard.TextLine1 = "Cephalometric";
                //profileWizard.ImageLarge = Resources.ProfileIcon;
                profileWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                profileWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                profileWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(profileWizard);
            }

            

            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_MANDIBLE))
            {
                //Bone
                StateWizard boneWizard = new StateWizard("Mandible", stateWizardController);
                boneWizard.TextLine1 = "Mandible";
                //boneWizard.ImageLarge = Resources.BoneWizardLarge;
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(boneWizard);
            }

            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DISC_SPACE))
            {
                //Disc
                StateWizard discWizard = new StateWizard("Disc Space", stateWizardController);
                discWizard.TextLine1 = "Disc Space";
                //discWizard.ImageLarge = Resources.DiscSpaceWizardIcon;
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscSpacePanel));
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscSpacePanel));
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(discWizard);
            }

            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DISC_CLOCK))
            {
                //Disc
                StateWizard discClockWizard = new StateWizard("Disc Clock Face", stateWizardController);
                discClockWizard.TextLine1 = "Disc";
                discClockWizard.TextLine2 = "Clock Face";
                //discClockWizard.ImageLarge = Resources.DiscWizardLarge;
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscClockFacePanel));
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscClockFacePanel));
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(discClockWizard);
            }

            //Create combination distortion wizards

            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_CEPHALOMETRIC_DENTITION))
            {
                //Profile + Teeth
                StateWizard profileTeethWizard = new StateWizard("Cephalometric and Dentition", stateWizardController);
                profileTeethWizard.TextLine1 = "Cephalometric";
                profileTeethWizard.TextLine2 = "and Dentition";
                //profileTeethWizard.ImageLarge = Resources.ProfileAndTeethWizardLarge;
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(profileTeethWizard);
            }

            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_CLINICAL_DOPPLER))
            {
                //Clinical
                StateWizard clinicalWizard = new StateWizard("Clinical and Doppler", stateWizardController);
                clinicalWizard.TextLine1 = "Clinical";
                clinicalWizard.TextLine2 = "and Doppler";
                //clinicalWizard.ImageLarge = Resources.ClinicalIcon;
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDopplerPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDopplerPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(clinicalWizard);
            }

            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_RADIOGRAPHY))
            {
                //CT/Radiography Wizard
                StateWizard ctWizard = new StateWizard("Clinical and Radiography", stateWizardController);
                ctWizard.TextLine1 = "Clinical and";
                ctWizard.TextLine2 = "Radiography";
                //ctWizard.ImageLarge = Resources.CTWizardLarge;
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscSpacePanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftFossa));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscSpacePanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightFossa));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethAdaptationPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(ctWizard);
            }

            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_MRI))
            {
                //MRI Wizard
                StateWizard mriWizard = new StateWizard("Clinical and MRI", stateWizardController);
                mriWizard.TextLine1 = "Clinical";
                mriWizard.TextLine2 = "and MRI";
                //mriWizard.ImageLarge = Resources.MRIWizardLarge;
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscClockFacePanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftFossa));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscClockFacePanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightFossa));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethAdaptationPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(mriWizard);
            }
        }
    }
}
