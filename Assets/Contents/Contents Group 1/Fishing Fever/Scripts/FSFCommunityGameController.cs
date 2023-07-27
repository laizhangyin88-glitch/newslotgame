using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using System.Collections;

namespace GameStudio.Slot.FSF
{
    public class FSFCommunityGameController : MonoBehaviour
    {
        [SerializeField]
        private List<BaseSlotMachine> subSlotMachineList;
        [SerializeField]
        private List<FSFWinFrameController> winFrameControllerList;
        [SerializeField]
        private ObjectPool copyingFramePool;
        [SerializeField]
        private ObjectPool frameBGPool;

        private int row;
        private int column;
        private List<int> visibleCounts;
        private bool hasInitSubSlots = false;

        private int communitySpinCount;
        private List<Blackboard> communityUserResultList;
        private List<Blackboard> reelOutputListPerSpin;
        private List<Blackboard> finalOutputListPerSpin;

        private List<GameObject> copyingFrameDestAnchorList;
        private List<PooledObject> copyingFrameList;
        private List<PooledObject> frameBGList;


        public IReadOnlyList<BaseSlotMachine> SubSlotMachineList { get => subSlotMachineList.AsReadOnly(); }
        public static bool IsOnCommunity { get => BlackboardUtils.FindVariable(null, "./bonus") != null; }


        public const string CREATE_COMMUNITY_SLOTS_EVENT = "CreateCommunitySlots";
        public const string ON_SLOT_EVENT = "OnSlotEvent";
        public const string ON_SPIN_SLOT_MAHCINE = "SpinSlotMachine";
        public const string STOP_SLOT_MACHINE_EVENT = "StopSlotMachine";
        public const string SPIN_DONE_EVENT = "CommunitySpinDone";
        public const string DELETE_COPIED_FRAME_EVENT = "DeleteAllCopiedFrame";

        private void Awake()
        {
            copyingFrameList = new List<PooledObject>();
            copyingFrameDestAnchorList = new List<GameObject>();
            frameBGList = new List<PooledObject>();
            for (int i = 0; i < winFrameControllerList.Count; i++) copyingFrameDestAnchorList.Add(new GameObject("Copying Frame Dest Anchor"));
        }

        private void OnEnable()
        {
            MessageDispatcher.Register(ON_SLOT_EVENT, OnSlotEventDelegator);
            MessageDispatcher.Register(FSFWinFrameController.ON_CONTENT_UI_EVENT, OnContentUIDelegator);
            copyingFrameList.Clear();
            var initFrame = new Frame();
            initFrame.width = 1; initFrame.height = 1;
            initFrame.row = SubSlotMachineList[0].RowCount / 2;
            initFrame.column = SubSlotMachineList[0].ColumnCount / 2;
            // prevent call anim at this frame because PreLateUpdate.DirectorUpdateAnimationEnd's high CPU usage will make frame move anim looks like blink 1 frame
            StartCoroutine(FSFWinFrameController.CallActionAfterDelay(() => { winFrameControllerList[0].StartFrameMove(initFrame); }, 0.1f));
            communitySpinCount = 0;
        }

        private void OnDisable()
        {
            if (frameBGList.Count > 0)
            {
                frameBGList.ForEach((bg) => { bg.ReturnToPool(); });
                frameBGList.Clear();
                copyingFrameList.Clear();
            }
            MessageDispatcher.UnRegister(ON_SLOT_EVENT, OnSlotEventDelegator);
            MessageDispatcher.UnRegister(FSFWinFrameController.ON_CONTENT_UI_EVENT, OnContentUIDelegator);
        }

        public void OnContentUIDelegator(EventData eventData) {
            if (eventData.name == CREATE_COMMUNITY_SLOTS_EVENT)
            {
                CreateSubSlotMachines();
                CacheCommunityResultList();
            }
            else if(eventData.name == DELETE_COPIED_FRAME_EVENT)
            {
                copyingFrameList.ForEach((frame) => { frame.GetComponent<Animator>().SetTrigger("Disappear"); });
                frameBGList.ForEach((bg) => { bg.GetComponentInChildren<Animator>().SetTrigger("Disappear"); });
                FSFWinFrameController.CallActionAfterDelay(()=>
                {
                    copyingFrameList.ForEach((frame) =>
                    {
                        frame.ReturnToPool();
                    });
                    frameBGList.ForEach((bg) => { bg.ReturnToPool(); });
                    frameBGList.Clear();
                    copyingFrameList.Clear();
                },1.2f);
            }
        }

