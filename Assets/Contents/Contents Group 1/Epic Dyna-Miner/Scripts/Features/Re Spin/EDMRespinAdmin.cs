using System;
using System.Collections;
using System.Collections.Generic;
using GameStudio.Slot.EDM.SymbolBehaviour;
using GameStudio.Slot.EDM.Utility;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.EDM.Feature
{
    public class EDMRespinAdmin : FeatureModule
    {
        public Animator mainAnimator;
        public Animator frameAnimator;
        public SlotMachine bonusSlotMachine;

        [Space(20)]
        [Header("Spin Left")]
        public Animator spinLeftAddAnimator;
        public List<Animator> spinLeftAnimatorList;

        [Space(20)]
        [Header("Expand")]
        public List<Animator> lockReelAnimatorList;
        public int currentActiveRow;

        [Space(20)]
        [Header("Upgrade")]
        public EDMRandomUpgradeController randomUpgradeController;

        [Space(20)]
        [Header("Collect")]
        public EDMCollectController collectController;


        private List<Vector2Int> jackpotCellList;
        [HideInInspector] public List<Vector2Int> creditCollectCellList = new List<Vector2Int>();
        [HideInInspector] public List<Vector2Int> jackpotCollectCellList = new List<Vector2Int>();
        [HideInInspector] public List<long> jackpotCreditAmountList = new List<long>();


        private void Awake()
        {
            RegisterEvent("EDM_ON_INITALIZED_BONUSGAME", OnInitializedBonusGame);
            RegisterEvent("EDM_ON_EXPAND_ROW", OnExpandRow);
            RegisterEvent("EDM_ON_MORE_SPIN", OnMoreSpin);
            RegisterEvent("EDM_ON_UPGRADE", OnUpgrade);
            RegisterEvent("EDM_ON_UPGRADE_EVERYSPIN", OnUpgradeEverySpin);
            RegisterEvent("EDM_ON_COLLECT", OnCollect);
            RegisterEvent("EDM_ON_SAVE", OnSave);
            RegisterEvent("EDM_ON_LAND_JACKPOT", OnLandJackpot);
            RegisterEvent("EDM_ON_OPEN_JACKPOT", OnOpenJackpot);
            RegisterEvent("EDM_ON_KEY", OnKey);
            RegisterEvent("EDM_ON_BEGINSPIN", OnBeginSpin);
            RegisterEvent("EDM_ON_RESET_SPIN_COUNT", OnResetSpinCount);
            RegisterEvent("EDM_ON_FULLSCREEN_EXPAND_BONUS", OnFullscreenExpandBonus);
        }

        private void OnInitializedBonusGame(EventData eventData)
        {
            if (BlackboardUtils.GetOrCreateVariable<bool>("./customData/isSuperBonus").value == true)
                OnInitializeSuperBonusGame();
            else
                OnInitializeBasicBonusGame();
        }

        private void OnInitializeBasicBonusGame()
        {
            currentActiveRow = 3;
            BlackboardUtils.GetOrCreateVariable<int>("./customData/currentRowCount").SetValue(currentActiveRow);
            mainAnimator.SetInteger("Row", currentActiveRow);
            BlackboardUtils.GetOrCreateVariable<int>("./customData/defaultSpinCount").SetValue(3);
            jackpotCellList = new List<Vector2Int>();
            BlackboardUtils.GetOrCreateVariable<int>("./bonus/spinCountForSignBoard").SetValue(0);

            StartCoroutine(InitializeSpinLeftUICoroutine());

            creditCollectCellList.Clear();
            jackpotCollectCellList.Clear();
            jackpotCreditAmountList.Clear();
        }
        private void OnInitializeSuperBonusGame()
        {
            mainAnimator.SetInteger("Row", 3);
            currentActiveRow = 4;
            BlackboardUtils.GetOrCreateVariable<int>("./customData/currentRowCount").SetValue(currentActiveRow);
            BlackboardUtils.GetOrCreateVariable<int>("./customData/defaultSpinCount").SetValue(4);
            jackpotCellList = new List<Vector2Int>();
            BlackboardUtils.GetOrCreateVariable<int>("./bonus/spinCountForSignBoard").SetValue(0);

            StartCoroutine(InitializeSpinLeftUICoroutine());

            creditCollectCellList.Clear();
            jackpotCollectCellList.Clear();
            jackpotCreditAmountList.Clear();
        }

        IEnumerator InitializeSpinLeftUICoroutine()
        {
            yield return new WaitForSeconds(0.25f);
            //  spinLeftAddAnimator.SetBool("SuperBonus", BlackboardUtils.GetOrCreateVariable<bool>("./customData/isSuperBonus").value);

            yield return new WaitForSeconds(1.5f);

            foreach (var each in spinLeftAnimatorList)
            {
                if (each.gameObject.activeInHierarchy == true)
                    each.SetTrigger("Appear");
                EDMUtility.PlaySound("Spin Appear");
                yield return new WaitForSeconds(0.1f);
            }
        }
        private void OnFullscreenExpandBonus(EventData eventData)
        {
            StartCoroutine(FullScreenExpandBonusCoroutine((int)eventData.value,
             () => ContentEvent.SendEvent("EDM_END_FULLSCREEN_EXPAND_BONUS")));
        }
        private void OnExpandRow(EventData eventData)
        {
            StartCoroutine(ExpandRowCoroutine((
                List<Blackboard>)eventData.value,
             () => ContentEvent.SendEvent("EDM_END_EXPAND_ROW")));
        }
        private void OnMoreSpin(EventData eventData)
        {
            StartCoroutine(MoreSpinCoroutine((
                List<Blackboard>)eventData.value,
             () => ContentEvent.SendEvent("EDM_END_MORE_SPIN")));
        }
        private void OnUpgradeEverySpin(EventData eventData)
        {
            StartCoroutine(UpgradeEverySpinCoroutine((
                List<Blackboard>)eventData.value,
             () => ContentEvent.SendEvent("EDM_END_UPGRADE_EVERYSPIN")));
        }
        private void OnKey(EventData eventData)
        {
            StartCoroutine(ActiveKeyCoroutine((
               Blackboard)eventData.value,
             () => ContentEvent.SendEvent("EDM_END_KEY")));
        }
        private void OnUpgrade(EventData eventData)
        {
            StartCoroutine(UpgradeCoroutine((
                List<Blackboard>)eventData.value,
             () => ContentEvent.SendEvent("EDM_END_UPGRADE")));
        }
        private void OnCollect(EventData eventData)
        {
            StartCoroutine(CollectCoroutine((
                List<Blackboard>)eventData.value,
             () => ContentEvent.SendEvent("EDM_END_COLLECT")));
        }
        private void OnSave(EventData eventData)
        {
            StartCoroutine(SaveCoroutine((
                List<Blackboard>)eventData.value,
             () => ContentEvent.SendEvent("EDM_END_SAVE")));
        }
        private void OnLandJackpot(EventData eventData)
        {
            StartCoroutine(LandJackpotCoroutine((
                List<Blackboard>)eventData.value,
             () => ContentEvent.SendEvent("EDM_END_LAND_JACKPOT")));
        }
        private void OnOpenJackpot(EventData eventData)
        {
            StartCoroutine(OpenJackpotCoroutine((
               Blackboard)eventData.value,
             () => ContentEvent.SendEvent("EDM_END_OPEN_JACKPOT")));
        }
        private void OnBeginSpin(EventData eventData)
        {
            StartCoroutine(BeginSpinCoroutine(
             () => ContentEvent.SendEvent("EDM_END_ON_BEGINSPIN")));
        }
        private void OnResetSpinCount(EventData eventData)
        {
            BlackboardUtils.FindVariable<int>("./bonus/spinCountForSignBoard").SetValue(0);
            UpdateSpinLeftCount(0);
        }
        private void UpdateSpinLeftCount(int spinCount)
        {
            for (int i = 0; i < 5; i++)
                if (spinLeftAnimatorList[i].gameObject.activeInHierarchy == true) spinLeftAnimatorList[i].SetBool("Active", false);

            int defaultSpinCount = BlackboardUtils.FindValue<int>("./customData/defaultSpinCount");

            for (int i = 0; i < defaultSpinCount - spinCount; i++)
                if (spinLeftAnimatorList[i].gameObject.activeInHierarchy == true)
                    spinLeftAnimatorList[i].SetBool("Active", true);

            MessageDispatcher.Dispatch("OnContentUIEvent", new EventData("UpdateSpinCount"));
            MessageDispatcher.Dispatch("OnWinEvent", new EventData("UpdateSpinCount"));
        }
        private IEnumerator ExpandRowCoroutine(List<Blackboard> expandDataList, Action onFinished)
        {
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < expandDataList.Count; i++)
            {
                Blackboard expandData = expandDataList[i];

                Blackboard cell = expandData.GetVariable<Blackboard>("cell").value;


                BaseSymbol symbol = EDMUtility.GetSymbolFromSpotSlotMachine(bonusSlotMachine, EDMUtility.ConvertCellBBToVector2Int(cell));
                symbol.Play("Active");
                lockReelAnimatorList[currentActiveRow - 3].SetTrigger("Unlock");
                currentActiveRow++;
                mainAnimator.SetInteger("Row", currentActiveRow);
                BlackboardUtils.FindVariable<int>("./customData/currentRowCount").SetValue(currentActiveRow);

                yield return new WaitForSeconds(1f);
                if (currentActiveRow == 6) frameAnimator.SetTrigger("Active");
                for (int j = 0; j < 5; j++)
                {
                    BaseSymbol expandedSymbol = EDMUtility.GetSymbolFromSpotSlotMachine(bonusSlotMachine, new Vector2Int(j, 6 - currentActiveRow));
                    if (expandedSymbol.symbolIndex >= 15 && expandedSymbol.symbolIndex <= 17)
                    {
                        expandedSymbol.Play("StopEffectWithExpand");
                    }
                }

                yield return new WaitForSeconds(1.75f);
            }
            yield return new WaitForSeconds(0.5f);
            onFinished.Invoke();
        }
        private IEnumerator FullScreenExpandBonusCoroutine(int count, Action onFinished)
        {
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < count; i++)
            {

                lockReelAnimatorList[currentActiveRow - 3].SetTrigger("Unlock");
                currentActiveRow++;
                mainAnimator.SetInteger("Row", currentActiveRow);
                BlackboardUtils.FindVariable<int>("./customData/currentRowCount").SetValue(currentActiveRow);
                EDMUtility.PlaySound("Row Unlock");
                yield return new WaitForSeconds(0.9f);
                if (currentActiveRow == 6) frameAnimator.SetTrigger("Active");

                for (int j = 0; j < 5; j++)
                {
                    BaseSymbol expandedSymbol = EDMUtility.GetSymbolFromSpotSlotMachine(bonusSlotMachine, new Vector2Int(j, 6 - (currentActiveRow)));
                    if (expandedSymbol.symbolIndex >= 15 && expandedSymbol.symbolIndex <= 17)
                    {
                        expandedSymbol.Play("StopEffectWithExpand");
                    }
                }
            }

            yield return new WaitForSeconds(1.5f);
            onFinished.Invoke();
        }

        private IEnumerator MoreSpinCoroutine(List<Blackboard> moreSpinDataList, Action onFinished)
        {
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < moreSpinDataList.Count; i++)
            {
                Blackboard moreSpinData = moreSpinDataList[i];

                Blackboard cell = moreSpinData.GetVariable<Blackboard>("cell").value;
                Transform targetAnchor = BlackboardUtils.GetOrCreateVariable<bool>("./customData/isSuperBonus").value == true ? spinLeftAnimatorList[4].transform : spinLeftAnimatorList[3].transform;
                BaseSymbol symbol = EDMUtility.GetSymbolFromSpotSlotMachine(bonusSlotMachine, EDMUtility.ConvertCellBBToVector2Int(cell));
                symbol.Play("Active");
                symbol.GetComponentInChildren<EDMMoreSpinActiveController>().Initialize(symbol.transform, targetAnchor);
                yield return new WaitForSeconds(1.34f);

                symbol.symbolIndex = 11;
                symbol.symbolMask = (int)ContentCustomData.GetSlotData(symbol.slotMachine.slotIndex).symbolMask.GetMask(symbol.symbolIndex);

                symbol.Change(symbol.symbolInfo);
                symbol.Apply();
            }
            yield return null;


            spinLeftAddAnimator.SetBool("Add", true);
            EDMUtility.PlaySound("Add Spin");
            MessageDispatcher.Dispatch("OnContentUIEvent", new EventData("UpdateSpinCount"));
            yield return new WaitForSeconds(0.25f);

            if (BlackboardUtils.GetOrCreateVariable<bool>("./customData/isSuperBonus").value == true)
            {
                spinLeftAnimatorList[4].SetTrigger("Appear");
                EDMUtility.PlaySound("Spin Appear");
                BlackboardUtils.FindVariable<int>("./customData/defaultSpinCount").SetValue(5);
            }
            else
            {
                spinLeftAnimatorList[3].SetTrigger("Appear");
                EDMUtility.PlaySound("Spin Appear");
                BlackboardUtils.FindVariable<int>("./customData/defaultSpinCount").SetValue(4);
            }
            UpdateSpinLeftCount(0);
            yield return new WaitForSeconds(1.25f);
            onFinished.Invoke();
        }
        private IEnumerator BeginSpinCoroutine(Action onFinished)
        {
            foreach (var each in jackpotCellList)
            {
                BaseSymbol symbol = EDMUtility.GetSymbolFromSpotSlotMachine(bonusSlotMachine, each);
                symbol.GetComponentInChildren<EDMJackpotStopController>().OnStopSpin();
            }
            int spinCount = BlackboardUtils.FindValue<int>("./bonus/spinCountForSignBoard");
            UpdateSpinLeftCount(spinCount);
            yield return null;
            onFinished.Invoke();
        }


        private IEnumerator CollectCoroutine(List<Blackboard> collectDataList, Action onFinished)
        {
            for (int i = 0; i < collectDataList.Count; i++)
            {
                Blackboard collectData = collectDataList[i];
                yield return StartCoroutine(collectController.CollectCoroutine(collectData));
            }
            yield return new WaitForSeconds(0.5f);
            onFinished.Invoke();
        }

        private IEnumerator SaveCoroutine(List<Blackboard> saveDataList, Action onFinished)
        {
            for (int i = 0; i < saveDataList.Count; i++)
            {
                Blackboard collectData = saveDataList[i];
                yield return StartCoroutine(collectController.CollectCoroutine(collectData));
            }
            yield return new WaitForSeconds(0.5f);
            onFinished.Invoke();
        }


        private IEnumerator UpgradeCoroutine(List<Blackboard> upgradeDataList, Action onFinished)
        {
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(randomUpgradeController.UpgradeCoroutine(bonusSlotMachine, upgradeDataList));
            yield return new WaitForSeconds(0.5f);
            onFinished.Invoke();
        }

        private IEnumerator UpgradeEverySpinCoroutine(List<Blackboard> upgradeDataList, Action onFinished)
        {
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(randomUpgradeController.UpgradeEverySpinCoroutine(bonusSlotMachine, upgradeDataList));
            yield return new WaitForSeconds(0.5f);
            onFinished.Invoke();
        }


        private IEnumerator LandJackpotCoroutine(List<Blackboard> jackpotDataList, Action onFinished)
        {
            for (int i = 0; i < jackpotDataList.Count; i++)
            {
                Blackboard landData = jackpotDataList[i];
                Blackboard cell = landData.GetVariable<Blackboard>("cell").value;
                jackpotCellList.Add(EDMUtility.ConvertCellBBToVector2Int(cell));
            }
            yield return new WaitForSeconds(0.5f);
            onFinished.Invoke();
        }


        private IEnumerator OpenJackpotCoroutine(Blackboard jackpotData, Action onFinished)
        {

            Blackboard CellBB = jackpotData.GetValue<Blackboard>("cell");
            jackpotCellList.Remove(EDMUtility.ConvertCellBBToVector2Int(CellBB));
            BaseSymbol symbol = EDMUtility.GetSymbolFromSpotSlotMachine(bonusSlotMachine, EDMUtility.ConvertCellBBToVector2Int(CellBB));
            long earnCredit = jackpotData.GetValue<long>("earnCredit");
            int jackpotIndex = jackpotData.GetValue<int>("jackpotIndex");
            if (symbol.symbolInfo.customData == null) symbol.symbolInfo.customData = new Dictionary<string, object>();
            if (symbol.symbolInfo.customData.ContainsKey("value"))
                symbol.symbolInfo.customData["value"] = (object)earnCredit;
            else
                symbol.symbolInfo.customData.Add("value", (object)earnCredit);

            if (symbol.symbolInfo.customData.ContainsKey("jackpotIndex"))
                symbol.symbolInfo.customData["jackpotIndex"] = (object)jackpotIndex;
            else
                symbol.symbolInfo.customData.Add("jackpotIndex", (object)jackpotIndex);

            yield return null;
            onFinished.Invoke();
        }

        private IEnumerator ActiveKeyCoroutine(Blackboard keyData, Action onFinished)
        {
            Blackboard chestCellBB = keyData.GetValue<Blackboard>("cell");
            jackpotCellList.Remove(EDMUtility.ConvertCellBBToVector2Int(chestCellBB));
            Blackboard keyCellBB = keyData.GetValue<Blackboard>("keyCell");

            BaseSymbol chestsymbol = EDMUtility.GetSymbolFromSpotSlotMachine(bonusSlotMachine, EDMUtility.ConvertCellBBToVector2Int(chestCellBB));
            long earnCredit = keyData.GetValue<long>("earnCredit");
            int jackpotIndex = keyData.GetValue<int>("jackpotIndex");
            if (chestsymbol.symbolInfo.customData == null)
                chestsymbol.symbolInfo.customData = new Dictionary<string, object>();
            if (chestsymbol.symbolInfo.customData.ContainsKey("value"))
                chestsymbol.symbolInfo.customData["value"] = (object)earnCredit;
            else
                chestsymbol.symbolInfo.customData.Add("value", (object)earnCredit);

            if (chestsymbol.symbolInfo.customData.ContainsKey("jackpotIndex"))
                chestsymbol.symbolInfo.customData["jackpotIndex"] = (object)jackpotIndex;
            else
                chestsymbol.symbolInfo.customData.Add("jackpotIndex", (object)jackpotIndex);

            BaseSymbol keySymbol = EDMUtility.GetSymbolFromSpotSlotMachine(bonusSlotMachine, EDMUtility.ConvertCellBBToVector2Int(keyCellBB));
            keySymbol.Play("Active");
            keySymbol.GetComponentInChildren<EDMKeyBehaviour>().SetDirectionalPositionController(keySymbol.transform, chestsymbol.transform);
            yield return new WaitForSeconds(2f);
            keySymbol.symbolIndex = 11;
            keySymbol.symbolMask = (int)ContentCustomData.GetSlotData(keySymbol.slotMachine.slotIndex).symbolMask.GetMask(keySymbol.symbolIndex);

            keySymbol.Change(keySymbol.symbolInfo);
            keySymbol.Apply();
            onFinished.Invoke();
        }
    }
}