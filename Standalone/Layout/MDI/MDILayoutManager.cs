﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.Controller
{
    public enum WindowAlignment
    {
        Left,
        Right,
        Top,
        Bottom,
    }

    public class MDILayoutManager : LayoutContainer, IDisposable
    {
        private List<MDIWindow> windows = new List<MDIWindow>();
        private LayoutContainer rootContainer = null;
        private List<MDILayoutContainer> childContainers = new List<MDILayoutContainer>();

        public MDILayoutManager()
        {

        }

        public void Dispose()
        {
            foreach (MDILayoutContainer child in childContainers)
            {
                child.Dispose();
            }
        }

        public void addWindow(MDIWindow window)
        {
            //Normal operation where the root is the MDILayoutContainer as expected
            if (rootContainer is MDILayoutContainer)
            {
                MDILayoutContainer mdiRoot = rootContainer as MDILayoutContainer;
                mdiRoot.addChild(window);
                window._CurrentContainer = mdiRoot;
            }
            //If no other containers have been added, use the root window directly.
            else if (rootContainer == null)
            {
                rootContainer = window;
                window._CurrentContainer = null;
                window._setParent(this);
            }
            //If one other container has been added, create a horizontal alignment and readd both containers to it
            else if (rootContainer is MDIWindow)
            {
                MDILayoutContainer horizRoot = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, 5);
                horizRoot._setParent(this);
                childContainers.Add(horizRoot);
                MDIWindow oldRoot = rootContainer as MDIWindow;
                horizRoot.addChild(oldRoot);
                oldRoot._CurrentContainer = horizRoot;
                rootContainer = horizRoot;

                horizRoot.addChild(window);
                window._CurrentContainer = horizRoot;
            }
            invalidate();
        }

        public void addWindow(MDIWindow window, MDIWindow previous, WindowAlignment alignment)
        {
            switch (alignment)
            {
                case WindowAlignment.Left:
                    if (previous._CurrentContainer == null && previous == rootContainer)
                    {
                        MDILayoutContainer parentContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, 5);
                        parentContainer._setParent(this);
                        previous._CurrentContainer = parentContainer;
                        window._CurrentContainer = parentContainer;
                        parentContainer.addChild(previous);
                        parentContainer.addChild(window);
                        rootContainer = parentContainer;
                    }
                    else if (previous._CurrentContainer.Layout == MDILayoutContainer.LayoutType.Horizontal)
                    {
                        window._CurrentContainer = previous._CurrentContainer;
                        previous._CurrentContainer.insertChild(window, previous, true);
                    }
                    else
                    {
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, 5);
                        MDILayoutContainer parentContainer = previous._CurrentContainer;
                        parentContainer.swapAndRemove(newContainer, previous);
                        previous._CurrentContainer = newContainer;
                        window._CurrentContainer = newContainer;
                        newContainer.addChild(previous);
                        newContainer.addChild(window);
                    }
                    break;
                case WindowAlignment.Right:
                    if (previous._CurrentContainer == null && previous == rootContainer)
                    {
                        MDILayoutContainer parentContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, 5);
                        parentContainer._setParent(this);
                        previous._CurrentContainer = parentContainer;
                        window._CurrentContainer = parentContainer;
                        parentContainer.addChild(window);
                        parentContainer.addChild(previous);
                        rootContainer = parentContainer;
                    }
                    else if (previous._CurrentContainer.Layout == MDILayoutContainer.LayoutType.Horizontal)
                    {
                        window._CurrentContainer = previous._CurrentContainer;
                        previous._CurrentContainer.insertChild(window, previous, false);
                    }
                    else
                    {
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, 5);
                        MDILayoutContainer parentContainer = previous._CurrentContainer;
                        parentContainer.swapAndRemove(newContainer, previous);
                        previous._CurrentContainer = newContainer;
                        window._CurrentContainer = newContainer;
                        newContainer.addChild(window);
                        newContainer.addChild(previous);
                    }
                    break;
                case WindowAlignment.Top:
                    if (previous._CurrentContainer == null && previous == rootContainer)
                    {
                        MDILayoutContainer parentContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, 5);
                        parentContainer._setParent(this);
                        previous._CurrentContainer = parentContainer;
                        window._CurrentContainer = parentContainer;
                        parentContainer.addChild(window);
                        parentContainer.addChild(previous);
                        rootContainer = parentContainer;
                    }
                    else if (previous._CurrentContainer.Layout == MDILayoutContainer.LayoutType.Vertical)
                    {
                        window._CurrentContainer = previous._CurrentContainer;
                        previous._CurrentContainer.insertChild(window, previous, false);
                    }
                    else
                    {
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, 5);
                        MDILayoutContainer parentContainer = previous._CurrentContainer;
                        parentContainer.swapAndRemove(newContainer, previous);
                        previous._CurrentContainer = newContainer;
                        window._CurrentContainer = newContainer;
                        newContainer.addChild(window);
                        newContainer.addChild(previous);
                    }
                    break;
                case WindowAlignment.Bottom:
                    if (previous._CurrentContainer == null && previous == rootContainer)
                    {
                        MDILayoutContainer parentContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, 5);
                        parentContainer._setParent(this);
                        previous._CurrentContainer = parentContainer;
                        window._CurrentContainer = parentContainer;
                        parentContainer.addChild(previous);
                        parentContainer.addChild(window);
                        rootContainer = parentContainer;
                    }
                    else if (previous._CurrentContainer.Layout == MDILayoutContainer.LayoutType.Vertical)
                    {
                        window._CurrentContainer = previous._CurrentContainer;
                        previous._CurrentContainer.insertChild(window, previous, true);
                    }
                    else
                    {
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, 5);
                        MDILayoutContainer parentContainer = previous._CurrentContainer;
                        parentContainer.swapAndRemove(newContainer, previous);
                        previous._CurrentContainer = newContainer;
                        window._CurrentContainer = newContainer;
                        newContainer.addChild(previous);
                        newContainer.addChild(window);
                    }
                    break;
            }
            invalidate();
        }

        public override void bringToFront()
        {
            if (rootContainer != null)
            {
                rootContainer.bringToFront();
            }
        }

        public override void setAlpha(float alpha)
        {
            if (rootContainer != null)
            {
                rootContainer.setAlpha(alpha);
            }
        }

        public override void layout()
        {
            if (rootContainer != null)
            {
                rootContainer.WorkingSize = WorkingSize;
                rootContainer.Location = Location;
                rootContainer.layout();
            }
        }

        public override Size2 DesiredSize
        {
            get 
            {
                if (rootContainer != null)
                {
                    return rootContainer.DesiredSize;
                }
                return new Size2();
            }
        }

        private bool visible = true;

        public override bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
                if (rootContainer != null)
                {
                    rootContainer.Visible = value;
                }
            }
        }
    }
}
