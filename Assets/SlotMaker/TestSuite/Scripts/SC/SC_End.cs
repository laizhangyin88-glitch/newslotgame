using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.TestSuite
{
    public class SC_End : TestSuiteRunner 
    {
#if DEV && !NEW_NET
        public override void Run(TestCaseRunner testCaseRunner, SlotMaker.TestSuite.TestSuite testSuite)
        {
            base.Run(testCaseRunner, testSuite);

            var parameter = new Dictionary<string, object>();
            parameter["reportId"] = ((ContentTestInfo)testCaseRunner.testCase.customData["testInfo"]).reportId;
            TestSuiteManager.Instance.ContentEnd(parameter, Complete);    
        }

        private void Complete()
        {
            testCaseRunner.Complete(testSuite);
            Stop();
        }
#endif
    }
}
