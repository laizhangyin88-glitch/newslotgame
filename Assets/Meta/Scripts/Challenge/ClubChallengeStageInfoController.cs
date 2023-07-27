using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace BagelCode
{
    public class ClubChallengeStageInfoController : MonoBehaviour
    {
        public enum StageType
        {
            UNKNOWN = 0,
            SINGLE = 1,
            TRIPLE = 3,
            QUINTUPLE = 5,
        }

        public ContextElement rootElement;

        private ClubChallengeMultipleStageController tripleController = null;
        private ClubChallengeMultipleStageController quintupleController = null;

        // // Single elements
        public ContextElement singleStageAreaElement;
        public ContextElement singleGaugeCountElement;
        public ContextElement singleGaugeCountTextElement;
        public List<ContextElement> singleGaugeFillList;
        // ////////////////////////////////////////////////

        private bool isInit = false;

        private StageType stageType = StageType.UNKNOWN;

        private int progressCount;
        private int stage;
        private List<long> rewardRatioNumeratorList;

        private const int MISSION_COUNT = 4;

        private void InitProperty()
        {
            if(isInit) return;

            rootElement = GetComponent<ContextElement>();
            rootElement.UpdateContext();

            InitSingleProperties();

            var tripleAreaElement = ContextUtils.FindElement(rootElement, "Area", ContextSearchingType.ChildrenSearch);
            var quintupleAreaElement = ContextUtils.FindElement(rootElement, "Area Level 5", ContextSearchingType.ChildrenSearch);

            if(tripleController == null)
            {
                tripleController = gameObject.AddComponent<ClubChallengeMultipleStageController>();
                tripleController.InitData(tripleAreaElement, StageType.TRIPLE);
            }

            if(quintupleController == null)
            {
                quintupleController = gameObject.AddComponent<ClubChallengeMultipleStageController>();
                quintupleController.InitData(quintupleAreaElement, StageType.QUINTUPLE);
            }

            isInit = true;
        }

        private void InitSingleProperties()
        {
            singleStageAreaElement = ContextUtils.FindElement(rootElement, "Area One", ContextSearchingType.ChildrenSearch);

            if(stageType != StageType.SINGLE) return;

            if(singleStageAreaElement == null) return;
            var singleGaugeElement = ContextUtils.FindElement(singleStageAreaElement, "Gauge", ContextSearchingType.ChildrenSearch);
            singleGaugeCountElement = ContextUtils.FindElement(singleGaugeElement, "Count", ContextSearchingType.ChildrenSearch);
            singleGaugeCountTextElement = ContextUtils.FindElement(singleGaugeCountElement, "Text", ContextSearchingType.ChildrenSearch);

            singleGaugeFillList = new List<ContextElement>();
            for(int i=0; i < 4; ++i)
            {
                singleGaugeFillList.Add(ContextUtils.FindElement(singleGaugeElement, string.Format("{0:00}", i+1), ContextSearchingType.ChildrenSearch));
            }
        }

        public void OnUpdateVariables(int stage, int maxStage, int progressCount, List<long> rewardRatioNumeratorList)
        {
            this.stage = stage;
            this.progressCount = progressCount;
            this.rewardRatioNumeratorList = rewardRatioNumeratorList;

            if(maxStage < 0)
                maxStage = MetaStringDefine.CLUB_CHALLENGE_STAGE_MAX;

            if(stageType != (StageType)maxStage)
                isInit = false;

            stageType = (StageType)maxStage;

            InitProperty();

            singleStageAreaElement?.gameObject.SetActive(false);
            tripleController.OnDisableController();
            quintupleController.OnDisableController();

            if(stage < 0) return;

            switch(stageType)
            {
                case StageType.SINGLE:
                    UpdateSingleGauge();
                    break;
                case StageType.TRIPLE:
                    tripleController.OnUpdateVariables(stage, progressCount, rewardRatioNumeratorList);
                    break;
                case StageType.QUINTUPLE:
                    quintupleController.OnUpdateVariables(stage, progressCount, rewardRatioNumeratorList);
                    break;
            }
        }

        private void UpdateSingleGauge()
        {
            // Show Single area
            if(singleStageAreaElement == null) return;
            singleStageAreaElement.gameObject.SetActive(true);

            string progressText = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_PROGRESS", progressCount, MISSION_COUNT);
            MetaContextElementUtils.SetText(singleGaugeCountTextElement, progressText);

            UpdateSingleGuageProgress();
        }

        private void UpdateSingleGuageProgress()
        {
            if(singleGaugeFillList == null) return;

            for(int i=0; i < singleGaugeFillList.Count; ++i)
            {
                singleGaugeFillList[i].gameObject.SetActive(progressCount > i);
            }

            if(progressCount < singleGaugeFillList.Count)
            {
                UpdateCountPosition(singleGaugeCountElement, singleGaugeFillList[progressCount].gameObject);
            }
            else
            {
                UpdateCountPosition(singleGaugeCountElement, singleGaugeFillList[singleGaugeFillList.Count - 1].gameObject, false);
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

#if UNITY_EDITOR
        public int targetStage;
        public int targetProgress;
        public int maxStage;
        [Button]
        private void TestProgress()
        {
            OnUpdateVariables(targetStage, maxStage, targetProgress, rewardRatioNumeratorList);
        }
#endif
    }
}
