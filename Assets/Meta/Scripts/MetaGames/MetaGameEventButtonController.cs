using System.Collections;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using ParadoxNotion;

namespace BagelCode
{
    public class MetaGameEventButtonController : EventMonoBehaviour
    {
        private const float BET_UP_BALLOON_DELAY = 1f;
        private const float BET_UP_BALLOON_DISPLAY_TIME = 2.5f;

        protected ContextElement root;
        protected Blackboard bb;

        protected ContextElement betUpBalloonAnimElement;

        protected GameObject earnEffectObj = null;

        protected bool isItemEarning = false;
        protected bool isInit = false;
        protected bool ignoreSpeechBalloon = false;

        protected Transform earningItemRootArea; // todo:user MetaGameAppearTransformManager instead 

        protected ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        public virtual void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            ignoreSpeechBalloon = bb.GetVariable<bool>("ignoreSpeechBalloon")?.value ?? false;

            root.UpdateContext(false);

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);

            Register(MetaEventDefine.ON_EARN_META_GAME_ITEM, OnEarnMetaGameItem);
            Register(MetaEventDefine.UPDATE_META_GAME_ITEM_EARNING_INSTANTLY, UpdateMetaItemInfo);

            var eventData = new EventData<GameObject>("OnClickInGameMetaGameIcon", gameObject);
            MetaContextElementUtils.SetClickableMetaUIEvent(GetButtonClickable(), eventData, true, null);

            if (BlackboardQueryUtils.IsIngame())
                earningItemRootArea = MetaGameAppearTransformManager.Instance.GetTransform("EarningItemRoot");

            // Bet Up Balloon
            bool isInGame = BlackboardQueryUtils.IsIngame();
            if (isInGame)
            {
                betUpBalloonAnimElement = ContextUtils.FindElement(root, "Bet Up Speech Balloon", CHILDREN);
                if (betUpBalloonAnimElement != null)
                {
                    string eventName = "";

                    var metaGameTypeVar = BlackboardUtils.GetOrCreateVariable<MetaGameType>(bb, "metaGameType");
                    if (string.IsNullOrEmpty(eventName) && metaGameTypeVar.value != MetaGameType.NONE)
                    {
                        eventName = BlackboardQueryUtils.GetMetaGameEventName(metaGameTypeVar.value);
                    }

                    if (string.IsNullOrEmpty(eventName))
                    {
                        eventName = BlackboardQueryUtils.GetMetaGameEventName();
                    }

                    if (string.IsNullOrEmpty(eventName))
                    {
                        Debug.LogError("MetaGameEventButtonController eventName parse error.");
                    }
                    else
                    {
                        // Events
                        MetaContextElementUtils.SimpleSetTextGlobal(betUpBalloonAnimElement, "Text", "META_GAME_BET_UP_BALLOON_TEXT", CHILDREN, eventName);
                        Register(MetaEventDefine.SHOW_BET_UP_BALLOON, () => StartCoroutine(ShowBetUpBalloonTextCoroutine()));
                    }
                }
            }
        }

        protected virtual ContextElement GetButtonClickable()
        {
            return root;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (isItemEarning)
            {
                isItemEarning = false;

                OnArriveEarningMetaGameItem();
            }
        }

        protected virtual void OnArriveEarningMetaGameItem()
        {
            isItemEarning = false;

            earnEffectObj = null;

            UpdateMetaItemInfo();

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_FINISH_GETTING_META_GAME_ITEM);
        }

        protected virtual void OnEarnMetaGameItem()
        {

        }

        protected virtual void UpdateMetaItemInfo()
        {

        }

        private IEnumerator ShowBetUpBalloonTextCoroutine()
        {
            var onCloseEventTrigger = new EventTrigger(gameObject, MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_CLOSE_META_EVENT_GROUP);
            var delayTrigger = new TimerTrigger(BET_UP_BALLOON_DELAY);

            yield return new WaitUntilTrigger(onCloseEventTrigger, delayTrigger);
            if (onCloseEventTrigger.IsTrigger) yield break;

            MetaContextElementUtils.SetPropertySafty(betUpBalloonAnimElement, true);

            var timerTrigger = new TimerTrigger(BET_UP_BALLOON_DISPLAY_TIME);

            yield return new WaitUntilTrigger(onCloseEventTrigger, timerTrigger);

            MetaContextElementUtils.SetPropertySafty(betUpBalloonAnimElement, false);
        }
    }
}
