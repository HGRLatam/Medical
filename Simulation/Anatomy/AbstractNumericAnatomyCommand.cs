﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Attributes;

namespace Medical
{
    public abstract class AbstractNumericAnatomyCommand : AbstractAnatomyCommand
    {
        [DoNotCopy]
        public override bool BooleanValue
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override void execute()
        {
            throw new NotSupportedException();
        }
    }
}
