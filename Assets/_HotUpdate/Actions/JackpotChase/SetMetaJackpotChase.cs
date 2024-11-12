using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/JackpotChase")]
    public class SetMetaJackpotChase : ActionTask<ContextElement>
    {
        public BBParameter<Blackboard> jackpotInfo;
        
        public BBParameter<string> elementName;
        public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;
        
        public BBParameter<string> key;
        public StringTable.StringTableType tableType;

        public BBParameter<bool> isNonStop;

        public BBParameter<EventInfoType> eventType;

        public BBParameter<long> saveAsJackpotMax;

        protected override void OnExecute()
        {
            BlackboardQueryUtils.InitJackpotInfo(jackpotInfo.value);

            saveAsJackpotMax.value = jackpotInfo.value.GetValue<long>("max");

            long multiplierNumerator = NumberUtils.GetGlobalDenominator();
            var eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(eventType.value);

            if(eventInfo != null)
            {
                multiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
                saveAsJackpotMax.value = NumberUtils.GetMultiplierNumeratorValue(saveAsJackpotMax.value, multiplierNumerator);
            }

            // Debug.LogError(string.Format("{0}/{1} : {2}", jackpotInfo.value.GetVariable<long>("progress").value, jackpotInfo.value.GetVariable<long>("current").value, jackpotInfo.value.GetVariable<long>("prev").value));

            var element = ContextUtils.FindElement(agent, elementName.value, searchingType);
            var jackpotChase = element.GetComponent<ChaseTypeLong>();
            jackpotChase.SetNonstopChase
            (
                jackpotChase.textElement,
                jackpotInfo.value.GetValue<long>("prev"),
                jackpotInfo.value.GetValue<long>("current"),
                jackpotInfo.value.GetValue<int>("deltaMs"),
                key.value,
                tableType,
                isNonStop.value,
                multiplierNumerator,
                jackpotInfo.value.GetVariable<long>("progress")
            );

            EndAction();
        }
    }

}
