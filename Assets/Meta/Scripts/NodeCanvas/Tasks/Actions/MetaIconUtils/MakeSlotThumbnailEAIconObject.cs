using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Utils")]

public class MakeSlotThumbnailEAIconObject : ActionTask<Blackboard>
{
    public BBParameter<string> gameTitleValue;
    public BBParameter<Transform> parent;
    public BBParameter<string> parentName;

    public BBParameter<GameObject> saveAs;

    public BBParameter<bool> isDestroyPrevObject;

    protected override string info
    {
        get
        {
            return string.Format("{0} = Load Slot Thumbnail EA Icon({1})", saveAs, gameTitleValue);
        }
    }

    protected override void OnExecute()
    {
        if(saveAs.value != null && isDestroyPrevObject.value)
            GameObject.Destroy(saveAs.value);

        var gameTitle = BlackboardUtils.FindVariable<string>(agent, gameTitleValue.value);

        //SLOT_THUMBNAIL_SQUARE

        if(gameTitle != null)
        {
            if(parent.value == null)
                parent.value = agent.transform;

            saveAs.value = MetaIconUtils.MakeSlotThumbnailEAIconObjectFromGameTitle(gameTitle.value, parent.value, parentName.value);
        }
        else
        {
            saveAs.value = null;
            Debug.Log("GameTitle is Null");
        }
        

        EndAction();
    }
}

}