        public void OnSlotEventDelegator(EventData eventData)
        {
            if (eventData.name == ON_SPIN_SLOT_MAHCINE && IsOnCommunity && eventData.id == 0)
            {
                if (frameBGList.Count > 0)
                {
                    frameBGList.ForEach((bg)=> { bg.GetComponentInChildren<Animator>().SetTrigger("Disappear"); });
                    copyingFrameList.Clear();
                    StartCoroutine(FSFWinFrameController.CallActionAfterDelay(() => { frameBGList.ForEach((bg) => { bg.ReturnToPool();  }); frameBGList.Clear(); }, 0.2f));
                }
                MoveAllFrames();
                ReplaceAllSlotMachinesReelStrip();
                for (int slotMachineIndex = 0; slotMachineIndex < subSlotMachineList.Count; slotMachineIndex++)
                {
                    var result = communityUserResultList[slotMachineIndex];
                    List<int> reelOutputList = reelOutputListPerSpin[communitySpinCount].GetValue<List<int>>("value");
                    UpdateDeck(slotMachineIndex, reelOutputList);
                }
                UpdateExpectationSpots(0, GetFrameList(communitySpinCount));
                SpinAllSubSlotMachine();
                StartCoroutine(StartCopyFramesFromSubSlots());
                StartCoroutine(FSFWinFrameController.CallActionAfterDelay(() => {
                    for (int slotMachineIndex = 0; slotMachineIndex < subSlotMachineList.Count; slotMachineIndex++)
                    {
                        MessageDispatcher.Dispatch(ON_SLOT_EVENT, new EventData(STOP_SLOT_MACHINE_EVENT, slotMachineIndex));
                    }
                    communitySpinCount++;
                },4f));
                StartCoroutine(FSFWinFrameController.CallActionAfterDelay(() => { EmergeFramesBG();MessageDispatcher.Dispatch(ON_SLOT_EVENT, new EventData(SPIN_DONE_EVENT)); }, 6.2f));
            }
        }

        private void CreateSubSlotMachines()
        {
            if (hasInitSubSlots)
            {
                foreach (var slotMachine in subSlotMachineList)
                    slotMachine.InitializeSymbols();
                return;
            };

            hasInitSubSlots = true;
            row = BlackboardUtils.FindValue<int>(null, "./customData/slotDataList/0/row");
            column = BlackboardUtils.FindValue<int>(null, "./customData/slotDataList/0/column");
            visibleCounts = BlackboardUtils.FindValue<List<int>>(null, "./customData/slotDataList/0/visibleCounts");

            for (int slotMachineIndex = 1; slotMachineIndex < subSlotMachineList.Count; slotMachineIndex++)
            {
                var slotMachine = subSlotMachineList[slotMachineIndex];
                for (int colIndex = 0; colIndex < column; ++colIndex)
                {
                    int beginRow = row - visibleCounts[colIndex];
                    int endRow = beginRow + visibleCounts[colIndex];

                    var reel = slotMachine.CreateReel(colIndex, beginRow, colIndex + 1, endRow, 0);
                    reel.Initialize(slotMachine, reel);
                }
                slotMachine.Shuffle();
                ContentCustomData.GetSlotData(slotMachine.slotIndex).slotMachine = slotMachine.gameObject;
            }
        }

        private void CacheCommunityResultList() {

            communityUserResultList = BlackboardUtils.FindValue<List<Blackboard>>(null, "./bonus/response/userGameResultList");
            reelOutputListPerSpin = BlackboardUtils.FindValue<List<Blackboard>>(null, "./bonus/response/reelOutputListPerSpin");
            finalOutputListPerSpin = BlackboardUtils.FindValue<List<Blackboard>>(null, "./bonus/response/finalOutputListPerSpin");
        }

