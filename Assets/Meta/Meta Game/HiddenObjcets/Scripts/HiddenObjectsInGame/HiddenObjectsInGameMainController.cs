using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using NodeCanvas.Framework;
using System.Collections;
using BagelCode.ClientModels;
using Sirenix.OdinInspector;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsInGameMainController : EventMonoBehaviour
    {
        // Settings
        [Title("Object Collider")]
        [PropertyOrder(0), LabelWidth(200f)]
        public float MIN_SCALE_WITH_MAX_MARGIN;
        [PropertyOrder(0), LabelWidth(200f)]
        public float MAX_MARGIN;
        [PropertyOrder(0), LabelWidth(200f)]
        public float MAX_SCALE_WITH_MIN_MARGIN;
        [PropertyOrder(0), LabelWidth(200f)]
        public float MIN_MARGIN;

        [Title("Hint")]
        [PropertyOrder(3)]
        public float PROJECTILE_MOVEMENT_TIME;
        [PropertyOrder(3)]
        public int MAX_COMBO_SOUND;
        //

        [Title("In Game Status")]
        [SerializeField, PropertyOrder(2)]
        private int warningLevel = 0;
        [SerializeField, PropertyOrder(2)]
        private int findCount = 0;
        [SerializeField, PropertyOrder(2)]
        private bool isPause;
        [SerializeField, PropertyOrder(2)]
        private long elapsedTimeMS = 0L;
        [SerializeField, PropertyOrder(2)]
        private List<int> remainFindObjectIDList = new List<int>();

        // Define
        private const int TARGET_COUNT = 3;
        private const int NONE = -1;
        private const int WARNING_TIME = 30; // sec
        private const int WARNING_TIME_2 = 10; // sec
        //

        private class FindObjectInfo
        {
            public GameObject obj;
            public ObjectInfo info;
        }

        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private bool isIntro = true; // true until first resume
        private bool isTimeUp = false;
        private bool isClear = false;

        private int chapter;
        private int stage;

        private int chapterTotalStarCount;
        private int stageStarCount;

        private ContextElement objectAreaElement;

        private ZoomInOutController zoomInOutController;

        private Blackboard clearResponseBB = null;

        // Find Object
        private List<ObjectInfo> objectInfoList = new List<ObjectInfo>();
        private List<int> wrongObjectIndexList;

        private HiddenObjectsInGameTargetInfo[] targetInfoes = new HiddenObjectsInGameTargetInfo[TARGET_COUNT];
        private Dictionary<int, FindObjectInfo> findObjectInfoDict = new Dictionary<int, FindObjectInfo>(); // key: id

        // Play Log
        private List<HiddenUniversePlayLog> playLogList = new List<HiddenUniversePlayLog>();
        private int pauseCount = 0; // for ae

        // Penalty & Timer
        private long penaltyDuration = 0L;
        private int penaltyTriggerCount = 0;
        private long penaltyTriggerDuration = 0L;
        private bool useTimeUp = false;
        private long timeLimit = 0L;
        private List<long> penaltyStackList = new List<long>();
        private ContextElement timerTextElement;

        // Combo
        private int currentCombo = 0;
        private int currentMultiplier = 0;
        private int prevCombo = 0;
        private long remainingComboDuration = 0L;
        private float remainingProgress = 0f;

        private int maxCombo;
        private long comboDuration;
        private List<int> comboMultiplierList;

        private Animator comboAnim;
        private ContextElement comboAreaElement;
        private ContextElement comboGaugeElement;
        private ContextElement comboMultiTextElement;

        // Hint
        private long hintRemaining = 0L;
        private bool isHintReady = false;
        private int currentHintTargetID = NONE;

        private long hintCooltime;

        private Animator hintAnim;
        private ContextElement hintButtonElement;
        private ContextElement hintProgressElement;
        private ContextElement particleAreaElement;

        private GameObject hintEffectObj;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

#if DEV
        [Button]
        [PropertyOrder(1)]
        private void SetRemainingTime(int time = 60)
        {
            long remaining = time * 1000L;
            elapsedTimeMS = timeLimit - remaining;
        }

        [Button]
        [PropertyOrder(1)]
        private void AddObject(int id)
        {
            var objectInfo = new ObjectInfo()
            {
                index = id,
                name = "test name",
                chapter = chapter,
                stage = stage,
                score = 3000,
            };
            objectInfoList.Add(objectInfo);
            remainFindObjectIDList.Insert(0, id);

            var objectElement = ContextUtils.FindElement(objectAreaElement, id.ToString(), CHILDREN);
            MetaContextElementUtils.SetActive(objectElement, true);
            objectElement.GetComponent<NonDrawingGraphic>().raycastTarget = false;

            var findObjectInfo = new FindObjectInfo()
            {
                info = objectInfo,
                obj = objectElement.gameObject,
            };
            findObjectInfoDict.Add(id, findObjectInfo);

            UpdateTargets();

            var objectRectTransform = objectElement.GetComponent<RectTransform>();
            var objectRect = objectRectTransform.rect;
            float marginedWidth = GetMarginedScale(objectRect.width);
            float marginedHeight = GetMarginedScale(objectRect.height);
            objectRectTransform.sizeDelta = new Vector2(marginedWidth, marginedHeight);

            var collider = objectElement.GetComponent<BoxCollider>();
            if (collider != null)
            {
                var colliderScale = new Vector3(marginedWidth, marginedHeight, 0.1f);
                collider.size = colliderScale;
            }

            if (PlayerPrefs.GetInt(HiddenObjects.Defines.PLAYER_PREFS_SHOW_RECT) == 1)
            {
                string bundle = "testsuite";
                string asset = "Boundary Visualizer";
                Transform parent = objectElement.transform;
                GameObject boundaryObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);

                var boundaryRectTransform = boundaryObj.GetComponent<RectTransform>();
                boundaryRectTransform.sizeDelta = new Vector2(marginedWidth, marginedHeight);
            }
        }

        [Button]
        [PropertyOrder(0)]
        private void ApplyScale()
        {
            for (int i = 0; i < objectInfoList.Count; ++i)
            {
                var objectInfo = objectInfoList[i];
                int id = objectInfo.index;

                var objectElement = ContextUtils.FindElement(objectAreaElement, id.ToString(), CHILDREN);
                if (objectElement == null) continue;
                objectElement.UpdateContext(true);

                var objectRectTransform = objectElement.GetComponent<RectTransform>();
                var objectRect = objectRectTransform.rect;
                float marginedWidth = GetMarginedScale(objectRect.width);
                float marginedHeight = GetMarginedScale(objectRect.height);
                objectRectTransform.sizeDelta = new Vector2(marginedWidth, marginedHeight);

                var collider = objectElement.GetComponent<BoxCollider>();
                if(collider != null)
                {
                    var colliderScale = new Vector3(marginedWidth, marginedHeight, 0.1f);
                    collider.size = colliderScale;
                }

                if (PlayerPrefs.GetInt(HiddenObjects.Defines.PLAYER_PREFS_SHOW_RECT) == 1)
                {
                    GameObject boundaryObj;
                    var boundaryElement = ContextUtils.FindElement(objectElement, "Boundary Visualizer", CHILDREN);
                    if (boundaryElement == null)
                    {
                        string bundle = "testsuite";
                        string asset = "Boundary Visualizer";
                        Transform parent = objectElement.transform;
                        boundaryObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
                    }
                    else
                    {
                        boundaryObj = boundaryElement.gameObject;
                    }

                    var boundaryRectTransform = boundaryObj.GetComponent<RectTransform>();
                    boundaryRectTransform.sizeDelta = new Vector2(marginedWidth, marginedHeight);
                }
            }
        }
