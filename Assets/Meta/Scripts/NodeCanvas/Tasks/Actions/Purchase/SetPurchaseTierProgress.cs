using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Purchase")]
public class SetPurchaseTierProgress : ActionTask<Blackboard>
{
    public BBParameter<Blackboard>  userSyncInfoBB;
    public BBParameter<Blackboard>  useItemBB;

    public BBParameter<int>         currentTier;
    public BBParameter<int>         prevTier;
    public BBParameter<int>         nextTier;

    public BBParameter<long>        currentAccRP;
    public BBParameter<long>        prevAccRP;
    public BBParameter<long>        nextNeedRP;

    // public BBParameter<string>      mainText;
    public BBParameter<string>      gainText;
    public BBParameter<string>      progressText;

    public BBParameter<float>       progressDelta;

    public BBParameter<bool>        isMaxTier;

    protected override string info
    {
        get { return "Set Purchase Tier Progress"; }
    }

    protected override void OnExecute()
    {
        if(userSyncInfoBB.value != null && useItemBB.value != null)
        {
            prevAccRP.value = BlackboardUtils.FindVariable<long>(null, "/me/accRp").value;
            prevTier.value = BlackboardUtils.FindVariable<int>(null, "/me/tier").value;

            UpdateGainText();
            UpdateProgress();
        }

        EndAction();
    }

    private void UpdateGainText()
    {
        bool error = false;

        var earnRP = BlackboardUtils.FindVariable<long>(useItemBB.value, "earnRp");
        if(earnRP != null && earnRP.value > 0)
        {
            gainText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_TIER_PROGRESS_GAINED_TEXT_RP", earnRP.value, out error);
        }
        else
        {
            gainText.value = "";
        }
    }

    private void UpdateProgress()
    {
        currentAccRP.value = BlackboardUtils.FindVariable<long>(userSyncInfoBB.value, "accRp").value;
        currentTier.value = TierUtils.GetTier(currentAccRP.value);

        isMaxTier.value = TierUtils.IsMaxTier(currentTier.value);
        if(!isMaxTier.value)
        {
            bool error = false;

            nextTier.value = currentTier.value + 1;
            nextNeedRP.value = TierUtils.GetTotalRP(nextTier.value);

            progressText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_TIER_PROGRESS_BAR_TEXT", currentAccRP.value, nextNeedRP.value, out error);

            // long requireRP = TierUtils.GetRequireRP(currentTier.value);
            // long currentRP = currentAccRP.value - currentNeedRP;

            progressDelta.value = (float)((double)currentAccRP.value/(double)nextNeedRP.value);
        }
        else
        {
            nextTier.value = currentTier.value;

            progressText.value = "";
            nextNeedRP.value = 0L;
            progressDelta.value = 1.0f;
        }
    }
}

}
