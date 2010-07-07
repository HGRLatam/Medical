﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Logging;

namespace Medical
{
    public delegate void AnimationCompletedDelegate(LayoutContainer oldChild);

    public class LeftPopoutLayoutContainer : LayoutContainer, UpdateListener
    {
        private UpdateTimer mainTimer;
        private LayoutContainer childContainer;
        private LayoutContainer oldChildContainer;

        private AnimationCompletedDelegate animationComplete;
        private float animationLength;
        private float currentTime;
        private bool animating = false;
        private float alpha = 1.0f;
        private Size2 oldSize;
        private Size2 newSize;
        private Size2 sizeDelta;
        private Size2 currentSize;

        public LeftPopoutLayoutContainer(UpdateTimer mainTimer)
        {
            this.mainTimer = mainTimer;
            mainTimer.addFixedUpdateListener(this);
        }

        public override void setAlpha(float alpha)
        {

        }

        public override void layout()
        {
            if (animating)
            {
                if (childContainer != null)
                {
                    childContainer.Location = Location;
                    childContainer.WorkingSize = currentSize;
                    childContainer.layout();
                }
                if (oldChildContainer != null)
                {
                    oldChildContainer.Location = Location;
                    oldChildContainer.WorkingSize = currentSize;
                    oldChildContainer.layout();
                }
            }
            else
            {
                if (childContainer != null)
                {
                    childContainer.Location = Location;
                    childContainer.WorkingSize = WorkingSize;
                    childContainer.layout();
                }
            }
        }

        public void changePanel(LayoutContainer childContainer, float animDuration, AnimationCompletedDelegate animationComplete)
        {
            //If we were animating when a new request comes in clear the old animation first.
            if (animating)
            {
                if (this.childContainer != null)
                {
                    this.childContainer.setAlpha(1.0f);
                    this.childContainer.WorkingSize = newSize;
                    this.childContainer.layout();
                }
                finishAnimation();
            }

            currentTime = 0.0f;
            animationLength = animDuration;
            this.animationComplete = animationComplete;

            oldChildContainer = this.childContainer;
            if (oldChildContainer != null)
            {
                oldSize = oldChildContainer.DesiredSize;
            }
            else
            {
                oldSize = new Size2(0.0f, 0.0f);
            }

            this.childContainer = childContainer;
            if (childContainer != null)
            {
                childContainer._setParent(this);
                newSize = childContainer.DesiredSize;
            }
            else
            {
                newSize = new Size2(0.0f, 0.0f);
            }

            sizeDelta = newSize - oldSize;
            animating = true;
        }

        public override Size2 DesiredSize
        {
            get 
            {
                return currentSize;
            }
        }

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        public void sendUpdate(Clock clock)
        {
            if (animating)
            {
                bool finishAnimatingThisFrame = false;
                currentTime += clock.fSeconds;
                if (currentTime > animationLength)
                {
                    currentTime = animationLength;
                    finishAnimatingThisFrame = true;

                    finishAnimation();
                    oldChildContainer = null;
                }
                alpha = currentTime / animationLength;
                if (childContainer != null && oldChildContainer != null)
                {
                    childContainer.setAlpha(alpha);
                }
                currentSize = new Size2(oldSize.Width + sizeDelta.Width * alpha, WorkingSize.Height);
                invalidate();
                if (finishAnimatingThisFrame)
                {
                    animating = false;
                }
            }
        }

        private void finishAnimation()
        {
            //reset the old child
            if (oldChildContainer != null)
            {
                oldChildContainer._setParent(null);
                oldChildContainer.setAlpha(1.0f);
                oldChildContainer.WorkingSize = oldSize;
                oldChildContainer.layout();
            }
            if (animationComplete != null)
            {
                animationComplete(oldChildContainer);
            }
        }

        public override void bringToFront()
        {
            if (childContainer != null)
            {
                childContainer.bringToFront();
            }
        }

        public override bool Visible
        {
            get
            {
                if (childContainer != null)
                {
                    return childContainer.Visible;
                }
                return false;
            }
            set
            {
                if (childContainer != null)
                {
                    childContainer.Visible = value;
                }
            }
        }
    }
}
