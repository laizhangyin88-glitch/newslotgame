using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using Sirenix.OdinInspector;
using ParadoxNotion;
using System.Collections;
using System.Linq;

namespace BagelCode.HiddenObjects
{
    public class HogDealInGameController : EventMonoBehaviour
    {
        // Settings
        [Title("Object Collider")]
        public float MIN_SCALE_WITH_MAX_MARGIN;
        public float MAX_MARGIN;
        public float MAX_SCALE_WITH_MIN_MARGIN;
        public float MIN_MARGIN;

        // Define
        private const int TARGET_COUNT = 3;
        private const int NONE = -1;
        private const float PROJECTILE_MOVEMENT_TIME = 0.6f;
        //

        private class FindObjectInfo
        {
            public GameObject obj;
            public HogDealObjectInfo info;
        }

        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private string chapterSymbol;
        private int stage;

        private HogDealJackpotType jackpotType = HogDealJackpotType.UNKNOWN;

        private long elapsedTimeMS = 0L;
        private long totalPrize = 0L;

        private bool isPause = false;
        private bool isInit = false;

        private ContextElement objectAreaElement;

        // Find Object
        private int findCount = 0;
        private List<HogDealObjectInfo> objectInfoList = new List<HogDealObjectInfo>();
        private List<int> remainFindObjectIDList = new List<int>();

        private HiddenObjectsInGameTargetInfo[] targetInfoes = new HiddenObjectsInGameTargetInfo[TARGET_COUNT];
        private Dictionary<int, FindObjectInfo> findObjectInfoDict = new Dictionary<int, FindObjectInfo>(); // key: id

        private class HogDealObjectInfo
        {
            public int index;
            public string name;
        }

        // Prize
        private List<long> prizeList = new List<long>();
        private List<HogDealPrizeType> prizeTypeList = new List<HogDealPrizeType>();

        // Play Log
        private List<HogDealPlayLog> playLogList = new List<HogDealPlayLog>();

        // Combo
        private int currentCombo = 0;
        private int prevCombo = -1;
        private long remainingComboDuration = 0L;
        private float remainingComboProgress = 0f;

        private int maxCombo;
        private long comboDuration;
        private List<long> comboMultiplierList = new List<long>();

        private Animator comboAnim;
        private ContextElement comboAreaElement;
        private ContextElement comboRewardTextElement;
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

        private const int MAX_COMBO_SOUND = 5;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        [Button]
        private void SetPrize(HogDealPrizeType prizeType)
        {
            prizeTypeList[findCount] = prizeType;
        }

        //

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            chapterSymbol = bb.GetValue<string>("symbol");
            stage = bb.GetValue<int>("stage");
            comboDuration = bb.GetValue<long>("comboDuration");
            hintCooltime = bb.GetValue<long>("hintCooltime");

            // Parse object list
            var objectBBList = bb.GetValue<List<Blackboard>>("objectList");
            for(int i = 0; i < objectBBList.Count; ++i)
            {
                objectInfoList.Add(new HogDealObjectInfo()
                {
                    index = objectBBList[i].GetValue<int>("index"),
                    name = objectBBList[i].GetValue<string>("name"),
                });
            }

            // Parse combo multi
            var comboMultiBBList = bb.GetValue<List<Blackboard>>("comboMultiplier");
            int combo = 1;
            for (int i = 0; i < comboMultiBBList.Count; ++i)
            {
                int comboCount = comboMultiBBList[i].GetValue<int>("comboCount");
                long multiplier = comboMultiBBList[i].GetValue<long>("multiplierNumerator");
                while (combo <= comboCount)
                {
                    comboMultiplierList.Add(multiplier);
                    ++combo;
                }
            }
            maxCombo = comboMultiplierList.Count;

