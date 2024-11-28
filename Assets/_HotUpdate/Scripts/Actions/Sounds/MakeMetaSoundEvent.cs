using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Action
{

[Category("★ BagelCode/Meta/Sounds")]
public class MakeMetaSoundEvent : ActionTask<Blackboard>
{
    public BBParameter<string> bundleName;
    public BBParameter<string> assetName;
    public BBParameter<bool> combineApplicationType;

    private const string ON_META_UI_EVENT = "OnMetaUIEvent";

    protected override string info
    {
        get
        {
            return string.Format("Send Global Meta Event (Make Sound {0}/{1}", bundleName, assetName );
        }
    }

    protected override void OnExecute()
    {
        EventData eventData = new EventData<KeyValuePair<string, string>>("OnMakeSound", new KeyValuePair<string, string>(GetBundleName(), assetName.value));
        MessageDispatcher.Dispatch(ON_META_UI_EVENT, eventData);

        EndAction();
    }

    protected string GetBundleName()
    {
        return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
    }
}

}
