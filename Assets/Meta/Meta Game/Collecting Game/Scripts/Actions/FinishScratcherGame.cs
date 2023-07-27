using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class FinishScrathcerGame : ActionTask<Blackboard>
    {
        private GameObject caller;

        private bool isAuto;
        private bool isReward;
        private bool playCoinSound;
        private bool clickContinue;
        private int remainingCount;
        private int prizeListCount;
        private string gameEndTriggerKey;
        private Animator anim;

        private Transform popupRoot;
        private Variable<int> scratcherId;
        private Variable<Blackboard> scratcherRewardResult;

        private GameObject scratcherPopup;

        private const string COIN_ADD_SOUND_ID = "UI_Coin_Add";
        private const string ON_FINISH_POPUP_SCRATCHER = "OnFinishPopupScratcher";
        private const string SCRATCHER_TOTAL_RESULT_POPUP = "Popup Contents Total Result Scene";

        protected override string info
        {
            get { return "Finish Scratcher Game"; }
        }

        protected override void OnExecute()
        {
            InitProperty();

            if (playCoinSound)
                GSManager.Instance.GetHandler(COIN_ADD_SOUND_ID).Play();

            StartCoroutine(DelayedExecuteCoroutine(0.1f));
        }

        private IEnumerator DelayedExecuteCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            DelayedExecute();
        }

        private void DelayedExecute()
        {
            if ((isAuto && remainingCount > 0) ||
                (!isAuto && clickContinue))
            {
                agent.StartCoroutine(PlayNextScratcher());
            }
            else if (prizeListCount > 0)
            {
                ShowTotalResult();
            }
            else
            {
                anim.SetTrigger(gameEndTriggerKey);
                FinishGame(0.1f, false);
            }
        }

        private void InitProperty()
        {
            caller = agent.GetValue<GameObject>("caller");
            isAuto = agent.GetValue<bool>("_isAuto");
            isReward = agent.GetValue<bool>("_isReward");
            playCoinSound = agent.GetValue<bool>("_playCoinSound");
            remainingCount = agent.GetValue<int>("_remainingCount");
            gameEndTriggerKey = agent.GetValue<string>("_booleanTrigger");
            anim = agent.GetComponent<Animator>();

            if (!isAuto) clickContinue = agent.GetValue<bool>("_clickContinue");

            var prizeList = agent.GetValue<List<long>>("_prizeList");
            prizeListCount = prizeList.Count;
            agent.AddVariable("_prizeListCount", prizeListCount);
            scratcherRewardResult = agent.GetVariable<Blackboard>("_scratcherRewardResult");

            // Collecting Game only
            scratcherId = agent.GetVariable<int>("_scratcherId");

            popupRoot = GameObject.Find("Popup Manager/Area").transform;
        }

        private IEnumerator PlayNextScratcher()
        {
            // Inbox Reward
            if (isReward)
            {
                // caller is inbox cell
                EventSender.SendEvent(caller, new EventData<bool>(InboxEvent.ACCEPT_NEXT_INBOX_ITEM, isAuto));

                // Finish current reward
                EventSender.SendCalleeCallback(agent.gameObject);
            }
            // Collecting Game
            else
            {
                // Open Loading Popup
                GameObject loadingPopup = null;
                yield return agent.StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                    (GameObject obj) => { loadingPopup = obj; }));

                DecreaseRemainingCount();

                // Open Scratcher Popup
                yield return agent.StartCoroutine(OpenScratcherPopupCoroutine());

                var scratcherBB = scratcherPopup.GetComponent<Blackboard>();

                // Request Scratcher
                yield return agent.StartCoroutine(BlackboardQueryUtils.RedeemCollectingGameScratcherCoroutine(
                    scratcherBB,
                    scratcherId.value));

                UpdateCredit();

                MetaPopupUtils.OpenPopup(scratcherPopup);

                // Close Loading Popup
                MetaPopupUtils.ClosePopup(loadingPopup);
            }

            MetaPopupUtils.ClosePopup(agent.gameObject);

            EndAction(true);
        }

        private void UpdateCredit()
        {
            var e = new EventData("UpdateNaviCredit");
            MessageDispatcher.Dispatch("OnCreditEvent", e);
        }

        private void DecreaseRemainingCount()
        {
            var remainingCountVar = agent.GetVariable<int>("_remainingCount");
            remainingCountVar.value--;
        }

        private IEnumerator OpenScratcherPopupCoroutine()
        {
            ScratcherName scratcherName = scratcherRewardResult.value.GetValue<ScratcherName>("scratcherName");
            string sceneName = PopupScratcherUtils.GetScratcherSceneName(scratcherName);

            string bundle = ApplicationSettings.MakeApplicationBundleName("lobby");
            yield return agent.StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, sceneName, popupRoot,
                (SceneLoadOperation scene) => { scratcherPopup = scene.GetScene(); }));

            var scratcherBB = scratcherPopup.GetComponent<Blackboard>();
            BlackboardQueryUtils.SetScratcherPrizeInfo(scratcherBB, agent, isReward);
            BlackboardQueryUtils.SetScratcherPopupInfo(scratcherBB, agent, caller);
        }

        private void ShowTotalResult()
        {
            StartCoroutine(ShowTotalResultCoroutine());
        }

        private IEnumerator ShowTotalResultCoroutine()
        {
            string bundle = ApplicationSettings.MakeApplicationBundleName("lobby");
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, SCRATCHER_TOTAL_RESULT_POPUP, popupRoot,
                (SceneLoadOperation scene) => { scratcherPopup = scene.GetScene(); }));

            var scratcherBB = scratcherPopup.GetComponent<Blackboard>();
            BlackboardQueryUtils.SetScratcherTotalResultPopupInfo(scratcherBB, agent, caller, isReward);

            MetaPopupUtils.OpenPopup(scratcherPopup);

            FinishGame(0f, true);
        }

        private void FinishGame(float delay, bool withTotalResult)
        {
            StartCoroutine(FinishGameCoroutine(delay, withTotalResult));
        }

        private IEnumerator FinishGameCoroutine(float delay, bool withTotalResult)
        {
            yield return new WaitForSeconds(delay);

            if(!withTotalResult)
            {
                anim.SetTrigger("End");

                var untilAnimationEndTrigger = new EventTrigger(agent.gameObject, "OnEndAnim");
                yield return new WaitUntilTrigger(untilAnimationEndTrigger);
            }

            if (isReward)
            {
                EventSender.SendCalleeCallback(agent.gameObject);
            }
            else
            {
                EventSender.SendEvent(caller, ON_FINISH_POPUP_SCRATCHER);
            }

            MetaPopupUtils.ClosePopup(agent.gameObject);

            EndAction();
        }
    }
}
