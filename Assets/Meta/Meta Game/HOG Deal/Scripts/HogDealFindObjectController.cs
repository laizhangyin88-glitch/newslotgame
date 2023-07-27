using UnityEngine;
using ParadoxNotion;
using SlotMaker;
using BagelCode.ClientModels;
using System.Collections;

namespace BagelCode.HiddenObjects
{
    public class HogDealFindObjectController : EventMonoBehaviour
    {
        private ContextElement root;
        private new BoxCollider collider;

        private bool isSearched = false;

        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        private void Start()
        {
            root = GetComponent<ContextElement>();
            collider = GetComponent<BoxCollider>();

            root.UpdateContext(false);

            // Init Events
            InitRouter();
            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_SEARCH, OnSearch);
        }

        private void OnSearch(EventData eventData)
        {
            if (isSearched) return;
            isSearched = true;

            // Collider Disable
            collider.enabled = false;
            GetComponent<NonDrawingGraphic>().raycastTarget = false;

            // Set Material Null
            var imageElement = ContextUtils.FindElement(root, "Image", CHILDREN);
            imageElement.GetComponent<UnityEngine.UI.Image>().material = null;

            var prizeTypePrize = ((Transform, Transform, HogDealPrizeType, long))eventData.value;
            StartCoroutine(OnSearchCoroutine(prizeTypePrize));
        }

        private IEnumerator OnSearchCoroutine((Transform, Transform, HogDealPrizeType, long) searchInfo)
        {
            Transform targetTransform = searchInfo.Item2;

            // Circle
            string bundle = HogDeal.Defines.CONTENTS_BUNDLE;
            string asset = "Hog Deal Object Correct Circle";
            Transform parent = searchInfo.Item1;

            var correctCircleObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            correctCircleObj.transform.position = transform.position;

            var prizeType = searchInfo.Item3;
            long prize = searchInfo.Item4;

            // Jackpot
            bool isJackpot = prizeType != HogDealPrizeType.DEFAULT;
            if (isJackpot)
            {
                string prizeText = TextDecoUtils.EnumTypeToText<HogDealPrizeType>((int)prizeType, TextDecoUtils.TextFormat.PASCAL_CASE);
                asset = string.Format("Hog Deal Object {0} Jackpot", prizeText);

                var jackpotObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
                jackpotObj.transform.position = transform.position;

                jackpotObj.SetActive(true);

                // Play Jackpot Sound
                string soundKey = string.Format("Meta_HOG_Object_{0}_Win", prizeText);
                GSManager.Instance.GetHandler(soundKey).Play();
            }

            // Score
            asset = "Hog Deal Object Score Text";
            var prizeTextObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            prizeTextObj.transform.position = transform.position;

            var prizeTextElement = prizeTextObj.GetComponent<ContextElement>();

            prizeTextElement.UpdateContext(true);

            var targetScoreTextElement = ContextUtils.FindElement(prizeTextElement,
                isJackpot ? "Anchor/Text Jackpot Score" : "Anchor/Text Normal Score", FULL);

            var targetScoreTextObj = targetScoreTextElement.gameObject;
            targetScoreTextObj.SetActive(true);

            MetaContextElementUtils.SetTextGlobal(targetScoreTextElement, "TEXT_COMMA_NUMBER", prize);

            // Fly
            var anchorElement = ContextUtils.FindElement(prizeTextElement, "Anchor", CHILDREN);
            var controller = anchorElement.GetComponent<DirectionalWeightPositionController>();

            var anim = prizeTextObj.GetComponent<Animator>();
            anim.SetTrigger("isFly");

            controller.from = prizeTextObj.transform;
            controller.to = targetTransform;
            bool isLeft = transform.position.x < targetTransform.position.x;
            controller.width = isLeft ? -0.8f : 0.8f;

            const float FLYING_TIME = 2f;
            yield return new WaitForSeconds(FLYING_TIME);

            EventSender.SendCalleeCallback(gameObject, prize, HiddenObjects.Events.ON_ARRIVE_PRIZE);

            // Play Disappear Sound
            GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_OBJECT_DISAPPEAR).Play();

            yield return new WaitForSeconds(0.5f);

            var scoreTextAnim = prizeTextObj.GetComponent<Animator>();
            scoreTextAnim.SetTrigger("Destroy");
        }
    }
}
