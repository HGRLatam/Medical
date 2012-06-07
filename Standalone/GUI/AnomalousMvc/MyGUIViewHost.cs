﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    public class MyGUIViewHost : ViewHost
    {
        private ViewHostComponent component;
        private MyGUILayoutContainer layoutContainer;
        private AnomalousMvcContext context;

        public MyGUIViewHost(AnomalousMvcContext context)
        {
            this.context = context;
        }

        public void setTopComponent(ViewHostComponent component)
        {
            this.component = component;
            layoutContainer = new MyGUILayoutContainer(component.Widget);
            layoutContainer.LayoutChanged += new Action(layoutContainer_LayoutChanged);
        }

        public void Dispose()
        {
            component.Dispose();
        }

        public void opening()
        {
            component.opening();
        }

        public void closing()
        {
            component.closing();
        }

        public LayoutContainer Container
        {
            get
            {
                return layoutContainer;
            }
        }

        public AnomalousMvcContext Context
        {
            get
            {
                return context;
            }
        }

        public bool _RequestClosed { get; set; }

        public void _animationCallback(LayoutContainer oldChild)
        {
            Dispose();
        }

        void layoutContainer_LayoutChanged()
        {
            component.topLevelResized();
        }
    }
}
