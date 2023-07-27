using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Popup/Vip Rewards")]
public class UpdateVipTierPage : ActionTask<Blackboard> 
{
    public BBParameter<int> meTier;

    private int nextTier = -1;
    private int maxTier = -1;

    private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

    private bool isInit = false;

    private ContextElement leftTierIconElement;
    private ContextElement leftTierTextElement;

    private ContextElement rightTierIconElement;
    private ContextElement rightTierTextElement;

    private ContextElement progressTextElement;
    private ContextElement progressBarElement;
    private ContextElement progressNextTextElement;

    protected override void OnExecute()
    {
        if(!isInit)
            InitProperty();

        UpdateTier();
        UpdateProgress();
        
        EndAction();
    }

    private void InitProperty()
    {
        ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();

        leftTierIconElement         = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Current Tier Area/Icon Current Tier", ContextSearchingType.FullNameSearch);
        leftTierTextElement         = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Current Tier Area/Text", ContextSearchingType.FullNameSearch);

        rightTierIconElement        = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Next Tier Area/Icon Next Tier", ContextSearchingType.FullNameSearch);
        rightTierTextElement        = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Next Tier Area/Text", ContextSearchingType.FullNameSearch);

        progressTextElement     = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Progress Bar Area/Anchor/Text", ContextSearchingType.FullNameSearch);
        progressBarElement      = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Progress Bar Area/Anchor/Progress Bar", ContextSearchingType.FullNameSearch);
        progressNextTextElement = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Progress Bar Area/Anchor/Text Next", ContextSearchingType.FullNameSearch);

        maxTier = TierUtils.GetMaxTier();

        isInit = true;
    }

    private void UpdateTier()
    {
        int currentTierGroup = TierUtils.GetTierGroup(meTier.value);
        
        MetaContextElementUtils.SetBlackboardValue<int>(leftTierIconElement, "tierGroup", currentTierGroup);
        MetaContextElementUtils.SetIntProperty(leftTierIconElement, currentTierGroup);
        MetaContextElementUtils.SetText(leftTierTextElement, StringTableUtils.GetString(tableType, "TEXT_TIER_STYLE", meTier.value));

        if(meTier.value < maxTier)
        {
            nextTier = meTier.value + 1;
            int nextTierGroup = TierUtils.GetTierGroup(nextTier);
            MetaContextElementUtils.SetBlackboardValue<int>(rightTierIconElement, "tierGroup", nextTierGroup);
            MetaContextElementUtils.SetIntProperty(rightTierIconElement, nextTierGroup);
            MetaContextElementUtils.SetText(rightTierTextElement, StringTableUtils.GetString(tableType, "TEXT_TIER_STYLE", nextTier));
        }

        // Max Tier is Inactive. 
        rightTierIconElement.gameObject.SetActive(meTier.value < maxTier);
        rightTierTextElement.gameObject.SetActive(meTier.value < maxTier);
    }

    private void UpdateProgress()
    {
        var accRP = BlackboardUtils.FindVariable<long>(null, "/me/accRp").value;
        long totalRP = meTier.value == maxTier ? 0 : TierUtils.GetTotalRP(meTier.value + 1);
        float progress = meTier.value == maxTier ? 1f : Mathf.Clamp((float)((double)accRP/(double)totalRP), 0f, 1f);

        MetaContextElementUtils.SetFloatProperty(progressBarElement, progress);
        progressNextTextElement.gameObject.SetActive(meTier.value < maxTier);

        if(meTier.value == maxTier)
            MetaContextElementUtils.SetText(progressTextElement, StringTableUtils.GetString(tableType, "VIP_REWARDS_PROGRESS_TEXT_MAX", accRP));
        else
            MetaContextElementUtils.SetText(progressTextElement, StringTableUtils.GetString(tableType, "VIP_REWARDS_PROGRESS_TEXT", accRP, totalRP));
        
    }
}

}
