using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Utils")]
public class SetOrientation: ActionTask<Transform>
{
    public BBParameter<ClientModels.Orientation> orientation;
    public BBParameter<ClientModels.Orientation> prevOrientation;
    public BBParameter<bool> manualFadeOut;

    public BBParameter<GameObject> saveAsFadeInOutObj;
    public BBParameter<bool> saveAsEnableFadeInOut;

    protected override string info
    {
        get
        {
            return string.Format("Set Orientation {0}", orientation);
        }
    }

    protected override void OnExecute()
    {
        saveAsEnableFadeInOut.value = false;

        var currentOrientation = BlackboardUtils.FindVariable<ClientModels.Orientation>(MainBlackboard.Get(), "currentOrientation");
        if(currentOrientation == null)
        {
            currentOrientation = BlackboardUtils.GetOrCreateVariable<ClientModels.Orientation>(MainBlackboard.Get(), "currentOrientation");
            currentOrientation.value = ClientModels.Orientation.LANDSCAPE;//orientation.value;
        }

        prevOrientation.value = currentOrientation.value;

        if(currentOrientation.value != orientation.value)
        {
            currentOrientation.value = orientation.value;

            var fadeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Change Orientation Fade In Out", null, "Popup Manager", "Fade In Out");
            saveAsFadeInOutObj.value = fadeObj;

            var bb = fadeObj.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue<GameObject>(bb, "caller", agent.gameObject);
            BlackboardUtils.SetOrCreateValue<ScreenOrientation>(bb, "targetOrientation", ScreenOrientation.AutoRotation);
            BlackboardUtils.SetOrCreateValue<bool>(bb, "autorotateToPortrait",           orientation.value == ClientModels.Orientation.PORTRAIT);
            BlackboardUtils.SetOrCreateValue<bool>(bb, "autorotateToPortraitUpsideDown", orientation.value == ClientModels.Orientation.PORTRAIT);
            BlackboardUtils.SetOrCreateValue<bool>(bb, "autorotateToLandscapeRight",     orientation.value != ClientModels.Orientation.PORTRAIT);
            BlackboardUtils.SetOrCreateValue<bool>(bb, "autorotateToLandscapeLeft",      orientation.value != ClientModels.Orientation.PORTRAIT);

            // manual fade out time.
            BlackboardUtils.SetOrCreateValue<bool>(bb, "isAutoFadeOut", manualFadeOut == null || !manualFadeOut.value);

            saveAsEnableFadeInOut.value = true;
        }

        EndAction();
    }
}

}
