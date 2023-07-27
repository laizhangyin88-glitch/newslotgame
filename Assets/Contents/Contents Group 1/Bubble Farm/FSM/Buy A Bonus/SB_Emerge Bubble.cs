using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BBF
{
	[Category("★ Game Task/BBF")]
	public class SB_EmergeBubble : ActionTask<Blackboard>
	{
		public BBParameter<BaseSlotMachine> slotMachine;
		public BBParameter<Blackboard> bubbleInfo;
		public BBParameter<ObjectPool> flyingObjectPool;

		public BBParameter<int> maxFakeBubbleEmergeCount = 1;
		public BBParameter<int> maxFakeBubbleEmergeRepeatCount = 5;
		public BBParameter<int> minFakeBubbleEmergeRepeatCount = 1;
		public BBParameter<float> fakeBubbleEmergePeriod = 0.75f;

		public BBParameter<int> totalRow;
		public BBParameter<int> totalCol;

		public BBParameter<double> bubbleStopDistanceThreshold = 0.05;
		public BBParameter<float> bubbleHaltCheckPeriod = 0.1f;

		public BBParameter<List<GameObject>> bubbleObjectList;
		public BBParameter<List<GameObject>> anchorList;
		public BBParameter<List<Cell>> cellList;
		public BBParameter<List<int>> sizeList;

		public static string BUBBLE_LIST_VARIABLE_NAME = "bubbleList";
		public static string BUBBLE_POSITION_VARIABLE_NAME = "position";
		public static string BUBBLE_POSITION_COLUMN_VARIABLE_NAME = "column";
		public static string BUBBLE_POSITION_ROW_VARIABLE_NAME = "row";
		public static string BUBBLE_SIZE_VARIABLE_NAME = "size";
		public static string ANCHOR_OBJECT_NAME = "anchor";

		private static int BUBBLE_START_ROW = 11;
		private static int BUBBLE_DEST_ROW = -8;
		private WaitForSeconds bubbleFlyingHaltCheckTerm;

		private int realBubbleCount = 0;
		private int currentFixedBubbleCount = 0;

		private Coroutine EmeregeBubblesCoroutine;

		protected override string info
		{
			get { return "Emerge Bubble as server's bubble info"; }
		}

		protected override void OnExecute()
		{
			currentFixedBubbleCount = 0;
			EmeregeBubblesCoroutine = StartCoroutine(EmergeBubble());
			MessageDispatcher.Register("OnContentEvent", OnLeaveGameDelegator);
		}

		private void OnLeaveGameDelegator(ParadoxNotion.EventData eventData)
        {
			if (eventData.name != "LeaveGame") return;
			StopCoroutine(EmeregeBubblesCoroutine);
			MessageDispatcher.UnRegister("OnContentEvent", OnLeaveGameDelegator);
		}

		private IEnumerator EmergeBubble()
		{
			var waitForFakeBubbleEmerge = new WaitForSeconds(fakeBubbleEmergePeriod.value);
			List<Coroutine> bubbleHaltCoroutineList = new List<Coroutine>();

			bubbleFlyingHaltCheckTerm = new WaitForSeconds(bubbleHaltCheckPeriod.value);
			List<Blackboard> bubbleList = bubbleInfo.value.GetValue<List<Blackboard>>(BUBBLE_LIST_VARIABLE_NAME);
			anchorList.value = new List<GameObject>(totalCol.value *2);
			// bubbleList.Shuffle();
			int bonusId = BlackboardUtils.FindVariable<int>(null, "./bonus/bonusId").value;
			int maxBubbleSize = -1;
			if (bonusId == 18153) maxBubbleSize = 4;
			else if (bonusId == 18152) maxBubbleSize = 3;
			else maxBubbleSize = 2;

			cellList.value = new List<Cell>(bubbleList.Count);
			sizeList.value = new List<int>(bubbleList.Count);
			bubbleObjectList.value = new List<GameObject>(bubbleList.Count);

			GameObject[] bubbleDestAnchorPerCol = new GameObject[totalCol.value];
			GameObject[] bubbleStartAnchorPerCol = new GameObject[totalCol.value];
			for (int i = 0; i < totalCol.value; i++)
			{
				BaseReel reel = slotMachine.value.GetReel(i);
				(bubbleDestAnchorPerCol[i] = new GameObject("Bubble Dest Anchor")).transform.SetParent(reel.transform);
				(bubbleStartAnchorPerCol[i] = new GameObject("Bubble Start Anchor")).transform.SetParent(reel.transform);

				bubbleDestAnchorPerCol[i].AddComponent<RectTransform>().anchoredPosition3D = reel.CalcSymbolPosition(reel.beginColumn, reel.beginRow, i, BUBBLE_DEST_ROW);
				bubbleStartAnchorPerCol[i].AddComponent<RectTransform>().anchoredPosition3D = reel.CalcSymbolPosition(reel.beginColumn, reel.beginRow, i, BUBBLE_START_ROW);

				anchorList.value.Add(bubbleDestAnchorPerCol[i]);
				anchorList.value.Add(bubbleStartAnchorPerCol[i]);
			}

			GSManager.Instance.GetHandler("SB Bubble Appear").Play();
			GSManager.Instance.GetHandler("SB Bubble Appear BGM").Play();
			realBubbleCount = bubbleList.Count;
			foreach (Blackboard bubble in bubbleList)
			{
				Blackboard cellBB = bubble.GetValue<Blackboard>(BUBBLE_POSITION_VARIABLE_NAME);
				int col = cellBB.GetValue<int>(BUBBLE_POSITION_COLUMN_VARIABLE_NAME);
				int row = cellBB.GetValue<int>(BUBBLE_POSITION_ROW_VARIABLE_NAME);
				int size = bubble.GetValue<int>(BUBBLE_SIZE_VARIABLE_NAME);
				cellList.value.Add(new Cell(col, row));
				sizeList.value.Add(size);

				BaseReel reel = slotMachine.value.GetReel(col);
				Vector3 destPosition = reel.CalcSymbolPosition(reel.beginColumn, reel.beginRow, col, row);
				var destAnchor = new GameObject("Real Bubble Dest Anchor");
				destAnchor.transform.SetParent(reel.transform,false);
				destAnchor.AddComponent<RectTransform>().anchoredPosition3D = destPosition;

				int fakeBubbleRepeatCount = Random.Range(minFakeBubbleEmergeRepeatCount.value, maxFakeBubbleEmergeRepeatCount.value + 1);
				bool hasRealBubbleAppear = false;
				for (int i = 0; i < fakeBubbleRepeatCount; i++)
				{
					if (!hasRealBubbleAppear)
					{
						bool emergeRealBubble = Random.Range(0, 1f) >= 0.5f;
						if (i == fakeBubbleRepeatCount - 1 || emergeRealBubble)
						{
							var bubbleObject = flyingObjectPool.value.GetObject();
							bubbleObject.transform.position = bubbleStartAnchorPerCol[col].transform.position;
							bubbleObjectList.value.Add(bubbleObject.gameObject);

							var positionController = bubbleObject.GetComponent<DirectionalWeightPositionController>();
							positionController.from = bubbleStartAnchorPerCol[col].transform;
							positionController.to = bubbleDestAnchorPerCol[col].transform;
							anchorList.value.Add(destAnchor);
							bubbleHaltCoroutineList.Add(StartCoroutine(CheckFlyingHaltOnDestArrive(bubbleObject.transform, destAnchor.transform)));

							var bubbleAnimator = bubbleObject.GetComponent<Animator>();
							bubbleAnimator.SetInteger("Bubble Size", size);
							bubbleAnimator.SetTrigger("DisappearMove");
							hasRealBubbleAppear = true;
						}
						else EmergeFakeBubbles(bubbleStartAnchorPerCol, bubbleDestAnchorPerCol, maxBubbleSize, maxFakeBubbleEmergeCount.value);
					}
					else EmergeFakeBubbles(bubbleStartAnchorPerCol, bubbleDestAnchorPerCol, maxBubbleSize, maxFakeBubbleEmergeCount.value);

					yield return waitForFakeBubbleEmerge;
				}
			}
			GSManager.Instance.GetHandler("SB Bubble Appear").Stop();
			StopAllCoroutineAfterSecond(bubbleHaltCoroutineList, 1.5f);
			MessageDispatcher.UnRegister("OnContentEvent", OnLeaveGameDelegator);
			EndAction();
		}

		private void EmergeFakeBubbles(GameObject[] startAnchorPerCol, GameObject[] destAnchorPerCol, int maxFakeBubbleSize, int maxBubbleCount)
		{
			int[] bubbleSizeList = new int[totalCol.value];
			int latestBubbleEmergeCol = 0;
			int latestBubbleSize = 0;
			int fakeBubbleCount = 0;

			int startIndex = 0;
			int adder = 1;
			bool startFromEnd = Random.Range(0, 1f) > 0.5f;
			if (startFromEnd)
			{
				startIndex = totalCol.value - 1;
				adder = -1;
			}

			for (int colIndex = startIndex; colIndex < totalCol.value && colIndex >= 0; colIndex += adder)
			{
				if (fakeBubbleCount >= maxBubbleCount) break;

				int randomSize = Random.Range(0, maxFakeBubbleSize + 1);
				if (colIndex == 0 || (IsLatestBubbleNotOverlapped(latestBubbleEmergeCol, latestBubbleSize, colIndex, randomSize) && IsBubbleNotOverColumn(randomSize, colIndex)))
				{
					latestBubbleEmergeCol = colIndex;
					latestBubbleSize = randomSize;
					fakeBubbleCount++;
				}
				else randomSize = 0;

				bubbleSizeList[colIndex] = randomSize;
			}

			for (int colIndex = 0; colIndex < totalCol.value; colIndex++)
			{
				EmergeFakeSoleBubble(bubbleSizeList[colIndex], startAnchorPerCol[colIndex].transform, destAnchorPerCol[colIndex].transform);
			}
		}

		private bool IsLatestBubbleNotOverlapped(int latestBubbleEmergeCol, int latestBubbleSize, int currentColIndex, int currentBubbleSize)
		{
			if (latestBubbleEmergeCol < currentColIndex) return latestBubbleEmergeCol + latestBubbleSize - 1 < currentColIndex;
			else return latestBubbleEmergeCol - latestBubbleSize + 1 > currentColIndex + currentBubbleSize - 1;
		}

		private bool IsBubbleNotOverColumn(int bubbleSize, int currentColIndex)
		{
			return currentColIndex + bubbleSize - 1 < totalCol.value;
		}

		private void EmergeFakeSoleBubble(int bubbleSize, Transform startAnchor, Transform destAnchor)
		{
			if (bubbleSize > 0)
			{
				var bubbleObject = flyingObjectPool.value.GetObject();
				bubbleObject.transform.position = startAnchor.transform.position;
				bubbleObject.transform.SetParent(flyingObjectPool.value.transform,false);

				var positionController = bubbleObject.GetComponent<DirectionalWeightPositionController>();
				positionController.from = startAnchor.transform;
				positionController.to = destAnchor.transform;

				var bubbleAnimator = bubbleObject.GetComponent<Animator>();
				bubbleAnimator.SetInteger("Bubble Size", bubbleSize);
				bubbleAnimator.SetTrigger("DisappearMove");
			}
		}

		private IEnumerator CheckFlyingHaltOnDestArrive(Transform bubble, Transform destAnchor)
		{
			while (true)
			{
				// for forced quit slot because if forced quit occurs all ingame object is destroyed without stop coroutines
				if(bubble == null || destAnchor == null) yield break;

				if ((bubble.position - destAnchor.position).magnitude <= bubbleStopDistanceThreshold.value) break;
				yield return bubbleFlyingHaltCheckTerm;
			}
			bubble.GetComponent<DirectionalWeightPositionController>().to = destAnchor;
			bubble.GetComponent<Animator>().SetTrigger("SkipMove");
			GSManager.Instance.GetHandler("Bubble Fix").Play();

			currentFixedBubbleCount++;
			if(currentFixedBubbleCount >= realBubbleCount) GSManager.Instance.GetHandler("Vox Bubble Fix").Play();
		}

		private IEnumerator StopAllCoroutineAfterSecond(List<Coroutine> coroutineList, float delay)
        {
			yield return new WaitForSeconds(delay);
			foreach(var coroutine in coroutineList)
            {
				StopCoroutine(coroutine);
            }
        }
	}
}