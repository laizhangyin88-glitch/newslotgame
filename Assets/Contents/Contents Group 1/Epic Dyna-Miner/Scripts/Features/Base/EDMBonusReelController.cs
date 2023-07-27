using System.Collections.Generic;
using System.Linq;
using BSS.Utils;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;

namespace GameStudio.Slot.EDM
{
    public class EDMBonusReelController : FeatureModule
    {
        public int slotIndex;
        public SlotMachine slotMachine;

        private void Awake()
        {
            RegisterEvent("EDM_INIT_BONUS_REELS", OnInitBonusReels);
            RegisterEvent("EDM_REVERT_BONUS_REELS", OnRevertBonusReels);
            RegisterEvent("EDM_CLEAR_BONUS_REELS", OnClearBonusReels);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            MessageDispatcher.Register("OnCreditEvent", OnUpdatedTotalBetCredit);
            EDMGameProgressManager.Instance.OnReceivedResponse.AddListener(OnReceivedReponse);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            MessageDispatcher.UnRegister("OnCreditEvent", OnUpdatedTotalBetCredit);
            EDMGameProgressManager.Instance.OnReceivedResponse.RemoveListener(OnReceivedReponse);
        }
        private void UpdateSpinReels(long totalBet, Blackboard gameProgressBB = null)
        {
            if (gameProgressBB == null) gameProgressBB = EDMGameProgressManager.Instance.gameProgressBB;
            if (gameProgressBB == null)
            {
                BlackboardUtils.FindVariable<List<bool>>("./customData/baseTopSpinReelList").SetValue(new List<bool>(15).Fill(true, 15).ToList());
                return;
            }

            Variable<Dictionary<long, Blackboard>> gameProgressVariable = BlackboardUtils.FindVariable<Dictionary<long, Blackboard>>(gameProgressBB, $"userDataPerBet");
            if (gameProgressVariable == null)
            {
                BlackboardUtils.FindVariable<List<bool>>("./customData/baseTopSpinReelList").SetValue(new List<bool>(15).Fill(true, 15).ToList());
                return;
            }

            if (gameProgressVariable.value.ContainsKey(totalBet) == false)
            {
                BlackboardUtils.FindVariable<List<bool>>("./customData/baseTopSpinReelList").SetValue(new List<bool>(15).Fill(true, 15).ToList());
                return;
            }

            Blackboard userDataBB = gameProgressVariable.value[totalBet];
            if (userDataBB.variables.Count == 0)
            {
                BlackboardUtils.FindVariable<List<bool>>("./customData/baseTopSpinReelList").SetValue(new List<bool>(15).Fill(true, 15).ToList());
                return;
            }
            List<bool> spinList = BlackboardUtils.FindVariable<List<bool>>(userDataBB, $"bonusSpotsNextSpinReelList").value;
            BlackboardUtils.FindVariable<List<bool>>("./customData/baseTopSpinReelList").SetValue(spinList);

        }
        private void UpdateBonusReel(long totalBet, Blackboard gameProgressBB = null)
        {
            if (gameProgressBB == null) gameProgressBB = EDMGameProgressManager.Instance.gameProgressBB;
            if (gameProgressBB == null)
            {
                for (int i = 0; i < slotMachine.reels.Count; i++)
                {
                    BaseSymbol symbol = slotMachine.reels[i].symbols[1];
                    symbol.symbolIndex = 11;

                    symbol.symbolMask = (int)ContentCustomData.GetSlotData(slotIndex).symbolMask.GetMask(11);
                    symbol.Change(symbol.symbolInfo);
                    symbol.Apply();
                }
                return;
            }

            Variable<Dictionary<long, Blackboard>> gameProgressVariable = BlackboardUtils.FindVariable<Dictionary<long, Blackboard>>(gameProgressBB, $"userDataPerBet");
            if (gameProgressVariable == null)
            {
                for (int i = 0; i < slotMachine.reels.Count; i++)
                {
                    BaseSymbol symbol = slotMachine.reels[i].symbols[1];
                    symbol.symbolIndex = 11;

                    symbol.symbolMask = (int)ContentCustomData.GetSlotData(slotIndex).symbolMask.GetMask(11);
                    symbol.Change(symbol.symbolInfo);
                    symbol.Apply();
                }
                return;
            }

            if (gameProgressVariable.value.ContainsKey(totalBet) == false)
            {
                for (int i = 0; i < slotMachine.reels.Count; i++)
                {
                    BaseSymbol symbol = slotMachine.reels[i].symbols[1];
                    symbol.symbolIndex = 11;

                    symbol.symbolMask = (int)ContentCustomData.GetSlotData(slotIndex).symbolMask.GetMask(11);
                    symbol.Change(symbol.symbolInfo);
                    symbol.Apply();
                }
                return;
            }
            Blackboard userDataBB = gameProgressVariable.value[totalBet];
            if (userDataBB.variables.Count == 0)
            {
                for (int i = 0; i < slotMachine.reels.Count; i++)
                {
                    BaseSymbol symbol = slotMachine.reels[i].symbols[1];
                    symbol.symbolIndex = 11;

                    symbol.symbolMask = (int)ContentCustomData.GetSlotData(slotIndex).symbolMask.GetMask(11);
                    symbol.Change(symbol.symbolInfo);
                    symbol.Apply();
                }
                return;
            }
            List<int> symbolIndexList = BlackboardUtils.FindVariable<List<int>>(userDataBB, $"bonusSpotsSymbolIndexList").value;
            List<long> bonusSpotsCreditList = BlackboardUtils.FindVariable<List<long>>(userDataBB, $"bonusSpotsCreditList").value;

            for (int i = 0; i < slotMachine.reels.Count; i++)
            {
                BaseSymbol symbol = slotMachine.reels[i].symbols[1];
                symbol.symbolIndex = symbolIndexList[i];
                symbol.symbolMask = (int)ContentCustomData.GetSlotData(slotIndex).symbolMask.GetMask(symbolIndexList[i]);
                if (symbol.symbolIndex == 12)
                {
                    if (symbol.symbolInfo.customData == null) symbol.symbolInfo.customData = new Dictionary<string, object>();
                    else symbol.symbolInfo.customData.Clear();
                    symbol.symbolInfo.customData.Add("value", bonusSpotsCreditList[i]);
                }

                symbol.Change(symbol.symbolInfo);
                symbol.Apply();

                if (symbol.symbolIndex >= 15 && symbol.symbolIndex <= 17)
                {
                    symbol.Play("UpgradeFinish");
                }
            }
        }

