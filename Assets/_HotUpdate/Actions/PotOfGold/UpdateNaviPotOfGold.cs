using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Meta/POG")]
public class UpdateNaviPotOfGold : ActionTask<Blackboard> 
{
    public BBParameter<Blackboard> productBB;

    public BBParameter<long>  saveAsRealPogCoins;
    public BBParameter<long>  saveAsTotalPogCoins;
    public BBParameter<int>   saveAsPogLevel;
    public BBParameter<float> saveAsProportion;
    public BBParameter<double> saveAsEventMultiplier;

    public BBParameter<bool> saveAsIsMaxLevel;
    public BBParameter<bool> saveAsIsChangedLevel;

    private bool isInit = false;
    private bool isPogEnabled = false;
    private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

    private int oldPogLevel = -1;

    private ContextElement sctionInfoTextElement;
    private ContextElement fullCoinTextElement;
    private Animator       pogAnimator;

    private const string POG_FULL_COIN_TEXT = "POT_OF_GOLD_COIN_TEXT";
    private const string POG_INFO_COIN_TEXT = "POT_OF_GOLD_INFORMATION";

    private const string ANI_STATE_POG_LEVEL      = "Level";
    private const string ANI_STATE_POG_FULL       = "IsFull";
    private const string ANI_STATE_POG_TRANSITION = "Transition";

    protected override void OnExecute()
    {
        saveAsIsMaxLevel.value      = false;
        saveAsIsChangedLevel.value  = false;
        saveAsTotalPogCoins.value   = 0L;
        saveAsPogLevel.value        = 0;
        saveAsProportion.value      = 0f;
        saveAsEventMultiplier.value = 1.0;

        InitProperty();
        UpdatePotOfGoldCoins();

        EndAction();
    }

    private void InitProperty()
    {
        if(isInit) return;

        ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();

        sctionInfoTextElement = ContextUtils.FindElement(agentElement, "Piggy Bank Information/Text", ContextSearchingType.FullNameSearch);
        fullCoinTextElement = ContextUtils.FindElement(agentElement, "Text Full Amount", ContextSearchingType.ChildrenSearch);

        pogAnimator = agent.gameObject.GetComponent<Animator>();

        isInit = true;
    }

    private void UpdatePotOfGoldCoins()
    {
        EventInfo multiplierEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.PIGGY_BANK_MULTIPLY);

        long pogEventMultiplierNumerator = NumberUtils.GetGlobalDenominator();
        if(multiplierEventInfo != null)
            pogEventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(multiplierEventInfo);
            
        long mePogCoins      = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/piggyCredit").value;
        int meTier           = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "me/tier").value;

        long totalCoins      = TierUtils.GetTierFractionCoin(mePogCoins, meTier);
        totalCoins           = LevelUtils.GetLevelMultiplierNumeratorValue(totalCoins, "pog");
        long eventTotalCoins = totalCoins;
        if(pogEventMultiplierNumerator > NumberUtils.GetGlobalDenominator())
            eventTotalCoins = NumberUtils.GetMultiplierNumeratorValue(eventTotalCoins, pogEventMultiplierNumerator);

        Blackboard pogInfoBB = BlackboardQueryUtils.GetItemFromProduct(productBB.value, ItemType.PIGGY_BANK);
        long minCoins        = pogInfoBB.GetValue<long>("minCredit");
        long maxCoins        = pogInfoBB.GetValue<long>("maxCredit");

        var sectionList      = BlackboardUtils.FindVariable<List<double>>(MainBlackboard.Get(), "values/misc/PIGGY_BANK_INFO_APPEAR_SECTION_LIST").value;

        MetaContextElementUtils.SetText(sctionInfoTextElement, StringTableUtils.GetString(tableType, POG_INFO_COIN_TEXT, eventTotalCoins));
        MetaContextElementUtils.SetText(fullCoinTextElement, StringTableUtils.GetString(tableType, POG_FULL_COIN_TEXT, eventTotalCoins));

        // Debug.LogError(fullCoinTextElement.transform.localScale);

        float proportion = (float)(mePogCoins - minCoins) / (float)(maxCoins - minCoins);
        proportion = Mathf.Clamp(proportion, 0f, 1f);

        int pogLevel = 0;
        for(int i=0; i<sectionList.Count; ++i)
        {
            if((float)sectionList[i] <= proportion)
                pogLevel = i + 1;
            else
                break;
        }

        saveAsIsMaxLevel.value = sectionList.Count == pogLevel;

        // if(oldPogLevel != -1 && oldPogLevel < pogLevel)
        if(oldPogLevel != pogLevel)
            saveAsIsChangedLevel.value = true;

        oldPogLevel = pogLevel;

        pogAnimator.SetBool(ANI_STATE_POG_FULL, saveAsIsMaxLevel.value);
        pogAnimator.SetInteger(ANI_STATE_POG_LEVEL, pogLevel);

        if(saveAsIsChangedLevel.value)
        {
            pogAnimator.SetTrigger(ANI_STATE_POG_TRANSITION);
        }

        saveAsRealPogCoins.value    = totalCoins;
        saveAsTotalPogCoins.value   = eventTotalCoins;
        saveAsPogLevel.value        = pogLevel;
        saveAsProportion.value      = proportion;
        saveAsEventMultiplier.value = NumberUtils.GetMultiplierFromNumerator(pogEventMultiplierNumerator);
    }
}

}
