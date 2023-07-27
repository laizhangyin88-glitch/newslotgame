using System.Collections;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using BagelCode.OSA_Scroll;
using System.Collections.Generic;
using BagelCode.ClientModels;

namespace BagelCode.VipLounge
{
    public class PopupVipLoungeJackpotController : MonoBehaviour
    {
        private ContextElement root;
        private Animator anim;
        private Blackboard bb;
        private Blackboard mainBB;
        private Blackboard loungeJackpotInfoBB;

        private Animator outlineAnimator = null;
        private ContextElement textVipPointElement = null;
        private ContextElement miniScoreTextElement = null;
        private ContextElement minorScoreTextElement = null;
        private ContextElement majorScoreTextElement = null;
        private ContextElement grandScoreTextElement = null;

        private OSA_VipLoungeJackpot osaController;

        private long baseWinCredit = 0L;
        private bool isSpin = false;
        private long usedExtraVLP = 0L;
        private long collectWinCredit = 0L;

        public void OnInit()
        {
            bb = GetComponent<Blackboard>();
            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();

            root.UpdateContext();

            mainBB = bb.GetValue<Blackboard>("mainBB");

            InitProperty();

            osaController?.SetMainBlackboard(mainBB);

            GSManager.Instance.GetHandler("UI_Coin_Riser_Start").Play();
            anim.SetBool("Active", true);
        }