        #region SlotMachine Funcs
        private void SpinAllSubSlotMachine()
        {
            for (int userIndex = 1; userIndex < communityUserResultList.Count; userIndex++)
            {
                 MessageDispatcher.Dispatch(ON_SLOT_EVENT, new EventData(ON_SPIN_SLOT_MAHCINE, userIndex));
            }
        }

        private void ReplaceAllSlotMachinesReelStrip()
        {
                List<int> reelOutputList = reelOutputListPerSpin[communitySpinCount].GetValue<List<int>>("value");
                List<Blackboard> finalOutputList = finalOutputListPerSpin[communitySpinCount].GetValue<List<Blackboard>>("value");

            for (int i = 0; i < communityUserResultList.Count; i++)
            {
                subSlotMachineList[i].SetStripIndices(reelOutputList, row);
                ReplaceReelStripsWithFinalOutputList(finalOutputList, reelOutputList);
            }
        }

        private void ReplaceReelStripsWithFinalOutputList(List<Blackboard> finalOutputList,List<int> reelOutputList)
        {
            var strips = GlobalReelStrips.Instance.GetReelStrips();
            var symbolMask = ContentCustomData.GetSlotData(0).symbolMask;

            for (int colIndex = 0; colIndex < column; colIndex++)
            {
                BaseReelStrip reelStrip = strips.GetReelStrip(colIndex);
                var newList = new List<SymbolInfo>();
                for (int rowIndex = 0; rowIndex < row; rowIndex++)
                {
                    var symbolIndex = finalOutputList[rowIndex].GetValue<List<int>>("value")[colIndex];
                    var newSymbolInfo = new SymbolInfo();
                    newSymbolInfo.symbol = symbolIndex;
                    newSymbolInfo.mask = symbolMask.GetMask(symbolIndex);
                    newList.Add(newSymbolInfo);
                }

                reelStrip.ReplaceRange(reelOutputList[colIndex] % reelStrip.stripCount, newList);
            }
        }
        #endregion

        private List<Frame> GetFrameList(int spinIndex)
        {
            var frameList = new List<Frame>();
            foreach(var communityResult in communityUserResultList)
            {
                var frame = FSFWinFrameController.GetFrameFromBlackboard(communityResult.GetValue<List<Blackboard>>("frameInfoPerSpin")[spinIndex]);
                frameList.Add(frame);
            }

            return frameList;
        }

        public static void UpdateDeck(int slotIndex, List<int> reelOutputList)
        {
            var slotData = ContentCustomData.GetSlotData(slotIndex);
            var deck = (Deck)slotData.deck.Clone();
            deck.stripIndices = reelOutputList;


            deck.deck = new List<List<SymbolInfo>>();
            deck.hitMap = new List<List<bool>>();
            var strips = GlobalReelStrips.Instance.GetReelStrips();
            for (int column = 0; column < slotData.column; ++column)
            {
                var hitReel = new List<bool>();
                var reel = new List<SymbolInfo>();
                var strip = strips.GetReelStrip(column);
                for (int row = 0; row < slotData.row; ++row)
                {
                    int idx = strip.CalcIndex(deck.stripIndices[column] + row);
                    reel.Add(SlotUtils.GetSymbol(slotIndex, column, strip, idx));
                    hitReel.Add(false);
                }
                deck.deck.Add(reel);
                deck.hitMap.Add(hitReel);
            }
            slotData.deck = deck;
        }

        public static void UpdateExpectationSpots(int slotIndex, List<Frame> frameList)
        {
            var slotData = ContentCustomData.GetSlotData(slotIndex);
            var expectaion = slotData.expectation;
            expectaion.expectations = new List<bool>();
            expectaion.expectationSpots = new List<List<Cell>>();

            for (int i = 0; i < slotData.column; ++i)
            {
                expectaion.expectations.Add(false);
                expectaion.expectationSpots.Add(new List<Cell>());
            }

            foreach (var frame in frameList)
            {
                for (int colIndex = frame.column; colIndex < frame.column + frame.width; colIndex++)
                    for (int rowIndex = frame.row; rowIndex > frame.row - frame.height; rowIndex--)
                        expectaion.expectationSpots[colIndex].Add(new Cell(colIndex, rowIndex));
            }
        }


