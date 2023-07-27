using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class ClubChallengeMultipleStageController : MonoBehaviour
    {
        public ContextElement areaElement;

        // Multiple Common. 
        public ContextElement gaugeElement;
        public ContextElement gaugeFillElement;
        public ContextElement stageElement;
        public ContextElement stageFillElement;
        public ContextElement gaugeCountElement;
        public ContextElement gaugeCountAreaElement;
        public ContextElement gaugeCountTextElement;
        public List<ContextElement> gaugeStageList;
        public List<ContextElement> gaugeFillList;
        public ContextElement stageInfoElement;
        public Animator stageInfoBalloonAnimator;
        public List<ContextElement> stageInfoRewardTextList;

        private GameObject stageCountObj = null;
        private List<long> rewardRatioNumeratorList;
        private ClubChallengeStageInfoController.StageType stageType = ClubChallengeStageInfoController.StageType.UNKNOWN;

        private int stage;
        private int progressCount;
        private int maxStage;

        private const int MISSION_COUNT = 4;


        private const string CLUB_CHALLENGE_INFO_TITLE = "CLUB_CHALLENGE_INFO_TITLE";
        private const string CLUB_CHALLENGE_INFO_SUBTITLE = "CLUB_CHALLENGE_INFO_SUBTITLE";
        private const string CLUB_CHALLENGE_INFO_STAGE_TEXT = "CLUB_CHALLENGE_INFO_STAGE_TEXT";
        private const string CLUB_CHALLENGE_INFO_REWARD_TEXT = "CLUB_CHALLENGE_INFO_REWARD_TEXT";

        private bool isInit = false;

        public void InitData(ContextElement areaElement, ClubChallengeStageInfoController.StageType stageType)
        {
            this.stageType = stageType;
            this.areaElement = areaElement;
            this.maxStage = (int)stageType;
        }

        private void InitProperty()
        {
            if(isInit) return;

            gaugeElement = ContextUtils.FindElement(areaElement, "Gauge", ContextSearchingType.ChildrenSearch);
            gaugeFillElement = ContextUtils.FindElement(gaugeElement, "Fill", ContextSearchingType.ChildrenSearch);

            stageElement = ContextUtils.FindElement(areaElement, "Stage", ContextSearchingType.ChildrenSearch);
            stageFillElement = ContextUtils.FindElement(stageElement, "Fill", ContextSearchingType.ChildrenSearch);

            gaugeCountAreaElement = ContextUtils.FindElement(stageElement, "Count Area", ContextSearchingType.ChildrenSearch);

            if(stageCountObj == null)
                stageCountObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Club Challenge Stage Count", gaugeCountAreaElement.transform);
            gaugeCountElement = stageCountObj.GetComponent<ContextElement>();
            gaugeCountElement.UpdateContext();
            gaugeCountTextElement = ContextUtils.FindElement(gaugeCountElement, "Text", ContextSearchingType.ChildrenSearch);

            gaugeStageList = new List<ContextElement>();
            gaugeFillList = new List<ContextElement>();
            for(int i=0; i < maxStage; ++i)
            {
                gaugeStageList.Add(ContextUtils.FindElement(stageFillElement, string.Format("{0:00}", i), ContextSearchingType.ChildrenSearch));
                gaugeStageList[i].gameObject.SetActive(false);

                var subFillElement = ContextUtils.FindElement(gaugeFillElement, string.Format("Fill {0:00}", i+1), ContextSearchingType.ChildrenSearch);
                for(int k=0; k < MISSION_COUNT; ++k)
                {
                    gaugeFillList.Add(ContextUtils.FindElement(subFillElement, string.Format("{0:00}", k+1), ContextSearchingType.ChildrenSearch));
                    gaugeFillList[k].gameObject.SetActive(false);
                }
            }

            // max stage 
            gaugeStageList.Add(ContextUtils.FindElement(stageFillElement, string.Format("{0:00}", maxStage), ContextSearchingType.ChildrenSearch));

            // additional percent information
            stageInfoElement = ContextUtils.FindElement(areaElement, "Info", ContextSearchingType.ChildrenSearch);
            var stageSpeechElement = ContextUtils.FindElement(stageInfoElement, "Stage Speech Bubble", ContextSearchingType.ChildrenSearch);
            var stageInfoButtonElement = ContextUtils.FindElement(stageInfoElement, "Button", ContextSearchingType.ChildrenSearch);

            stageInfoBalloonAnimator = stageSpeechElement.GetComponent<Animator>();

            MetaContextElementUtils.SimpleSetTextGlobal(stageSpeechElement, "Title Text", CLUB_CHALLENGE_INFO_TITLE, ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(stageSpeechElement, "Sub Text", CLUB_CHALLENGE_INFO_SUBTITLE, ContextSearchingType.ChildrenSearch);

            stageInfoRewardTextList = new List<ContextElement>();
            for(int i = 0; i < maxStage; ++i)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(stageSpeechElement, string.Format("Stage Text {0}", i+1), CLUB_CHALLENGE_INFO_STAGE_TEXT, ContextSearchingType.ChildrenSearch, i+1);
                stageInfoRewardTextList.Add(ContextUtils.FindElement(stageSpeechElement, string.Format("Reward Text {0}", i+1), ContextSearchingType.ChildrenSearch));
            }

            MetaContextElementUtils.SetClickable(
                stageInfoButtonElement,
                () =>
                {
                    stageInfoBalloonAnimator.gameObject.SetActive(true);

                    var stateInfo = stageInfoBalloonAnimator.GetCurrentAnimatorStateInfo(0);
                    if(stateInfo.IsName("Default.Default"))
                        stageInfoBalloonAnimator.SetTrigger("Disappear");
                    else
                        stageInfoBalloonAnimator.SetTrigger("Appear");
                }
            );

            isInit = true;
        }

        public void OnDisableController()
        {
            areaElement?.gameObject.SetActive(false);
        }

        public void OnUpdateVariables(int stage, int progressCount, List<long> rewardRatioNumeratorList)
        {
            InitProperty();

            this.stage = stage;
            this.progressCount = progressCount;
            this.rewardRatioNumeratorList = rewardRatioNumeratorList;

            UpdateeGauge();
        }

        private void UpdateeGauge()
        {
            areaElement.gameObject.SetActive(true);

            string progressText = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_PROGRESS", stage, maxStage);
            MetaContextElementUtils.SetText(gaugeCountTextElement, progressText);

            UpdateGaugeProgress();
            UpdateStageInformation();
        }

        private void UpdateGaugeProgress()
        {
            for(int i=0; i < gaugeStageList.Count; ++i)
            {
                gaugeStageList[i].gameObject.SetActive(stage >= i);
            }

            int stageProgressCount = stage * MISSION_COUNT + progressCount;

            for(int i=0; i < gaugeFillList.Count; ++i)
            {
                gaugeFillList[i].gameObject.SetActive(stageProgressCount > i);
            }

            if(stageProgressCount < gaugeFillList.Count)
            {
                UpdateCountPosition(gaugeCountElement, gaugeFillList[stageProgressCount].gameObject);
            }
            else
            {
                UpdateCountPosition(gaugeCountElement, gaugeFillList[gaugeFillList.Count - 1].gameObject, false);
            }
        }

        private void UpdateCountPosition(ContextElement countObj, GameObject targetObj, bool leftAnchor = true)
        {
            RectTransform targetRect = targetObj.GetComponent<RectTransform>();
            RectTransform countRect = countObj.GetComponent<RectTransform>();

            Vector2 targetPosition = targetRect.position;
            countRect.position = targetRect.position;

            if(leftAnchor)
                countRect.localPosition -= new Vector3(targetRect.rect.width/2f, 0f, 0f);
            else
                countRect.localPosition += new Vector3(targetRect.rect.width/2f, 0f, 0f);
        }

        private void UpdateStageInformation()
        {
            if(rewardRatioNumeratorList == null) return;

            // additional numerator list
            for(int i = 0; i < maxStage; ++i)
            {
                if(rewardRatioNumeratorList.Count <= i) break;

                MetaContextElementUtils.SetTextGlobal(stageInfoRewardTextList[i], CLUB_CHALLENGE_INFO_REWARD_TEXT, NumberUtils.GetPercentFromNumerator(rewardRatioNumeratorList[i]) );
            }
        }
    }
}
