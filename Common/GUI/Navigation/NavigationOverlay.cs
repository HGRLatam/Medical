﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;
using Engine.Platform;
using Engine;
using Logging;

namespace Medical
{
    enum NavigationEvents
    {
        ClickButton,
    }

    class NavigationOverlay : IDisposable, UpdateListener
    {
        private Overlay mainOverlay;
        private List<NavigationButton> buttons = new List<NavigationButton>();
        private bool showOverlay = false;
        private EventManager eventManager;
        private CameraMotionValidator motionValidator = null;
        private NavigationButton currentButton = null;
        private NavigationController navigationController;
        private String name;
        private OrbitCameraController orbitCamera;

        static NavigationOverlay()
        {
            OgreResourceGroupManager.getInstance().addResourceLocation(Engine.Resources.Resource.ResourceRoot + "/GUI", "EngineArchive", "Embedded", false);
            OgreResourceGroupManager.getInstance().initializeAllResourceGroups();

            MessageEvent clickButton = new MessageEvent(NavigationEvents.ClickButton);
            clickButton.addButton(MouseButtonCode.MB_BUTTON0);
            DefaultEvents.registerDefaultEvent(clickButton);
        }

        public NavigationOverlay(String name, EventManager eventManager, CameraMotionValidator motionValidator, NavigationController navigationController, OrbitCameraController orbitCamera)
        {
            this.name = name;
            this.eventManager = eventManager;
            this.motionValidator = motionValidator;
            this.navigationController = navigationController;
            this.orbitCamera = orbitCamera;

            mainOverlay = OverlayManager.getInstance().create(name + "_NavigationOverlay");
            //NavigationButton rightButton = new NavigationButton(name + "_RightButton", "NavigationArrow", new OverlayRect(-40, -20, 40, 40), new OverlayRect(0f, 0f, .25f, .5f), new OverlayRect(.25f, 0f, .5f, .5f), new OverlayRect(.5f, 0f, .75f, .5f));
            //rightButton.HorizontalAlignment = GuiHorizontalAlignment.GHA_RIGHT;
            //rightButton.VerticalAlignment = GuiVerticalAlignment.GVA_CENTER;
            //rightButton.Clicked += new NavigationButtonClicked(rightButton_Clicked);
            //mainOverlay.add2d(rightButton.PanelElement);
            //buttons.Add(rightButton);

            //NavigationButton leftButton = new NavigationButton(name + "_LeftButton", "NavigationArrow", new OverlayRect(0, -20, 40, 40), new OverlayRect(.25f, 0f, 0f, .5f), new OverlayRect(.5f, 0f, .25f, .5f), new OverlayRect(.75f, 0f, .5f, .5f));
            //leftButton.HorizontalAlignment = GuiHorizontalAlignment.GHA_LEFT;
            //leftButton.VerticalAlignment = GuiVerticalAlignment.GVA_CENTER;
            //leftButton.Clicked += new NavigationButtonClicked(leftButton_Clicked);
            //mainOverlay.add2d(leftButton.PanelElement);
            //buttons.Add(leftButton);

            //NavigationButton upButton = new NavigationButton(name + "_UpButton", "NavigationArrow", new OverlayRect(-20, 0, 40, 40), new OverlayRect(0f, .5f, .25f, 1.0f), new OverlayRect(.5f, .5f, .75f, 1.0f), new OverlayRect(.75f, .5f, .5f, 1.0f));
            //upButton.HorizontalAlignment = GuiHorizontalAlignment.GHA_CENTER;
            //upButton.VerticalAlignment = GuiVerticalAlignment.GVA_TOP;
            //upButton.Clicked += new NavigationButtonClicked(upButton_Clicked);
            //mainOverlay.add2d(upButton.PanelElement);
            //buttons.Add(upButton);

            //NavigationButton downButton = new NavigationButton(name + "_DownButton", "NavigationArrow", new OverlayRect(-20, -40, 40, 40), new OverlayRect(0f, 1.0f, .25f, 0.5f), new OverlayRect(.5f, 1.0f, .75f, 0.5f), new OverlayRect(.75f, 1.0f, .5f, 0.5f));
            //downButton.HorizontalAlignment = GuiHorizontalAlignment.GHA_CENTER;
            //downButton.VerticalAlignment = GuiVerticalAlignment.GVA_BOTTOM;
            //downButton.Clicked += new NavigationButtonClicked(downButton_Clicked);
            //mainOverlay.add2d(downButton.PanelElement);
            //buttons.Add(downButton);
        }

