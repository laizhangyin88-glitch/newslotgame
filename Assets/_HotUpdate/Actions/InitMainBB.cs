using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Main")]
public class InitMainBB : ActionTask
{
    protected override void OnExecute()
    {
        var bb = MainBlackboard.Get();

        bb.AddVariable("isPossibleUpdateNavi", true);

        bb.AddVariable("inGame", false);
        bb.AddVariable("isEarlyAccess", false);
        bb.AddVariable("isSimpleMenuButtons", false);
        bb.AddVariable("sessionAlive", false);

        // Sassion Varaible
        bb.AddVariable("facebookLikeEnabled", true);
        bb.AddVariable("rateUsEnabled", true);
        bb.AddVariable("chatType", BagelCode.ChattingType.Normal);

        bb.AddVariable("isSpin", false);

        bb.AddVariable("currentSceneState", SceneState.LOGIN);
        bb.AddVariable("prevSceneState", SceneState.LOGIN);

#if UNITY_WSA
        bb.AddVariable("isWindowsPinned", true);
#endif

        EndAction();
    }
}

}
