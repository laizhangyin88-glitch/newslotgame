using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class UserTutorialSkip : ActionTask
{
    public BBParameter<List<Blackboard>> tutorialList;

    protected override void OnExecute()
    {
        // var keyList = new List<string>();
        // foreach (var tutorial in tutorialList.value)
        // {
        //     var tutorialBB = tutorial.GetValue<Blackboard>("tutorialBB");
        //     if (!TutorialManager.Instance.IsFinishedTutorial(tutorialBB))
        //     {
        //         keyList.Add(tutorialBB.GetValue<string>("key"));
        //         tutorialBB.SetValue("status", ClientModels.TutorialStatusType.SKIPPED);
        //     }
        // }

        // BagelCodeClientAPI.UserTutorialSkip(keyList,
        // (response) =>
        // {
        //     EndAction(true);
        // },
        // (error) =>
        // {
        //     GlobalErrorHandler.GlobalError(error);
        // });
    }
}

}
