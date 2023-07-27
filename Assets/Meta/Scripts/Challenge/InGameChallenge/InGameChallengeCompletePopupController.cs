using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;
using System.Collections;
using System.Collections.Generic;

namespace BagelCode
{
    public class InGameChallengeCompletePopupController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private Blackboard completeInfo;
        private ChallengeType challengeType;

        private void Start()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            completeInfo = bb.GetValue<Blackboard>("_completeInfo");
            challengeType = BlackboardUtils.FindVariable<ChallengeType>(completeInfo, "challenge/challengeType").value;

            // Claim Button
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Claim/Text", "BUTTON_CLAIM", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SimpleSetClickable(root, "Button Claim", OpenChallengePopup);

            // Title Text
            var titleText = StringTableUtils.GetString(StringTable.StringTableType.Global, "CHALLENGE_COMPLETE_TITLE", challengeType.ToString());
            MetaContextElementUtils.SimpleSetText(root, "Text Title", titleText);

            // Reward Text
            var rewardList = BlackboardUtils.FindVariable<List<Blackboard>>(completeInfo, "challenge/rewardList");
            long eventMultiplierNumerator = GetPassiveEventMultiplierNumerator();
            var rewardText = BlackboardQueryUtils.GetChallengeInfoRewardResultText(rewardList.value, eventMultiplierNumerator);
            MetaContextElementUtils.SimpleSetText(root, "Text Reward Amount", rewardText);

            // Anim
            anim.SetBool("IsActive", true);

            // Event Tag
            bool isEvent = false;
            long endTimestamp = 0L;
            if(challengeType == ChallengeType.EVENT)
            {
                var eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.PERSONAL_EVENT_CHALLENGE);
                if(eventInfo != null)
                {
                    var multiplierTextElement = ContextUtils.FindElement(root, "Event Tag/Text", ContextSearchingType.FullNameSearch);
                    MetaContextElementUtils.SetTextGlobal(multiplierTextElement, "LOBBY_CHALLENGE_BUTTON_EVENT_TAG_TEXT");

                    endTimestamp = eventInfo.endTimestamp;
                    isEvent = true;
                }
            }
            else
            {
                var eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CHALLENGE_CLAIM_MULTIPLY);
                if(eventInfo != null)
                {
                    var multiplierTextElement = ContextUtils.FindElement(root, "Event Tag/Text", ContextSearchingType.FullNameSearch);
                    var multiplier = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);
                    MetaContextElementUtils.SetTextGlobal(multiplierTextElement, "TEXT_COMMON_PASSIVE_EVENT_MULTIPLIER", multiplier);

                    endTimestamp = eventInfo.endTimestamp;
                    isEvent = true;
                }
            }

            if (isEvent)
            {
                var timerElement = ContextUtils.FindElement(root, "Event Tag/Remaining Timer", ContextSearchingType.FullNameSearch);
                EventSender.SendEvent(timerElement.gameObject, new ParadoxNotion.EventData<long>("OnStartTimer", endTimestamp));

                anim.SetBool("IsEvent", true);
            }
            else
            {
                anim.SetBool("IsEvent", false);
            }

            StartCoroutine(CloseCoroutine());
        }

        private IEnumerator CloseCoroutine()
        {
            yield return new WaitForSeconds(4f);

            Close();
        }

        private void OpenChallengePopup()
        {
            StartCoroutine(OpenChallengePopupCoroutine());
        }

        private IEnumerator OpenChallengePopupCoroutine()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Challenge Scene";
            Transform root = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject challengePopupObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, root,
                (GameObject sceneObj) => challengePopupObj = sceneObj));

            challengePopupObj.name = "Popup Challenge";

            Blackboard popupBB = challengePopupObj.GetComponent<Blackboard>();
            popupBB.AddVariable("caller", gameObject);

            MetaPopupUtils.OpenPopup(challengePopupObj);

            Close();
        }

        private void Close()
        {
            EventSender.SendCalleeCallback(gameObject);
            anim.SetBool("IsActive", false);
            Destroy(gameObject);
        }

        public long GetPassiveEventMultiplierNumerator()
        {
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CHALLENGE_CLAIM_MULTIPLY);

            var challengeType = BlackboardUtils.FindVariable<ChallengeType>(completeInfo, "challenge/challengeType");
            if (challengeType.value == ChallengeType.EVENT)
            {
                return NumberUtils.GetGlobalDenominator();
            }

            if (eventInfo != null)
                return PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);

            return NumberUtils.GetGlobalDenominator();
        }
    }
}
