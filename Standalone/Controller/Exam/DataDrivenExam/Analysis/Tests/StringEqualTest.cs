﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.Exam
{
    class StringEqualTest : TestAction
    {
        public StringEqualTest(AnalysisAction successAction, AnalysisAction failureAction)
            :base(successAction, failureAction)
        {

        }

        protected override bool performTest(DataDrivenExam exam)
        {
            return Data.getData<String>(exam, DefaultDataValue) == TestValue;
        }

        public String TestValue { get; set; }

        public DataRetriever Data { get; set; }

        public String DefaultDataValue { get; set; }

        protected StringEqualTest(LoadInfo info)
            :base(info)
        {
            TestValue = info.GetString("TestValue");
            Data = info.GetValue<DataRetriever>("Data");
            DefaultDataValue = info.GetString("DefaultDataValue");
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("TestValue", TestValue);
            info.AddValue("Data", Data);
            info.AddValue("DefaultDataValue", DefaultDataValue);
        }
    }
}
