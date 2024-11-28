using System;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/StatusMatch")]

public class GetTierByOtherProgram : ActionTask<Blackboard>
{
    public BBParameter<int> vipProgramIndex;

    public BBParameter<int> vipStatusIndex;
    public BBParameter<int> calculatedTier;

    protected override string info
    {
        get { return "Get Tier by Other Program Status"; }
    }

    protected override void OnExecute()
    {
        int[] tierMap = {};
        // setup applist & status list
        // sorted index
        // unkwon = 0, playtika = 1, loyalty = 2, huge = 3, ss = 4, jj =5, other =6
        // ... incremental
        // it also use in DynamicScrollStatusMatchDetailListCreator.cs
        switch (vipProgramIndex.value)
        {
            case (int)StatusMatchVipProgram.PLAYTIKA_REWARDS:
                tierMap = new[] {8, 15, 19, 20};
            break;
            case (int)StatusMatchVipProgram.LOYALTY_LOUNGE:
                tierMap = new[] {6, 10, 14, 17, 20, 20};
            break;
            case (int)StatusMatchVipProgram.HUUGE_CASINO:
                tierMap = new[] {6, 6, 6, 12, 12, 12, 15, 15, 15, 18};
            break;
            case (int)StatusMatchVipProgram.STAR_SPINS_VIP:
                tierMap = new[] {6, 10, 13, 15, 17, 19, 20};
            break;
            case (int)StatusMatchVipProgram.JACKPOTJOY_REWARDS:
                tierMap = new[] {6, 10, 13, 15, 17, 19, 20};
            break;
            case (int)StatusMatchVipProgram.OTHER: // default case. because of UI sorting
                tierMap = new[] {-1};
            break;
            default: // OTHER case. because of UI sorting
                tierMap = new[] {-1};
            break;
        }

        calculatedTier.value = tierMap[vipStatusIndex.value];

        EndAction();
    }


}

}
