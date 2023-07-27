using UnityEngine;
using SlotMaker;
using System.Collections;
using ParadoxNotion;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class LevelUpDashEventTagController : EventMonoBehaviour
    {
        private ContextElement root;
        private Animator anim;
        private RemainingTimerController remainingTimer;

        private ContextElement levelUpDashAreaElement;
        private ContextElement leftLevelTextElement;
        private ContextElement leftTimeTextElement;
        private ContextElement dashBoostTextElement;

        private NonDrawingGraphic clickableArea;

        // private bool isDisplay = false;
        private bool isInit = false;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        private const float FLIP_INTERVAL = 3.9f;
        private const float FLIP_DELAY = 0.1f;

        private enum DisplayType
        {
            LEVEL_UP_DASH,
            LEFT_LEVEL,
            LEFT_TIME,
            DASH_BOOST,
        }

        private void Start()
        {
            InitProperty();

            // Events
            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_PASSIVE_EVENT);

            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_LEVEL_UP, UpdateLeftLevelText);
            Register(MetaEventDefine.ON_META_UI_EVENT, LevelUpDash.LevelUpDash.Events.ON_UPDATE_LEVEL_UP_DASH, UpdateAll);
            Register(MetaEventDefine.ON_PASSIVE_EVENT, MetaEventDefine.REFRESH_PASSIVE_EVENT, OnRefreshPassive);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // enable 되는 경우는 NavigationProfileController에서 처리
            if (clickableArea != null)
                clickableArea.raycastTarget = false;
        }

        private void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();
            remainingTimer = GetComponent<RemainingTimerController>();

            root.UpdateContext(false);

            levelUpDashAreaElement = ContextUtils.FindElement(root, "Text Level Up Dash", CHILDREN);
            leftLevelTextElement = ContextUtils.FindElement(root, "Text Left Level", CHILDREN);
            leftTimeTextElement = ContextUtils.FindElement(root, "Text Left Time", CHILDREN);
            dashBoostTextElement = ContextUtils.FindElement(root, "Text Dash Boost", CHILDREN);

            clickableArea = ContextUtils.FindElement(root, "Button Area", CHILDREN).GetComponent<NonDrawingGraphic>();

            MetaContextElementUtils.SimpleSetClickable(root, "Button Area", gameObject,
                MetaEventDefine.ON_META_UI_EVENT, LevelUpDash.LevelUpDash.Events.OPEN_LEVEL_UP_DASH, true);

            MetaContextElementUtils.SimpleSetClickable(root, "Button Area", OpenLevelDash);

            isInit = true;
        }

        public void InitTag()
        {
            InitProperty();
            UpdateTag();
        }

        public void UpdateTag()
        {
            UpdateAll();

            StopCoroutine(FlipCoroutine());
            StartCoroutine(FlipCoroutine());
        }

        private void OpenLevelDash()
        {
            EventSender.SendGlobalMetaEvent(LevelUpDash.LevelUpDash.Events.OPEN_LEVEL_UP_DASH);
        }

        private IEnumerator FlipCoroutine()
        {
            clickableArea.raycastTarget = false;

            if (LevelUpDash.LevelUpDash.Utils.IsActiveLevelUpDash())
            {
                clickableArea.raycastTarget = true;

                anim.SetBool("Appear", true);
                ActiveTarget(DisplayType.LEVEL_UP_DASH);
                yield return new WaitForSeconds(FLIP_INTERVAL);

                anim.SetBool("Appear", false);
                yield return new WaitForSeconds(FLIP_DELAY);

                int limit = LevelUpDash.LevelUpDash.Defines.MAX_REMAINING_LEVEL_FOR_DISPLAYING_LEFT_LEVEL;
                if (LevelUpDash.LevelUpDash.Utils.IsLevelRemainingLessThan(limit + 1))
                {
                    anim.SetBool("Appear", true);
                    ActiveTarget(DisplayType.LEFT_LEVEL);
                    yield return new WaitForSeconds(FLIP_INTERVAL);

                    anim.SetBool("Appear", false);
                    yield return new WaitForSeconds(FLIP_DELAY);
                }

                anim.SetBool("Appear", true);
                ActiveTarget(DisplayType.LEFT_TIME);
                yield return new WaitForSeconds(FLIP_INTERVAL);
            }

            if (LevelUpDash.LevelUpDash.Utils.IsActivePurchaseBooster())
            {
                if(LevelUpDash.LevelUpDash.Utils.IsActiveLevelUpDash())
                {
                    anim.SetBool("Appear", false);
                    yield return new WaitForSeconds(FLIP_DELAY);

                    anim.SetBool("Appear", true);
                    ActiveTarget(DisplayType.DASH_BOOST);
                    yield return new WaitForSeconds(FLIP_INTERVAL);
                }
                else
                {
                    ActiveTarget(DisplayType.DASH_BOOST);
                    yield break;
                }
            }

            clickableArea.raycastTarget = false;
            anim.SetBool("Appear", false);
        }

        private void UpdateAll()
        {
            UpdateLeftLevelText();
            UpdateRemainingTimer();

            clickableArea.raycastTarget = LevelUpDash.LevelUpDash.Utils.IsActiveLevelUpDash();
        }

        private void ActiveTarget(DisplayType type)
        {
            MetaContextElementUtils.SetActive(levelUpDashAreaElement, type == DisplayType.LEVEL_UP_DASH);
            MetaContextElementUtils.SetActive(leftLevelTextElement, type == DisplayType.LEFT_LEVEL);
            MetaContextElementUtils.SetActive(leftTimeTextElement, type == DisplayType.LEFT_TIME);
            MetaContextElementUtils.SetActive(dashBoostTextElement, type == DisplayType.DASH_BOOST);
        }

        private void UpdateLeftLevelText()
        {
            int targetLevel = LevelUpDash.LevelUpDash.Utils.GetTargetMissionLevel();
            int left;
            if (targetLevel < 0)
            {
                left = 0;
            }
            else
            {
                left = targetLevel - BlackboardQueryUtils.GetMyLevel();
            }
            MetaContextElementUtils.SetTextGlobal(leftLevelTextElement, "LEVEL_UP_DASH_EVENT_TAG_LEFT", left);
        }

        private void UpdateRemainingTimer()
        {
            remainingTimer.Init(
                leftTimeTextElement,
                "TIME_FORMAT_HHMMSS_TOTALHOUR",
                "",
                "",
                "Ended",
                true,
                null);

            remainingTimer.StartTimer(LevelUpDash.LevelUpDash.Utils.EndTimestamp, 0L);
        }

        private void OnRefreshPassive(EventData eventData)
        {
            var eventInfo = PassiveEventManager.Instance.GetEventInfoFromID((int)eventData.value, true);

            if (eventInfo != null)
            {
                if (eventInfo.type == EventInfoType.LEVEL_UP_DASH_MISSION)
                {
                    clickableArea.raycastTarget = LevelUpDash.LevelUpDash.Utils.IsActiveLevelUpDash();
                }
            }
        }
    }
}
