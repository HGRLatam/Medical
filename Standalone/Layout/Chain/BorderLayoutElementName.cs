﻿using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class BorderLayoutElementName : LayoutElementName
    {
        public BorderLayoutElementName(String name, BorderLayoutLocations viewLocation)
            :base(name)
        {
            this.viewLocation = viewLocation;
        }

        private BorderLayoutLocations viewLocation;
        public BorderLayoutLocations ViewLocation
        {
            get
            {
                return viewLocation;
            }
        }

        protected override string UniqueDerivedName
        {
            get
            {
                return Name + ViewLocation;
            }
        }
    }
}
