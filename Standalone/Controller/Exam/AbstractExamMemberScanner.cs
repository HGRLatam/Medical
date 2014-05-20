﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Reflection;

namespace Medical
{
    class AbstractExamMemberScanner : MemberScannerFilter
    {
        static FilteredMemberScanner memberScanner;

        static AbstractExamMemberScanner()
        {
            memberScanner = new FilteredMemberScanner(new AbstractExamMemberScanner());
            memberScanner.ProcessFields = false;
            memberScanner.ProcessNonPublicFields = false;
            memberScanner.ProcessNonPublicProperties = false;
        }

        public static MemberScanner MemberScanner
        {
            get
            {
                return memberScanner;
            }
        }

        private AbstractExamMemberScanner()
        {

        }

        public bool allowMember(MemberWrapper wrapper)
        {
            return !wrapper.getCustomAttributes(typeof(HiddenAttribute), true).Any();
        }

        public bool allowType(Type type)
        {
            return true;
        }
    }
}
