﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;
using Medical.Controller;
using Anomalous.GuiFramework;

namespace Medical.GUI.AnomalousMvc
{
    class DecoratorComponentFactory : ViewHostComponentFactory
    {
        private List<ViewHostComponentFactory> concreteComponentFactories = new List<ViewHostComponentFactory>();
        private MDILayoutManager mdiManager;

        public DecoratorComponentFactory(MDILayoutManager mdiManager)
        {
            this.mdiManager = mdiManager;
        }

        public void addFactory(ViewHostComponentFactory factory)
        {
            concreteComponentFactories.Add(factory);
        }

        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            ViewHostComponent component = null;
            foreach (ViewHostComponentFactory factory in concreteComponentFactories)
            {
                component = factory.createViewHostComponent(view, context, viewHost);
                if (component != null)
                {
                    break;
                }
            }

            if(component == null)
            {
                return component;
            }

            switch (view.ElementName.ViewType)
            {
                case ViewType.Window:
                    MDIDialogDecorator dialogDecorator = new MDIDialogDecorator(mdiManager, component, view);
                    switch (view.ElementName.LocationHint)
                    {
                        case ViewLocations.Left:
                            dialogDecorator.CurrentDockLocation = DockLocation.Left;
                            break;
                        case ViewLocations.Right:
                            dialogDecorator.CurrentDockLocation = DockLocation.Right;
                            break;
                        case ViewLocations.Top:
                            dialogDecorator.CurrentDockLocation = DockLocation.Top;
                            break;
                        case ViewLocations.Bottom:
                            dialogDecorator.CurrentDockLocation = DockLocation.Bottom;
                            break;
                        case ViewLocations.Floating:
                            dialogDecorator.CurrentDockLocation = DockLocation.Floating;
                            break;
                    }
                    component = dialogDecorator;
                    break;
                case ViewType.Panel:
                    switch (view.ElementName.LocationHint)
                    {
                        case ViewLocations.Left:
                            component = new SidePanelDecorator(component, view.Buttons, view.Transparent);
                            break;
                        case ViewLocations.Right:
                            component = new SidePanelDecorator(component, view.Buttons, view.Transparent);
                            break;
                        case ViewLocations.Top:
                            component = new TopBottomPanelDecorator(component, view.Buttons, view.Transparent);
                            break;
                        case ViewLocations.Bottom:
                            component = new TopBottomPanelDecorator(component, view.Buttons, view.Transparent);
                            break;
                        case ViewLocations.Floating:
                            component = new FloatingPanelDecorator(component, view.Buttons, view);
                            break;
                    }
                    break;
            }

            return component;
        }

        public void createViewBrowser(Browser browser)
        {
            foreach (ViewHostComponentFactory factory in concreteComponentFactories)
            {
                factory.createViewBrowser(browser);
            }
        }
    }
}