            // Parse prize list
            var prizeBBList = bb.GetValue<List<Blackboard>>("prizeList");
            for (int i = 0; i < prizeBBList.Count; ++i)
            {
                prizeList.Add(prizeBBList[i].GetValue<long>("credit"));
                prizeTypeList.Add(prizeBBList[i].GetValue<HogDealPrizeType>("type"));

#if DEV
                if(PlayerPrefs.GetInt(HogDeal.Defines.PLAYER_PREFS_JACKPOT) == 1)
                {
                    if (i < 5) prizeTypeList[i] = (HogDealPrizeType)i;
                }
#endif
            }

            // Disable Kudo
            KudoEventManager.Instance.DisableKudo();

            // Elements
            string stageName = string.Format("{0} Stage {1}", chapterSymbol, stage);
            objectAreaElement = ContextUtils.FindElement(root, "Stage Area/" + stageName + "/Area", FULL);

            particleAreaElement = ContextUtils.FindElement(root, "Particle Area", CHILDREN);

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

            // Combo
            comboAreaElement = ContextUtils.FindElement(root, "Combo Area", CHILDREN);
            comboRewardTextElement = ContextUtils.FindElement(comboAreaElement, "Combo UI Base/Text", FULL);
            comboGaugeElement = ContextUtils.FindElement(comboAreaElement, "Multiple Base/Progress Bar", FULL);
            comboMultiTextElement = ContextUtils.FindElement(comboAreaElement, "Multiple Base/Text", FULL);
            MetaContextElementUtils.SetText(comboRewardTextElement, "0");
            MetaContextElementUtils.SimpleSetTextGlobal(comboAreaElement, "Multiple Base/Combo Text", "POPUP_HOG_DEAL_IN_GAME_COMBO", FULL);
            comboAnim = comboAreaElement.GetComponent<Animator>();

            // 
            MetaContextElementUtils.SimpleSetTextGlobal(comboAreaElement, "Bonus Text", "HIDDEN_OBJECTS_IN_GAME_BONUS", CHILDREN);

            // Hint
            hintButtonElement = ContextUtils.FindElement(root, "Hint Area", CHILDREN);
            hintProgressElement = ContextUtils.FindElement(hintButtonElement, "Progress Bar", CHILDREN);
            MetaContextElementUtils.SetClickable(hintButtonElement, gameObject, HiddenObjects.Events.ON_USE_HINT, false);

            hintAnim = hintButtonElement.GetComponent<Animator>();
            hintAnim.SetBool("isActive", true);

            // Handle Events
            GestureManager.Instance.EnableGestureHandler(GestureManager.GestureHandlerType.CORRECT_CLICK);

            MessageDispatcher.Register(GestureManager.ON_GESTURE_EVENT, OnGestureEvent);

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(HiddenObjects.Events.ON_USE_HINT, OnUseHint);
            Register(HiddenObjects.Events.ON_ARRIVE_PRIZE, OnArrivePrize);

            // Update All
            UpdateTargets();

            // Active
            anim.SetTrigger("Active");
            comboAnim.SetBool("isAppear", true);

            isInit = true;
        }

