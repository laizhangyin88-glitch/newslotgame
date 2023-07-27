using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using ParadoxNotion;
using SlotMaker;
using TMPro;
using NodeCanvas.Framework;

namespace GameStudio.Slot.FSF
{
    public class FSFTopBoardController : FeatureController
    {
        [SerializeField]
        private FSF_GaugeController gaugeController;
        [SerializeField]
        private FSF_CommunityBetController communityBetController;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Transform flyingDest;
        [SerializeField]
        private ObjectPool flyingScatterPool;
        [SerializeField]
        private float boardValueUpdateDuration = 1f;
        [SerializeField]
        private float boardValueUpdateTerm= 0.05f;

        private long lastAccumulatedBet;
        private long accumulatedCreditAfterBonus;
        private int roomTicketAfterBonus;


        public const int COMMUNITY_BONUS_ID = 20601;

        protected override string ON_FEATURE_END_EVENT => "EndEarnScatter";
        protected override string ON_FEATURE_BEGIN_EVENT => "EarnScatter";

        public const string ON_BOARD_INIT_EVENT = "InitBaseTopBoard";
        public const string END_BOARD_INIT_EVENT = "EndInitBaseTopBoard";
        public const string WIN_EVENT = "OnWinEvent";
        public const string SIGNBOARD_CREDIT_SHOW = "ShowAccumulatedCredit";

        public const string OTHER_PLAYER_COLLECT_EVENT = "OnCommunityTicketEarn";


        private void Awake()
        {
            gaugeController.InitializeToGameEnterState();
            communityBetController.InitializeToGameEnterState();
        }
        protected override void OnStart()
        {
        }

        protected override void OnFinish()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterEvent(ON_BOARD_INIT_EVENT,(EventData eventData)=> {
                gaugeController.SetGagueValue(roomTicketAfterBonus);
                communityBetController.SetCreditText(accumulatedCreditAfterBonus);
                StartCoroutine(FSFWinFrameController.CallActionAfterDelay(() => { ContentEvent.SendEvent(END_BOARD_INIT_EVENT); }, 0.1f));
            });
            MessageDispatcher.Register(FSFWinFrameController.ON_CONTENT_UI_EVENT,OnContentUIEvent);
            lastAccumulatedBet =  BlackboardUtils.FindValue<long>(null, "./game/myCommunityTicketCount");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterEvent(ON_BOARD_INIT_EVENT);
            MessageDispatcher.UnRegister(FSFWinFrameController.ON_CONTENT_UI_EVENT, OnContentUIEvent);
        }

        private void OnContentUIEvent(EventData eventData)
        {
            bool hadStartedCommunity = ContentCustomData.Instance.GetComponent<Blackboard>().GetValue<bool>("hadCommunityStarted");
            if (hadStartedCommunity == false && eventData.name == OTHER_PLAYER_COLLECT_EVENT && eventData.value is Transform)
            {
                var flyingObj = flyingScatterPool.GetObject(false);
                var positionController = flyingObj.GetComponentInChildren<DirectionalWeightPositionController>();
                var from = eventData.value as Transform;
                positionController.from = from;
                positionController.to = flyingDest;
                flyingObj.transform.position = from.transform.position;
                flyingObj.gameObject.SetActive(true);

                StartCoroutine(FSFWinFrameController.CallActionAfterDelay(() =>
                {
                    animator.SetTrigger("Collect");
                    flyingObj.GetComponent<PooledObject>().ReturnToPool();
                }, 1f));
            }
        }

