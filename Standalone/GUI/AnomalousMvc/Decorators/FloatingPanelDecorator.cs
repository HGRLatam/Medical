﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;
using Engine;

namespace Medical.GUI.AnomalousMvc
{
    class FloatingPanelDecorator : Component, ViewHostComponent
    {
        private ViewHostComponent child;

        public FloatingPanelDecorator(ViewHostComponent child, ButtonCollection buttons, View view)
            : base(view.Transparent ? "Medical.GUI.AnomalousMvc.Decorators.SidePanelDecoratorTransparent.layout" : "Medical.GUI.AnomalousMvc.Decorators.SidePanelDecorator.layout")
        {
            if (buttons.Count > 0)
            {
                child = new ButtonDecorator(child, buttons);
            }

            this.child = child;
            widget.setSize(child.Widget.Right + int.Parse(widget.getUserString("ChildWidthOffset")), child.Widget.Bottom + int.Parse(widget.getUserString("ChildHeightOffset")));
            child.Widget.attachToWidget(widget);
            child.Widget.setPosition(int.Parse(widget.getUserString("ChildX")), int.Parse(widget.getUserString("ChildY")));
            child.Widget.Align = Align.HStretch | Align.VStretch;
        }

        public override void Dispose()
        {
            child.Dispose();
            base.Dispose();
        }

        public void topLevelResized()
        {
            child.topLevelResized();
        }

        public void animatedResizeStarted(IntSize2 finalSize)
        {
            child.animatedResizeStarted(finalSize);
        }

        public void animatedResizeCompleted(IntSize2 finalSize)
        {
            child.animatedResizeCompleted(finalSize);
        }

        public void opening()
        {
            child.opening();
        }

        public void closing()
        {
            child.closing();
        }

        public void populateViewData(IDataProvider dataProvider)
        {
            child.populateViewData(dataProvider);
        }

        public void analyzeViewData(IDataProvider dataProvider)
        {
            child.analyzeViewData(dataProvider);
        }

        public ViewHostControl findControl(string name)
        {
            return child.findControl(name);
        }

        public void changeScale(float newScale)
        {
            child.changeScale(newScale);
        }

        public MyGUIViewHost ViewHost
        {
            get
            {
                return child.ViewHost;
            }
        }

        public Widget Widget
        {
            get
            {
                return widget; 
            }
        }
    }
}
