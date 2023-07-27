using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace BagelCode.GemJackpot
{
    public class GemJackpotSceneController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Animator rootAnimator;
        private Blackboard rootBB;

        private ContextElement contentsAreaElement;
        private ContextElement contentsBaseElement;
        private ContextElement jackpotGrandElement;
        private ContextElement jackpotMinorElement;
        private ContextElement jackpotMajorElement;
        private ContextElement jackpotMiniElement;

        private ContextElement buttonCloseElement;
        private ContextElement buttonSpinElement;

        private ContextElement saleTextElement;
        private ContextElement multiplyTextElement;

        private SlotMachine slotMachine;

        private Animator bulbLeft;
        private Animator bulbRight;

        private MetaSlotMachineSendEvent mainSlotMachineSendEvent;

        private NonDrawingGraphic userGemNonDrawingGraphic;

        private List<GemJackpotWinRewardObjectController> listWinRewardObjController;
        private List<ContextElement> listWinGoldElement;

        private bool isInit = false;
        private bool isTriggerSaleEvent = false;

        private long gemForSpin = 0;
        private int freeSpinCount = 0;

        private readonly int winGoldCount = 24;
        private readonly int winRewardCount = 8;
        private readonly int maxFreeSpinCount = 3;

        public float testDisableTime = 0.75f;

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();
            rootBB = gameObject.GetComponent<Blackboard>();

            GemJackpotUtils.Init();
            Blackboard gemJackpotInfo = GemJackpotUtils.GemJackpotInfo;

            // Element setting
            contentsAreaElement = ContextUtils.FindElement(rootElement, "Contents Area", ContextSearchingType.ChildrenSearch);
            contentsBaseElement = ContextUtils.FindElement(contentsAreaElement, "Base", ContextSearchingType.ChildrenSearch);

            buttonCloseElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);
            buttonSpinElement = ContextUtils.FindElement(contentsBaseElement, "Button Spin/Text", ContextSearchingType.FullNameSearch);

            jackpotGrandElement = ContextUtils.FindElement(contentsBaseElement, "Jackpot Grand/Text", ContextSearchingType.FullNameSearch);
            jackpotMinorElement = ContextUtils.FindElement(contentsBaseElement, "Jackpot Minor/Text", ContextSearchingType.FullNameSearch);
            jackpotMajorElement = ContextUtils.FindElement(contentsBaseElement, "Jackpot Major/Text", ContextSearchingType.FullNameSearch);
            jackpotMiniElement = ContextUtils.FindElement(contentsBaseElement, "Jackpot Mini/Text", ContextSearchingType.FullNameSearch);

            saleTextElement = ContextUtils.FindElement(contentsBaseElement, "Mutiply Badge Area/Text", ContextSearchingType.FullNameSearch);
            multiplyTextElement = ContextUtils.FindElement(rootElement, "Event Header Banner Area/Text", ContextSearchingType.FullNameSearch);

            ContextElement slotAreaElement = ContextUtils.FindElement(contentsBaseElement, "Slot Area", ContextSearchingType.ChildrenSearch);
            slotMachine = slotAreaElement.GetComponentInChildren<SlotMachine>();
            ContextElement slotEffectElement = ContextUtils.FindElement(slotAreaElement, "Effect", ContextSearchingType.ChildrenSearch);

            ContextElement bulbLeftEmelent = ContextUtils.FindElement(contentsBaseElement, "Bulb Left Area", ContextSearchingType.ChildrenSearch);
            bulbLeft = (bulbLeftEmelent != null) ? bulbLeftEmelent.GetComponent<Animator>() : null;
            ContextElement bulbRightEmelent = ContextUtils.FindElement(contentsBaseElement, "Bulb Right Area", ContextSearchingType.ChildrenSearch);
            bulbRight = (bulbRightEmelent != null) ? bulbRightEmelent.GetComponent<Animator>() : null;


            ContextElement buttonSpin = ContextUtils.FindElement(contentsBaseElement, "Button Spin", ContextSearchingType.ChildrenSearch);
            mainSlotMachineSendEvent = (buttonSpin != null) ? buttonSpin.GetComponent<MetaSlotMachineSendEvent>() : null;

            ContextElement userGemElement = ContextUtils.FindElement(rootElement, "User Gem", ContextSearchingType.ChildrenSearch);
            if (userGemElement != null)
                userGemNonDrawingGraphic = userGemElement.GetComponent<NonDrawingGraphic>();

            if (!GemJackpotUtils.EnabledGemDisplay) userGemElement.gameObject.SetActive(false);

            // SlotMachine set caller
            Blackboard slotMachineBB = slotMachine.GetComponent<Blackboard>();
            if (slotMachineBB != null)
                BlackboardUtils.SetOrCreateValue(slotMachineBB, "caller", this.gameObject);

            GemJackpotUtils.UpdateJackpotInfo(GemJackpotUtils.PrevGrandJackpotMultiplyNumerator);
            SetJackpotInfo();
            InitWinGoldElement();
            InitWinRewardObjController();
            InitMetaDataValues(gemJackpotInfo);

            // Button event setting (Close[x] button same NoDeal Button)
            MetaContextElementUtils.SetClickable(
                buttonCloseElement,
                () =>
                {
                    SetClickedNoDealButton(false);
                    MetaContextElementUtils.SendEvent(rootElement, "OnNoDeal", null, null);
                });

            // Set blackboard
            BlackboardUtils.SetOrCreateValue(rootBB, "metaGameEnterInfo", gemJackpotInfo);
            BlackboardUtils.SetOrCreateValue(rootBB, "_slotMachine", slotMachine);

            // temp save BB
            BlackboardUtils.SetOrCreateValue(rootBB, "_saleText", saleTextElement);
            BlackboardUtils.SetOrCreateValue(rootBB, "_multiplyText", multiplyTextElement);
            BlackboardUtils.SetOrCreateValue(rootBB, "_gemForSpin", gemForSpin);
            BlackboardUtils.SetOrCreateValue(rootBB, "_freeSpinCount", freeSpinCount);
            BlackboardUtils.SetOrCreateValue(gemJackpotInfo, "reserveClose", false);
            BlackboardUtils.SetOrCreateValue(rootBB, "_effectAnim", slotEffectElement.GetComponent<Animator>());
            SetAdsValues();

            CheckScreenScale();
            isInit = true;
        }

        private void InitMetaDataValues(Blackboard bb)
        {
            gemForSpin = GemJackpotUtils.GemForSpin;
            freeSpinCount = GemJackpotUtils.FreeSpinCount;

            BlackboardUtils.SetOrCreateValue(bb, "noDealAction", 0);
        }

        private void InitWinRewardObjController()
        {
            if (listWinRewardObjController == null)
                listWinRewardObjController = new List<GemJackpotWinRewardObjectController>();
            else
                listWinRewardObjController.Clear();

            if (contentsBaseElement != null)
            {
                for(int i = 0; i < winRewardCount; ++i)
                {
                    var obj = ContextUtils.FindElement(contentsBaseElement, string.Format("Win Reward {0:00}", i + 1), ContextSearchingType.ChildrenSearch);
                    if (obj != null)
                    {
                        GemJackpotWinRewardObjectController ctrl = obj.GetComponent<GemJackpotWinRewardObjectController>();
                        if (ctrl != null)
                        {
                            ctrl.OnInit();
                            listWinRewardObjController.Add(ctrl);
                        }
                    }
                }
            }
            BlackboardUtils.SetOrCreateValue(rootBB, "winRewardList", listWinRewardObjController);
        }

        private void InitWinGoldElement()
        {
            if (listWinGoldElement == null)
                listWinGoldElement = new List<ContextElement>();
            else
                listWinGoldElement.Clear();

            if (contentsBaseElement != null)
            {
                ContextElement areaElement = ContextUtils.FindElement(contentsBaseElement, "Win Base Area", ContextSearchingType.ChildrenSearch);
                for (int i = 0; i < winGoldCount; ++i)
                {
                    ContextElement element = ContextUtils.FindElement(areaElement, string.Format("Win Gold {0:00}", i + 1), ContextSearchingType.ChildrenSearch);
                    if (element != null)
                    {
                        listWinGoldElement.Add(element);
                    }
                }
            }
            BlackboardUtils.SetOrCreateValue(rootBB, "winGoldList", listWinGoldElement);
        }

        public void OnInit()
        {
            InitProperty();

            OnUpdateObject();
            ResetJackpotInfo();
            InitTextWinRewardObjectController();
            SetJackpotCredit(false);
        }

        public void UpdateContentData()
        {
            Blackboard gemJackpotInfo = GemJackpotUtils.GemJackpotInfo;
            MetaSlotMachineContentCustomData.Instance.InitCustomData(gemJackpotInfo);
        }

        public void OnUpdateObject()
        {
            UpdateSpinButton();
            SetJackpotInfo();
            SetActiveWinGoldObject();
            SetActiveWinRewardObjectController();
            UpdateClose();
        }

        public void OnResetData()
        {
            GemJackpotUtils.NextProgress = GemJackpotUtils.PrevProgress = 0;
        }

        public void UpdateRewardObject()
        {
            UpdateSpinButton();
        }

        public void UpdatePassiveEvent()
        {
            List<EventInfo> listEventInfo = BlackboardQueryUtils.GetActiveEventInfoList(new List<EventInfoType>(
                new EventInfoType[] { EventInfoType.GEM_JACKPOT_REWARD_MULTIPLY, EventInfoType.GEM_JACKPOT_SPIN_GEM_SALE }));
            bool isSave = false, isMutiply = false;
            if (CheckListIsNotEmpty(listEventInfo))
            {
                foreach (EventInfo eventInfo in listEventInfo)
                {
                    switch (eventInfo.type)
                    {
                        case EventInfoType.GEM_JACKPOT_REWARD_MULTIPLY:
                            if (BlackboardQueryUtils.IsShopEventPercentText(ShopType.GEM_JACKPOT))
                            {
                                // n % MORE
                                ContextUtils.SetText(multiplyTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "GEM_JACKPOT_REWARD_MULTIPLY_MAIN_TEXT_1", PassiveEventManager.Instance.GetEventInfoViewAddPercent(eventInfo)));
                            }
                            else
                            {
                                // n x WIN
                                ContextUtils.SetText(multiplyTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "GEM_JACKPOT_REWARD_MULTIPLY_MAIN_TEXT_2", PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo)));
                            }
                            isMutiply = true;
                            break;
                        case EventInfoType.GEM_JACKPOT_SPIN_GEM_SALE:
                            ContextUtils.SetText(saleTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "GEM_JACKPOT_SPIN_GEM_SALE_MAIN_TEXT", PassiveEventManager.Instance.GetEventInfoViewPercent(eventInfo)));
                            isSave = true;
                            break;
                        default:
                            break;
                    }
                }
            }

            BlackboardUtils.SetOrCreateValue(rootBB, "_isSave", isSave);
            BlackboardUtils.SetOrCreateValue(rootBB, "_isMultiply", isMutiply);
        }

        private void UpdateSpinButton()
        {
            InitMetaDataValues(GemJackpotUtils.GemJackpotInfo);

            if (freeSpinCount > 0)
                ContextUtils.SetText(buttonSpinElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "GEM_JACKPOT_USE_FREE_BUTTON_TEXT", freeSpinCount) + (freeSpinCount > 1 ? "S" : ""));
            else
                ContextUtils.SetText(buttonSpinElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "GEM_JACKPOT_USE_GEM_BUTTON_TEXT", (rootAnimator.GetBool("isSave") && isTriggerSaleEvent) ? GemJackpotUtils.GemForSpinSale : GemJackpotUtils.GemForSpin));
        }

        public void SetOrientationBB(GameObject target, Orientation orientation)
        {
            Blackboard bb = target.GetComponent<Blackboard>();
            BlackboardQueryUtils.SetOrientationBB(bb, "orientation", orientation);
        }

        private void SetJackpotInfo()
        {
            BlackboardUtils.SetOrCreateValue(rootBB, "jackpotInfo", GemJackpotUtils.JackpotInfo);
        }

        private void SetJackpotCredit(bool isMultiply)
        {
            List<Blackboard> jackpotReelRewardList = GemJackpotUtils.JackpotReelRewardList;
            
            if (CheckListIsNotEmpty(jackpotReelRewardList))
            {
                long eventRewardMultiply = GemJackpotUtils.EventRewardMultiply;
                foreach(Blackboard bb in jackpotReelRewardList)
                {
                    GemJackpotJackpotSymbolType type = BlackboardUtils.FindValue<GemJackpotJackpotSymbolType>(bb, "type");
                    long credit = BlackboardUtils.FindValue<long>(bb, "credit");
                    credit = isMultiply ? NumberUtils.GetMultiplierNumeratorValue(BlackboardUtils.FindValue<long>(bb, "credit"), eventRewardMultiply) : credit;

                    switch (type)
                    {
                        case GemJackpotJackpotSymbolType.MINI:
                            ContextUtils.SetText(jackpotMiniElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_CREDIT", credit));
                            break;
                        case GemJackpotJackpotSymbolType.MINOR:
                            ContextUtils.SetText(jackpotMinorElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_CREDIT", credit));
                            break;
                        case GemJackpotJackpotSymbolType.MAJOR:
                            ContextUtils.SetText(jackpotMajorElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_CREDIT", credit));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void SetActiveWinGoldObject()
        {
            int nextProgress = GetNextProgress();

            if (CheckListIsNotEmpty(listWinGoldElement))
            {
                for (int i = 0; i < listWinGoldElement.Count; ++i)
                {
                    listWinGoldElement[i].gameObject.SetActive(i < nextProgress);
                }
            }
        }

        private void SetActiveWinRewardObjectController()
        {
            int nextProgress = GetNextProgress();

            if (CheckListIsNotEmpty(listWinRewardObjController))
            {
                for (int i = GemJackpotUtils.PrevProgress / 3; i < listWinRewardObjController.Count; ++i)
                {
                    int checkValue = i * 3;
                    listWinRewardObjController[i].SetWinRewardActive(checkValue < nextProgress && checkValue + 3 >= nextProgress);
                }
            }
        }

        private void SetAdsValues()
        {
            Variable<bool> inhouseAdsEnabled = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/INHOUSE_ADS_ENABLED");
            Variable<bool> videoAdsEnabled = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/VIDEO_ADS_ENABLED");

            if (inhouseAdsEnabled.value)
            {
                if (IAMRouter.Instance.CheckTriggerIAM(InAppMessageTriggerType.INHOUSE_ADS_FOR_GEM_JACKPOT))
                    rootBB.SetValue("placementKey", "");
            }
            else if (videoAdsEnabled.value)
            {
                string placement = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "videoAdsPlacementNames/gemJackpot").value;
                if (!string.IsNullOrEmpty(placement) && VideoAdsController.Instance.IsVideoAdsAvailable(placement))
                    rootBB.SetValue("placementKey", placement);
            }
            else
                rootBB.SetValue("placementKey", "");

            BlackboardUtils.SetOrCreateValue(rootBB, "_inhouseAds", inhouseAdsEnabled.value);
            BlackboardUtils.SetOrCreateValue(rootBB, "_videoAds", videoAdsEnabled.value);
            BlackboardUtils.SetOrCreateValue(rootBB, "_isShowAds", GemJackpotUtils.IsShowCloseAds);
        }

        private void InitTextWinRewardObjectController()
        {
            List<long> slotReelRewardList = GemJackpotUtils.SlotReelRewardList;

            if (CheckListIsNotEmpty(listWinRewardObjController) && CheckListIsNotEmpty(slotReelRewardList))
            {
                long eventRewardMultiply = GemJackpotUtils.EventRewardMultiply;
                for (int i = 0; i < listWinRewardObjController.Count; ++i)
                {
                    long reward = (slotReelRewardList[(i * 3) + 1] + slotReelRewardList[(i * 3) + 1] + slotReelRewardList[(i * 3) + 1]) / 3;
                    listWinRewardObjController[i].SetMultiplyCredit(NumberUtils.GetMultiplierNumeratorValue(reward, eventRewardMultiply));
                    listWinRewardObjController[i].SetWinRewardText(reward);
                }
            }
        }

        public void SetBuldSpinAnimation(bool isActive)
        {
            if (bulbLeft != null)   bulbLeft.SetBool("isSpin", isActive);
            if (bulbRight != null)  bulbRight.SetBool("isSpin", isActive);
        }

        public void SetBuldWinAnimation(bool isActive)
        {
            if (bulbLeft != null) bulbLeft.SetBool("isWin", isActive);
            if (bulbRight != null) bulbRight.SetBool("isWin", isActive);
        }

        private bool CheckListIsNotEmpty<T>(List<T> list)
        {
            bool isNotEmpty = false;
            if (list != null && list.Count > 0)
                isNotEmpty = true;
            return isNotEmpty;
        }

        private void CheckScreenScale()
        {
            //GemJackpotUtils.CheckScreenScale(contentsAreaElement, Vector3.one);
        }

        public void SetBonusBlackboard(bool isCreate)
        {
            Blackboard bb = GemJackpotUtils.GemJackpotInfo;
            if (isCreate)
            {
                IBlackboard bonusBB = BlackboardUtils.GetOrCreateBlackboard(bb, "bonus");
                List<int> jackpotReelSetResultIndexList = new List<int>();
                int jackpotIndex = BlackboardUtils.FindValue<int>(bb, "jackpotInfo/index");
                jackpotReelSetResultIndexList.Add(jackpotIndex);
                List<int> jackpotReelSetList = BlackboardUtils.FindValue<List<int>>(bb, "jackpotReelSetList");

                long credit = GemJackpotUtils.GetRewardCredit();

                BlackboardUtils.SetOrCreateValue(bonusBB, "jackpotReelSetResultIndexList", jackpotReelSetResultIndexList);
                BlackboardUtils.SetOrCreateValue(bonusBB, "credit", credit);
                BlackboardUtils.SetOrCreateValue(bonusBB, "type", (jackpotIndex < 0) ? GemJackpotJackpotSymbolType.NONE : (GemJackpotJackpotSymbolType)jackpotReelSetList[jackpotIndex]);
            }
            else
            {
                BlackboardUtils.DestroyBlackboard(bb, "bonus");
            }
        }

        public void SetCloseBonus(GameObject obj)
        {
            // Bonus jackpot scene close (obj : Gem Jackpot Bonus Scene)
            if (obj == null) return;
            GemJackpotBonusSceneController controller = obj.GetComponent<GemJackpotBonusSceneController>();
            if (controller == null) return;

            controller.OnOutro();
        }

        public string CreateBonusResultPopup()
        {
            // Bonus jackpot result popup
            string name = "Popup Gem Jackpot Win Scene";
            Blackboard bb = GemJackpotUtils.GemJackpotInfo;
            if (bb != null)
            {
                Blackboard bonusBB = BlackboardUtils.FindValue<Blackboard>(bb, "bonus");
                if (bonusBB != null)
                {
                    GemJackpotJackpotSymbolType type = BlackboardUtils.FindValue<GemJackpotJackpotSymbolType>(bonusBB, "type");
                    
                    switch (type)
                    {
                        case GemJackpotJackpotSymbolType.MINI:
                            name = "Popup Gem Jackpot Win Mini Jackpot Scene";
                            break;
                        case GemJackpotJackpotSymbolType.MINOR:
                            name = "Popup Gem Jackpot Win Minor Jackpot Scene";
                            break;
                        case GemJackpotJackpotSymbolType.MAJOR:
                            name = "Popup Gem Jackpot Win Major Jackpot Scene";
                            break;
                        case GemJackpotJackpotSymbolType.GRAND:
                            name = "Popup Gem Jackpot Win Grand Jackpot Scene";
                            break;
                    }
                }
            }

            return name;
        }

        public int GetNextProgress()
        {
            return GemJackpotUtils.NextProgress;
        }

        public void SetPressButtonPopupNull(string key)
        {
            BlackboardUtils.SetOrCreateValue<GameObject>(rootBB, key, null);
        }

        public void SendMainSlotSpinEvent(string eventName)
        {
            if (mainSlotMachineSendEvent != null)
                mainSlotMachineSendEvent.DispatchMetaSlotMachineSpinButtonEvent(eventName);
        }

        private void UpdateClose()
        {
            bool isClose = BlackboardUtils.FindValue<bool>(GemJackpotUtils.GemJackpotInfo, "reserveClose");
            if (isClose)
                EventSender.SendGlobalEvent("OnClose");
        }

        public void UpdatePassiveSaleEvent()
        {
            if (BlackboardUtils.FindValue<bool>(rootBB, "_isSave") && GemJackpotUtils.FreeSpinCount == 0 && rootAnimator.GetBool("isSave") == false)
                rootAnimator.SetBool("isSave", true);
        }

        public void reserveClose()
        {
            BlackboardUtils.SetOrCreateValue(GemJackpotUtils.GemJackpotInfo, "reserveClose", true);
        }

        public void SetClickedNoDealButton(bool isNoDealButton)
        {
            // AE [client_click_gem_jackpot_cloase] check value
            GemJackpotUtils.IsClickedNoDealButton = isNoDealButton;
        }

        private void ResetJackpotInfo()
        {
            var jackpotInfoResultBB = BlackboardUtils.GetOrCreateBlackboard(GemJackpotUtils.GemJackpotInfo, "jackpotInfo");
            BlackboardUtils.SetOrCreateValue<int>(jackpotInfoResultBB, "index", -1);
            BlackboardUtils.SetOrCreateValue<long>(jackpotInfoResultBB, "credit", 0L);
        }

        public void SetUserGemRaycast(bool isActive)
        {
            if (userGemNonDrawingGraphic != null)
                userGemNonDrawingGraphic.raycastTarget = isActive;
        }
        // Animator event
        private void SetChangeAnimationEvent(int index)
        {
            if (CheckListIsNotEmpty(listWinRewardObjController))
                listWinRewardObjController[index - 1].SetChangeAnimationEvent();
            if (index == 1) GSManager.Instance.GetHandler("Meta_Gemjackpot_Multiply").Play();
        }
        // Animator event : Gem Jackpot Main Passive Save.anim
        private void SetChangeSpinButtonSaleEvent()
        {
            // Spin Sale Event
            isTriggerSaleEvent = true;
            ContextUtils.SetText(buttonSpinElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "GEM_JACKPOT_USE_GEM_BUTTON_TEXT", GemJackpotUtils.GemForSpinSale));
        }

        private void SetChangeJackpotCreditEvent()
        {
            SetJackpotCredit(true);
        }

        private void SetChangeGrandCreditEvent()
        {
            Blackboard jackpotInfo = GemJackpotUtils.JackpotInfo;
            //BlackboardQueryUtils.InitJackpotInfo(jackpotInfo);

            var jackpotChase = rootElement.GetComponent<ChaseTypeLong>();
            jackpotChase.SetNonstopChase
            (
                jackpotChase.textElement,
                jackpotInfo.GetValue<long>("prev"),
                jackpotInfo.GetValue<long>("current"),
                jackpotInfo.GetValue<int>("deltaMs"),
                "",
                StringTable.StringTableType.Global,
                false,
                GemJackpotUtils.EventRewardMultiply,
                jackpotInfo.GetVariable<long>("progress")
            );
        }

        public void SetShowCloseAdsValue(bool isAds)
        {
            GemJackpotUtils.IsShowCloseAds = isAds;
            if (isAds == false)
                GemJackpotUtils.LastAdsTimestamp = GemJackpotUtils.LastVideoAdsClaimTimestamp;
        }

        public void StartSpin()
        {
            GemJackpotUtils.StartSpin();
            UpdateSpinButton();
            UpdatePassiveSaleEvent();
        }

        public void CheckActiveIAM()
        {
            if (PopupManager.Instance.popupCount > 0)
            {
                Popup popup = PopupManager.Instance.stack[PopupManager.Instance.stack.Count - 1];
                if (popup == null)
                    return;
                Blackboard iamBB = popup.GetComponent<Blackboard>();
                iamBB.SetValue("isNotVIPLounge", true);
            }
        }
    }
}
