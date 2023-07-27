using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.TestSuite
{
    public class TestSuiteRunner : MonoBehaviour
    {
    #if DEV
        public TestCaseRunner testCaseRunner { get; set; }
        public TestCase testCase { get; set; }
        public TestSuite testSuite { get; set; }

        public bool stopped { get; set; }

        public virtual void Run(TestCaseRunner testCaseRunner, TestSuite testSuite)
        {
            this.testCaseRunner = testCaseRunner;
            this.testCase = testCaseRunner.testCase;
            this.testSuite = testSuite;
        }

        public virtual void Stop()
        {
            if (stopped)
                return;

            stopped = true;
            GameObject.Destroy(gameObject);
        }
    #endif
    }
}