        public IEnumerator OnClearCoroutine()
        {
            isPause = true;

            // Set Play Log
            var log = new HogDealPlayLog
            {
                time = elapsedTimeMS,
                type = HogDealPlayTypes.CLEAR,
                content = new HogDealClearContent(),
            };
            playLogList.Add(log);

            bool isSuccess = false;
            bool isFail = false;

            // Request Collect
            if (ApplicationSettings.LogTest())
                Debug.Log("Start RequestHogDealCollect");

            BagelCodeClientAPI.RequestHogDealCollect(playLogList, "", // todo set contextId
                (response) =>
                {
                    if (ApplicationSettings.LogTest())
                        Debug.Log("Success RequestHogDealCollect");

                    isSuccess = true;
                    
                    jackpotType = response.jackpotType;
                    BlackboardQueryUtils.AddCoins(response.earnCredit);
                    BlackboardQueryUtils.ApplyUserSyncInfo();
                },
                (error) =>
                {
                    if (ApplicationSettings.LogTest())
                        Debug.Log("Failed RequestHogDealCollect");

                    isFail = true;
                    Debug.LogError(error.errorCode);
                    GlobalErrorHandler.GlobalError(error);
                });

            var timerTrigger = new TimerTrigger(3f); // Delay
            var conditionTrigger = new WaitUntilConditionTrigger(() => isSuccess || isFail);
            yield return new WaitUntilTrigger(TrueCase.ALL_TRUE, timerTrigger, conditionTrigger);

            // Hide Combo
            comboAnim.SetBool("isAppear", false);

            if (isFail)
            {
                Debug.LogError("RequestHogDealCollect failed");
                // GlobalErrorHandler.OpenErrorOKPopup()
                yield break;
            }

            // Make Clear Popup
            string bundle = HogDeal.Defines.CONTENTS_BUNDLE;
            string asset = "Popup Hog Deal Game Clear Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject clearPopupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (GameObject popupObj) => clearPopupObj = popupObj));

            var clearPopupBB = clearPopupObj.GetComponent<Blackboard>();

            int remainingCount = bb.GetValue<int>("remainingCount");

            // Set Popup Data
            BlackboardUtils.SetOrCreateValue(clearPopupBB, "jackpotType", jackpotType);
            BlackboardUtils.SetOrCreateValue(clearPopupBB, "totalPrize", totalPrize);
            BlackboardUtils.SetOrCreateValue(clearPopupBB, "remainingCount", remainingCount);
            MetaObjectUtils.SetCalleeCaller(clearPopupObj, gameObject);

            MetaPopupUtils.OpenPopup(clearPopupObj);
        }

        public void PlayNext()
        {
            // caller is inbox cell
            var caller = bb.GetValue<GameObject>("caller");
            EventSender.SendEvent(caller, new EventData<bool>(InboxEvent.ACCEPT_NEXT_INBOX_ITEM, false));
        }

        public void SendCallback(bool isPlayNext)
        {
            // Add Total Prize Info
            var caller = bb.GetValue<GameObject>("caller");
            var callerBB = caller.GetComponent<Blackboard>();

            BlackboardQueryUtils.AddHogDealTotalPrizeInfo(callerBB, totalPrize, jackpotType);

            EventSender.SendCalleeCallback(gameObject, (isPlayNext, gameObject));
        }

        public void Close()
        {
            MetaPopupUtils.ClosePopup(gameObject);
        }

        private void Update()
        {
            if (isInit && !isPause)
            {
                UpdateHintProgress();
                UpdateComboProgress();

                elapsedTimeMS += (long)(Time.deltaTime * 1000f);
            }
        }