#endif

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(this.GetHashCode());
        }

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            // Variables
            var objectToFindListBB = bb.GetValue<List<Blackboard>>("objectToFindList");
            for(int i = 0; i < objectToFindListBB.Count; ++i)
                objectInfoList.Add(BlackboardQueryUtils.DeserializeObjectInfo(objectToFindListBB[i]));

            chapter = bb.GetValue<int>("chapter");
            stage = bb.GetValue<int>("stage");
            wrongObjectIndexList = bb.GetValue<List<int>>("wrongObjectIndexList");
            comboDuration = bb.GetValue<long>("comboDuration");
            comboMultiplierList = bb.GetValue<List<int>>("comboMultiplierList");
            maxCombo = Mathf.Min(bb.GetValue<int>("maxCombo"));
            hintCooltime = bb.GetValue<long>("hintCooltime");
            penaltyDuration = bb.GetValue<long>("penaltyDuration");
            penaltyTriggerCount = bb.GetValue<int>("penaltyTriggerCount");
            penaltyTriggerDuration = bb.GetValue<long>("penaltyTriggerDuration");
            timeLimit = bb.GetValue<long>("timeLimit");
            useTimeUp = timeLimit > 0L;

            // Disable Kudo
            KudoEventManager.Instance.DisableKudo();

            // Elements
            string stageName = HiddenObjects.Utils.GetStageAssetName(chapter, stage);
            objectAreaElement = ContextUtils.FindElement(root, "Stage Area/"+ stageName + "/Area", FULL);

            particleAreaElement = ContextUtils.FindElement(root, "Particle Area", CHILDREN);

            zoomInOutController = objectAreaElement.GetComponent<ZoomInOutController>();

            // Setup
            for (int i = 0; i < TARGET_COUNT; ++i)
                targetInfoes[i] = new HiddenObjectsInGameTargetInfo();

            // Objects
            InitObjects();

            // Target Text
            var findTargetTextAreaElement = ContextUtils.FindElement(root, "Find Object Area", CHILDREN);
            for (int i = 0; i < TARGET_COUNT; ++i)
            {
                targetInfoes[i].element = ContextUtils.FindElement(findTargetTextAreaElement,
                    string.Format("Find Object Base {0:00}", i + 1), CHILDREN);

                targetInfoes[i].anim = targetInfoes[i].element.GetComponent<Animator>();
            }

            // Pause
            var pauseButtonElement = ContextUtils.FindElement(root, "Pause", CHILDREN);
            MetaSystem.SubscribeBackButton(this.GetHashCode(),
                () => EventSender.SendEvent(gameObject, HiddenObjects.Events.ON_PAUSE));
            MetaContextElementUtils.SimpleSetClickable(root, "Pause",
                () => EventSender.SendEvent(gameObject, HiddenObjects.Events.ON_PAUSE));

            // Timer
            var timerAreaElement = ContextUtils.FindElement(root, "Timer Area", CHILDREN);
            MetaContextElementUtils.SetActive(timerAreaElement, useTimeUp);
            if (useTimeUp)
            {
                timerTextElement = ContextUtils.FindElement(timerAreaElement, "Timer Text", CHILDREN);
            }

            // Combo
            comboAreaElement = ContextUtils.FindElement(root, "Bonus Area", CHILDREN);
            comboGaugeElement = ContextUtils.FindElement(comboAreaElement, "Progress Bar", CHILDREN);
            comboMultiTextElement = ContextUtils.FindElement(comboAreaElement, "Multiple Base/Text", FULL);
            comboAnim = comboAreaElement.GetComponent<Animator>();

            MetaContextElementUtils.SimpleSetTextGlobal(comboAreaElement, "Bonus Text", "HIDDEN_OBJECTS_IN_GAME_BONUS", CHILDREN);

            // Hint
            hintButtonElement = ContextUtils.FindElement(root, "Hint Area", CHILDREN);
            hintProgressElement = ContextUtils.FindElement(hintButtonElement, "Progress Bar", CHILDREN);
            MetaContextElementUtils.SetClickable(hintButtonElement, gameObject, HiddenObjects.Events.ON_USE_HINT, false);

            hintAnim = hintButtonElement.GetComponent<Animator>();
            hintAnim.SetBool("isActive", true);

            // Handle Events
            GestureManager.Instance.EnableGestureHandler(GestureManager.GestureHandlerType.CORRECT_CLICK);
            GestureManager.Instance.EnableGestureHandler(GestureManager.GestureHandlerType.SCROLL);
