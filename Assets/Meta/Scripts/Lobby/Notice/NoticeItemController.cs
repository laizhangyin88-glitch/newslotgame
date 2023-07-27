using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class NoticeItemController : MonoBehaviour
    {
        private Blackboard rootBB;
        private ContextElement rootElement;
        private Animator rootAnimator;
        private GameObject rootObject;

        private NoticeTypes noticeTypes;

        private const string NOTICE_TYPE = "noticeInfo/type";
        private const string IMAGE_LOADED = "IsLoaded";
        private const string END_TIME_STAMP = "endTimestamp";

        private const string URGENT_TEXT_TITLE = "Urgent/Text Title";
        private const string URGENT_TEXT_CONTEXT = "Urgent/Text Context";
        private const string NOTICE_INFO_TITLE = "noticeInfo/title";
        private const string NOTICE_INFO_MESSAGE = "noticeInfo/message";
        private const string NOTICE_INFO_ACTION = "noticeInfo/action";
        private const string NOTICE_INFO_CONSTRAINTS_SHOWENDTIMER = "noticeInfo/constraints/showEndTimer";
        private const string NOTICE_INFO_CONSTRAINTS_TIMER_POS_X = "noticeInfo/constraints/timerPosX";
        private const string NOTICE_INFO_CONSTRAINTS_TIMER_POS_Y = "noticeInfo/constraints/timerPosY";
        private const string NOTICE_INFO_NOTICE_ID = "noticeInfo/id";
        private const string NOTICE_FROM_TYPE = "notice";

        private const string SET_PARAMETER_BOOL = "Active";
        private const string SET_CONTEXT_ELEMENT_NAME_URGENT = "Urgent";
        private const string SET_CONTEXT_ELEMENT_NAME_TIMER = "Timer";

        private const string SET_CONTEXT_WEB_IMAGE_URL = "noticeInfo/imageUrl";
        private const string SET_TIMER_REMAINING_TIMER = "Timer/Remaining Timer";

        private const string REMAING_TIME_FORMAT_KEY = "TIME_FORMAT_HHMMSS_TOTALHOUR";
        private const string CLICK_SEND_EVENT_NAME = "DoAction";

        public Variable<bool> isWebImageLoaded = null;
        public Variable<bool> showEndTimer = null;
        public Variable<long> endTimeStamp;

        public Variable<int> timerPosX;
        public Variable<int> timerPosY;

        public ContextElement timerElement;

        public bool isWebImageLoadedFailed = false;

        private bool isInit = false;

        private void OnEnable()
        {
            //UpdateVariables();
        }

        private void InitProperty()
        {
            if (isInit) return;

            // Component Settings
            rootBB = gameObject.GetComponent<Blackboard>();
            rootElement = gameObject.GetComponent<ContextElement>();
            rootAnimator = gameObject.GetComponent<Animator>();
            rootObject = this.gameObject;

            rootElement.UpdateContext(false);

            // Member values Settings
            GetBlackboardValue();

            isInit = true;
        }

        public void UpdateVariables()
        {
            InitProperty();
        }

        private void GetBlackboardValue()
        {
            var types = BlackboardUtils.FindVariable<NoticeTypes>(rootBB, NOTICE_TYPE);
            noticeTypes = (types == null) ? NoticeTypes.UNKNOWN : types.value;
            rootBB.AddVariable("_type", noticeTypes);

            isWebImageLoaded = BlackboardUtils.FindVariable<bool>(rootBB, IMAGE_LOADED);
        }

        public void OnCheckNoticeTypes(NoticeTypes types)
        {
            // original code [if (types == NoticeTypes.TEXT_POPUP)]
            if (noticeTypes == types)
            {
                SetTextPopup();
            }
            else
            {
                SetNoticeItemPopup();
            }
        }

        private void SetTextPopup()
        {
            // Title Setting
            ContextElement titleElement = ContextUtils.FindElement(rootElement, URGENT_TEXT_TITLE, ContextSearchingType.FullNameSearch);
            string title = BlackboardUtils.FindValue<string>(rootBB, NOTICE_INFO_TITLE);
            ContextUtils.SetText(titleElement, title);

            // Message Setting
            ContextElement msgElement = ContextUtils.FindElement(rootElement, URGENT_TEXT_CONTEXT, ContextSearchingType.FullNameSearch);
            string logMessage = BlackboardUtils.FindValue<string>(rootBB, NOTICE_INFO_MESSAGE);
            ContextUtils.SetText(msgElement, logMessage);

            // Set Parameter Bool
            rootAnimator.SetBool(SET_PARAMETER_BOOL, false);

            // Set Context Active
            ContextElement contextActiveElement = ContextUtils.FindElement(rootElement, SET_CONTEXT_ELEMENT_NAME_URGENT, ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetActive(contextActiveElement, true);
        }

        private void SetNoticeItemPopup()
        {
            SetBlackboardTimer();
            SetContextWebImage();
        }

        private void SetContextWebImage()
        {
            IContextImage imageElement = rootElement as IContextImage;
            if (imageElement == null)
            {
                DebugLog("[Context] " + rootElement.ContextName + " is not IContextImage");
                isWebImageLoadedFailed = true;
                return;
            }

            var imageUrl = BlackboardUtils.FindVariable<string>(rootBB, SET_CONTEXT_WEB_IMAGE_URL);
            if (imageUrl == null)
            {
                DebugLog("[Blackboard](" + rootElement.name + ") Null variable founded in " + SET_CONTEXT_WEB_IMAGE_URL);
                isWebImageLoadedFailed = true;
                return;
            }

            if (string.IsNullOrEmpty(imageUrl.value))
            {
                DebugLog("ImageUrl is null or empty");
                isWebImageLoadedFailed = true;
                return;
            }

            string downloadUrl = imageUrl.value;
            imageElement.SetHash(downloadUrl.GetHashCode().ToString());

            WebImageDownloader.Instance.LoadWebImage(
                downloadUrl,
                CacheType.FileCache,
                false,
                null,
                delegate (Sprite img)
                {
                    if (rootObject != null && imageElement != null && isWebImageLoaded != null)
                    {
                        isWebImageLoaded.value = true;

                        if (imageElement.CheckHash(downloadUrl.GetHashCode().ToString()))
                            imageElement.SetSprite(img);

                        rootAnimator.SetBool(SET_PARAMETER_BOOL, true);
                    }
                },
                null,
                delegate (WebImageDownloader.WebImageDownloadError error)
                {
                    if (rootObject != null && imageElement != null && imageUrl != null)
                    {
                        DebugLog(error.ToString());
                        isWebImageLoadedFailed = true;
                    }
                }
            );

            OnCheckTimer();
            OnCheckAction();
        }

        private void SetBlackboardTimer()
        {
            showEndTimer = BlackboardUtils.FindVariable<bool>(rootBB, NOTICE_INFO_CONSTRAINTS_SHOWENDTIMER);
            timerElement = ContextUtils.FindElement(rootElement, SET_TIMER_REMAINING_TIMER, ContextSearchingType.FullNameSearch);
            timerPosX = BlackboardUtils.FindVariable<int>(rootBB, NOTICE_INFO_CONSTRAINTS_TIMER_POS_X);
            timerPosY = BlackboardUtils.FindVariable<int>(rootBB, NOTICE_INFO_CONSTRAINTS_TIMER_POS_Y);

            rootBB.AddVariable("_showEndTimer", showEndTimer.value);
            rootBB.AddVariable("timer", timerElement);
            rootBB.AddVariable("_timerPosX", timerPosX.value);
            rootBB.AddVariable("_timerPosY", timerPosY.value);
        }

        private void SetTimerOffset(ContextElement element)
        {
            if (element == null)
                return;

            RectTransform rect = element.gameObject.GetComponent<RectTransform>();
            if (rect == null)
                return;

            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;

            Vector2 pos = rect.anchoredPosition;

            pos.x = (float)timerPosX.value;
            pos.y = (float)(timerPosY.value * -1);

            rect.anchoredPosition = pos;
        }

        private void OnCheckTimer()
        {
            endTimeStamp = BlackboardUtils.FindVariable<long>(rootBB, END_TIME_STAMP);
            ContextElement contextActiveElement = ContextUtils.FindElement(rootElement, SET_CONTEXT_ELEMENT_NAME_TIMER, ContextSearchingType.ChildrenSearch);

            if (showEndTimer.value == true && !(endTimeStamp.value == 0))
            {
                // Timer offset Settings
                SetTimerOffset(contextActiveElement);
                // Active Timer
                MetaContextElementUtils.SetActive(contextActiveElement, true);
                MetaContextElementUtils.SetCommonRemainingTimer(timerElement,
                                                                    endTimeStamp.value,
                                                                    0,
                                                                    REMAING_TIME_FORMAT_KEY,
                                                                    null,
                                                                    null,
                                                                    "Ended",
                                                                    true,
                                                                    null);
            }
            else
            {
                // Deactive Timer
                MetaContextElementUtils.SetActive(contextActiveElement, false);
            }
        }

        private void OnCheckAction()
        {
            var checkAction = BlackboardUtils.FindVariable(rootBB, NOTICE_INFO_ACTION);
            if (!(checkAction == null || checkAction.value == null))
            {
                IContextClickable clickableElement = rootElement as IContextClickable;
                if (clickableElement != null)
                {
                    clickableElement.RemoveAllListener();

                    GraphOwner owner = gameObject.GetComponent<GraphOwner>();
                    if (owner != null)
                    {
                        clickableElement.AddListenerOnClick((ContextElement sender) =>
                        {
                            owner.SendEvent<ContextElement>(CLICK_SEND_EVENT_NAME, sender);
                        });
                    }

                    var noticeID = BlackboardUtils.FindVariable<int>(rootBB, NOTICE_INFO_NOTICE_ID);

                    rootBB.AddVariable("_action", checkAction.value);
                    rootBB.AddVariable("_noticeID", noticeID.value);
                    rootBB.AddVariable("_fromType", NOTICE_FROM_TYPE);
                }
                else
                    DebugLog("IContextClickable is not exist");
            }
        }

        private void DebugLog(string msg)
        {
            if (ApplicationSettings.LogSystem())
                Debug.LogWarning("[NoticeItemController] " + msg);
        }
    }
}
