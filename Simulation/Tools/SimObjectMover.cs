﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.ObjectManagement;
using Engine.Renderer;
using Engine.Platform;

namespace Medical
{
    public class SimObjectMover
    {
        static SimObjectMover()
        {
            MessageEvent pickEvent = new MessageEvent(ToolEvents.Pick);
            pickEvent.addButton(MouseButtonCode.MB_BUTTON0);
            DefaultEvents.registerDefaultEvent(pickEvent);

            MessageEvent increaseToolSize = new MessageEvent(ToolEvents.IncreaseToolSize);
            increaseToolSize.addButton(KeyboardButtonCode.KC_EQUALS);
            DefaultEvents.registerDefaultEvent(increaseToolSize);

            MessageEvent decreaseToolSize = new MessageEvent(ToolEvents.DecreaseToolSize);
            decreaseToolSize.addButton(KeyboardButtonCode.KC_MINUS);
            DefaultEvents.registerDefaultEvent(decreaseToolSize);
        }

        private PluginManager pluginManager;
        private String name;
        private DebugDrawingSurface drawingSurface = null;
        private List<MovableObjectTools> movableObjects = new List<MovableObjectTools>();
        private EventManager events;
        private MovableObjectTools currentTools = null;
        private bool showMoveTools = false;
        private bool showRotateTools = false;

        public SimObjectMover(String name, PluginManager pluginManager, EventManager events)
        {
            this.pluginManager = pluginManager;
            this.name = name;
            this.events = events;
        }

        public void sceneLoaded(SimScene scene)
        {
            SimSubScene subScene = scene.getDefaultSubScene();
            if (subScene != null)
            {
                drawingSurface = pluginManager.RendererPlugin.createDebugDrawingSurface(name + "DebugSurface", subScene);
                drawingSurface.setDepthTesting(false);
            }
        }

        public void sceneUnloading(SimScene scene)
        {
            if (drawingSurface != null)
            {
                pluginManager.RendererPlugin.destroyDebugDrawingSurface(drawingSurface);
                drawingSurface = null;
            }
        }

        public void update(Clock clock)
        {
            //Process the mouse
            Mouse mouse = events.Mouse;
            Vector3 mouseLoc = mouse.getAbsMouse();
            Ray3 spaceRay = new Ray3();
            Vector3 cameraPos = Vector3.Zero;
            CameraMotionValidator validator = CameraResolver.getValidatorForLocation((int)mouseLoc.x, (int)mouseLoc.y);
            if (validator != null)
            {
                validator.getLocalCoords(ref mouseLoc.x, ref mouseLoc.y);
                SceneView camera = validator.getCamera();
                spaceRay = camera.getCameraToViewportRay(mouseLoc.x / validator.getMouseAreaWidth(), mouseLoc.y / validator.getMouseAreaHeight());
                cameraPos = camera.Translation;              
            }
            //Check collisions and draw shapes
            if (!events[ToolEvents.Pick].Down)
            {
                float closestDistance = float.MaxValue;
                MovableObjectTools closestTools = null;
                foreach (MovableObjectTools tools in movableObjects)
                {
                    if (tools.checkBoundingBoxCollision(ref spaceRay))
                    {
                        if (tools.processAxes(ref spaceRay))
                        {
                            float distance = (tools.Movable.ToolTranslation - cameraPos).length2();
                            if (distance < closestDistance)
                            {
                                //If we had a previous closer tool clear its selection.
                                if (closestTools != null)
                                {
                                    closestTools.clearSelection();
                                }
                                closestTools = tools;
                                closestDistance = distance;
                            }
                            //If this tool was not closer clear its selection.
                            else
                            {
                                tools.clearSelection();
                            }
                        }
                        else
                        {
                            tools.clearSelection();
                        }
                    }
                    else
                    {
                        tools.clearSelection();
                    }
                }
                if (events[ToolEvents.Pick].FirstFrameDown)
                {
                    currentTools = closestTools;
                    if (closestTools != null)
                    {
                        closestTools.processSelection(events, ref cameraPos, ref spaceRay);
                    }
                }
            }
            else
            {
                if (currentTools != null)
                {
                    currentTools.processSelection(events, ref cameraPos, ref spaceRay);
                }
            }  
            foreach (MovableObjectTools tools in movableObjects)
            {
                tools.drawTools(drawingSurface);
            }
        }

        public void addMovableObject(String name, MovableObject movable)
        {
            MovableObjectTools tools = new MovableObjectTools(name, movable);
            movableObjects.Add(tools);
            tools.MoveToolVisible = showMoveTools;
            tools.RotateToolVisible = showRotateTools;
        }

        public void removeMovableObject(MovableObject movable)
        {
            MovableObjectTools tools = null;
            foreach (MovableObjectTools currentTools in movableObjects)
            {
                if (currentTools.Movable == movable)
                {
                    tools = currentTools;
                    break;
                }
            }
            if (tools != null)
            {
                movableObjects.Remove(tools);
            }
        }

        public void setDrawingSurfaceVisible(bool visible)
        {
            if (drawingSurface != null)
            {
                drawingSurface.setVisible(visible);
            }
        }

        public void setActivePlanes(MovementAxis axes, MovementPlane planes)
        {
            foreach (MovableObjectTools currentTools in movableObjects)
            {
                currentTools.setActiveAxes(axes);
                currentTools.setActivePlanes(planes);
            }
        }

        public bool ShowMoveTools
        {
            get
            {
                return showMoveTools;
            }
            set
            {
                showMoveTools = value;
                foreach (MovableObjectTools tools in movableObjects)
                {
                    tools.MoveToolVisible = value;
                }
            }
        }

        public bool ShowRotateTools
        {
            get
            {
                return showRotateTools;
            }
            set
            {
                showRotateTools = value;
                foreach (MovableObjectTools tools in movableObjects)
                {
                    tools.RotateToolVisible = value;
                }
            }
        }
    }
}