        protected override IEnumerator OnPlayCoroutine()
        {
            var slotMachine = BlackboardUtils.FindValue<GameObject>(null, "./slotMachine").GetComponent<BaseSlotMachine>();
            Frame frame = FSFWinFrameController.GetFrameFromBlackboard(BlackboardUtils.FindValue<IBlackboard>("./spin/response/frame"));
            Deck deck = ContentCustomData.GetSlotData(0).deck;

            List<BaseSymbol> scatterList = new List<BaseSymbol>();
            for (int colIndex = frame.column; colIndex < frame.column + frame.width; colIndex++)
                for (int rowIndex = frame.row; rowIndex > frame.row - frame.height; rowIndex--)
                    if (SymbolMask.HasAttribute(deck.GetSymbol(colIndex, rowIndex), SymbolAttribute.Scatter1))
                    {
                        scatterList.Add(slotMachine.GetSymbol(colIndex, rowIndex));
                    }

            if (scatterList.Count > 0) {
                GSManager.Instance.GetHandler("Fever Symbol Fly").Play();
                List<PooledObject> flyingObjList = new List<PooledObject>(scatterList.Count);
                foreach (var symbol in scatterList)
                {
                    var flyingObj = flyingScatterPool.GetObject(false);
                    var positionController = flyingObj.GetComponentInChildren<DirectionalWeightPositionController>();
                    positionController.from = symbol.transform;
                    positionController.to = flyingDest;
                    flyingObj.transform.position = symbol.transform.position;
                    flyingObj.gameObject.SetActive(true);

                    flyingObjList.Add(flyingObj.GetComponent<PooledObject>());
                }

                IBlackboard bb = ContentBlackboard.Get();
                var spin = bb.GetVariable<Blackboard>("spin").value;
                var response = ContentBlackboardUtils.GetBonusResponse(spin, COMMUNITY_BONUS_ID);

                long newMyTicket;
                float newRoomTicket;
                if (response != null)
                {
                    accumulatedCreditAfterBonus = BlackboardUtils.FindValue<long>(null, "./spin/response/myCommunityTicketCount");
                    roomTicketAfterBonus = BlackboardUtils.FindValue<int>(null, "./spin/response/roomCommunityTicketCount");

                    newRoomTicket = gaugeController.Gauge.maxValue;
                    var enterBet = response.GetValue<List<Blackboard>>("userGameResultList")[0].GetValue<long>("enterBet");
                    newMyTicket = enterBet * communityBetController.MyTicketDivider;
                }
                else
                {
                    newMyTicket = BlackboardUtils.FindValue<long>(null, "./spin/response/myCommunityTicketCount");
                    newRoomTicket = BlackboardUtils.FindValue<int>(null, "./spin/response/roomCommunityTicketCount");
                }

                StartCoroutine(FSFWinFrameController.CallActionAfterDelay(() =>
                {
                    animator.SetTrigger("Collect");

                    foreach (var pooledObj in flyingObjList) pooledObj.ReturnToPool();
                    GSManager.Instance.GetHandler("Fever Symbol Collect").Play();
                    StartCoroutine(FSFWinFrameController.CallActionAfterDelay(() => { GSManager.Instance.GetHandler("Coin Shake").Play(); }, 0.55f));
                    StartCoroutine(StartBoardValueProgress(newMyTicket, newRoomTicket));
                    ContentCustomData.Instance.GetComponent<Blackboard>().SetValue("accumulatedBet",  newMyTicket / communityBetController.MyTicketDivider);

                    if (newMyTicket != lastAccumulatedBet)
                    {
                        ContentCustomData.Instance.GetComponent<Blackboard>().SetValue("accumulatedBet",  newMyTicket / communityBetController.MyTicketDivider);
                        MessageDispatcher.Dispatch(WIN_EVENT, new EventData(SIGNBOARD_CREDIT_SHOW));
                    }
                    lastAccumulatedBet = newMyTicket;
                }, 0.8f));
            }
            else
            {
                IBlackboard bb = ContentBlackboard.Get();
                var spin = bb.GetVariable<Blackboard>("spin").value;
                var response = ContentBlackboardUtils.GetBonusResponse(spin, COMMUNITY_BONUS_ID);
                if (response != null)
                {
                    gaugeController.SetGagueValue(gaugeController.Gauge.maxValue);

                    accumulatedCreditAfterBonus = BlackboardUtils.FindValue<long>(null, "./spin/response/myCommunityTicketCount");
                    roomTicketAfterBonus = BlackboardUtils.FindValue<int>(null, "./spin/response/roomCommunityTicketCount");
                }
                else
                {
                    var newRoomTicket =  BlackboardUtils.FindValue<int>(null, "./spin/response/roomCommunityTicketCount");
                    gaugeController.SetGagueValue(newRoomTicket);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }

        private IEnumerator StartBoardValueProgress(long newMyCommunityTicket, float newRoomTicket)
        {
            long lastMyCommunityTicket = communityBetController.LastValue;
            float lastRoomTicket = gaugeController.LastValue;

            long myCommunityTickeyGap = newMyCommunityTicket - lastMyCommunityTicket;
            float roomTicketGap = newRoomTicket - lastRoomTicket;

            float elapasedTime = 0f;
            var waitForSecond = new WaitForSeconds(boardValueUpdateTerm);
            string lastCreditString = communityBetController.CurrentCreditString;
            GSManager.Instance.GetHandler("Fever Credit Up").Play();
            while (elapasedTime <= boardValueUpdateDuration)
            {
                float progress = elapasedTime / boardValueUpdateDuration;
                long myTicketProgress = myCommunityTickeyGap * Convert.ToInt64(progress * 100f) / 100;
                float roomTicketProgress = roomTicketGap * progress;
                communityBetController.SetCreditText(lastMyCommunityTicket+ myTicketProgress);
                gaugeController.SetGagueValue(lastRoomTicket + roomTicketProgress);

                yield return waitForSecond;
                elapasedTime += boardValueUpdateTerm;
                elapasedTime = (float)Math.Round(elapasedTime, 2);
            }
            GSManager.Instance.GetHandler("Fever Credit Up End").Play();
            GSManager.Instance.GetHandler("Fever Credit Up").Stop();
        }
    }


    [Serializable]
    public class FSF_GaugeController
    {
        [SerializeField]
        private Slider gauge;
        private float lastValue;

        public Slider Gauge { get => gauge; }
        public float LastValue { get => lastValue; }

        public void InitializeToGameEnterState()
        {
            gauge.maxValue = BlackboardUtils.FindValue<int>(null, "./game/roomCommunityTicketMaxCount");
            gauge.minValue = 0;
            gauge.value = BlackboardUtils.FindValue<int>(null, "./game/roomCommunityTicketCount");
            lastValue = gauge.value;
        }

        public void Initialize()
        {
            gauge.maxValue = BlackboardUtils.FindValue<int>(null, "./game/roomCommunityTicketMaxCount");
            gauge.minValue = 0;
            gauge.value = 0;
            lastValue = 0;
        }

        public void AddGagueValue(float addedGaugeValue)
        {
            gauge.value += addedGaugeValue;
            lastValue = gauge.value;
        }

        public void SetGagueValue(float newGaugeValue)
        {
            gauge.value = newGaugeValue;
            lastValue = gauge.value;
        }
    }

    [Serializable]
    public class FSF_CommunityBetController
    {
        [SerializeField]
        private TextMeshProUGUI creditText;

        private long myTicketDivider;
        private long lastValue;

        public long LastValue { get => lastValue; }
        public long MyTicketDivider { get => myTicketDivider; }
        public string CurrentCreditString { get => creditText.text; }


        public void InitializeToGameEnterState()
        {
            myTicketDivider = BlackboardUtils.FindValue<long>(null, "./game/myCommunityTicketDivider");
            lastValue = BlackboardUtils.FindValue<long>(null, "./game/myCommunityTicketCount") / myTicketDivider;
            SetCreditText(BlackboardUtils.FindValue<long>(null, "./game/myCommunityTicketCount"));
        }

        public void Initialize()
        {
            myTicketDivider = BlackboardUtils.FindValue<long>(null, "./game/myCommunityTicketDivider");
            creditText.text = 0.ToString();
            lastValue = 0;
        }

        public void SetCreditText(long newMyCommunityTicket)
        {
            lastValue = newMyCommunityTicket;
            creditText.text = FormatUtility.SimpleNumberFormat(newMyCommunityTicket / myTicketDivider);
        }
    }
}