        private void InitObjects()
        {
            // Find Objects
            for (int i = 0; i < objectInfoList.Count; ++i)
            {
                var objectInfo = objectInfoList[i];
                int id = objectInfo.index;

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

                var findObjInfo = new FindObjectInfo()
                { obj = objectElement.gameObject, info = objectInfo };
                findObjectInfoDict.Add(id, findObjInfo);

                MetaContextElementUtils.SetActive(objectElement, true);

                string objectName = findObjInfo.info.name;

                var targetCollider = objectElement.gameObject.AddComponent<BoxCollider>();
                Blackboard findObjBB = objectElement.gameObject.AddComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue(findObjBB, "objectId", id);
                BlackboardUtils.SetOrCreateValue(findObjBB, "objectName", objectName);

                MetaObjectUtils.SetCalleeCaller(objectElement.gameObject, gameObject);

                var rectTransform = objectElement.GetComponent<RectTransform>();
                var colliderScale = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 0.1f);
                targetCollider.size = colliderScale;

                // Collider Enable
                targetCollider.enabled = true;
                objectElement.GetComponent<NonDrawingGraphic>().raycastTarget = true;
            }
        }

        private void OnGestureEvent(EventData eventData)
        {
            if (isPause) return;

            var gestureType = Common.GetEnumTypeByString<GestureManager.GestureType>(eventData.name);

            switch (gestureType)
            {
                case GestureManager.GestureType.CORRECT_CLICK:
                    {
                        if (eventData.value is GestureCorrectClickData clickData)
                        {
                            var clickPos = clickData.clickPos;
                            Ray ray = new Ray(new Vector3(clickPos.x, clickPos.y, -10f), Vector3.forward);
                            bool isHit = Physics.Raycast(ray, out RaycastHit hitInfo, 100f);
                            if (isHit)
                            {
                                // Find
                                OnClickObject(hitInfo.transform.gameObject, clickPos);
                            }
                            // Hint
                            else if (clickData.clickedGameObject != null &&
                                clickData.clickedGameObject.name == "Hint Area")
                            {
                                if (ApplicationSettings.LogTest())
                                    Debug.Log("Click Hint");
                            }
                            else
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

        public void TapToContinue()
        {
            isPause = true;

            string bundle = HogDeal.Defines.CONTENTS_BUNDLE;
            string asset = "Popup Hog Deal In Game Text";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var tapObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            MetaObjectUtils.SetCalleeCaller(tapObj, gameObject);
            MetaPopupUtils.OpenPopup(tapObj);
        }

        public void Play()
        {
            isPause = false;
        }

        private void OnUseHint()
        {
            if (!isHintReady || IsAllTargetSearched())
                return;

            hintAnim.SetBool("isActive", false);

            // Set Play Log
            var log = new HogDealPlayLog
            {
                time = elapsedTimeMS,
                type = HogDealPlayTypes.HINT,
                content = new HogDealHintContent(),
            };
            playLogList.Add(log);

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

            StartCoroutine(PlayHintEffectCoroutine(currentHintTargetID));
        }

        private void UpdateComboProgress()
        {
            // Update Combo
            if (currentCombo > prevCombo)
            {
                prevCombo = currentCombo;

                // Start Combo
                if (currentCombo == 0)
                {
                    // Appear
                    comboAnim.SetBool("isComboFx", false);
                    UpdateComboText();

                    remainingComboDuration = 0L;
                    remainingComboProgress = 0f;
                }
                else
                {
                    // Highlight ComboText
                    comboAnim.SetBool("isComboFx", true);
                    if (currentCombo <= maxCombo)
                        comboAnim.SetTrigger("isMultiple");

                    StartCoroutine(UpdateComboTextCoroutine());

                    remainingComboDuration = comboDuration;
                    remainingComboProgress = 1f;
                }

                MetaContextElementUtils.SetSliderValue(comboGaugeElement, remainingComboProgress);

            }
            // Finish Combo
            else if (currentCombo == 0 && prevCombo > 0)
            {
                prevCombo = currentCombo;
            }
            // Decrease Combo Time
            else if (currentCombo == prevCombo)
            {
                remainingComboDuration -= TimeUtils.GetDeltaTimestamp();
                remainingComboProgress = (float)remainingComboDuration / comboDuration;

                MetaContextElementUtils.SetSliderValue(comboGaugeElement, remainingComboProgress);

                // Reset Combo
                if (remainingComboDuration <= 0L)
                {
                    comboAnim.SetBool("isComboFx", false);

                    currentCombo = 0;
                    UpdateComboText();
                }
            }
        }

        private IEnumerator UpdateComboTextCoroutine(float delay = 0.12f)
        {
            yield return new WaitForSeconds(delay);

            UpdateComboText();
        }

        private void UpdateComboText()
        {
            int combo = Mathf.Min(maxCombo, currentCombo);
            MetaContextElementUtils.SetText(comboMultiTextElement, combo.ToString());
        }

        private float GetMarginedScale(float length)
        {
            float maxScale = (MAX_SCALE_WITH_MIN_MARGIN - MIN_SCALE_WITH_MAX_MARGIN);
            if (maxScale < 1f) return length;

            float t = (length - MIN_SCALE_WITH_MAX_MARGIN) / maxScale;
            float delta = Mathf.Lerp(MAX_MARGIN, MIN_MARGIN, t);
            return length + delta;
        }

        private IEnumerator PlayHintEffectCoroutine(int targetID)
        {
            var findTargetObj = findObjectInfoDict[targetID].obj;

            Transform hintTransform = hintButtonElement.transform;
            Transform findObjTransform = findTargetObj.transform;

            // Hint Projectile
            string bundle = HogDeal.Defines.CONTENTS_BUNDLE;
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

                    var findObj = findObjectInfoDict[id].obj;
                    findObj.AddComponent<HogDealFindObjectController>();
                    Blackboard findObjBB = findObj.GetComponent<Blackboard>();
                    var objectElement = findObj.GetComponent<ContextElement>();

                    // Set Material
                    var shaderData = HogDealCustomShaderData.Instance.data;
                    if (shaderData != null)
                    {
                        var imageElement = ContextUtils.FindElement(objectElement, "Image", ContextSearchingType.ChildrenSearch);
                        var image = imageElement.GetComponent<UnityEngine.UI.Image>();

                        Material targetMaterial = shaderData.defaultMaterial;
                        var matInfo = shaderData.materialList.FirstOrDefault(m => m.chapterSymbol == chapterSymbol && m.stageNumber == stage);
                        if (matInfo != null)
                        {
                            if(matInfo.targetMaterial != null)
                            {
                                targetMaterial = matInfo.targetMaterial;
                            }
                            if(matInfo.attachComponent != null)
                            {
                                imageElement.gameObject.AddComponent(matInfo.attachComponent.GetType());
                            }
                        }
                        image.material = targetMaterial;
                    }
                    else
                    {
                        Debug.LogError("shaderData is null.");
                    }

                    BlackboardUtils.SetOrCreateValue(findObjBB, "targetTextTransform", targetInfo.element.transform);
                    BlackboardUtils.SetOrCreateValue(findObjBB, "chapter", chapterSymbol);
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

        private IEnumerator FlipTargetTextCoroutine(int targetIndex, string text)
        {
            targetInfoes[targetIndex].anim.SetTrigger("isActive");

            yield return new WaitForSeconds(0.15f);

            MetaContextElementUtils.SimpleSetText(targetInfoes[targetIndex].element, "Text", text);
        }

        private void OnClickObject(GameObject clickedObj, Vector2 clickPos)
        {
            var objBB = clickedObj.GetComponent<Blackboard>();
            var targetObjectController = clickedObj.GetComponent<HogDealFindObjectController>();
            if (targetObjectController == null)
            {
                // Send BI
                if (objBB != null)
                {
                    string objectName = objBB.GetValue<string>("objectName");
                    SendObjectClickBIEvent(true, false, objectName);
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
                    OnSearch(i, searchedId);
                    return;
                }
            }
        }

        private void OnClickWrongTarget(Vector2 clickPos)
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("Wrong Target");

            // Set Play Log
            var log = new HogDealPlayLog
            {
                time = elapsedTimeMS,
                type = HogDealPlayTypes.WRONG,
                content = new HogDealWrongContent(),
            };
            playLogList.Add(log);

            // Play Effect
            MakeWrongEffect(clickPos);
        }

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

        private void MakeWrongEffect(Vector2 pos)
        {
            string bundle = HogDeal.Defines.CONTENTS_BUNDLE;
            string asset = "Particle Wrong";
            Transform parent = particleAreaElement.transform;
            var wrongEffectObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);

            wrongEffectObj.transform.position = pos;
        }

        private void SendObjectClickBIEvent(bool isTarget, bool isAnswer, string objectName = "", long multi = 0, int jackpotIndex = 0, long earnCoin = 0L)
        {
            if (isTarget)
            {
                Dictionary<string, object> customData = new Dictionary<string, object>();
                customData["chapter"] = 0;
                customData["stage"] = stage;
                customData["chapter_star_count"] = 0;
                customData["star"] = 0;
                customData["time_limit"] = 0;
                customData["object"] = objectName;
                customData["is_correct_answer"] = isAnswer;
                customData["base_score"] = 0L;
                customData["score"] = 0L;
                customData["type"] = jackpotIndex > 0 ? "jackpot_object" : "object";
                customData["bonus_multiplier"] = multi;
                customData["hog_type"] = "hog_deal";
                customData["earn_coin"] = earnCoin;
                customData["jackpot_index"] = jackpotIndex;
                customData["symbol"] = chapterSymbol;
                Analytics.CustomEvent("client_click_hog_object", customData);
            }
            else
            {
                Dictionary<string, object> customData = new Dictionary<string, object>();
                customData["chapter"] = 0;
                customData["stage"] = stage;
                customData["chapter_star_count"] = 0;
                customData["star"] = 0;
                customData["time_limit"] = 0;
                customData["object"] = null;
                customData["is_correct_answer"] = false;
                customData["base_score"] = null;
                customData["score"] = null;
                customData["type"] = "blank";
                customData["bonus_multiplier"] = multi;
                customData["hog_type"] = "hog_deal";
                customData["earn_coin"] = 0L;
                customData["jackpot_index"] = 0;
                customData["symbol"] = chapterSymbol;
                Analytics.CustomEvent("client_click_hog_object", customData);
            }
        }

        private void OnSearch(int targetIndex, int searchedId)
        {
            var targetObj = findObjectInfoDict[searchedId].obj;
            targetInfoes[targetIndex].id = NONE;
            targetInfoes[targetIndex].updatedTime = long.MaxValue;
            long originPrize = prizeList[findCount];
            var prizeType = prizeTypeList[findCount];

            MetaObjectUtils.SetCalleeCaller(targetObj, gameObject);

            // Set Order
            var targetBB = targetObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(targetBB, "findOrder", findCount);
            ++findCount;

            // Set Play Log
            var log = new HogDealPlayLog
            {
                time = elapsedTimeMS,
                type = HogDealPlayTypes.FIND,
                content = new HogDealFindContent()
                { objectIndex = searchedId, score = originPrize }
            };
            playLogList.Add(log);

            // Add Combo
            ++currentCombo;
            UpdateComboProgress();

            // Show Score, Move Find Object
            int comboIndex = Mathf.Min(maxCombo, currentCombo) - 1;
            var currentMultiplierNumerator = comboMultiplierList[comboIndex];
            long multipliedPrize = NumberUtils.GetMultiplierNumeratorValue(originPrize, currentMultiplierNumerator);

            Transform parent = particleAreaElement.transform;
            Transform targetTransform = comboRewardTextElement.transform;

            // Send BI
            string objectName = findObjectInfoDict[searchedId].info.name;
            SendObjectClickBIEvent(true, true, objectName, comboMultiplierList[comboIndex], (int)prizeType, multipliedPrize);

            // Search
            var eventData = new EventData<(Transform, Transform, HogDealPrizeType, long)>
                (HiddenObjects.Events.ON_SEARCH, (parent, targetTransform, prizeType, multipliedPrize));
            EventSender.SendEvent(targetObj, MetaEventDefine.ON_META_UI_EVENT, eventData);

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

            // Check Clear
            if (IsAllTargetSearched())
                EventSender.SendEvent(gameObject, HiddenObjects.Events.ON_CLEAR);

            // Update Target
            UpdateTarget(targetIndex);
        }

        private void OnArrivePrize(EventData eventData)
        {
            if(eventData.value is long prize)
            {
                totalPrize += prize;
                MetaContextElementUtils.SetTextGlobal(comboRewardTextElement, "TEXT_COMMA_NUMBER", totalPrize);
            }
        }
    }
}
