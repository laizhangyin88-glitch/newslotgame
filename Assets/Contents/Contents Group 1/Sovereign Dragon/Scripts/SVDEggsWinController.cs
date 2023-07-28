using BagelCode.Tasks.Actions.BI;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using SlotMaker.Tasks.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.Slot.SVD
{
    public class SVDEggsWinController : FeatureController
    {
        protected override string ON_FEATURE_BEGIN_EVENT => "StartEggsWin";
        protected override string ON_FEATURE_END_EVENT => "EggsWinDone";

        [SerializeField] private Blackboard mainFSMBB;

        private const string ON_CONTENT_UI_EVENT = "OnContentUIEvent";
        private const string ON_META_UI_EVENT = "OnMetaUIEvent";
        private const string ON_WIN_EVENT = "OnWinEvent";

        private const int ColumnCount = 5;
        private const int RowCount = 7;
        private const int ProgressiveJpIndex = 3;

        private bool isJackpotReset;

        enum EggType
        {
            Gold = 11,
            Silver = 12,
        }

        private bool isJPClaimed;
        private bool isUneligibleInWinPositions = false;

        private readonly List<string> JPPopupNames = new List<string>() {
            "SVD JP Mini Popup",
            "SVD JP Minor Popup",
            "SVD JP Major Popup",
            "SVD JP Mega Popup"
        };

        protected override IEnumerator OnPlayCoroutine()
        {
            isJackpotReset = BlackboardUtils.GetOrCreateVariable<bool>(mainFSMBB, "_isJackpotReset").value;
            var totalLineWin = BlackboardUtils.GetOrCreateVariable<double>(mainFSMBB, "_totalLineWin");
            totalLineWin.value = 0;

            var isStart = BlackboardUtils.FindVariable<bool>(mainFSMBB, "isStart").value;
            var winSymbolsInfo = isStart
                ? BlackboardUtils.FindVariable<List<Blackboard>>(mainFSMBB, "./bonus/response/initialWinSymbolPositions").value
                : BlackboardUtils.FindVariable<List<Blackboard>>(
                    BlackboardUtils.FindVariable<Blackboard>(mainFSMBB, "_currentStep").value,
                    "winSymbolPositions").value;

            int winSymbolsCount = winSymbolsInfo.Count;
            if (winSymbolsCount == 0)
            {
                yield break;
            }

            int winMultiplier = winSymbolsCount / ColumnCount;
            MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData<int>("ShowMultiplier", winMultiplier));

            ChangeJPToEligible(winSymbolsInfo, winSymbolsCount);

            if (isUneligibleInWinPositions)
            {
                isUneligibleInWinPositions = false;
                yield return new WaitForSeconds(1.5f);
            }

            BlackboardUtils.GetOrCreateVariable<int>(mainFSMBB, "./customData/goldSymbolWinCount").value = 0;
            BlackboardUtils.GetOrCreateVariable<int>(mainFSMBB, "./customData/goldSymbolWinCollectCount").value = 0;
            MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData("StartCollectSymbol"));
            int expandDiff = 3; // TODO: need to check initial value
            if (!isStart)
            {
                var currentStepBB = BlackboardUtils.FindVariable<Blackboard>(mainFSMBB, "_currentStep").value;
                var initialExpandType = BlackboardUtils.FindVariable<int>(currentStepBB, "initialExpandType").value;
                var finalExpandType = BlackboardUtils.FindVariable<int>(currentStepBB, "finalExpandType").value;
                var currentSilverEggsProgressInt = GlobalBlackboard.Find("Animator_BB").GetVariable<int>("amountSilverEggsO`nTopPanel").value;
                mainFSMBB.SetValue("_currentSilverEggsProgressInt", currentSilverEggsProgressInt);
                expandDiff = finalExpandType - initialExpandType;
            }

            var slotMachineExpand = BlackboardUtils.FindVariable<GameObject>(mainFSMBB, "_slotMachineExpand");
            var sm = slotMachineExpand.value.GetComponent<BaseSlotMachine>();
            var dropCellWinList = new List<Cell>();

            var timeWaitBetweenSymbols = 0.25f;
            var hasGoldEgg = false;

            for (int winEggIndex = 0; winEggIndex < winSymbolsCount; winEggIndex++)
            {
                var winEggInfo = winSymbolsInfo[winEggIndex];

                var rowWinSymbolPos = BlackboardUtils.FindVariable<int>(winEggInfo, "row").value;
                var columnWinSymbolPos = BlackboardUtils.FindVariable<int>(winEggInfo, "column").value;
                var coinValue = BlackboardUtils.FindVariable<double>(winEggInfo, "coin").value;
                var isJackpot = BlackboardUtils.FindVariable<bool>(winEggInfo, "isJackpot").value;
                var symbolType = BlackboardUtils.FindVariable<int>(winEggInfo, "symbolType").value;
                var jackpotIndex = BlackboardUtils.FindVariable<int>(winEggInfo, "jackpotIndex").value;

                var serverPosition = RowCount * columnWinSymbolPos + rowWinSymbolPos;
                var clientPosition = ColumnCount * rowWinSymbolPos + columnWinSymbolPos;

                var dropSymbolCell = new Cell(clientPosition, rowWinSymbolPos);
                dropCellWinList.Add(dropSymbolCell);

                var winSymbolGO = sm.GetSymbol(dropSymbolCell.column, dropSymbolCell.row).gameObject;
                var winSymbolTransform = winSymbolGO.GetComponent<Transform>();
                var winSymbolBB = winSymbolGO.GetComponent<Blackboard>();

                // TODO: If you have time, extract all strings, such as 'listFlatGameObj' to cost string fields
                var listFlatGameObj = BlackboardUtils.FindVariable<List<GameObject>>(mainFSMBB, "listFlatGameObj");
                var winDropGO = listFlatGameObj.value[serverPosition];

                if (symbolType == (int)EggType.Gold)
                {
                    hasGoldEgg = true;
                    BaseSymbol symbol = sm.GetSymbol(dropSymbolCell.column, dropSymbolCell.row);
                    symbol.symbolIndex = (int)EggType.Gold;
                    symbol.symbolMask = OperationUtils.Operate(symbol.symbolMask, 0, OperationMethod.Set);
                    winSymbolGO.GetComponent<BaseSymbol>().Apply();

                    winSymbolBB.SetValue("isJackpot", isJackpot);
                    winSymbolBB.SetValue("jpIndex", jackpotIndex);

                    if (!isJackpot)
                    {
                        var betCredit = Convert.ToDouble(BlackboardUtils.FindVariable<long>(mainFSMBB, "_betCredit").value);
                        coinValue *= betCredit;
                    }
                    else if (jackpotIndex == ProgressiveJpIndex && !isJackpotReset)
                    {
                        isJackpotReset = true;
                        mainFSMBB.SetValue("_isJackpotReset", true);
                        var progressiveCoinValue = FindUnusedProgressiveJpPay(ProgressiveJpIndex, coinValue) / winMultiplier;
                        coinValue = Math.Max(coinValue, progressiveCoinValue);
                        winEggInfo.SetValue("coin", coinValue);
                        MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData("UpdateJackpotInfo"));
                    }

                    winSymbolBB.SetValue("applyCoinValue", coinValue);
                    winSymbolBB.SetValue("coinMultiplier", Convert.ToDouble(winMultiplier));

                    mainFSMBB.SetValue("_multipliedCoinValue", coinValue * Convert.ToDouble(winMultiplier)); // set double
                    totalLineWin.value += coinValue * winMultiplier;
                    BlackboardUtils.GetOrCreateVariable<int>(mainFSMBB, "./customData/goldSymbolWinCount").value++;
                }

                if (symbolType == (int)EggType.Silver)
                {
                    var symbol = sm.GetSymbol(dropSymbolCell.column, dropSymbolCell.row);
                    symbol.symbolIndex = (int)EggType.Silver;
                    symbol.symbolMask = OperationUtils.Operate(symbol.symbolMask, 0, OperationMethod.Set);
                    winSymbolGO.GetComponent<BaseSymbol>().Apply();
                    winSymbolBB.SetValue("_panelTransform", GlobalBlackboard.Find("Animator_BB").GetVariable<RectTransform>("tooltipRectTransform").value);
                    BlackboardUtils.GetOrCreateVariable<int>(mainFSMBB, "_currentSilverEggsProgressInt").value--;
                }

                BlackboardUtils.GetOrCreateVariable<List<GameObject>>(mainFSMBB, "winDropSymbolStack").value.Add(winDropGO);
                var winDropGraphOwner = winDropGO.GetComponent<GraphOwner>();
                winDropGraphOwner.SendEvent(new EventData("Disappear"), winDropGO);

                listFlatGameObj.value[serverPosition] = null;

                sm.GetSymbol(dropSymbolCell.column, dropSymbolCell.row).Play("DropSymbolWin");
                yield return new WaitForSeconds(timeWaitBetweenSymbols);

                if (!isStart && BlackboardUtils.GetOrCreateVariable<int>(mainFSMBB, "_currentSilverEggsProgressInt").value <= 0 && expandDiff > 0)
                {
                    var countUnderZero = BlackboardUtils
                        .GetOrCreateVariable<int>(mainFSMBB, "_currentSilverEggsProgressInt").value;

                    BlackboardUtils.GetOrCreateVariable<bool>(mainFSMBB, "./customData/canFlySymbol").value = false;

                    BlackboardUtils.GetOrCreateVariable<int>(mainFSMBB, "_currentSilverEggsProgressInt").value = 15 + countUnderZero;
                    expandDiff--;
                    MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData("DragonBurnDelayed"));
                }
            }

            GSManager.Instance.GetHandler("Full Row Special Symbol").Play();
            var respinTotalWinCredit = Convert.ToDouble(BlackboardUtils.GetOrCreateVariable<long>(mainFSMBB, "./customData/respinTotalWinCredit").value);
            BlackboardUtils.GetOrCreateVariable<long>(mainFSMBB, "./customData/respinTotalWinCredit").value = Convert.ToInt64(respinTotalWinCredit + totalLineWin.value);
            MessageDispatcher.Dispatch(ON_WIN_EVENT, new EventData<double>("totalLinesWin", 1, totalLineWin.value));

            var symbolFlyCount = 0;
            var isSymbolFly = false;

            if (!BlackboardUtils.FindVariable<bool>(mainFSMBB, "./customData/canFlySymbol").value)
            {
                yield return new WaitForSeconds(5f);
                BlackboardUtils.FindVariable<bool>(mainFSMBB, "./customData/canFlySymbol").value = true;
                isSymbolFly = true;
                for (int i = 0; i < dropCellWinList.Count; ++i)
                {
                    var cell = dropCellWinList[i];
                    if (!sm.GetSymbol(cell.column, cell.row).GetComponent<Blackboard>()
                            .GetVariable<bool>("_finishedFly").value)
                    {
                        symbolFlyCount++;
                        sm.GetSymbol(cell.column, cell.row).Play("RetryFly");
                        yield return new WaitForSeconds(timeWaitBetweenSymbols);
                    }
                }
            }

            if (isSymbolFly)
            {
                yield return new WaitForSeconds(symbolFlyCount > 0 ? 2f : 0f);
            }
            else
            {
                yield return new WaitForSeconds(4f);
            }

            if (hasGoldEgg)
            {
                yield return new WaitUntil(() =>
                    BlackboardUtils.FindVariable<bool>(mainFSMBB, "./customData/collectDone").value);
            }
            BlackboardUtils.FindVariable<bool>(mainFSMBB, "./customData/collectDone").value = false;
            PlaySymbolAnimationByCell(sm, dropCellWinList, "DropResult");
            dropCellWinList.Clear();

            var winDropSymbolStack = BlackboardUtils.GetOrCreateVariable<List<GameObject>>(mainFSMBB, "winDropSymbolStack").value;
            foreach (var symbol in winDropSymbolStack)
            {
                symbol.GetComponent<Blackboard>().SetValue("isJackpot", false);
                var go = symbol.transform.GetComponent<PooledObject>();
                go.ReturnToPool();
            }
            winDropSymbolStack.Clear();


            foreach (var winSymbolInfo in winSymbolsInfo)
            {
                var isJackpot = BlackboardUtils.GetOrCreateVariable<bool>(winSymbolInfo, "isJackpot").value;
                var symbolType = BlackboardUtils.GetOrCreateVariable<int>(winSymbolInfo, "symbolType").value;

                if (symbolType == (int)EggType.Gold && isJackpot)
                {
                    var jackpotIndex = BlackboardUtils.GetOrCreateVariable<int>(winSymbolInfo, "jackpotIndex").value;
                    var coinValue = BlackboardUtils.GetOrCreateVariable<double>(winSymbolInfo, "coin").value;

                    BlackboardUtils.GetOrCreateVariable<double>(winSymbolInfo, "./customData/totalTicketPrize").value = coinValue * winMultiplier;
                    MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData("OnJackpotWinPopUp"));
                    yield return new WaitForSeconds(0.5f);

                    BlackboardUtils.FindVariable<bool>(mainFSMBB, "needPlayBGM").value = true;

                    OpenJPPopup(jackpotIndex);
                    isJPClaimed = false;

                    // event from JP popup closed
                    RegisterEvent("ClosedPopup", (EventData eventData) => ClaimJPBonus(jackpotIndex));
                    yield return new WaitUntil(() => isJPClaimed);
                    UnRegisterEvent("ClosedPopup");
                }
            }

            if (BlackboardUtils.FindVariable<bool>(mainFSMBB, "needPlayBGM").value)
            {
                MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData("playDropBGM"));
                BlackboardUtils.FindVariable<bool>(mainFSMBB, "needPlayBGM").value = false;
            }
            BlackboardUtils.GetOrCreateVariable<int>(mainFSMBB, "./customData/remainReSpinCount").value = BlackboardUtils.GetOrCreateVariable<int>(mainFSMBB, "_initialSpinCount").value;
            MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData("UpdateSpinCount"));
            MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData("UpdateFlySymbolCount"));
            yield return new WaitForSeconds(1f);
        }

        private void OpenJPPopup(int jackpotIndex)
        {
            var popupName = JPPopupNames[jackpotIndex];
            new OpenContentPopup { assetName = popupName, uniqueName = popupName }.ExecuteAction(mainFSMBB, mainFSMBB);
        }

        private void ClaimJPBonus(int findingJPIndex)
        {
            BlackboardUtils.GetOrCreateVariable<bool>(mainFSMBB, "_jpBonusIsFinded").value = false;
            var jackpotWinBonusList = BlackboardUtils.FindVariable<List<Blackboard>>(mainFSMBB, "jackpotWinBonusList").value;
            for (int bonusIndex = 0; bonusIndex < jackpotWinBonusList.Count; bonusIndex++)
            {
                var jackpotBonus = jackpotWinBonusList[bonusIndex];
                var currentJPBonusIsClaimed = BlackboardUtils.GetOrCreateVariable<bool>(jackpotBonus, "claimed").value;
                var jackpotIndex = BlackboardUtils.GetOrCreateVariable<int>(jackpotBonus, "jackpotIndex").value;
                var jackpotAwardAmount = Convert.ToDouble(BlackboardUtils.GetOrCreateVariable<long>(jackpotBonus, "jackpotAwardAmount").value);

                if (!currentJPBonusIsClaimed && findingJPIndex == jackpotIndex &&
                    OperationUtils.Compare(
                        jackpotAwardAmount,
                        BlackboardUtils.GetOrCreateVariable<double>(mainFSMBB, "./customData/totalTicketPrize").value,
                        CompareMethod.EqualTo,
                        0.05f)
                   )
                {
                    var uid = BlackboardUtils.GetOrCreateVariable<string>(jackpotBonus, "uid").value;
                    MetaSystem.SlotClaimBonus(uid, 0, null, null);
                    new BI_bonus1 { bonusResponse = jackpotBonus }.ExecuteAction(mainFSMBB, mainFSMBB);
                    jackpotBonus.SetValue("claimed", true);
                    break;
                }
            }

            isJPClaimed = true;
        }

        private static void PlaySymbolAnimationByCell(BaseSlotMachine sm, List<Cell> dropCellWinList, string animationName)
        {
            for (int i = 0; i < dropCellWinList.Count; ++i)
            {
                var cell = dropCellWinList[i];
                sm.GetSymbol(cell.column, cell.row).Play(animationName);
            }
        }

        private void ChangeJPToEligible(List<Blackboard> winSymbolsInfo, int winSymbolsCount)
        {
            for (int winEggIndex = 0; winEggIndex < winSymbolsCount; winEggIndex++)
            {
                var winEggInfo = winSymbolsInfo[winEggIndex];

                var isJackpot = BlackboardUtils.FindVariable<bool>(winEggInfo, "isJackpot").value;
                var symbolType = BlackboardUtils.FindVariable<int>(winEggInfo, "symbolType").value;

                if (symbolType != (int)EggType.Gold || !isJackpot)
                {
                    continue;
                }

                var rowWinSymbolPos = BlackboardUtils.FindVariable<int>(winEggInfo, "row").value;
                var columnWinSymbolPos = BlackboardUtils.FindVariable<int>(winEggInfo, "column").value;

                var serverPosition = RowCount * columnWinSymbolPos + rowWinSymbolPos;
                var listFlatGameObj = BlackboardUtils.FindVariable<List<GameObject>>(mainFSMBB, "listFlatGameObj");
                var dropSymbolGO = listFlatGameObj.value[serverPosition];
                var dropSymbolBB = dropSymbolGO.GetComponent<Blackboard>();
                BlackboardUtils.FindVariable<bool>(dropSymbolBB, "changeJPToEligible").value = true;
                var jpIndexOnDropPrefab = BlackboardUtils.FindVariable<int>(dropSymbolBB, "jpIndex").value;
                var globalMaxJPIndex = GlobalBlackboard.Find("Animator_BB").GetVariable<int>("globalMaxJPIndex").value;
                if (jpIndexOnDropPrefab > globalMaxJPIndex)
                {
                    isUneligibleInWinPositions = true;
                }
            }
        }

        private double FindUnusedProgressiveJpPay(int findingJPIndex, double nonProgressivePayValue)
        {
            var jackpotWinBonusList = BlackboardUtils.FindVariable<List<Blackboard>>(mainFSMBB, "jackpotWinBonusList").value;
            double jpPay = 0;

            for (int bonusIndex = 0; bonusIndex < jackpotWinBonusList.Count; bonusIndex++)
            {
                var jackpotBonus = jackpotWinBonusList[bonusIndex];
                var currentJPBonusIsClaimed = BlackboardUtils.GetOrCreateVariable<bool>(jackpotBonus, "claimed").value;
                var jackpotIndex = BlackboardUtils.GetOrCreateVariable<int>(jackpotBonus, "jackpotIndex").value;
                var jackpotAwardAmount = Convert.ToDouble(BlackboardUtils.GetOrCreateVariable<long>(jackpotBonus, "jackpotAwardAmount").value);

                if (!currentJPBonusIsClaimed && findingJPIndex == jackpotIndex)
                {
                    var progressivePart = jackpotAwardAmount % nonProgressivePayValue;
                    if (progressivePart != 0)
                    {
                        jpPay = jackpotAwardAmount;
                        break;
                    }
                }
            }

            return jpPay;
        }
    }
}
