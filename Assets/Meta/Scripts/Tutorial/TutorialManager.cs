using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.BehaviourTrees;
using BagelCode.ClientModels;
using SlotMaker;
using SlotMaker.Json;

namespace BagelCode
{

public class TutorialManager : MonoWeakSingleton<TutorialManager>
{
    public MirrorableRectTransform viewport;

    private BehaviourTreeOwner _owner;
    public BehaviourTreeOwner owner { get { return _owner ?? (_owner = GetComponent<BehaviourTreeOwner>()); } }

    private Blackboard _blackboard;
    public Blackboard blackboard { get { return _blackboard ?? (_blackboard = GetComponent<Blackboard>()); } }

    public RectTransform targetRect { get { return viewport.targetRect; } set { viewport.targetRect = value; } }

    public static bool completeAll = true;

    public bool Initialize()
    {
        completeAll = true;

        var tutorials = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/tutorial").value;
        foreach (var tutorial in tutorials)
        {
            string key = tutorial.GetValue<string>("key");
            var bb = BlackboardUtils.FindVariable<Blackboard>(blackboard, "tutorials/" + key).value;
            bb.SetValue("tutorialBB", tutorial);

            if (!IsFinishedTutorial(tutorial))
            {
                string category = bb.GetValue<string>("category");
                blackboard.GetValue<List<Blackboard>>(category + "_tutorials").Add(bb);
                completeAll = false;
            }
        }

        return completeAll;
    }

    public static void TriggerEvent(string key, RectTransform rectTransform)
    {
        if (completeAll || Instance == null)
            return;

        var variable = BlackboardUtils.FindVariable<RectTransform>(Instance.blackboard,
            string.Format("tutorials/{0}/rectTransform", key));
        variable.value = rectTransform;
    }

    public bool IsFinishedTutorial(Blackboard bb)
    {
        return false;
        // return (bb.GetValue<TutorialStatusType>("status") != TutorialStatusType.UNCOMPLETED) || !bb.GetValue<bool>("enabled");
    }
}

}
