using System.Collections;
using System.Collections.Generic;
using GameStudio.Slot.EDM.Utility;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.EDM.Feature
{

    public class EDMBaseGameRandomUpgradeAdmin : FeatureController
    {
        protected override string ON_FEATURE_BEGIN_EVENT => "EDM_BEGIN_RANDOMUPGRADE";
        protected override string ON_FEATURE_END_EVENT => "EDM_END_RANDOMUPGRADE";

        public SlotMachine bonusReelSlotMachine;
        public Animator characterAnimator;
        public ObjectPool dynamitePool;
        public Transform dynamiteFlyingParent;
        public Transform dynamiteInitialParent;

        protected override IEnumerator OnPlayCoroutine()
        {
            yield return null;
            Blackboard bonusResponse = BlackboardUtils.FindVariable<Blackboard>("./bonus/response")?.value;

            List<long> newCreditValueList = bonusResponse.GetVariable<List<long>>("newCreditValueList").value;
            List<int> newSymbolIndexList = bonusResponse.GetVariable<List<int>>("newSymbolIndexList").value;
            List<Blackboard> upgradedCreditSymbolCellList = bonusResponse.GetVariable<List<Blackboard>>("upgradedCreditSymbolCellList")?.value;
            List<Blackboard> upgradedJackpotSymbolCellList = bonusResponse.GetVariable<List<Blackboard>>("upgradedJackpotSymbolCellList")?.value;

            List<EDMBaseGameRandomUpgradeFlyData> flyDataList = new List<EDMBaseGameRandomUpgradeFlyData>();
            List<Blackboard> cellList = new List<Blackboard>();
            int jpIndex = 0;
            int creditIndex = 0;
            for (int i = 0; i < upgradedCreditSymbolCellList.Count + upgradedJackpotSymbolCellList.Count; i++)
            {
                if (UnityEngine.Random.Range(0, 100) > 50)
                {
                    Blackboard cellBB = null;
                    if (upgradedCreditSymbolCellList.Count <= creditIndex)
                    {
                        cellBB = upgradedJackpotSymbolCellList[jpIndex];
                        jpIndex++;
                    }
                    else
                    {
                        cellBB = upgradedCreditSymbolCellList[creditIndex];
                        creditIndex++;
                    }


                    Vector2Int cell = EDMUtility.ConvertCellBBToVector2Int(cellBB);
                    BaseSymbol symbol = EDMUtility.GetSymbolFromSpotSlotMachine(bonusReelSlotMachine, cell);

                    flyDataList.Add(new EDMBaseGameRandomUpgradeFlyData()
                    {
                        targetCell = cell,
                        type = symbol.symbolIndex == 12 ? EDMUpgradeCellType.CREDIT : EDMUpgradeCellType.JACKPOT,
                        targetCreditUpgradeAmount = symbol.symbolIndex == 12 ? newCreditValueList[creditIndex - 1] : 0,
                    });

                }
                else
                {
                    Blackboard cellBB = null;
                    if (upgradedJackpotSymbolCellList.Count <= jpIndex)
                    {
                        cellBB = upgradedCreditSymbolCellList[creditIndex];
                        creditIndex++;
                    }
                    else
                    {
                        cellBB = upgradedJackpotSymbolCellList[jpIndex];
                        jpIndex++;
                    }


                    Vector2Int cell = EDMUtility.ConvertCellBBToVector2Int(cellBB);
                    BaseSymbol symbol = EDMUtility.GetSymbolFromSpotSlotMachine(bonusReelSlotMachine, cell);

                    flyDataList.Add(new EDMBaseGameRandomUpgradeFlyData()
                    {
                        targetCell = cell,
                        type = symbol.symbolIndex == 12 ? EDMUpgradeCellType.CREDIT : EDMUpgradeCellType.JACKPOT,
                        targetCreditUpgradeAmount = symbol.symbolIndex == 12 ? newCreditValueList[creditIndex - 1] : 0,
                    });

                }
            }


            ContentEvent.SendEvent<bool>("EDM_RAMDOM_UPGRADE_CHARACTER", true);
            int randomNumber = UnityEngine.Random.Range(0, 2);
            EDMUtility.PlaySound($"Dwarf Appear {randomNumber + 1}");


            bool relaod = false;
            void OnReloadDynamite(EventData eventData)
            {
                relaod = true;
            }
            RegisterEvent("EDM_RELOAD_DYNAMITE", OnReloadDynamite);
            yield return new WaitUntil(() => relaod == true);
            UnRegisterEvent("EDM_RELOAD_DYNAMITE");
            GameObject dynamite = dynamitePool.GetObject().gameObject;
            dynamite.transform.SetParent(dynamiteInitialParent);
            dynamite.transform.localPosition = Vector3.zero;
            dynamite.transform.localRotation = Quaternion.identity;

            yield return new WaitForSeconds(0.75f);

            for (int i = 0; i < flyDataList.Count; i++)
            {
                characterAnimator.SetTrigger("Shot");


                bool shotTrigger = false;
                void OnShotDynamite(EventData eventData)
                {
                    shotTrigger = true;
                }

                RegisterEvent("EDM_SHOT_DYNAMITE", OnShotDynamite);
                yield return new WaitUntil(() => shotTrigger == true);
                if (i == 0) EDMUtility.PlaySound("Dynamite Throw 1");
                else
                {
                    EDMUtility.PlaySound($"Dynamite Throw {UnityEngine.Random.Range(2, 4)}");
                }
                UnRegisterEvent("EDM_SHOT_DYNAMITE");
                dynamite.transform.SetParent(dynamiteFlyingParent, true);

                EDMBaseGameRandomUpgradeFly fly = dynamite.GetComponent<EDMBaseGameRandomUpgradeFly>();
                fly.Initialize(bonusReelSlotMachine, flyDataList[i]);
                dynamite.GetComponent<Animator>().SetTrigger("Fly");
                characterAnimator.SetBool("Reload", i + 1 < flyDataList.Count);

                if (i + 1 < flyDataList.Count)
                {
                    relaod = false;

                    RegisterEvent("EDM_RELOAD_DYNAMITE", OnReloadDynamite);
                    yield return new WaitUntil(() => relaod == true);
                    UnRegisterEvent("EDM_RELOAD_DYNAMITE");

                    dynamite = dynamitePool.GetObject().gameObject;
                    dynamite.transform.SetParent(dynamiteInitialParent);
                    dynamite.transform.localPosition = Vector3.zero;
                    dynamite.transform.localRotation = Quaternion.identity;
                }
                yield return new WaitUntil(() => dynamiteFlyingParent.childCount == 0);
                yield return new WaitForSeconds(0.5f);
            }

            ContentEvent.SendEvent<bool>("EDM_RAMDOM_UPGRADE_CHARACTER", false);
            EDMUtility.PlaySound($"Dwarf Disappear");
            yield return new WaitForSeconds(1f);
        }
    }
}