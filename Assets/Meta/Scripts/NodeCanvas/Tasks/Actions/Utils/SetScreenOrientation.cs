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
public class SetScreenOrientation: ActionTask<Transform>
{
    public BBParameter<ScreenOrientation> orientation;
    public BBParameter<bool> autorotateToPortrait;
    public BBParameter<bool> autorotateToPortraitUpsideDown;
    public BBParameter<bool> autorotateToLandscapeLeft;
    public BBParameter<bool> autorotateToLandscapeRight;

    protected override void OnExecute()
    {
        if(autorotateToLandscapeLeft.value || autorotateToLandscapeRight.value)
        {
            OrientationUtils.Instance.contentOrientation = ScreenOrientation.LandscapeLeft;
        }
        else
        {
            OrientationUtils.Instance.contentOrientation = ScreenOrientation.Portrait;
        }

        if(OrientationUtils.Instance.PossibleChangeOrientation())
        {
            Screen.autorotateToPortrait             = autorotateToPortrait.value;
            Screen.autorotateToPortraitUpsideDown   = autorotateToPortraitUpsideDown.value;
            Screen.autorotateToLandscapeRight       = autorotateToLandscapeRight.value;
            Screen.autorotateToLandscapeLeft        = autorotateToLandscapeLeft.value;

            Screen.orientation = orientation.value;
        }

        MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData<ScreenOrientation>("OnChangeOrientation", orientation.value));

        EndAction();
    }
}

}
