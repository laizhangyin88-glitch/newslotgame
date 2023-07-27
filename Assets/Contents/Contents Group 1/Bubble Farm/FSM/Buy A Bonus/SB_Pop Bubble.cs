using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NodeCanvas.Framework;
using SlotMaker;
using ParadoxNotion.Design;
using ParadoxNotion;

namespace BBF
{
	[Category("★ Game Task/BBF")]
	public class SB_PopBubble : ActionTask<Blackboard>
	{
		public BBParameter<BaseSlotMachine> slotMachine;
		public BBParameter<List<GameObject>> hitBubbleObjectList;
		public BBParameter<List<GameObject>> totalBubbleObjectList;
		public BBParameter<List<Blackboard>> hitBubbleBonusResponseList;
		public BBParameter<ObjectPool> spotSlotMachinePool;
		public BBParameter<ObjectPool> flyingCreditPool;
		public BBParameter<Transform> flyingDest;
		public BBParameter<List<GameObject>> spotSlotMachineList;
		public BBParameter<List<List<GameObject>>> bonusSymbolListPerSpotSlotMachine;
		public BBParameter<float> termBetweenPop;
		public BBParameter<float> termBetweenFlyingCredit;
		public BBParameter<int> row;
		public BBParameter<int> column;

		public static string BUBBLE_LIST_VARIABLE_NAME = "bubbleList";
		public static string BUBBLE_POSITION_VARIABLE_NAME = "position";
		public static string BUBBLE_POSITION_COLUMN_VARIABLE_NAME = "column";
		public static string BUBBLE_POSITION_ROW_VARIABLE_NAME = "row";
		public static string BUBBLE_SIZE_VARIABLE_NAME = "size";
		public static string ANCHOR_OBJECT_NAME = "anchor";

		private List<Cell> hitBonusCellList;
		private WaitForSeconds waitTerm;
		private WaitForSeconds flyingCreditTerm;

		private Coroutine PopBubblesCoroutine;

		protected override string info
		{
			get { return "pop bubbles"; }
		}

		protected override void OnExecute()
		{
			bonusSymbolListPerSpotSlotMachine.value = new List<List<GameObject>>();
			hitBonusCellList = new List<Cell>();
			for (int index = 0; index < hitBubbleBonusResponseList.value.Count; index++)
			{
				IBlackboard hitResponse = hitBubbleBonusResponseList.value[index];
				var hitCellBBList = hitResponse.GetValue<List<Blackboard>>("hitBonusCellList");
				foreach (Blackboard hitCellBB in hitCellBBList)
				{
					var cellCol = hitCellBB.GetValue<int>("column");
					var cellRow = hitCellBB.GetValue<int>("row");
					hitBonusCellList.Add(new Cell(cellCol, cellRow));
				}
			}
			spotSlotMachineList.value = new List<GameObject>();
			OffNonHitSymbols();

			waitTerm = new WaitForSeconds(termBetweenPop.value);
			flyingCreditTerm = new WaitForSeconds(termBetweenFlyingCredit.value);
			PopBubblesCoroutine =StartCoroutine(PopAllBubble());
			MessageDispatcher.Register("OnContentEvent", OnLeaveGameDelegator);
		}

		private void OnLeaveGameDelegator(ParadoxNotion.EventData eventData)
		{
			if (eventData.name != "LeaveGame") return;
			StopCoroutine(PopBubblesCoroutine);
			MessageDispatcher.UnRegister("OnContentEvent", OnLeaveGameDelegator);
		}

