using System;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using UnityEngine;

namespace SlotMaker.TestSuite
{

public class TestSuiteDebugSpinRunner : MonoBehaviour
{
#if DEV && !NEW_NET
    public List<DebugSequence> debugSequenceList { get; set; }

    private ContentTestInfo testInfo;
    private int spinIndex;
    private int spinCount;
    private bool debugSpin;
    private string location;

    public bool stopped { get; set; }

    private const int SPIN_BUTTON_INTERVAL = 1;
    private const int EVENT_SOLVING_INTERVAL = 2;

    private MessageDelegates delegates;

    private const string NORMAL_SPIN = "spin";
    private const string FREE_SPIN_TRIGGER = "free_spin_trigger";
    private const string FREE_SPIN = "free_spin";

    private void Awake()
    {
        delegates = new MessageDelegates
        (
            new Dictionary<string, MessageDispatcher.EventDelegate>
            {
                { "EndGame",    EndGame     },
                { "EndSpin",    EndSpin     },
                { "EndTurn",    EndTurn     },
                { "BeginBonus", BeginBonus  }
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

    public void Run(List<DebugSequence> DebugSequenceList)
    {
        debugSequenceList = DebugSequenceList;

        spinCount = debugSequenceList.Count;
        TestSuiteManager.Instance.DebugParam = debugSequenceList[0].debugParam;

        ShowLog();
    }

    public void Stop()
    {
        // autoSpin.value = false;

        if (stopped)
            return;

        stopped = true;
        GameObject.Destroy(gameObject);
    }

    private void EndGame(EventData eventData)
    {
        Stop();
    }

    private void EndSpin(EventData eventData)
    {
        if (spinIndex + 1 >= spinCount)
            Stop();
        else if (string.Equals(debugSequenceList[spinIndex].type, FREE_SPIN))
            TestSuiteManager.Instance.DebugParam = debugSequenceList[++spinIndex].debugParam;

        if (spinIndex < spinCount)
            ShowLog();
    }

    private void BeginBonus(EventData eventData)
    {
        if (spinIndex + 1 >= spinCount)
            Stop();
        else if (string.Equals(debugSequenceList[spinIndex].type, FREE_SPIN_TRIGGER))
            TestSuiteManager.Instance.DebugParam = debugSequenceList[++spinIndex].debugParam;

        if (spinIndex < spinCount)
            ShowLog();
    }

    private void EndTurn(EventData eventData)
    {
        if (spinIndex + 1 >= spinCount)
            Stop();
        else if (string.Equals(debugSequenceList[spinIndex].type, NORMAL_SPIN))
            TestSuiteManager.Instance.DebugParam = debugSequenceList[++spinIndex].debugParam;

        if (spinIndex < spinCount)
            ShowLog();
    }

    private IEnumerator SpinButtonEventTrigger()
    {
        while (true)
        {
            yield return new WaitForSeconds(SPIN_BUTTON_INTERVAL);

            MessageDispatcher.Dispatch("OnSpinButtonEvent", new EventData("OnSpinButtonEvent"));
        }
    }

    private IEnumerator EventSolver()
    {
        while (true)
        {
            yield return new WaitForSeconds(EVENT_SOLVING_INTERVAL);

#if NEW_NET
                yield return null;
#else
            if (TestSuiteEventSolver.HasSolver("Meta"))
                TestSuiteEventSolver.Solve("Meta");
            else if (TestSuiteEventSolver.HasSolver("Contents"))
                TestSuiteEventSolver.Solve("Contents");
#endif
            }
        }

    private void ShowLog()
    {
#if NEW_NET
            return;
#else
            TestSuiteManager.Instance.tsDescription.text = string.Format("{0}/{1}", (spinIndex + 1), spinCount);
#endif

    }
#endif
    }

}