        private void ClearBonusReel()
        {

            for (int i = 0; i < slotMachine.reels.Count; i++)
            {
                BaseSymbol symbol = slotMachine.reels[i].symbols[1];
                symbol.symbolIndex = 11;

                symbol.symbolMask = (int)ContentCustomData.GetSlotData(slotIndex).symbolMask.GetMask(11);
                symbol.Change(symbol.symbolInfo);
                symbol.Apply();
            }
        }



        public void OnInitBonusReels(EventData eventData)
        {
            UpdateBonusReel(BlackboardUtils.FindVariable<long>("./totalBetCredit").value);
            UpdateSpinReels(BlackboardUtils.FindVariable<long>("./totalBetCredit").value);
        }
        public void OnRevertBonusReels(EventData eventData)
        {
            UpdateBonusReel(BlackboardUtils.FindVariable<long>("./customData/totalBet").value, (Blackboard)eventData.value);
            UpdateSpinReels(BlackboardUtils.FindVariable<long>("./customData/totalBet").value, (Blackboard)eventData.value);
        }

        public void OnClearBonusReels(EventData eventData)
        {
            ClearBonusReel();
        }

        public void OnUpdatedTotalBetCredit(EventData eventData)
        {
            if (eventData.name == "UpdateBetCredit" && BlackboardUtils.FindValue<bool>("./initializedContent") == true)
            {
                if (BlackboardUtils.FindValue<bool>("./customData/isSuperBonus") == false)
                {
                    UpdateBonusReel(BlackboardUtils.FindVariable<long>("./totalBetCredit").value);
                    UpdateSpinReels(BlackboardUtils.FindVariable<long>("./totalBetCredit").value);
                }
            }
        }

        public void OnReceivedReponse()
        {
            UpdateSpinReels(BlackboardUtils.FindVariable<long>("./totalBetCredit").value);
            GameObject.Destroy(BlackboardUtils.FindValue<Blackboard>("./latestGameProgress").gameObject);
            BlackboardUtils.FindValue<Blackboard>("./spin/response/gameProgress").transform.SetParent(ContentBlackboard.Get().transform);
            ContentBlackboard.Get().GetVariable<Blackboard>("latestGameProgress").SetValue(BlackboardUtils.FindValue<Blackboard>("./spin/response/gameProgress"));
        }
    }
}