		private IEnumerator PopAllBubble()
		{
			BlackboardUtils.GetOrCreateVariable<bool>(null, "./customData/offBonusWinAnim").value = false;

			List<Blackboard> totalWinCellBBList = new List<Blackboard>();
			List<Animator> nonHitBubbleAnimatorList = new List<Animator>();

			for (int j = 0; j < totalBubbleObjectList.value.Count; j++)
			{
				var bubbleObj = totalBubbleObjectList.value[j];
				if (!hitBubbleObjectList.value.Contains(bubbleObj)) nonHitBubbleAnimatorList.Add(bubbleObj.GetComponent<Animator>());
			}

			for (int i = 0; i < hitBubbleBonusResponseList.value.Count; i++)
			{
				var bubbleObj = hitBubbleObjectList.value[i];
				Blackboard hitResponse = hitBubbleBonusResponseList.value[i];
				BeginBonus(hitResponse);

				Animator bubbleAniamtor = bubbleObj.GetComponent<Animator>();
				bubbleAniamtor.SetTrigger("Stop");
				bubbleAniamtor.SetBool("SkipStop", true);
				bubbleAniamtor.SetTrigger("Win");
				int bubbleSize = bubbleAniamtor.GetInteger("Bubble Size");

				var hitCellBBList = hitResponse.GetValue<List<Blackboard>>("hitBonusCellList");
				foreach (Blackboard hitCellBB in hitCellBBList)
				{
					totalWinCellBBList.Add(hitCellBB);
					var cellCol = hitCellBB.GetValue<int>("column");
					var cellRow = hitCellBB.GetValue<int>("row");

					slotMachine.value.GetOverlaySymbol(new Cell(cellCol, cellRow).GetHashCode()).Play("Win");
					slotMachine.value.GetSymbol(cellCol, cellRow).gameObject.SetActive(false);
				}

				var bonusCredit = BlackboardUtils.FindVariable<long>(null, "./bonus/response/earnCredit");
				var bonus = BlackboardUtils.FindVariable<Blackboard>(null, "./bonus").value;
				ContentBlackboardUtils.AddEarnCredit(bonus, bonusCredit.value);

				var bubblePositionBB = hitResponse.GetValue<Blackboard>("bubblePosition");
				var bubbleCol = bubblePositionBB.GetValue<int>("column");
				var bubbleRow = bubblePositionBB.GetValue<int>("row");
				List<GameObject> symbolList;
				if (bubbleSize > 1 &&  (symbolList = GetSymbolListWithinBubble(new Cell(bubbleCol, bubbleRow), bubbleSize)).Count == bubbleSize * bubbleSize)
				{
					var spotSlotMachine = spotSlotMachinePool.value.GetObject();
					spotSlotMachine.transform.SetParent(spotSlotMachinePool.value.transform);
					spotSlotMachine.transform.position = bubbleAniamtor.transform.position;
					spotSlotMachine.GetComponentInChildren<Animator>().SetInteger("Size", bubbleSize);
					GSManager.Instance.GetHandler("Jackpot Outline Appear").Play();

					spotSlotMachineList.value.Add(spotSlotMachine.gameObject);
					bonusSymbolListPerSpotSlotMachine.value.Add(symbolList);
				}
				EndBonus();
				yield return waitTerm;

				foreach (Blackboard hitCellBB in hitCellBBList)
				{
					var cellCol = hitCellBB.GetValue<int>("column");
					var cellRow = hitCellBB.GetValue<int>("row");

					slotMachine.value.GetOverlaySymbol(new Cell(cellCol, cellRow).GetHashCode()).Play("Skip");
				}
			}

			foreach(var bubbleAnimator in nonHitBubbleAnimatorList)
            {
				bubbleAnimator.SetTrigger("Stop");
				bubbleAnimator.SetBool("SkipStop", true);
				bubbleAnimator.SetTrigger("Win");

				yield return waitTerm;
			}

			totalWinCellBBList.Sort(new CellBBComparer());
			foreach (Blackboard hitCellBB in totalWinCellBBList)
			{
				var cellCol = hitCellBB.GetValue<int>("column");
				var cellRow = hitCellBB.GetValue<int>("row");

				var symbol = TurnOverlayToBaseSymbol(slotMachine.value.GetOverlaySymbol(new Cell(cellCol, cellRow).GetHashCode()));
				symbol.gameObject.SetActive(true);
				symbol.Play("Win");

				var flyingObject = flyingCreditPool.value.GetObject();
				flyingObject.transform.SetParent(symbol.transform, false);
				flyingObject.transform.position = symbol.transform.position;
				GSManager.Instance.GetHandler("Credit Fly").Play();
				var directionalWeightMove = flyingObject.GetComponent<DirectionalWeightPositionController>();
				directionalWeightMove.from = symbol.transform;
				directionalWeightMove.to = flyingDest.value;
				BlackboardUtils.GetOrCreateVariable<bool>(agent, "./customData/isOnFlyingCredit").value = true;
				MessageDispatcher.Dispatch("OnContentUIDetailEvent", new EventData("FlyCredit"));
				yield return flyingCreditTerm;
			}

			BlackboardUtils.GetOrCreateVariable<bool>(agent, "./customData/isOnFlyingCredit").value = false;
			BlackboardUtils.GetOrCreateVariable<bool>(null, "./customData/offBonusWinAnim").value = true;
			slotMachine.value.overlay.Clear();

			spotSlotMachineList.value.Sort(new SlotMachineSizeComparer());
			bonusSymbolListPerSpotSlotMachine.value.Sort(new SpotSlotMachineSizeComparerBySymbolCount());
			MessageDispatcher.UnRegister("OnContentEvent", OnLeaveGameDelegator);
			EndAction();
		}

