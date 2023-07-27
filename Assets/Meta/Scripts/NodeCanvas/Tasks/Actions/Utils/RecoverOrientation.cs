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
public class RecoverOrientation: ActionTask<Transform>
{
    protected override void OnExecute()
    {
        OrientationUtils.Instance.contentOrientation = ScreenOrientation.LandscapeLeft;

#if UNITY_ANDROID || UNITY_IOS
    #if UNITY_IOS
        if( OrientationUtils.Instance.PossibleChangeOrientation() )
    #endif
        {
            if(    Screen.orientation == ScreenOrientation.Portrait
                || Screen.orientation == ScreenOrientation.PortraitUpsideDown
                || Screen.autorotateToPortrait
                || Screen.autorotateToPortraitUpsideDown
            )
            {
                Screen.autorotateToPortrait             = false;
                Screen.autorotateToPortraitUpsideDown   = false;
                Screen.autorotateToLandscapeRight       = true;
                Screen.autorotateToLandscapeLeft        = true;

        #if UNITY_ANDROID
                Screen.orientation = ScreenOrientation.LandscapeLeft;
        #endif
                Screen.orientation = ScreenOrientation.AutoRotation;

                MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData<ScreenOrientation>("OnChangeOrientation", Screen.orientation));
            }
        }
#endif

        EndAction();
    }
}

}
