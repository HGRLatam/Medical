﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    delegate void CanvasSizeChanged(float newSize);
    delegate void CanvasPositionChanged(CanvasEventArgs info);

    class TimelineScrollView
    {
        private ScrollView scrollView;
        public event CanvasSizeChanged CanvasWidthChanged;
        public event CanvasSizeChanged CanvasHeightChanged;
        public event CanvasPositionChanged CanvasPositionChanged;

        public TimelineScrollView(ScrollView scrollView)
        {
            this.scrollView = scrollView;
            scrollView.CanvasPositionChanged += new MyGUIEvent(scrollView_CanvasPositionChanged);
        }

        void scrollView_CanvasPositionChanged(Widget source, EventArgs e)
        {
            if (CanvasPositionChanged != null)
            {
                CanvasEventArgs cea = e as CanvasEventArgs;
                CanvasPositionChanged.Invoke(cea);
            }
        }

        public Button createButton(float left, float width)
        {
            Button button = scrollView.createWidgetT("Button", "Button", (int)left, 0, width > 10 ? (int)width : 10, 10, Align.Left | Align.Top, "") as Button;
            if (button.Right > scrollView.CanvasSize.Width)
            {
                CanvasWidth = button.Right;
            }
            return button;
        }

        public float CanvasWidth
        {
            get
            {
                return scrollView.CanvasSize.Width;
            }
            set
            {
                Size2 canvasSize = scrollView.CanvasSize;
                canvasSize.Width = value;
                scrollView.CanvasSize = canvasSize;
                if (CanvasWidthChanged != null)
                {
                    CanvasWidthChanged.Invoke(value);
                }
            }
        }

        public float CanvasHeight
        {
            get
            {
                return scrollView.CanvasSize.Height;
            }
            set
            {
                Size2 canvasSize = scrollView.CanvasSize;
                if (canvasSize.Height != value)
                {
                    canvasSize.Height = value;
                    scrollView.CanvasSize = canvasSize;
                    if (CanvasHeightChanged != null)
                    {
                        CanvasHeightChanged.Invoke(value);
                    }
                }
            }
        }

        public IntCoord ClientCoord
        {
            get
            {
                return scrollView.ClientCoord;
            }
        }

        public Vector2 CanvasPosition
        {
            get
            {
                return scrollView.CanvasPosition;
            }
            set
            {
                scrollView.CanvasPosition = value;
            }
        }

        public bool AllowMouseScroll
        {
            get
            {
                return scrollView.AllowMouseScroll;
            }
            set
            {
                scrollView.AllowMouseScroll = value;
            }
        }

        public int AbsoluteLeft
        {
            get
            {
                return scrollView.AbsoluteLeft;
            }
        }

        public int AbsoluteTop
        {
            get
            {
                return scrollView.AbsoluteTop;
            }
        }

        public int Width
        {
            get
            {
                return scrollView.Width;
            }
        }

        public int Height
        {
            get
            {
                return scrollView.Height;
            }
        }
    }
}
