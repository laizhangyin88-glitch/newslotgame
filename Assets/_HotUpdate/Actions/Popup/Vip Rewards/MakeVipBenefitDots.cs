using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Popup/Vip Rewards")]
public class MakeVipBenefitDots : ActionTask<Blackboard> 
{
    public BBParameter<int> meTier;

    public BBParameter<int> saveAsCurrentPageIndex;
    public BBParameter<int> saveAsMaxPageCount;
    public BBParameter<List<int>> saveAsPageTierList;

    public string dotAssetName = "VIP Rewards Dots";

    private bool isUseTierEven = false;

    private int maxTier = 0;
    private bool isMaxTierEven = false;

    protected override void OnExecute()
    {
        if(saveAsPageTierList.value == null || saveAsPageTierList.value.Count == 0)
        {
            isUseTierEven = (meTier.value%2) == 0;

            maxTier = TierUtils.GetMaxTier();
            isMaxTierEven = (maxTier%2) == 0;

            bool isFirstEven = IsFirstEven(meTier.value);

            MakeDots(isFirstEven);

            saveAsMaxPageCount.value = saveAsPageTierList.value.Count;
        }

        EndAction();
    }

    private bool IsFirstEven(int userTier)
    {
        if(userTier == 0) return true;

        if(userTier == maxTier)
        {
            if(!isMaxTierEven) return false;
        }
        else
        {
            if(!isUseTierEven) return false;
        }

        return true;
    }

    private void MakeDots(bool isFirstEven)
    {
        ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();
        ContextElement dotAreaElement = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Dots Area", ContextSearchingType.FullNameSearch);

        saveAsPageTierList.value = new List<int>();

        Transform dotParent = dotAreaElement.transform;

        int page = 0;
        for(int i=0; i<=maxTier;)
        {
            saveAsPageTierList.value.Add(i);
            var go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, dotAssetName, dotParent, "");
            go.name = string.Format("Dot {0}", i);

            if(meTier.value == i)
            {
                saveAsCurrentPageIndex.value = page;
            }

            if( i==0 && !isFirstEven)
                ++i;
            else
                i+=2;

            ++page;
        }

        dotAreaElement.UpdateContext(true);
    }
}

}