		private List<GameObject> GetSymbolListWithinBubble(Cell bubblePosition, int bubbleSize)
		{
			var symbolList = new List<GameObject>(bubbleSize * bubbleSize);
			if (bubbleSize <= 1) return  symbolList;

			var slotMachineIndex = slotMachine.value.slotIndex;
			for (int rowIndex = bubblePosition.row; rowIndex > bubblePosition.row - bubbleSize && rowIndex >=0; rowIndex--)
			{
				for (int colIndex = bubblePosition.column; colIndex < bubblePosition.column + bubbleSize && colIndex < column.value; colIndex++)
				{
					bool isBonusSymbol = SymbolMask.HasAnyAttribute(ContentCustomData.GetSlotData(slotMachineIndex).deck.GetSymbol(colIndex, rowIndex).mask, SymbolAttribute.Scatter2);
					if (isBonusSymbol) symbolList.Add(slotMachine.value.GetSymbol(colIndex, rowIndex).gameObject);
				}
			}
			return symbolList;
		}


		private void OffNonHitSymbols()
		{
			for (int colIndex = 0; colIndex < column.value; colIndex++)
				for (int rowIndex = 0; rowIndex < row.value; rowIndex++)
				{
					BaseSymbol overlaySymbol;
					var targetCell = new Cell(colIndex, rowIndex);
					if ((overlaySymbol = slotMachine.value.GetOverlaySymbol(targetCell.GetHashCode())) != null && overlaySymbol.symbolIndex == 11)
					{
						bool isWinCell = false;
						foreach (var cell in hitBonusCellList)
							if (cell.GetHashCode() == targetCell.GetHashCode())
							{
								isWinCell = true;
								break;
							}

						if (!isWinCell)
						{
							var symbol = TurnOverlayToBaseSymbol(overlaySymbol);
							symbol.gameObject.SetActive(true);
							symbol.Play("Off");
						}
					}
				}
		}

		private BaseSymbol TurnOverlayToBaseSymbol(BaseSymbol overlaySymbol)
		{
			overlaySymbol.gameObject.SetActive(false);
			var overlayBB = overlaySymbol.GetComponent<Blackboard>();
			var isJackpotSymbol = overlayBB.GetValue<bool>("_isJackpot");
			string creditText = overlayBB.GetValue<ContextTextMeshProUGUI>("creditText").GetText();
			Sprite jackpotSprite = overlayBB.GetValue<GameObject>("jackpotImage").GetComponent<SpriteRenderer>().sprite;

			var symbol = slotMachine.value.GetSymbol(overlaySymbol.column, overlaySymbol.row);
			var symbolBB = symbol.GetComponent<Blackboard>();
			var symbolCreditText = symbolBB.GetValue<ContextTextMeshProUGUI>("creditText");
			var symbolJackpotImage = symbolBB.GetValue<GameObject>("jackpotImage").GetComponent<SpriteRenderer>();

			symbol.symbolIndex = overlaySymbol.symbolIndex;
			symbol.symbolMask = overlaySymbol.symbolMask;
			symbol.Apply();

			symbolCreditText.SetText(creditText);
			symbolJackpotImage.sprite = jackpotSprite;
			symbolCreditText.gameObject.SetActive(!isJackpotSymbol);
			symbolJackpotImage.gameObject.SetActive(isJackpotSymbol);

			return symbol;
		}


