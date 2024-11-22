using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
[Category("★ BagelCode/JackpotChase")]
public class GetMetaJackpotInfo : ActionTask
{
    public MetaJackpotType metaJackpotType;

    public BBParameter<Blackboard> saveAs;

    protected override string info
    {
        get
        {
            return string.Format("{0} = Get Meta Jackpot Info {1}", saveAs, metaJackpotType);
        }
    }

    protected override void OnExecute()
    {
        saveAs.value = BlackboardQueryUtils.GetMetaJackpotInfo(metaJackpotType);
        EndAction();
    }
}

}
