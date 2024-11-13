using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
[Category("★ BagelCode/JackpotChase")]
public class CreateDummyMetaJackpotInfo : ActionTask
{
    public BBParameter<MetaJackpotType> metaJackpotType;

    protected override string info
    {
        get
        {
            return string.Format("Crate Dummy {0} Meta jackpot info", metaJackpotType);
        }
    }

    protected override void OnExecute()
    {
        BlackboardQueryUtils.CreateDummyJackpotInfo(metaJackpotType.value);
        EndAction();
    }
}

}