        private void MoveAllFrames()
        {
            for (int userIndex = 0; userIndex < communityUserResultList.Count; userIndex++)
            {
                var result = communityUserResultList[userIndex];
                Blackboard frameBB = result.GetValue<List<Blackboard>>("frameInfoPerSpin")[communitySpinCount];
                long multiplier = userIndex == 0 ? result.GetValue<List<long>>("frameMultiplierPerSpin")[communitySpinCount] : 0;
                winFrameControllerList[userIndex].StartFrameMove(FSFWinFrameController.GetFrameFromBlackboard(frameBB), System.Convert.ToInt32(multiplier));
            }
        }

        private IEnumerator StartCopyFramesFromSubSlots()
        {
            var waitForSeconds = new WaitForSeconds(0.7f);
            yield return waitForSeconds;
            for(int i =1;i< winFrameControllerList.Count; i++)
            {
                var frameHandler = winFrameControllerList[i];
                var flyingFrame = copyingFramePool.GetObject();
                Frame originFrameInfo = frameHandler.CurrentFrameInfo;
                string originScaleStateName = string.Format(FSFWinFrameController.SCALE_ANIM_STATE_NAME_FORMAT, originFrameInfo.height, originFrameInfo.width);
                flyingFrame.GetComponent<Animator>().Play(originScaleStateName, FSFWinFrameController.ANIMATOR_SCALE_LAYER_INDEX);
                copyingFrameList.Add(flyingFrame);

                var destAnchor = copyingFrameDestAnchorList[i];
                var targetReel = SubSlotMachineList[0].reels[originFrameInfo.column];
                Vector3 targetPosition = targetReel.CalcSymbolPosition(targetReel.beginColumn, targetReel.beginRow, originFrameInfo.column, originFrameInfo.row);
                targetPosition = targetReel.transform.TransformVector(targetPosition);
                targetPosition = targetReel.transform.position + targetPosition;
                destAnchor.transform.position = targetPosition;

                var positionController = flyingFrame.GetComponent<DirectionalWeightPositionController>();
                positionController.from = frameHandler.transform;
                positionController.to = destAnchor.transform;

                yield return waitForSeconds;
            }
        }

        private void EmergeFramesBG()
        {
            var cellSetWithinFrames = new HashSet<Cell>(new CellEqualityComparer());
            var mainFrameCellList = new List<Cell>();

            for (int i = 0; i < winFrameControllerList.Count; i++)
            {
                var frameHandler = winFrameControllerList[i];

                Frame frame = frameHandler.CurrentFrameInfo;
                for (int colIndex = frame.column; colIndex < frame.column + frame.width; colIndex++)
                    for (int rowIndex = frame.row; rowIndex > frame.row - frame.height; rowIndex--)
                    {
                        var cell = new Cell(colIndex, rowIndex);
                        cellSetWithinFrames.Add(cell);
                        // if it  main slot frame
                        if (i == 0) mainFrameCellList.Add(cell);
                    }

                // if it is not main slot frame
                if (i > 0) copyingFrameList[i - 1].GetComponent<Animator>().SetTrigger("Disappear");
            }


            foreach(var cell in cellSetWithinFrames)
            {
                var frameBG = frameBGPool.GetObject();
                frameBG.transform.position = SubSlotMachineList[0].GetSymbol(cell.column,cell.row).transform.position;

                bool isMainFrameCell = mainFrameCellList.Contains(cell);
                Animator frameAnimator = frameBG.GetComponentInChildren<Animator>();
                frameAnimator.SetBool("IsMainFrame", isMainFrameCell);

                frameBGList.Add(frameBG);
            }
        }
    }


    public class CellEqualityComparer : IEqualityComparer<Cell>
    {
        public bool Equals(Cell cell0, Cell cell1)
        {
            return cell0.column == cell1.column && cell0.row == cell1.row;

        }

        public int GetHashCode(Cell cell)
        {
            return cell.GetHashCode();

        }
    }
}
