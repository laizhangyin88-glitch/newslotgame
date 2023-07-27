using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using TMPro;
using ParadoxNotion;

namespace GameStudio.Slot.FSF
{
    public class FSFCommunityWinController : FeatureController
    {
        [SerializeField]
        private List<FSFCommunityFrameWinHandler> slotWinHandlerList;
        [SerializeField]
        private TextMeshProUGUI betText;
        [SerializeField]
        private TextMeshProUGUI spinCountUI;
        [SerializeField]
        private float creditUpdateDuration = 1f;
        [SerializeField]
        private float creditUpdateTerm = 0.02f;


        private List<Blackboard> communityUserResultList;
        private int winCalcTryCount;
        private long enterBet;

        public const string REFRESH_SPIN_COUNT_EVENT = "UpdateSpinCount";
        public const string MULTIPLIER_APPLY_TO_BET_EVENT = "ApplyMultiplierToBet";
        public const string MULTIPLIER_APPLY_TO_BET_END_EVENT = "EndApplyMultiplierToBet";

        protected override string ON_FEATURE_BEGIN_EVENT { get => "StartCommunityFrameWin"; }
        protected override string ON_FEATURE_END_EVENT { get => "EndCommunityFrameWin"; }

        protected override IEnumerator OnPlayCoroutine()
        {
            for(int userIndex =1;userIndex < communityUserResultList.Count; userIndex++)
            {
                var result = communityUserResultList[userIndex];
                var winHandler = slotWinHandlerList[userIndex];
                var frame = FSFWinFrameController.GetFrameFromBlackboard(result.GetValue<List<Blackboard>>("frameInfoPerSpin")[winCalcTryCount]);
                var frameMultiplier = result.GetValue<List<long>>("frameMultiplierPerSpin")[winCalcTryCount];
                StartCoroutine(winHandler.StartFrameWinFlow(frame, frameMultiplier));
            }

            var mainResult = communityUserResultList[0];
            var mainFrame = FSFWinFrameController.GetFrameFromBlackboard(mainResult.GetValue<List<Blackboard>>("frameInfoPerSpin")[winCalcTryCount]);
            var mainFrameMultiplier = mainResult.GetValue<List<long>>("frameMultiplierPerSpin")[winCalcTryCount];
            winCalcTryCount++;
            yield return StartCoroutine(slotWinHandlerList[0].StartFrameWinFlow(mainFrame, mainFrameMultiplier));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            communityUserResultList = BlackboardUtils.FindValue<List<Blackboard>>(null, "./bonus/response/userGameResultList");
            slotWinHandlerList.ForEach((handler) => { handler.OnEnable(); });
            winCalcTryCount = 0;

            enterBet = communityUserResultList[0].GetValue<long>("enterBet");
            betText.text = FormatUtility.CommaNumberFormat(enterBet);
            RegisterEvent(MULTIPLIER_APPLY_TO_BET_EVENT,(eventData)=> {
                StartCoroutine(UpdateBetToEarnCredit());
            }
            );
        }

        private IEnumerator UpdateBetToEarnCredit() {
            var earnCredit = BlackboardUtils.FindValue<long>(null,"./bonus/response/earnCredit");
            var currentCredit = enterBet;
            var creditAdderPerTerm =((earnCredit - enterBet) * (long)((creditUpdateTerm / creditUpdateDuration) * 100))/100;

            var waitForSeconds = new WaitForSeconds(creditUpdateTerm);
            GSManager.Instance.GetHandler("Multiplier Win").Play();
            while (currentCredit < earnCredit)
            {
                currentCredit += creditAdderPerTerm;
                betText.text = FormatUtility.CommaNumberFormat(currentCredit);
                yield return waitForSeconds;
            }
            GSManager.Instance.GetHandler("Multiplier Win").Stop();
            GSManager.Instance.GetHandler("Multiplier Win End").Play();
            ContentEvent.SendEvent(MULTIPLIER_APPLY_TO_BET_END_EVENT);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterEvent(MULTIPLIER_APPLY_TO_BET_EVENT);
        }

        protected override void OnFinish()
        {
        }

        protected override void OnStart()
        {
        }
    }
}