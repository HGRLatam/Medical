﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Platform;

namespace Medical.GUI
{
    class LayersPopup : IDisposable
    {
        private LayerGUIMenu skinMenu;
        private LayerGUIMenu musclesMenu;
        private LayerGUISkullMenu skullMenu;
        private LayerGUIMenu mandibleMenu;
        private LayerGUIMenu discsMenu;
        private LayerGUIMenu spineMenu;
        private LayerGUIMenu hyoidMenu;
        private LayerGUIMenu topTeethMenu;
        private LayerGUIMenu bottomTeethMenu;

        private LayerController layerController;

        private Layout layout;
        private Widget layerPanel;
        private PopupContainer popupContainer;

        public LayersPopup(LayerController layerController)
        {
            layout = LayoutManager.Instance.loadLayout("Medical.Controller.GUIController.Taskbar.LayersPopup.layout");
            layerPanel = layout.getWidget(0);
            layerPanel.Visible = false;
            popupContainer = new PopupContainer(layerPanel);

            //Predefined layers
            this.layerController = layerController;

            layerController.LayerStateApplied += new LayerControllerEvent(layerStateChanged);

            skinMenu = new LayerGUIMenu(layerPanel.findWidget("Layers/Skin") as Button, layerPanel.findWidget("Layers/SkinMenu") as Button);
            skinMenu.createShortcuts(KeyboardButtonCode.KC_F1);
            skinMenu.TransparencyChanged += changeSkinTransparency;

            musclesMenu = new LayerGUIMenu(layerPanel.findWidget("Layers/Muscles") as Button, layerPanel.findWidget("Layers/MusclesMenu") as Button);
            musclesMenu.createShortcuts(KeyboardButtonCode.KC_F2);
            musclesMenu.TransparencyChanged += changeMuscleTransparency;

            skullMenu = new LayerGUISkullMenu(layerPanel.findWidget("Layers/Skull") as Button, layerPanel.findWidget("Layers/SkullMenu") as Button);
            skullMenu.createShortcuts(KeyboardButtonCode.KC_F3);
            skullMenu.createEminanceShortcut(KeyboardButtonCode.KC_F4);
            skullMenu.TransparencyChanged += changeSkullTransparency;
            skullMenu.ToggleEminance += toggleShowEminance;

            mandibleMenu = new LayerGUIMenu(layerPanel.findWidget("Layers/Mandible") as Button, layerPanel.findWidget("Layers/MandibleMenu") as Button);
            mandibleMenu.createShortcuts(KeyboardButtonCode.KC_F5);
            mandibleMenu.TransparencyChanged += changeMandibleTransparency;

            discsMenu = new LayerGUIMenu(layerPanel.findWidget("Layers/Discs") as Button, layerPanel.findWidget("Layers/DiscsMenu") as Button);
            discsMenu.createShortcuts(KeyboardButtonCode.KC_F6);
            discsMenu.TransparencyChanged += changeDiscTransparency;

            spineMenu = new LayerGUIMenu(layerPanel.findWidget("Layers/Spine") as Button, layerPanel.findWidget("Layers/SpineMenu") as Button);
            spineMenu.createShortcuts(KeyboardButtonCode.KC_F7);
            spineMenu.TransparencyChanged += changeSpineTransparency;

            hyoidMenu = new LayerGUIMenu(layerPanel.findWidget("Layers/Hyoid") as Button, layerPanel.findWidget("Layers/HyoidMenu") as Button);
            hyoidMenu.createShortcuts(KeyboardButtonCode.KC_F8);
            hyoidMenu.TransparencyChanged += changeHyoidTransparency;

            topTeethMenu = new LayerGUIMenu(layerPanel.findWidget("Layers/MaxillaryTeeth") as Button, layerPanel.findWidget("Layers/MaxillaryTeethMenu") as Button);
            topTeethMenu.createShortcuts(KeyboardButtonCode.KC_F9);
            topTeethMenu.TransparencyChanged += changeTopToothTransparency;

            bottomTeethMenu = new LayerGUIMenu(layerPanel.findWidget("Layers/MandibularTeeth") as Button, layerPanel.findWidget("Layers/MandibularTeethMenu") as Button);
            bottomTeethMenu.createShortcuts(KeyboardButtonCode.KC_F10);
            bottomTeethMenu.TransparencyChanged += changeBottomToothTransparency;

            TransparencyController.ActiveTransparencyStateChanged += new EventHandler(TransparencyController_ActiveTransparencyStateChanged);
        }

        public void Dispose()
        {
            skinMenu.Dispose();
            musclesMenu.Dispose();
            skullMenu.Dispose();
            mandibleMenu.Dispose();
            discsMenu.Dispose();
            spineMenu.Dispose();
            hyoidMenu.Dispose();
            topTeethMenu.Dispose();
            bottomTeethMenu.Dispose();
            LayoutManager.Instance.unloadLayout(layout);
        }

        public void show(int left, int top)
        {
            popupContainer.show(left, top);
        }

        public void resetMenus()
        {
            skinMenu.setAlpha(1.0f);
            musclesMenu.setAlpha(1.0f);
            skullMenu.setAlpha(1.0f);
            mandibleMenu.setAlpha(1.0f);
            hyoidMenu.setAlpha(1.0f);
            spineMenu.setAlpha(1.0f);
            discsMenu.setAlpha(1.0f);
            topTeethMenu.setAlpha(1.0f);
            bottomTeethMenu.setAlpha(1.0f);
        }

        void layerStateChanged(LayerController controller)
        {
            synchronizeLayerMenus();
        }

        void TransparencyController_ActiveTransparencyStateChanged(object sender, EventArgs e)
        {
            synchronizeLayerMenus();
        }