		private void BeginBonus(Blackboard bonusResponse)
		{
			var cb = ContentBlackboard.Get();
			var parent = cb.GetValue<Blackboard>("current");

			var bonus = (Blackboard)BlackboardUtils.CreateBlackboard("bonus");
			var bonusList = BlackboardUtils.AddToBlackboardList(parent, "bonusList", bonus);
			BlackboardUtils.SetOrCreateValue<Blackboard>(cb, "bonus", bonus);
			cb.SetValue("current", bonus);

			string guid = System.Guid.NewGuid().ToString();
			long timestamp = MetaSystem.GetTimeStamp();
			int bonusId = BlackboardUtils.FindVariable<int>(bonusResponse, "bonusId").value;

			BlackboardUtils.SetOrCreateValue<int>(bonus, "bonusIndex", bonusList.Count - 1);
			BlackboardUtils.SetOrCreateValue<ContentNodeType>(bonus, "type", ContentNodeType.Bonus);
			BlackboardUtils.SetOrCreateValue<Blackboard>(bonus, "parent", parent);
			BlackboardUtils.SetOrCreateValue(bonus, "uid", guid);
			BlackboardUtils.SetOrCreateValue<int>(bonus, "bonusId", bonusId);
			BlackboardUtils.SetOrCreateValue<Blackboard>(bonus, "response", bonusResponse);
			BlackboardUtils.SetOrCreateValue<long>(bonus, "earnCredit", 0L);
			BlackboardUtils.SetOrCreateValue<long>(bonus, "singleCredit", 0L);
			BlackboardUtils.SetOrCreateValue<long>(bonus, "beginTime", timestamp);

			ContentEvent.BeginBonus(bonus);
		}

		private void EndBonus()
		{
			var cb = ContentBlackboard.Get();
			var bonus = cb.GetValue<Blackboard>("bonus");
			var parent = bonus.GetValue<Blackboard>("parent");
			var parentType = parent.GetValue<ContentNodeType>("type");
			long timestamp = MetaSystem.GetTimeStamp();

			BlackboardUtils.SetOrCreateValue<long>(bonus, "endTime", timestamp);

			ContentEvent.EndBonus(bonus);

			cb.SetValue("current", parent);
			if (parentType == ContentNodeType.Spin)
			{
				BlackboardUtils.SetOrCreateValue<Blackboard>(cb, "spin", parent);

				var spinParent = parent.GetValue<Blackboard>("parent");
				var spinParentType = spinParent.GetValue<ContentNodeType>("type");
				if (spinParentType == ContentNodeType.Turn)
				{
					cb.RemoveVariable("bonus");
				}
				else if (spinParentType == ContentNodeType.Bonus)
				{
					cb.SetValue("bonus", spinParent);
				}
				else
				{
					Debug.LogError("[Content] Spin circle detected.");
				}
			}
			else if (parentType == ContentNodeType.Bonus)
			{
				cb.SetValue("bonus", parent);
			}
			else
			{
				Debug.LogError("[Content] EndBonus failed. Bonus could not be placed under Turn.");
				return;
			}
		}
	}


	class CellBBComparer : IComparer<Blackboard>
	{
		public int Compare(Blackboard cellBB, Blackboard cellBB1)
		{
			var cellCol = cellBB.GetValue<int>("column");
			var cellRow = cellBB.GetValue<int>("row");

			var cellCol1 = cellBB1.GetValue<int>("column");
			var cellRow1 = cellBB1.GetValue<int>("row");

			if (cellCol == cellCol1) return cellRow - cellRow1;
			else return cellCol - cellCol1;
		}
	}

	class SpotSlotMachineSizeComparerBySymbolCount : IComparer<List<GameObject>>
	{
		public int Compare(List<GameObject> symbolList1, List<GameObject> symbolList2)
		{
			return symbolList1.Count - symbolList2.Count;
		}
	}


	class SlotMachineSizeComparer : IComparer<GameObject>
	{
		public int Compare(GameObject slotMachine1, GameObject slotMachine2)
		{
			var size1 = slotMachine1.GetComponentInChildren<Animator>().GetInteger("Size");
			var size2 = slotMachine2.GetComponentInChildren<Animator>().GetInteger("Size");

			var compareValue = size1 - size2;
			if (compareValue != 0) return compareValue;

			float slotMachine1X = slotMachine1.transform.position.x;
			float slotMachine2X = slotMachine2.transform.position.x;

			float slotMachine1Y = slotMachine1.transform.position.y;
			float slotMachine2Y = slotMachine2.transform.position.y;

			if (slotMachine1X == slotMachine2X) return (int)((slotMachine2Y - slotMachine1Y)*10000);
			else return (int)((slotMachine1X - slotMachine2X)*10000);
		}
	}
}