#if DEV
            if (PlayerPrefs.GetInt(HiddenObjects.Defines.PLAYER_PREFS_ZOOM_ENABLE) == 0)
#endif
                GestureManager.Instance.EnableGestureHandler(GestureManager.GestureHandlerType.ZOOM_IN_OUT);

            MessageDispatcher.Register(GestureManager.ON_GESTURE_EVENT, OnGestureEvent);

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_CONTENT_EVENT);
            Register(MetaEventDefine.ON_META_UI_EVENT, ZoomInOutController.ON_CHANGE_SCREEN_SIZE, UpdateTargetFade);
            Register(HiddenObjects.Events.ON_USE_HINT, OnUseHint);

            // Update All
            UpdateTargets();
            UpdateTimer();

            SetPause(true);
        }

#if UNITY_ANDROID || UNITY_IPHONE
        private void OnApplicationPause(bool pause)
        {
            if(pause)
            {
                EventSender.SendEvent(gameObject, HiddenObjects.Events.ON_PAUSE);
            }
        }
#endif

        private void InitObjects()
        {
            int totalObjectCount = 75; // todo shk chapter 별로 수동으로 지정
            var activingObjects = new bool[totalObjectCount];

            // Find Objects
            for (int i = 0; i < objectInfoList.Count; ++i)
            {
                var objectInfo = objectInfoList[i];
                int id = objectInfo.index;
                activingObjects[id - 1] = true;

                remainFindObjectIDList.Add(id);

                var objectElement = ContextUtils.FindElement(objectAreaElement, id.ToString(), CHILDREN);
                if (objectElement == null)
                {
                    Debug.LogError(string.Format("{0} is invalid object ID.", id));
                    continue;
                }

                var objectRectTransform = objectElement.GetComponent<RectTransform>();
                var objectRect = objectRectTransform.rect;
                float marginedWidth = GetMarginedScale(objectRect.width);
                float marginedHeight = GetMarginedScale(objectRect.height);
                objectRectTransform.sizeDelta = new Vector2(marginedWidth, marginedHeight);

#if DEV
                if (PlayerPrefs.GetInt(HiddenObjects.Defines.PLAYER_PREFS_SHOW_RECT) == 1)
                {
                    Transform parent = objectElement.transform;
                    var boundaryObj = MetaObjectUtils.MakePrefab("testsuite", "Boundary Visualizer", parent);
                    var boundaryRectTransform = boundaryObj.GetComponent<RectTransform>();
                    boundaryRectTransform.sizeDelta = new Vector2(marginedWidth, marginedHeight);
                }

                if (PlayerPrefs.GetInt(HiddenObjects.Defines.PLAYER_PREFS_SHOW_ALL) == 1)
                {
                    Transform parent = objectElement.transform;
                    var numberObj = MetaObjectUtils.MakePrefab("testsuite", "Test Object Number", parent);
                    root.UpdateContext(true);
                    var numberTextElement = numberObj.GetComponent<ContextElement>();
                    MetaContextElementUtils.SetText(numberTextElement, id.ToString());
                }
#endif

                var findObjInfo = new FindObjectInfo()
                { obj = objectElement.gameObject, info = objectInfo };
                findObjectInfoDict.Add(id, findObjInfo);
            }

            // Wrong Objects
            for (int i = 0; i < wrongObjectIndexList.Count; ++i)
            {
                int id = wrongObjectIndexList[i];
                activingObjects[id - 1] = true;
            }

            // All Objects
            for (int i = 0; i < totalObjectCount; ++i)
            {
                int id = i + 1;
                var objectElement = ContextUtils.FindElement(objectAreaElement, id.ToString(), CHILDREN);
                if (objectElement == null)
                {
                    Debug.LogError(string.Format("{0} is invalid object ID.", id));
                    continue;
                }

                MetaContextElementUtils.SetActive(objectElement, activingObjects[i]);

                // Target
                if (activingObjects[i])
                {
                    long objectScore = 0L;
                    string objectName = "";
                    if(findObjectInfoDict.TryGetValue(id, out FindObjectInfo objectInfo))
                    {
                        objectScore = objectInfo.info.score;
                        objectName = objectInfo.info.name;
                    }

                    // Target Objects has collider, blackboard
                    var targetCollider = objectElement.gameObject.AddComponent<BoxCollider>();
                    Blackboard findObjBB = objectElement.gameObject.AddComponent<Blackboard>();
                    BlackboardUtils.SetOrCreateValue(findObjBB, "objectId", id);
                    BlackboardUtils.SetOrCreateValue(findObjBB, "baseScore", objectScore);
                    BlackboardUtils.SetOrCreateValue(findObjBB, "objectName", objectName);

                    var rectTransform = objectElement.GetComponent<RectTransform>();
                    var colliderScale = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 0.1f);
                    targetCollider.size = colliderScale;

                    // Collider Enable
                    targetCollider.enabled = true;
                    objectElement.GetComponent<NonDrawingGraphic>().raycastTarget = true;
                }
            }
        }

        private float GetMarginedScale(float length)
        {
            float maxScale = (MAX_SCALE_WITH_MIN_MARGIN - MIN_SCALE_WITH_MAX_MARGIN);
            if (maxScale < 1f) return length;

            float t = (length - MIN_SCALE_WITH_MAX_MARGIN) / maxScale;
            float delta = Mathf.Lerp(MAX_MARGIN, MIN_MARGIN, t);
            return length + delta;
        }

        private void Update()
        {
            if (!isPause)
            {
                elapsedTimeMS += (long)(Time.deltaTime * 1000f);

                UpdateHintProgress();
                UpdateComboProgress();
                UpdatePenaltyStatus();

                if(useTimeUp)
                {
                    UpdateTimer();

                    if (elapsedTimeMS > timeLimit) // Time Up
                        EventSender.SendEvent(gameObject, HiddenObjects.Events.ON_CLEAR);
                }
            }
        }

        private void UpdateTimer()
        {
            long totalSec = System.Math.Max((timeLimit - elapsedTimeMS) / 1000L, 0L);
            long sec = totalSec % 60L;
            long min = totalSec / 60L;
            string timeText = TimeUtils.GetTimeTextMMSS(min, sec);

            MetaContextElementUtils.SetText(timerTextElement, timeText);

            // Warning !!
            if (totalSec <= WARNING_TIME && timeLimit > 0L)
            {
                if (totalSec > WARNING_TIME_2)
                {
                    if (warningLevel == 0)
                    {
                        warningLevel = 1;
                        GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_HURRY_UP1).Play();
                    }
                }
                else if (totalSec <= WARNING_TIME_2)
                {
                    if (warningLevel == 0)
                    {
                        warningLevel = 2;
                        GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_HURRY_UP2).Play();
                    }
                    else if (warningLevel == 1)
                    {
                        warningLevel = 2;
                        GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_HURRY_UP1).Stop();
                        GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_HURRY_UP2).Play();
                    }
                }

                float angle = elapsedTimeMS * 0.00005f * 180f;
                float y = (Mathf.Cos(angle) + 1) * 0.25f;
                Color color = new Color(1f, y, 0f);
                MetaContextElementUtils.SetColor(timerTextElement, color);
            }
        }

        private void UpdateHintProgress()
        {
            if (currentHintTargetID != NONE)
            {
                isHintReady = false;
                MetaContextElementUtils.SetSliderValue(hintProgressElement, 0f);
            }
            else if (hintRemaining > 0L)
            {
                isHintReady = false;

                hintRemaining -= TimeUtils.GetDeltaTimestamp();
                float ratio = 1f - ((float)hintRemaining / hintCooltime);
                MetaContextElementUtils.SetSliderValue(hintProgressElement, ratio);
            }
            else
            {
                if (!isHintReady)
                {
                    isHintReady = true;

                    hintAnim.SetBool("isActive", true);
                    MetaContextElementUtils.SetSliderValue(hintProgressElement, 1f);
                    MetaContextElementUtils.SetClickable(hintButtonElement, gameObject, HiddenObjects.Events.ON_USE_HINT, false);
                }
            }
        }

        private void UpdateComboProgress()
        {
            // Update Combo
            if (currentCombo > prevCombo)
            {
                prevCombo = currentCombo;

                int comboIndex = Mathf.Min(maxCombo - 1, currentCombo -1);
                currentMultiplier = comboMultiplierList[comboIndex];

                // Start Combo
                if (currentCombo == 2)
                {
                    // Appear
                    comboAnim.SetBool("isAppear", true);
                    MetaContextElementUtils.SetTextGlobal(comboMultiTextElement, "TEXT_MULTIPLIER", currentMultiplier);
                }
                else
                {
                    // Highlight ComboText
                    comboAnim.SetTrigger("isMultiple");
                    StartCoroutine(HighlightComboText(comboMultiTextElement, currentMultiplier));
                }

                remainingComboDuration = comboDuration;
                remainingProgress = 1f;

                MetaContextElementUtils.SetSliderValue(comboGaugeElement, remainingProgress);
            }
            // Finish Combo
            else if (currentCombo == 0 && prevCombo > 0)
            {
                prevCombo = currentCombo;

                comboAnim.SetBool("isAppear", false);
            }
            // Decrease Combo Time
            else if (currentCombo == prevCombo)
            {
                remainingComboDuration -= TimeUtils.GetDeltaTimestamp();
                remainingProgress = (float)remainingComboDuration / comboDuration;

                MetaContextElementUtils.SetSliderValue(comboGaugeElement, remainingProgress);

                // Reset Combo
                if (remainingComboDuration <= 0L)
                {
                    currentCombo = 0;
                }
            }
        }

        private IEnumerator HighlightComboText(ContextElement comboMultiTextElement, int currentMultiplier)
        {
            yield return new WaitForSeconds(0.12f);
            MetaContextElementUtils.SetTextGlobal(comboMultiTextElement, "TEXT_MULTIPLIER", currentMultiplier);
        }

        private void UpdatePenaltyStatus()
        {
            for (int i = penaltyStackList.Count - 1; i >= 0; --i)
            {
                penaltyStackList[i] += TimeUtils.GetDeltaTimestamp();

                if(penaltyStackList[i] >= penaltyTriggerDuration)
                {
                    penaltyStackList.RemoveAt(i);
                }
            }

            if(penaltyStackList.Count >= penaltyTriggerCount)
            {
                penaltyStackList.Clear();
                EventSender.SendEvent(gameObject, HiddenObjects.Events.ON_TRIGGER_PENALTY);
            }
        }

        public IEnumerator DisplayCoverTextCoroutine()
        {
            // Make Background Scene
            string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
            string asset = "Popup Hidden Objects In Game Text Scene";
            Transform popupArea = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject backgroundObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, popupArea,
                (GameObject sceneObj) => backgroundObj = sceneObj));

            var backgroundBB = backgroundObj.GetComponent<Blackboard>();
            // already paused = starting cover
            BlackboardUtils.SetOrCreateValue(backgroundBB, "isStartingText", isPause);
            if (!isPause) // Penalty
            {
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_STAGE_PENALTY).Play();
                BlackboardUtils.SetOrCreateValue(backgroundBB, "penaltyDuration", penaltyDuration);
            }
            else // Intro
            {
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_STAGE_READY).Play();
            }

            MetaObjectUtils.SetCalleeCaller(backgroundObj, gameObject);

            MetaPopupUtils.OpenPopup(backgroundObj);

            SetPause(true);

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        public void OnUseHint()
        {
            if (!isHintReady || IsAllTargetSearched())
                return;

            hintAnim.SetBool("isActive", false);

            // Play Sounds
            GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_HINT).Play();

            // Block Hint Button
            if (hintButtonElement is IContextClickable clickableElement)
                clickableElement.RemoveAllListener();

            int hintTargetInfoIndex = GetLongestRunningTargetIndex();
            int findTargetID = targetInfoes[hintTargetInfoIndex].id;

            // Set Hint Target
            currentHintTargetID = findTargetID;

            hintRemaining = hintCooltime;

            // Set Play Log
            var log = new HiddenUniversePlayLog
            {
                time = elapsedTimeMS,
                type = HiddenUniversePlayTypes.HINT,
                content = new HiddenUniverseHintContent(),
            };
            playLogList.Add(log);

            StartCoroutine(PlayHintEffectCoroutine(currentHintTargetID));
        }

        public IEnumerator QuitCoroutine()
        {
            bool done = false;
            BagelCodeClientAPI.RequestHiddenObjectsLeaveGame(chapter, stage,
                (response) =>
                {
                    done = true;
                    HiddenObjects.Utils.UpdateFinderCount(response.finder, response.maxFinder);
                },
                (error) =>
                {
                    done = true;
                    Debug.LogError("RequestHiddenObjectsLeaveGame failed with error: " +
                        error.errorCode.ToString());
                });

            yield return new WaitUntil(() => done);
        }

        public void OnClose(bool playAgain)
        {
            var caller = bb.GetValue<GameObject>("caller");
            var callerBB = caller.GetComponent<Blackboard>();

            // Destroy Prev Info
            BlackboardUtils.DestroyBlackboard(callerBB, "clearStageInfo");

            // Set Stage Clear Info
            if (clearResponseBB != null)
            {
                var newClearStageInfo = BlackboardUtils.GetOrCreateBlackboard(callerBB, "clearStageInfo") as Blackboard;
                var clearStageInfo = BlackboardUtils.GetOrCreateBlackboard(clearResponseBB, "stageInfo");
                BlackboardUtils.CopyBlackboard(clearStageInfo, newClearStageInfo);
                BlackboardUtils.SetOrCreateValue(newClearStageInfo, "playAgain", playAgain);
            }

            // Enable Kudo
            KudoEventManager.Instance.EnableKudo();

            EventSender.SendEvent(caller, HiddenObjects.Events.ON_FINISH_STAGE);
        }

        public void Close()
        {
            GestureManager.Instance.DisableGestureHandler(GestureManager.GestureHandlerType.CORRECT_CLICK);
            GestureManager.Instance.DisableGestureHandler(GestureManager.GestureHandlerType.SCROLL);
            GestureManager.Instance.DisableGestureHandler(GestureManager.GestureHandlerType.ZOOM_IN_OUT);

            Destroy(gameObject);
        }

        public void OnResume()
        {
            SetPause(false);

            isIntro = false;
        }

        public IEnumerator OnClearCoroutine()
        {
            if (useTimeUp && elapsedTimeMS > timeLimit)
                isTimeUp = true;

            isClear = true;

            // Stop Sound
            if (warningLevel == 1)
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_HURRY_UP1).Stop();
            else if (warningLevel == 2)
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_HURRY_UP2).Stop();
            warningLevel = -1;

            // Delay
            if (isTimeUp)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return new WaitForSeconds(2f);
            }

            SetPause(true);

            // Delay
            yield return new WaitForSeconds(0.5f);

            // Set Play Log
            if(isTimeUp)
            {
                var log = new HiddenUniversePlayLog
                {
                    time = timeLimit,
                    type = HiddenUniversePlayTypes.TIME_UP,
                    content = new HiddenUniverseTimeUpContent(),
                };
                playLogList.Add(log);
            }
            else
            {
                var log = new HiddenUniversePlayLog
                {
                    time = elapsedTimeMS,
                    type = HiddenUniversePlayTypes.CLEAR,
                    content = new HiddenUniverseClearContent(),
                };
                playLogList.Add(log);
            }

            bool isPerfect = false;

            bool isSuccess = false;
            bool isFail = false;

            string token = bb.GetValue<string>("token");
            HiddenUniversePlayClearResponse clearResponse = null;
            BagelCodeClientAPI.RequestHiddenObjectsClear(chapter, stage, token, pauseCount, playLogList,
                (response) =>
                {
                    isSuccess = true;

                    isPerfect = response.isPerfect;
                    HiddenObjects.Utils.UpdateFinderCount(response.finder, response.maxFinder);

                    BlackboardQueryUtils.AddCoins(response.earnCredit);
                    BlackboardQueryUtils.AddGems(response.earnGem);

                    clearResponse = response;
                    clearResponseBB = BlackboardUtils.GetOrCreateBlackboard(bb, "clearResponse") as Blackboard;
                    ClientAPI2Blackboard.Serialize(clearResponseBB, clearResponse);
                },
                (error) =>
                {
                    isFail = true;
                    Debug.LogError(error.errorCode);
                    // HIDDEN_UNIVERSE_IS_NOT_ACTIVE_ERROR
                    GlobalErrorHandler.GlobalError(error);
                });

            MetaSystem.BackupUserSyncInfo();

            string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
            string asset = "Popup Hidden Objects Stage Clear Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject clearPopupObj = null;
            StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => clearPopupObj = sceneLoadOperation.GetScene()));

            yield return new WaitUntil(() => (isSuccess || isFail) && clearPopupObj != null);

            if (isFail)
            {
                // GlobalErrorHandler.OpenErrorOKPopup()
                Destroy(clearPopupObj);
                yield break;
            }

            var clearPopupBB = clearPopupObj.GetComponent<Blackboard>();
            ClientAPI2Blackboard.Serialize(clearPopupBB, clearResponse);

            MetaObjectUtils.SetCalleeCaller(clearPopupObj, gameObject);

            BlackboardUtils.SetOrCreateValue(clearPopupBB, "isTimeUp", isTimeUp);
            BlackboardUtils.SetOrCreateValue(clearPopupBB, "isPerfect", isPerfect);
            BlackboardUtils.SetOrCreateValue(clearPopupBB, "needFinderCount", bb.GetValue<int>("needFinderCount"));
            BlackboardUtils.SetOrCreateValue(clearPopupBB, "prevStageInfo", bb.GetValue<HiddenUniverseStageInfo>("prevStageInfo"));

            MetaPopupUtils.OpenPopup(clearPopupObj);
        }

        public IEnumerator OnPauseCoroutine()
        {
            ++pauseCount;

            // Stop Hurry
            if (warningLevel == 1)
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_HURRY_UP1).Stop();
            else if (warningLevel == 2)
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_HURRY_UP2).Stop();
            warningLevel = 0;

            SetPause(true);

            string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
            string asset = "Popup Hidden Objects Stop Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            // Make Popup
            GameObject pausePopupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => pausePopupObj = sceneLoadOperation.GetScene()));

            MetaObjectUtils.SetCalleeCaller(pausePopupObj, gameObject);

            MetaPopupUtils.OpenPopup(pausePopupObj);
        }

        //

        private bool IsAllTargetSearched()
        {
            if (remainFindObjectIDList.Count > 0)
                return false;

            for (int i = 0; i < TARGET_COUNT; ++i)
                if (targetInfoes[i].id != NONE)
                    return false;

            return true;
        }

        private int GetLongestRunningTargetIndex()
        {
            int result = 0;
            long min = long.MaxValue;

            for (int i = 0; i < TARGET_COUNT; ++i)
                if (targetInfoes[i].updatedTime < min)
                {
                    min = targetInfoes[i].updatedTime;
                    result = i;
                }

            return result;
        }

        private void UpdateTargetFade()
        {
            for (int i = 0; i < TARGET_COUNT; ++i)
            {
                var targetInfo = targetInfoes[i];
                if (targetInfo.id != NONE)
                {
                    // Check In Screen
                    Vector3 targetPos = findObjectInfoDict[targetInfo.id].obj.transform.position;
                    bool isInScreen = MetaScreenManager.IsWorldPointInScreen(targetPos);

                    targetInfo.anim.SetBool("isFade", !isInScreen);
                }
            }
        }

        private void UpdateTarget(int i)
        {
            targetInfoes[i].anim.SetBool("isFade", false);

            var targetInfo = targetInfoes[i];

            // Check NONE
            if (targetInfo.id == NONE)
            {
                targetInfo.updatedTime = TimeUtils.GetTimeStamp();

                if (remainFindObjectIDList.Count > 0)
                {
                    int id = remainFindObjectIDList[0];
                    remainFindObjectIDList.RemoveAt(0);
                    targetInfo.id = id;

                    // Answer Objects has <HiddenObjectsInGameFindObjectController>
                    var findObj = findObjectInfoDict[id].obj;
                    findObj.AddComponent<HiddenObjectsInGameFindObjectController>();
                    Blackboard findObjBB = findObj.GetComponent<Blackboard>();

                    BlackboardUtils.SetOrCreateValue(findObjBB, "targetTextTransform", targetInfo.element.transform);
                    BlackboardUtils.SetOrCreateValue(findObjBB, "chapter", chapter);
                    BlackboardUtils.SetOrCreateValue(findObjBB, "stage", stage);

                    var info = findObjectInfoDict[id].info;
                    StartCoroutine(FlipTargetTextCoroutine(i, info.name));
                }
                else
                {
                    targetInfoes[i].anim.SetTrigger("isHide");
                }
            }
        }

        private void UpdateTargets()
        {
            for (int i = 0; i < TARGET_COUNT; ++i)
            {
                UpdateTarget(i);
            }
        }

        private void SetPause(bool _isPause)
        {
            isPause = _isPause;
            anim.SetBool("Active", !isPause || isIntro || isTimeUp);
            zoomInOutController.enabled = !isPause;
        }

        private IEnumerator PlayHintEffectCoroutine(int targetID)
        {
            var findTargetObj = findObjectInfoDict[targetID].obj;

            Transform hintTransform = hintButtonElement.transform;
            Transform findObjTransform = findTargetObj.transform;

            // Hint Projectile
            string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
            string asset = "Particle Hint Projectile";
            Transform parent = hintTransform;
            var hintProjectileObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            hintProjectileObj.transform.SetParent(particleAreaElement.transform);

            bool arrived = false;
            AsyncActionUtils.ApplyMovement(this, hintProjectileObj.transform, hintTransform, findObjTransform,
                PROJECTILE_MOVEMENT_TIME, TweenUtils.VectorTweenCollectMove, 0f, () => arrived = true);

            // Wait effect arriving || find hint target
            yield return new WaitUntil(() => arrived || currentHintTargetID == NONE);

            Destroy(hintProjectileObj);

            if (currentHintTargetID != NONE)
            {
                // Hint Effect
                asset = "Particle Hint";
                parent = findTargetObj.transform;
                hintEffectObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);

                hintEffectObj.transform.position = findObjTransform.position;
            }

            yield return new WaitUntil(() => currentHintTargetID == NONE);

            if (hintEffectObj != null)
                Destroy(hintEffectObj);
        }

        private IEnumerator FlipTargetTextCoroutine(int targetIndex, string text)
        {
            targetInfoes[targetIndex].anim.SetTrigger("isActive");

            yield return new WaitForSeconds(0.15f);

            MetaContextElementUtils.SimpleSetText(targetInfoes[targetIndex].element, "Text", text);
        }

        private void OnGestureEvent(EventData eventData)
        {
            if (isPause || isClear) return;

            var gestureType = Common.GetEnumTypeByString<GestureManager.GestureType>(eventData.name);

            switch (gestureType)
            {
                case GestureManager.GestureType.CORRECT_CLICK:
                    {
                        if (eventData.value is GestureCorrectClickData clickData)
                        {
                            var clickPos = clickData.clickPos;
                            Ray ray = new Ray(new Vector3(clickPos.x, clickPos.y, -10f), Vector3.forward);
                            var hits = Physics.RaycastAll(ray, 100f);
                            bool isHitExist = hits.Length > 0;
                            bool isAnswerExist = false;
                            GameObject firstAnswer = null;
                            GameObject firstWrong = null;

                            for (int i = 0; i < hits.Length; ++i)
                            {
                                var hit = hits[i];

                                var hitObject = hit.transform.gameObject;
                                bool isAnswer = IsAnswerObject(hitObject);
                                if (isAnswer)
                                {
                                    isAnswerExist = isAnswer;

                                    if (firstAnswer == null)
                                        firstAnswer = hitObject;
                                }
                                else
                                {
                                    if (firstWrong == null)
                                        firstWrong = hitObject;
                                }
                            }

                            // Something Clicked
                            if (isHitExist)
                            {
                                // Answer
                                if (isAnswerExist)
                                {
                                    OnClickObject(firstAnswer, clickPos);
                                }
                                // Wrong
                                else
                                {
                                    OnClickObject(firstWrong, clickPos);
                                }
                            }
                            // Without Pause, Hint Click
                            else if (!clickData.isPointerOverGameObject)
                            {
                                // Send Bi
                                SendObjectClickBIEvent(false, false);

                                OnClickWrongTarget(clickPos);
                            }
                        }
                    }
                    break;
            }
        }

        private bool IsAnswerObject(GameObject clickedObj)
        {
            if (clickedObj == null) return false;

            var targetObjectController = clickedObj.GetComponent<HiddenObjectsInGameFindObjectController>();
            return targetObjectController != null;
        }

        private void OnClickObject(GameObject clickedObj, Vector2 clickPos)
        {
            var objBB = clickedObj.GetComponent<Blackboard>();
            var targetObjectController = clickedObj.GetComponent<HiddenObjectsInGameFindObjectController>();
            if(targetObjectController == null)
            {
                // Send BI
                if (objBB != null)
                {
                    long baseScore = objBB.GetValue<long>("baseScore");
                    string objectName = objBB.GetValue<string>("objectName");
                    SendObjectClickBIEvent(true, false, objectName, baseScore, baseScore);
                }
                else
                {
                    SendObjectClickBIEvent(false, false);
                }

                OnClickWrongTarget(clickPos);
                return;
            }

            int searchedId = BlackboardUtils.FindVariable<int>(objBB, "objectId")?.value ?? -1;
            if (searchedId == -1)
            {
                Debug.LogError(string.Format("{0} is invalid hog target.", clickedObj.name));
                return;
            }

            if (ApplicationSettings.LogTest())
                Debug.Log("Find Object: " + clickedObj.name);

            for (int i = 0; i < TARGET_COUNT; ++i)
            {
                if (targetInfoes[i].id == searchedId)
                {
                    StartCoroutine(OnSearchCoroutine(i, searchedId));
                    return;
                }
            }

            // Assertion Failed
            Debug.LogError("Assertion Failed: The searched object isn't a target.");
        }

        private void OnClickWrongTarget(Vector2 clickPos)
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("Wrong Target");

            // Add Penalty Stack
            penaltyStackList.Add(0L);

            // Set Play Log
            var log = new HiddenUniversePlayLog
            {
                time = elapsedTimeMS,
                type = HiddenUniversePlayTypes.WRONG,
                content = new HiddenUniverseWrongContent(),
            };
            playLogList.Add(log);

            // Play Effect
            MakeWrongEffect(clickPos);
        }

        private void MakeFindEffect(Vector2 pos)
        {
            string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
            string asset = "Particle Find";
            Transform parent = particleAreaElement.transform;
            var findEffectObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);

            findEffectObj.transform.position = pos;
        }

        private void MakeWrongEffect(Vector2 pos)
        {
            string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
            string asset = "Particle Wrong";
            Transform parent = particleAreaElement.transform;
            var wrongEffectObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);

            wrongEffectObj.transform.position = pos;
        }

        private IEnumerator OnSearchCoroutine(int targetIndex, int searchedId)
        {
            if (!findObjectInfoDict.ContainsKey(searchedId)) yield break;

            var targetObj = findObjectInfoDict[searchedId].obj;

            // Reset Target Info
            targetInfoes[targetIndex].id = NONE;
            targetInfoes[targetIndex].updatedTime = long.MaxValue;

            long originScore = findObjectInfoDict[searchedId].info.score;
            string objectName = findObjectInfoDict[searchedId].info.name;

            MetaObjectUtils.SetCalleeCaller(targetObj, gameObject);

            // Set Order
            var targetBB = targetObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(targetBB, "findOrder", findCount);
            ++findCount;

            // Set Play Log
            var log = new HiddenUniversePlayLog
            {
                time = elapsedTimeMS,
                type = HiddenUniversePlayTypes.FIND,
                content = new HiddenUniverseFindContent()
                { objectIndex = searchedId, score = originScore }
            };
            playLogList.Add(log);

            // Play Find Effect
            MakeFindEffect(targetObj.transform.position);

            // Add Combo
            ++currentCombo;
            UpdateComboProgress();

            // Show Score, Move Find Object
            int comboIndex = Mathf.Min(maxCombo - 1, currentCombo - 1);
            long multipliedScore = originScore * comboMultiplierList[comboIndex];
            var eventData = new EventData<long>(HiddenObjects.Events.ON_SEARCH, multipliedScore);
            EventSender.SendEvent(targetObj, MetaEventDefine.ON_META_UI_EVENT, eventData);

            // Send BI
            SendObjectClickBIEvent(true, true, objectName, originScore, multipliedScore, comboMultiplierList[comboIndex]);

            // Play Sounds
            if (currentCombo < 2)
            {
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_OBJECT_NOCOMBO).Play();
            }
            else
            {
                int comboSound = Mathf.Min(MAX_COMBO_SOUND, currentCombo);
                string handlerId = string.Format(HiddenObjects.Defines.SOUNDS_OBJECT_COMBO_FORMAT, comboSound);
                GSManager.Instance.GetHandler(handlerId).Play();
            }

            // Reset Hint Target
            if (currentHintTargetID == searchedId)
                currentHintTargetID = NONE;

            // Reset Penalty
            penaltyStackList.Clear();

            // Fade Target Text
            targetInfoes[targetIndex].anim.SetBool("isFade", true);

            // Disable Image
            var targetElement = targetObj.GetComponent<ContextElement>();
            MetaContextElementUtils.SimpleSetActive(targetElement, "Image", false);
            targetObj.GetComponent<NonDrawingGraphic>().raycastTarget = false;

            // Check Clear
            if (IsAllTargetSearched())
                EventSender.SendEvent(gameObject, HiddenObjects.Events.ON_CLEAR);

            // Wait until target destroyed
            var onFindObjectDestroyedTrigger = new EventTrigger(gameObject, HiddenObjects.Events.ON_FIND_OBJECT_DESTROYED);
            yield return new WaitUntil(() =>
                onFindObjectDestroyedTrigger.IsTrigger &&
                (onFindObjectDestroyedTrigger.EventData as EventData<int>).value == searchedId);

            // Update Target
            UpdateTarget(targetIndex);
        }

        private void SendObjectClickBIEvent(bool isTarget, bool isAnswer, string objectName = "", long baseScore = 0L, long score = 0L, int multi = 0)
        {
            if (isTarget)
            {
                Dictionary<string, object> customData = new Dictionary<string, object>();
                customData["chapter"] = chapter;
                customData["stage"] = stage;
                customData["chapter_star_count"] = chapterTotalStarCount;
                customData["star"] = stageStarCount;
                customData["time_limit"] = (int)timeLimit;
                customData["object"] = objectName;
                customData["is_correct_answer"] = isAnswer;
                customData["base_score"] = baseScore;
                customData["score"] = score;
                customData["type"] = "object";
                customData["bonus_multiplier"] = (long)multi;
                customData["hog_type"] = "default";
                customData["earn_coin"] = 0L;
                customData["jackpot_index"] = 0;
                customData["symbol"] = HiddenObjects.Utils.ChapterNumberToSymbol(chapter);
                Analytics.CustomEvent("client_click_hog_object", customData);
            }
            else
            {
                Dictionary<string, object> customData = new Dictionary<string, object>();
                customData["chapter"] = chapter;
                customData["stage"] = stage;
                customData["chapter_star_count"] = chapterTotalStarCount;
                customData["star"] = stageStarCount;
                customData["time_limit"] = (int)timeLimit;
                customData["object"] = null;
                customData["is_correct_answer"] = false;
                customData["base_score"] = null;
                customData["score"] = null;
                customData["type"] = "blank";
                customData["bonus_multiplier"] = (long)multi;
                customData["hog_type"] = "default";
                customData["earn_coin"] = 0L;
                customData["jackpot_index"] = 0;
                customData["symbol"] = HiddenObjects.Utils.ChapterNumberToSymbol(chapter);
                Analytics.CustomEvent("client_click_hog_object", customData);
            }
        }
    }
}
