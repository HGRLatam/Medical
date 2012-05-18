﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    public abstract class CachedResource
    {
        public CachedResource(String file)
        {
            this.File = file;
        }

        public abstract Stream openStream();

        public String File { get; private set; }
    }
}