        private void InitProperty()
        {
            ContextElement osaScrollViewElement = ContextUtils.FindElement(root, "Scroll View", ContextSearchingType.ChildrenSearch);
            osaController = osaScrollViewElement?.GetComponent<OSA_VipLoungeJackpot>();

            ContextElement outlineElement = ContextUtils.FindElement(root, "Outline", ContextSearchingType.ChildrenSearch);
            outlineAnimator = outlineElement.GetComponent<Animator>();
            ContextElement spinButtonElement = ContextUtils.FindElement(root, "Button Spin", ContextSearchingType.ChildrenSearch);
            BlackboardUtils.SetOrCreateValue(bb, "_buttonElement", spinButtonElement);
            //MetaContextElementUtils.SetClickable(spinButtonElement,
            //    () => EventSender.SendEvent(gameObject, "OnClickButtonSpin"));    // Code -> FSM
            // Lounge Point Setting
            ContextElement loungePointElement = ContextUtils.FindElement(root, "Lounge Points", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(loungePointElement, "Text Desc", "VIP_LOUNGE_POPUP_JACKPOT_DESC", ContextSearchingType.ChildrenSearch);
            textVipPointElement = ContextUtils.FindElement(loungePointElement, "Text VIP Point", ContextSearchingType.ChildrenSearch);
            usedExtraVLP = mainBB.GetVariable<long>("usedExcessLoungePoint")?.value ?? 0L;
            MetaContextElementUtils.SetTextGlobal(textVipPointElement, "TEXT_COMMA_NUMBER", usedExtraVLP);
            // Info Area Setting
            loungeJackpotInfoBB = BlackboardUtils.FindVariable<Blackboard>(mainBB, "loungeJackpotInfo").value;
            baseWinCredit = VipLounge.Utils.LoungeJackpotBaseWinCredit(mainBB);
            List<Blackboard> jackpotTableList = VipLounge.Utils.LoungeJackpotTableList(mainBB);

            ContextElement infoAreaElement = ContextUtils.FindElement(root, "Info Area", ContextSearchingType.ChildrenSearch);
            ContextElement majorElement = ContextUtils.FindElement(infoAreaElement, "Base Major", ContextSearchingType.ChildrenSearch);
            ContextElement minorElement = ContextUtils.FindElement(infoAreaElement, "Base Minor", ContextSearchingType.ChildrenSearch);
            ContextElement miniElement = ContextUtils.FindElement(infoAreaElement, "Base Mini", ContextSearchingType.ChildrenSearch);
            ContextElement grandJackpotElement = ContextUtils.FindElement(infoAreaElement, "Grand Jackpot Area", ContextSearchingType.ChildrenSearch);

            miniScoreTextElement = ContextUtils.FindElement(miniElement, "Text Score", ContextSearchingType.ChildrenSearch);
            minorScoreTextElement = ContextUtils.FindElement(minorElement, "Text Score", ContextSearchingType.ChildrenSearch);
            majorScoreTextElement = ContextUtils.FindElement(majorElement, "Text Score", ContextSearchingType.ChildrenSearch);
            grandScoreTextElement = ContextUtils.FindElement(grandJackpotElement, "Text Score", ContextSearchingType.ChildrenSearch);

            for (int i = 0; i < jackpotTableList.Count; ++i)
            {
                long jackpotCredit = NumberUtils.GetMultiplierNumeratorValue(baseWinCredit, jackpotTableList[i].GetValue<long>("minMultiplierNumerator"));
                long minCredit = jackpotTableList[i].GetValue<long>("minValue");
                switch (jackpotTableList[i].GetValue<LoungeJackpotWinType>("winType"))
                {
                    case LoungeJackpotWinType.MINI:
                        MetaContextElementUtils.SetTextGlobal(miniScoreTextElement, "TEXT_COMMA_NUMBER", GetCredit(false, jackpotCredit, minCredit));
                        break;
                    case LoungeJackpotWinType.MINOR:
                        MetaContextElementUtils.SetTextGlobal(minorScoreTextElement, "TEXT_COMMA_NUMBER", GetCredit(false, jackpotCredit, minCredit));
                        break;
                    case LoungeJackpotWinType.MAJOR:
                        MetaContextElementUtils.SetTextGlobal(majorScoreTextElement, "TEXT_COMMA_NUMBER", GetCredit(false, jackpotCredit, minCredit));
                        break;
                }
            }

            Blackboard jackpotInfoBB = BlackboardUtils.FindVariable<Blackboard>(mainBB, "grandJackpotInfo").value;
            ChaseTypeLong jackpotChase = grandScoreTextElement.GetComponent<ChaseTypeLong>();
            if (jackpotChase == null)
                jackpotChase = grandScoreTextElement.gameObject.AddComponent<ChaseTypeLong>();

            jackpotChase.SetNonstopChase(
                grandScoreTextElement,
                jackpotInfoBB.GetValue<long>("prev"),
                jackpotInfoBB.GetValue<long>("current"),
                jackpotInfoBB.GetValue<int>("deltaMs"),
                null,
                StringTable.StringTableType.Global,
                false,
                NumberUtils.GetGlobalDenominator(),
                BlackboardUtils.GetOrCreateVariable<long>(jackpotInfoBB, "progress"));
        }

        private long GetCredit(bool isMax, long baseCredit, long minCredit)
        {
            if (isMax)
                return System.Math.Max(baseCredit, minCredit);
            else
            {
                if (baseCredit > minCredit)
                    return minCredit;
                else
                    return 0L;
            }
        }

        public void OnClickSpin()
        {
            if (isSpin)
                return;
            int wheelIndex = mainBB.GetVariable<int>("wheelIndex")?.value ?? 0;
#if DEV
            // todo : Test case only - Lounge Jackpot Open
            if (mainBB.GetVariable<bool>("_isTest")?.value ?? false)
            {
                Variable<LoungeJackpotWinType> winType = BlackboardUtils.FindVariable<LoungeJackpotWinType>(MainBlackboard.Get(), VipLounge.Defines.LOUNGE_JACKPOT_DEBUG_SPIN);
                if (winType == null)
                {
                    winType = BlackboardUtils.GetOrCreateVariable<LoungeJackpotWinType>(MainBlackboard.Get(), VipLounge.Defines.LOUNGE_JACKPOT_DEBUG_SPIN);
                    winType.value = LoungeJackpotWinType.UNKNOWN;
                }
                List<Blackboard> wheelPreset = VipLounge.Utils.LoungeJackpotWheelPreset(mainBB);
                if (winType.value != LoungeJackpotWinType.UNKNOWN && wheelPreset != null)
                {
                    for (int i = 0; i < wheelPreset.Count; ++i)
                    {
                        if (wheelPreset[i].GetValue<LoungeJackpotWinType>("winType") == winType.value)
                        {
                            wheelIndex = i;
                            break;
                        }
                    }
                }
                else
                    wheelIndex = Random.Range(0, 23);
                winType.value = LoungeJackpotWinType.UNKNOWN;
                BlackboardUtils.SetOrCreateValue(mainBB, "wheelIndex", wheelIndex);
            }
#endif
            osaController?.Simulation(wheelIndex, 10.0f, 5, CallEndSpin, CallDurationSpin, CallSpinSuccess);
            anim?.SetTrigger("Spin");
            outlineAnimator?.SetTrigger("isSpin");
            isSpin = true;
        }

        private void UpdateInfoCredit(float animationTime)
        {
            List<Blackboard> jackpotTableList = VipLounge.Utils.LoungeJackpotTableList(mainBB);
            for (int i = 0; i < jackpotTableList.Count; ++i)
            {
                long jackpotCredit = NumberUtils.GetMultiplierNumeratorValue(baseWinCredit, jackpotTableList[i].GetValue<long>("minMultiplierNumerator"));
                jackpotCredit = VipLounge.Utils.GetLoungeJackpotCreditFloor(jackpotCredit);
                long minCredit = VipLounge.Utils.GetLoungeJackpotCreditFloor(jackpotTableList[i].GetValue<long>("minValue"));
                switch (jackpotTableList[i].GetValue<LoungeJackpotWinType>("winType"))
                {
                    case LoungeJackpotWinType.MINI:
                        GetOrCreateJackpotCellCredit(miniScoreTextElement)?.SetJackpotCredit(
                            GetCredit(false, jackpotCredit, minCredit),
                            GetCredit(true, jackpotCredit, minCredit),
                            animationTime);
                        break;
                    case LoungeJackpotWinType.MINOR:
                        GetOrCreateJackpotCellCredit(minorScoreTextElement)?.SetJackpotCredit(
                            GetCredit(false, jackpotCredit, minCredit),
                            GetCredit(true, jackpotCredit, minCredit),
                            animationTime);
                        break;
                    case LoungeJackpotWinType.MAJOR:
                        GetOrCreateJackpotCellCredit(majorScoreTextElement)?.SetJackpotCredit(
                            GetCredit(false, jackpotCredit, minCredit),
                            GetCredit(true, jackpotCredit, minCredit),
                            animationTime);
                        break;
                }
            }
        }

        private PopupVipLoungeJackpotCellCreditController GetOrCreateJackpotCellCredit(ContextElement context)
        {
            if (context == null)
                return null;

            PopupVipLoungeJackpotCellCreditController contentJackpotCredit = context.GetComponent<PopupVipLoungeJackpotCellCreditController>();
            if (contentJackpotCredit == null)
                contentJackpotCredit = context.gameObject.AddComponent<PopupVipLoungeJackpotCellCreditController>();

            return contentJackpotCredit;
        }

        private MetaIncreaseNumber GetOrCreateContextJackpotCredit(ContextElement context)
        {
            if (context == null)
                return null;

            MetaIncreaseNumber contentJackpotCredit = context.GetComponent<MetaIncreaseNumber>();
            if (contentJackpotCredit == null)
                contentJackpotCredit = context.gameObject.AddComponent<MetaIncreaseNumber>();

            return contentJackpotCredit;
        }

        public void AppearAnimationExtraPoint()
        {
            if (textVipPointElement == null)
                return;

            float animationTime = 1.0f;
            GetOrCreateContextJackpotCredit(textVipPointElement)?.Reset(textVipPointElement as IContextText, "", usedExtraVLP, 0L, animationTime, 1, false);
            UpdateInfoCredit(animationTime);
            osaController?.AppearAnimationExtraPoint(animationTime);
        }

        public void DisappearAnimation()
        {
            SendCallback();
        }

        private void CallDurationSpin()
        {
            anim?.SetTrigger("SpinDuration");
        }

        private void CallEndSpin()
        {
            outlineAnimator?.SetBool("isEnd", true);
        }

        private void CallSpinSuccess()
        {
            EventSender.SendEvent(gameObject, "OnSpinEnd");
        }

        public void OpenResultPopup()
        {
            int wheelIndex = BlackboardUtils.FindVariable<int>(mainBB, "wheelIndex").value;
            List<Blackboard> wheelPresetList = VipLounge.Utils.LoungeJackpotWheelPreset(mainBB);
            LoungeJackpotWinType winType = (wheelPresetList != null && wheelIndex < wheelPresetList.Count) ? wheelPresetList[wheelIndex].GetValue<LoungeJackpotWinType>("winType") : LoungeJackpotWinType.UNKNOWN;
            StartCoroutine(OpenResultPopupCoroutine(winType));
        }

        private IEnumerator OpenResultPopupCoroutine(LoungeJackpotWinType winType)
        {
            string lobbyBundle = VipLounge.Defines.CONTENTS_BUNDLE;
            string assetName = winType == LoungeJackpotWinType.CREDIT ? "Popup VIP Lounge Jackpot Reward Scene" : "Popup VIP Lounge Grand Jackpot Trigger Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject popupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(lobbyBundle, assetName, parent,
                (SceneLoadOperation sceneLoadOperation) => popupObj = sceneLoadOperation.GetScene()));

