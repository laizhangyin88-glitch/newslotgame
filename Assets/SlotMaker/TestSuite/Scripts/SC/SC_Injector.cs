using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace SlotMaker.TestSuite
{
    public class SC_Injector : TestSuiteRunner
    {
#if DEV
        private MessageDelegates delegates;

        private void Awake()
        {
            delegates = new MessageDelegates
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                    { "ReadyGame", ReadyGame }
                }
            );
        }

        private void OnEnable()
        {
            MessageDispatcher.Register("OnContentEvent", delegates.Delegate);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister("OnContentEvent", delegates.Delegate);
        }

        public override void Run(TestCaseRunner testCaseRunner, SlotMaker.TestSuite.TestSuite testSuite)
        {
            base.Run(testCaseRunner, testSuite);

            var contentInfo = (ContentInfo)testCase.customData["contentInfo"];

            MetaSystem.SelectGame(contentInfo.gameId);
            MetaSystem.EnterGame();
        }

        private void ReadyGame(EventData eventData)
        {
            testCaseRunner.Complete(testSuite);
            Stop();
        }
#endif
    }
}