        void synchronizeLayerMenus()
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
            if (group != null)
            {
                TransparencyInterface skull = group.getTransparencyObject("Skull");
                skullMenu.setAlpha(skull.CurrentAlpha);
                TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                skullMenu.ShowEminance = leftEminence.CurrentAlpha == skull.CurrentAlpha;
            }
            group = TransparencyController.getTransparencyGroup(RenderGroup.TMJ);
            if (group != null)
            {
                TransparencyInterface leftDisc = group.getTransparencyObject("Left TMJ Disc");
                discsMenu.setAlpha(leftDisc.CurrentAlpha);
            }
            group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
            if (group != null)
            {
                TransparencyInterface mandible = group.getTransparencyObject("Mandible");
                mandibleMenu.setAlpha(mandible.CurrentAlpha);
            }
            group = TransparencyController.getTransparencyGroup(RenderGroup.Teeth);
            if (group != null)
            {
                TransparencyInterface topTooth = group.getTransparencyObject("Tooth 1");
                topTeethMenu.setAlpha(topTooth.CurrentAlpha);
                TransparencyInterface bottomTooth = group.getTransparencyObject("Tooth 17");
                bottomTeethMenu.setAlpha(bottomTooth.CurrentAlpha);
            }
            group = TransparencyController.getTransparencyGroup(RenderGroup.Skin);
            if (group != null)
            {
                TransparencyInterface skin = group.getTransparencyObject("Skin");
                skinMenu.setAlpha(skin.CurrentAlpha);
            }
            group = TransparencyController.getTransparencyGroup(RenderGroup.Muscles);
            if (group != null)
            {
                TransparencyInterface muscle = group.getTransparencyObject("Left Masseter");
                musclesMenu.setAlpha(muscle.CurrentAlpha);
            }
            group = TransparencyController.getTransparencyGroup(RenderGroup.Spine);
            if (group != null)
            {
                TransparencyInterface spine = group.getTransparencyObject("C1");
                spineMenu.setAlpha(spine.CurrentAlpha);
            }
            group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
            if (group != null)
            {
                TransparencyInterface hyoid = group.getTransparencyObject("Hyoid");
                hyoidMenu.setAlpha(hyoid.CurrentAlpha);
            }
        }

        public bool AllowShortcuts
        {
            get
            {
                if (skinMenu != null)
                {
                    return skinMenu.AllowShortcuts;
                }
                return false;
            }
            set
            {
                if (skinMenu != null)
                {
                    skinMenu.AllowShortcuts = value;
                    musclesMenu.AllowShortcuts = value;
                    skullMenu.AllowShortcuts = value;
                    mandibleMenu.AllowShortcuts = value;
                    discsMenu.AllowShortcuts = value;
                    spineMenu.AllowShortcuts = value;
                    hyoidMenu.AllowShortcuts = value;
                    topTeethMenu.AllowShortcuts = value;
                    bottomTeethMenu.AllowShortcuts = value;
                }
            }
        }

        #region Transparency Helper Functions

        void changeHyoidTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
            TransparencyInterface hyoid = group.getTransparencyObject("Hyoid");
            hyoid.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
        }

        private void changeSkullTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
            TransparencyInterface skull = group.getTransparencyObject("Skull");
            skull.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            TransparencyInterface skullInterior = group.getTransparencyObject("Skull Interior");
            skullInterior.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            if (skullMenu.ShowEminance)
            {
                TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
                leftEminence.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
                rightEminence.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            }
            TransparencyInterface maxillarySinus = group.getTransparencyObject("Maxillary Sinus");
            maxillarySinus.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
        }

        private void changeDiscTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.TMJ);
            TransparencyInterface leftDisc = group.getTransparencyObject("Left TMJ Disc");
            leftDisc.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            TransparencyInterface rightDisc = group.getTransparencyObject("Right TMJ Disc");
            rightDisc.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
        }

        private void changeMandibleTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
            TransparencyInterface skull = group.getTransparencyObject("Mandible");
            skull.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
        }

        private void changeTopToothTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Teeth);
            for (int i = 1; i < 17; ++i)
            {
                group.getTransparencyObject("Tooth " + i).smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            }
        }

        private void changeBottomToothTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Teeth);
            for (int i = 17; i < 33; ++i)
            {
                group.getTransparencyObject("Tooth " + i).smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            }
        }

        private void changeSkinTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Skin);
            TransparencyInterface skin = group.getTransparencyObject("Skin");
            skin.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            TransparencyInterface leftEye = group.getTransparencyObject("Left Eye");
            leftEye.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            TransparencyInterface rightEye = group.getTransparencyObject("Right Eye");
            rightEye.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            TransparencyInterface eyebrowsAndEyelashes = group.getTransparencyObject("Eyebrows and Eyelashes");
            eyebrowsAndEyelashes.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
        }

        private void changeMuscleTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Muscles);
            foreach (TransparencyInterface item in group.getTransparencyObjectIter())
            {
                item.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            }
        }

        private void changeSpineTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Spine);
            foreach (TransparencyInterface item in group.getTransparencyObjectIter())
            {
                item.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            }
        }

        private void toggleShowEminance(bool show)
        {
            if (show)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Skull");
                TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
                leftEminence.smoothBlend(skull.CurrentAlpha, MedicalConfig.TransparencyChangeMultiplier);
                rightEminence.smoothBlend(skull.CurrentAlpha, MedicalConfig.TransparencyChangeMultiplier);
            }
            else
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
                leftEminence.smoothBlend(0.0f, MedicalConfig.TransparencyChangeMultiplier);
                rightEminence.smoothBlend(0.0f, MedicalConfig.TransparencyChangeMultiplier);
            }
        }

        #endregion
    }
}
