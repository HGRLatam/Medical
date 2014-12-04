﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Medical.Controller;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class AnatomyContextWindowManager : IDisposable
    {
        private static readonly int ThumbRenderSize = ScaleHelper.Scaled(150);

        private AnatomyContextWindow currentAnatomyWindow;
        private SceneViewController sceneViewController;
        private AnatomyFinder anatomyFinder;

        private AnatomyController anatomyController;
        private LayerController layerController;
        private List<AnatomyContextWindow> pinnedWindows = new List<AnatomyContextWindow>();

        private LiveThumbnailController liveThumbnailController;

        public AnatomyContextWindowManager(SceneViewController sceneViewController, AnatomyController anatomyController, LayerController layerController, AnatomyFinder anatomyFinder)
        {
            this.sceneViewController = sceneViewController;
            this.anatomyController = anatomyController;
            this.anatomyController.SelectedAnatomy.SelectedAnatomyChanged += anatomyController_SelectedAnatomyChanged;
            this.anatomyFinder = anatomyFinder;
            this.layerController = layerController;

            liveThumbnailController = new LiveThumbnailController("ContextWindows_", new IntSize2(ThumbRenderSize, ThumbRenderSize), sceneViewController);
            liveThumbnailController.MaxPoolSize = 1;
        }

        public void Dispose()
        {
            if (currentAnatomyWindow != null)
            {
                currentAnatomyWindow.Dispose();
                currentAnatomyWindow = null;
            }
            liveThumbnailController.Dispose();
        }

        public void sceneUnloading()
        {
            foreach (AnatomyContextWindow window in pinnedWindows)
            {
                window.Dispose();
            }
            pinnedWindows.Clear();
            if (currentAnatomyWindow != null)
            {
                currentAnatomyWindow.Dispose();
                currentAnatomyWindow = null;
            }
        }

        public AnatomyContextWindow showWindow(Anatomy anatomy, IntVector2 position, IntCoord deadZone)
        {
            if (currentAnatomyWindow == null)
            {
                currentAnatomyWindow = new AnatomyContextWindow(this, layerController);
                currentAnatomyWindow.SmoothShow = true;
            }
            currentAnatomyWindow.Anatomy = anatomy;
            currentAnatomyWindow.Visible = true;

            IntCoord windowCoord = new IntCoord(position.x, position.y, currentAnatomyWindow.Width, currentAnatomyWindow.Height);
            currentAnatomyWindow.Position = calculateChildPosition(anatomyFinder.DeadZone, windowCoord, anatomyFinder.TriggeredSelection);
            
            currentAnatomyWindow.ensureVisible();

            return currentAnatomyWindow;
        }

        public void closeUnpinnedWindow()
        {
            if (currentAnatomyWindow != null)
            {
                currentAnatomyWindow.Visible = false;
            }
        }

        public AnatomyCommandPermissions CommandPermissions
        {
            get
            {
                return anatomyController.CommandPermissions;
            }
        }

        internal void alertWindowPinned(AnatomyContextWindow window)
        {
            currentAnatomyWindow = null;
            pinnedWindows.Add(window);
        }

        internal void alertPinnedWindowClosed(AnatomyContextWindow window)
        {
            pinnedWindows.Remove(window);
        }

        internal AnatomyContextWindowLiveThumbHost getThumbnail(AnatomyContextWindow window)
        {
            Anatomy anatomy = window.Anatomy;
            Radian theta = sceneViewController.ActiveWindow.Camera.getFOVy();

            //Generate thumbnail
            AxisAlignedBox boundingBox = anatomy.WorldBoundingBox;
            Vector3 center = boundingBox.Center;

            Vector3 translation = center;
            Vector3 direction = anatomy.PreviewCameraDirection;
            translation += direction * boundingBox.DiagonalDistance / (float)Math.Tan(theta);

            LayerState layers = new LayerState(anatomy.TransparencyNames, 1.0f);

            //Create a new thumb host or update an existing one
            if (window.ThumbHost == null)
            {
                AnatomyContextWindowLiveThumbHost host = new AnatomyContextWindowLiveThumbHost(window)
                {
                    Layers = layers,
                    Translation = translation,
                    LookAt = center
                };
                liveThumbnailController.addThumbnailHost(host);
                liveThumbnailController.setVisibility(host, true);
                return host;                
            }
            else
            {
                window.ThumbHost.Translation = translation;
                window.ThumbHost.LookAt = center;
                window.ThumbHost.Layers = layers;
                liveThumbnailController.updateCameraAndLayers(window.ThumbHost);
                return window.ThumbHost;
            }
        }

        internal void returnThumbnail(AnatomyContextWindow window)
        {
            liveThumbnailController.removeThumbnailHost(window.ThumbHost);
        }

        internal void centerAnatomy(AnatomyContextWindow requestingWindow)
        {
            AxisAlignedBox boundingBox = requestingWindow.Anatomy.WorldBoundingBox;
            SceneViewWindow window = sceneViewController.ActiveWindow;
            Vector3 center = boundingBox.Center;

            float nearPlane = window.Camera.getNearClipDistance();
            float theta = window.Camera.getFOVy();
            float aspectRatio = window.Camera.getAspectRatio();
            if (aspectRatio < 1.0f)
            {
                theta *= aspectRatio;
            }

            Vector3 translation = center;
            Vector3 direction = (window.Translation - window.LookAt).normalized();
            translation += direction * boundingBox.DiagonalDistance / (float)Math.Tan(theta);
            CameraPosition cameraPosition = new CameraPosition()
            {
                Translation = translation,
                LookAt = center
            };

            window.setPosition(cameraPosition, MedicalConfig.CameraTransitionTime);
        }

        internal void showOnly(Anatomy anatomy)
        {
            LayerState currentLayers = LayerState.CreateAndCapture();
            TransparencyController.smoothSetAllAlphas(0.0f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
            anatomy.smoothBlend(1.0f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
            layerController.pushUndoState(currentLayers);
        }

        internal bool isContextWindowAtPoint(int x, int y)
        {
            if (currentAnatomyWindow != null && currentAnatomyWindow.Visible && currentAnatomyWindow.contains(x, y))
            {
                return true;
            }

            foreach(var window in pinnedWindows)
            {
                if(window.contains(x, y))
                {
                    return true;
                }
            }

            return false;
        }

        internal int determineContextWindowX(int left, int right)
        {
            if(currentAnatomyWindow != null && currentAnatomyWindow.Visible && currentAnatomyWindow.AbsoluteLeft < left)
            {
                return left - currentAnatomyWindow.Width;
            }
            return right;
        }

        void anatomyController_SelectedAnatomyChanged(AnatomySelection anatomySelection)
        {
            Anatomy anatomy = anatomySelection.Anatomy;
            if (anatomy != null)
            {
                showWindow(anatomy, anatomyFinder.DisplayHintLocation, anatomyFinder.DeadZone);
            }
            else
            {
                closeUnpinnedWindow();
            }
        }

        /// <summary>
        /// Callback from a context dialog to show the anatomy finder if its show button was pressed.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal void showAnatomyFinderFromContextDialog(AnatomyContextWindow window)
        {
            if (anatomyFinder.AllowAutoPosition)
            {
                IntCoord deadzone = new IntCoord(window.AbsoluteLeft, window.AbsoluteTop, window.Width, window.Height);
                IntCoord anatomyFinderCoord = new IntCoord(deadzone.Right, deadzone.top, anatomyFinder.Width, anatomyFinder.Height);
                anatomyFinder.Position = calculateChildPosition(deadzone, anatomyFinderCoord, false);
            }
            anatomyFinder.ensureVisible();
            anatomyFinder.Visible = true;
            anatomyFinder.bringToFront();
        }

        /// <summary>
        /// Compare two coordinates to find a position that a window can be opened relative to another. This will
        /// make sure the windows do not overlap and that they are in the desired orientation to one another. If eitherSide
        /// is true and child is to the left of parent it will be opened attached to parent's left side and to the right otherwise
        /// if eitherSide is false the child will always open to the right.
        /// </summary>
        /// <param name="parent">The coords of the parent window.</param>
        /// <param name="child">The coords of the child window.</param>
        /// <param name="eitherSide">True to determine the side based on a comparison of the lefts and false to always open to the right.</param>
        /// <returns>A new IntVector of coords to put the child window at.</returns>
        private static IntVector2 calculateChildPosition(IntCoord parent, IntCoord child, bool eitherSide)
        {
            IntVector2 position = new IntVector2(child.left, child.top);
            if (eitherSide && position.x < parent.left) //If we were triggerd by the anatomy finder and are trying to be to the right make sure that we are.
            {
                int widthOffset = child.width + 1;
                if (widthOffset < parent.left)
                {
                    position.x = parent.left - widthOffset;
                }
            }

            child.left = position.x;

            int windowTop = child.top;
            int windowBottom = child.Bottom;
            int windowLeft = child.left;
            int windowWidth = child.width;
            int windowRight = child.Right;

            int deadzoneTop = parent.top;
            int deadzoneBottom = parent.Bottom;
            int deadzoneLeft = parent.left;
            int deadzoneRight = parent.Right;

            //Check to see if the window is in the dead zone.
            if (((windowTop >= deadzoneTop && windowTop <= deadzoneBottom) ||
                (windowBottom >= deadzoneTop && windowBottom <= deadzoneBottom)) &&
                ((windowLeft >= deadzoneLeft && windowLeft <= deadzoneRight) ||
                (windowRight >= deadzoneLeft && windowRight <= deadzoneRight)))
            {
                if (windowWidth < RenderManager.Instance.ViewWidth - deadzoneRight) //We can fit to the right, but don't want to be on top of the window
                {
                    position = new IntVector2(deadzoneRight, windowTop);
                }
                else //Cannot fit to the right go to the left instead
                {
                    position = new IntVector2(deadzoneLeft - windowWidth, windowTop);
                }
            }

            return position;
        }
    }
}
