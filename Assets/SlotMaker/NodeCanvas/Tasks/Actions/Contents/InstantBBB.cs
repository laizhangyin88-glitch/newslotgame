using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
[Category("★ BagelCode/Contents")]
public class InstantBBB : ActionTask<Blackboard>
{
    public BBParameter<string> bbbCredit = "./ticketClaim/insBbbCredit";

	protected AssetBundleLoadAssetOperation loadSceneInfoOperation = null;
	protected SceneLoadOperation sceneLoadOperation = null;
    protected bool isWaiting = false;


    protected override string info
    {
        get { return string.Format("Add Instant bonus BBB credit {0}", bbbCredit); }
    }

    protected override void OnExecute()
    {
        var bonusCredit = BlackboardUtils.FindVariable<long>(agent, bbbCredit.value);
        // var bonus = BlackboardUtils.FindVariable<Blackboard>(null, "./bonus").value;

        if (bonusCredit != null && bonusCredit.value > 0L)
        {
            loadSceneInfoOperation = null;
		    sceneLoadOperation 	   = null;
            isWaiting              = false;
        }
        else
        {
            EndAction();
        }        
    }
	protected override void OnUpdate()
	{
        if (isWaiting) return;
        
		if (loadSceneInfoOperation == null)
			loadSceneInfoOperation = AssetBundleManager.LoadAssetAsync<SceneInfoObject>(GetBundleName(), "Popup INS BBB Scene");

		if (loadSceneInfoOperation.IsDone())
		{
			if (sceneLoadOperation == null)
			{
				Transform root = GameObject.Find("Popup Manager").transform;
				var sceneInfo = loadSceneInfoOperation.GetAsset<SceneInfoObject>().GetSceneInfo();

				sceneLoadOperation = SceneManager.LoadSceneAsync(root, sceneInfo, true);
			}

			if (sceneLoadOperation.IsDone())
			{
                GameObject popup = sceneLoadOperation.GetScene();
                PopupManager.Instance.Open(popup);

                IBlackboard popupBB = popup.GetComponent<NodeCanvas.Framework.IBlackboard>();
                var name =  BlackboardUtils.GetOrCreateVariable<string>(popupBB, "userName");
                name.value = BlackboardUtils.FindVariable<string>(null, "/me/name").value;

                var earnCredit = BlackboardUtils.GetOrCreateVariable<long>(popupBB, "insBbbCredit");
                earnCredit.value = BlackboardUtils.FindVariable<long>(agent, bbbCredit.value).value;

                var action = BlackboardUtils.FindVariable<System.Action>(popupBB, "endAction");
                action.value = EndAction;

                popup.SetActive(true);
                
                isWaiting = true;
			}
		}
	}

	protected string GetBundleName()
	{
		return ApplicationSettings.MakeApplicationBundleName("lobby");
	}
}

}