        public void Dispose()
        {
            foreach (NavigationButton button in buttons)
            {
                button.Dispose();
            }
            OverlayManager.getInstance().destroy(mainOverlay);
        }

        public void setNavigationState(NavigationState state)
        {
            foreach (NavigationButton button in buttons)
            {
                mainOverlay.remove2d(button.PanelElement);
                button.Dispose();
            }
            buttons.Clear();
            foreach (NavigationState adjacent in state.AdjacentStates)
            {
                NavigationButton navButton = new NavigationButton(name + "_Navigation_" + state.Name, "NavigationArrow", new OverlayRect(-20, -40, 40, 40), new OverlayRect(0f, 1.0f, .25f, 0.5f), new OverlayRect(.5f, 1.0f, .75f, 0.5f), new OverlayRect(.75f, 1.0f, .5f, 0.5f));
                navButton.HorizontalAlignment = GuiHorizontalAlignment.GHA_CENTER;
                navButton.VerticalAlignment = GuiVerticalAlignment.GVA_BOTTOM;
                navButton.Clicked += new NavigationButtonClicked(navButton_Clicked);
                navButton.State = adjacent;
                mainOverlay.add2d(navButton.PanelElement);
                buttons.Add(navButton);
            }
        }

        /// <summary>
        /// Allow the overlay to render when it is being shown by its camera.
        /// </summary>
        public bool ShowOverlay
        {
            get
            {
                return showOverlay;
            }
            set
            {
                showOverlay = value;
                //hide the overlay if it is visible.
                if (!showOverlay && mainOverlay.isVisible())
                {
                    mainOverlay.hide();
                }
            }
        }

        /// <summary>
        /// Called by the camera when it is rendering.
        /// </summary>
        /// <param name="visible"></param>
        internal void setVisible(bool visible)
        {
            if (showOverlay && visible && !mainOverlay.isVisible())
            {
                mainOverlay.show();
            }
            else if (showOverlay && !visible && mainOverlay.isVisible())
            {
                mainOverlay.hide();
            }
        }

        void navButton_Clicked(NavigationButton source)
        {
            orbitCamera.setNewPosition(source.State.Translation, source.State.LookAt);
            setNavigationState(source.State);
        }

        #region UpdateListener Members

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        public void sendUpdate(Clock clock)
        {
            if (showOverlay)
            {
                bool firstFrame = eventManager[NavigationEvents.ClickButton].FirstFrameDown;
                if (firstFrame)
                {
                    currentButton = null;
                }
                bool mouseClicked = eventManager[NavigationEvents.ClickButton].Down || firstFrame || eventManager[NavigationEvents.ClickButton].FirstFrameUp;
                Vector3 mouseCoords = eventManager.Mouse.getAbsMouse();
                if (motionValidator != null && motionValidator.allowMotion((int)mouseCoords.x, (int)mouseCoords.y))
                {
                    motionValidator.getLocalCoords(ref mouseCoords.x, ref mouseCoords.y);
                    foreach (NavigationButton button in buttons)
                    {
                        if (button.process(mouseCoords, mouseClicked, motionValidator.getMouseAreaWidth(), motionValidator.getMouseAreaHeight()))
                        {
                            if (eventManager[NavigationEvents.ClickButton].FirstFrameDown)
                            {
                                currentButton = button;
                            }
                            else if (eventManager[NavigationEvents.ClickButton].FirstFrameUp && currentButton == button)
                            {
                                button.fireClickEvent();
                                currentButton = null;
                            }
                            break;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