            Blackboard popupBB = popupObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(popupBB, "winCredit", collectWinCredit);
            BlackboardUtils.SetOrCreateValue(popupBB, "winType", winType);
            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);

            popupObj.GetComponent<PopupVipLoungeJackpotResultBase>()?.OnInit();
            GSManager.Instance.GetAudioMixerSnapshot("Content_Popup")?.TransitionTo(0);
            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
            MetaPopupUtils.ClosePopup(popupObj);
            GSManager.Instance.GetAudioMixerSnapshot("Lobby_Main")?.TransitionTo(0);
        }

        public IEnumerator RequestVipLoungeJackpotCollect()
        {
            bool success = false;
            bool fail = false;
#if DEV
            // todo : Test case only - Lounge Jackpot Open
            if (mainBB.GetVariable<bool>("_isTest")?.value ?? false)
            {
                collectWinCredit = 10000L;
                yield break;
            }
#endif
            BagelCodeClientAPI.RequestVipLoungeJackpotCollect(
                (response) =>
                {
                    success = true;
                    collectWinCredit = response.winCredit;

                    if (response.userSyncInfo != null)
                    {
                        BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                        BlackboardQueryUtils.ApplyUserSyncInfo();
                    }
                },
                (error) =>
                {
                    fail = true;
                    GlobalErrorHandler.GlobalError(error);
                });
            yield return new WaitUntil(() => (success || fail));
        }

        public void SendCallback()
        {
            EventSender.SendCalleeCallback(gameObject);
        }
    }
}