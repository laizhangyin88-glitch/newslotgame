using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    // [RequireComponent(typeof(Blackboard))]
    public class EventTagController : MonoBehaviour
    {
        protected SimpleReserveTimer reserveTimer = null;

        protected ContextElement root;
        // private Blackboard rootBB;
        protected Animator anim;

        protected ContextElement remainingTimerElement;
        protected ContextElement eventTextElement;
        protected ContextElement remainingTimerTextElement;

        public List<string> textList;

        public float changeDelay;
        public float changeSpd;

        public int currentIndex;
        public int maxIndex;

        private bool isRunning = false;
        protected bool isInit = false;

        private long deadlineTimestamp;

        private bool IsAvailableChange
        {
            get
            {
                if (remainingTimerTextElement == null) return false;
                if (eventTextElement == null) return false;

                return true;
            }
        }

        // private void Awake()
        // {
        //     rootBB = GetComponent<Blackboard>();
        // }

        protected virtual void InitContext()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            root.UpdateContext(false);

            anim = GetComponent<Animator>();

            eventTextElement = ContextUtils.FindElement(root, "Text", ContextSearchingType.ChildrenSearch);
            remainingTimerElement = ContextUtils.FindElement(root, "Remaining Timer", ContextSearchingType.ChildrenSearch);
            remainingTimerTextElement = ContextUtils.FindElement(remainingTimerElement, "Text", ContextSearchingType.ChildrenSearch);

            reserveTimer = GetComponent<SimpleReserveTimer>();
            if (reserveTimer == null)
                reserveTimer = gameObject.AddComponent<SimpleReserveTimer>();

            isInit = true;
        }

        private void OnEnable()
        {
            if (isRunning)
            {
                StartChange();
            }
        }

        private void OnDisable()
        {
            if (isRunning)
            {
                CancelInvoke();
                StopAllCoroutines();
            }
        }

        public void ResetIndex()
        {
            if (isRunning)
            {
                CancelInvoke();
                StopAllCoroutines();
            }
            StartChange();
        }

        // public void SetFlipTextData(List<string> eventTextList, float textChangeDelay, float textChangeSpd)
        // {
        //     textList = eventTextList;
        //     if(textList == null)
        //         textList = new List<string>();

        //     maxIndex = textList.Count + 1;

        //     changeDelay = textChangeDelay;
        //     if(changeDelay <= 0f) changeDelay = 1f;

        //     changeSpd = textChangeSpd;
        //     if(changeSpd <= 0f) changeSpd = 0.5f;
        // }

        // public void SetEventText(string eventText)
        // {
        //     MetaContextElementUtils.SetText(eventTextElement, eventText);
        // }

        // Use text count 3 greater

        public void Initialize(long targetTime,
                                string timeFormatKey,
                                string outputFormatKey,
                                string expireText,
                                bool useCommonTimer,
                                GameObject caller,
                                List<string> eventTextList,
                                float textChangeDelay,
                                float textChangeSpd
                             )
        {
            Initialize(targetTime,
                        0,
                        timeFormatKey,
                        outputFormatKey,
                        null,
                        expireText,
                        useCommonTimer,
                        caller,
                        eventTextList,
                        textChangeDelay,
                        textChangeSpd,
                        ""
                        );
        }

        // Use only one event text
        public void Initialize(long targetTime,
                                string timeFormatKey,
                                string outputFormatKey,
                                string expireText,
                                bool useCommonTimer,
                                GameObject caller,
                                string defaultEventText
                             )
        {
            Initialize(targetTime,
                        0,
                        timeFormatKey,
                        outputFormatKey,
                        null,
                        expireText,
                        useCommonTimer,
                        caller,
                        null,
                        0f,
                        0f,
                        defaultEventText
                        );
        }

        public void Initialize(long targetTime,
                                long warningTime,
                                string timeFormatKey,
                                string outputFormatKey,
                                string warningFormatKey,
                                string expireText,
                                bool useCommonTimer,
                                GameObject caller,
                                List<string> eventTextList,
                                float textChangeDelay,
                                float textChangeSpd,
                                string defaultEventText
            )
        {
            Clear();

            InitContext();

            textList = eventTextList;
            if (textList == null)
                textList = new List<string>();

            maxIndex = textList.Count + 1;

            // // Set Remaining Timer. 
            // long targetTime = BlackboardUtils.GetOrCreateVariable<long>(rootBB, "targetTimestamp").value;
            // long warningTime = BlackboardUtils.GetOrCreateVariable<long>(rootBB, "warningTimestamp").value;
            // string timeFormatKey = BlackboardUtils.GetOrCreateVariable<string>(rootBB, "textFormatKey").value;
            // string outputFormatKey = BlackboardUtils.GetOrCreateVariable<string>(rootBB, "outputFormatKey").value;
            // string warningFormatKey = BlackboardUtils.GetOrCreateVariable<string>(rootBB, "outputWarningKey").value;
            // string expireText = BlackboardUtils.GetOrCreateVariable<string>(rootBB, "expireText").value;
            // bool useCommonTimer = BlackboardUtils.GetOrCreateVariable<bool>(rootBB, "useCommonTimer").value;
            // GameObject caller = BlackboardUtils.GetOrCreateVariable<GameObject>(rootBB, "caller").value;

            MetaContextElementUtils.SetCommonRemainingTimer(remainingTimerElement, targetTime, warningTime, timeFormatKey, outputFormatKey, warningFormatKey, expireText, useCommonTimer, caller);

            if (!string.IsNullOrEmpty(defaultEventText))
                MetaContextElementUtils.SetText(eventTextElement, defaultEventText);

            changeDelay = textChangeDelay;
            if (changeDelay <= 0f) changeDelay = 1f;

            changeSpd = textChangeSpd;
            if (changeSpd <= 0f) changeSpd = 0.5f;

            deadlineTimestamp = targetTime;
            if (deadlineTimestamp != 0)
            {
                deadlineTimestamp = targetTime - (TimeUtils.ONE_HOUR_MS * 2L);
            }

            UpdateDeadline();

            StartChange();
        }

        public void Clear()
        {
            if (isRunning)
            {
                CancelInvoke();
                StopAllCoroutines();
            }

            currentIndex = 0;
            maxIndex = 0;
            isRunning = false;
            deadlineTimestamp = 0;

            UpdateDeadline();

            if (textList != null) textList.Clear();
        }

        private void StartChange()
        {
            isRunning = true;
            currentIndex = 0;
            UpdateData(0);

            if (maxIndex > 1 && gameObject.activeInHierarchy && IsAvailableChange)
            {
                // Invoke Repeat?
                InvokeRepeating("Change", changeDelay, changeDelay + changeSpd);
            }
        }

        private GameObject GetCurrentObject()
        {
            if (!IsAvailableChange) return null;

            if (textList != null && currentIndex < textList.Count)
            {
                return eventTextElement.gameObject;
            }

            return remainingTimerTextElement.gameObject;
        }

        private void Change()
        {
            StopCoroutine(Changing());
            StartCoroutine(Changing());
        }

        private IEnumerator Changing()
        {
            float targetTimeDelta = changeSpd / 2f;
            float currentDelta = 0f;
            GameObject currentObject = GetCurrentObject();

            if (currentObject != null)
            {
                while (targetTimeDelta > currentDelta)
                {
                    currentDelta += Time.deltaTime;
                    if (currentDelta < targetTimeDelta)
                        currentObject.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, currentDelta / targetTimeDelta);

                    yield return null;
                }

                currentObject.transform.localScale = Vector3.zero;
            }

            Next();
            UpdateData(currentIndex);

            currentDelta = 0f;
            currentObject = GetCurrentObject();
            if (currentObject != null)
            {
                while (targetTimeDelta > currentDelta)
                {
                    currentDelta += Time.deltaTime;
                    if (currentDelta < targetTimeDelta)
                        currentObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, currentDelta / targetTimeDelta);

                    yield return null;
                }

                currentObject.transform.localScale = Vector3.one;
            }
        }

        private void UpdateData(int index)
        {
            if (textList.Count == 0) return;

            if (index < textList.Count)
            {
                MetaContextElementUtils.SetText(eventTextElement, textList[index]);
            }

            if (eventTextElement != null)
            {
                eventTextElement.gameObject.SetActive(index < textList.Count);
                eventTextElement.transform.localScale = index < textList.Count ? Vector3.one : Vector3.zero;
            }

            if (remainingTimerTextElement != null)
            {
                remainingTimerTextElement.gameObject.SetActive(index >= textList.Count);
                remainingTimerTextElement.transform.localScale = index < textList.Count ? Vector3.zero : Vector3.one;
            }
        }

        private void Next()
        {
            ++currentIndex;
            currentIndex %= maxIndex;
        }

        private void UpdateDeadline()
        {
            bool isActive = false;

            if (deadlineTimestamp != 0)
            {
                long currentTimestamp = TimeUtils.GetTimeStamp();
                if (deadlineTimestamp > currentTimestamp)
                {
                    reserveTimer.SetReserveCallback(deadlineTimestamp,
                        () =>
                        {
                            UpdateDeadline();
                        }
                    );
                }
                else
                {
                    reserveTimer.Stop();
                    isActive = true;
                }
            }

            if (anim != null)
            {
                anim.SetBool("Deadline", isActive);
            }
        }
    }
}
