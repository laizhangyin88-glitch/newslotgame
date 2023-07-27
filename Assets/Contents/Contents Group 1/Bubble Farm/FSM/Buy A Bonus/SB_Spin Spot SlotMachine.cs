using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using ParadoxNotion;
using System;

namespace BBF
{
	[Category("★ Game Task/BBF")]
	public class SB_SpinSpotSlotMachine : ActionTask<Blackboard>
	{
		public BBParameter<List<GameObject>> spotSlotMachineList;
		public BBParameter<List<Blackboard>> jackpotResponseList;
		public BBParameter<List<Blackboard>> jackpotMachineBonusResponseList;

		public string popupName;
		public BBParameter<int> miniJackpotSymbolIndex;
		public BBParameter<int> row;
		public BBParameter<int> column;
		public BBParameter<int> defaultSpotSlotReelTypeIndex = 3;
		public BBParameter<int> coinSymbolIndex = 18;
		public BBParameter<int> sortingOrderIncreaseOffsetPerSlotMachineAppear = 200;
		public BBParameter<int> baseCanvasSortingOrder = 1100;
		public BBParameter<int> baseSortingGroupOrder = 1000;

		public BBParameter<float> delayAfterActive;
		public BBParameter<float> delayAfterSpin;
		public BBParameter<float> delayAfterSlotMachineStop;
		public BBParameter<float> delayAfterWinAnimation;

		public static string BUBBLE_LIST_VARIABLE_NAME = "bubbleList";
		public static string BUBBLE_POSITION_VARIABLE_NAME = "position";
		public static string BUBBLE_POSITION_COLUMN_VARIABLE_NAME = "column";
		public static string BUBBLE_POSITION_ROW_VARIABLE_NAME = "row";
		public static string BUBBLE_SIZE_VARIABLE_NAME = "size";
		public static string ANCHOR_OBJECT_NAME = "anchor";
		public static string ON_CONTENT_UI_DETAIL_EVENT = "OnContentUIDetailEvent";

		private bool closedPopup = false;
		private Coroutine SpinMachinesCoroutine;

		protected override string info
		{
			get { return "spin all slot machine made by large bubble pop"; }
		}

		protected override void OnExecute()
		{
			if (spotSlotMachineList.value.Count < 1)
			{
				EndAction();
				return;
			}
			MessageDispatcher.Register("OnContentUIDetailEvent", OnClosedPopup);

			SpinMachinesCoroutine =StartCoroutine(SpinAllSpotSlotMachines());
			MessageDispatcher.Register("OnContentEvent", OnLeaveGameDelegator);
		}

		private void OnLeaveGameDelegator(ParadoxNotion.EventData eventData)
		{
			if (eventData.name != "LeaveGame") return;
			StopCoroutine(SpinMachinesCoroutine);
			MessageDispatcher.UnRegister("OnContentEvent", OnLeaveGameDelegator);
		}

		private void OnClosedPopup(EventData receivedEvent)
		{
			if (receivedEvent.name.Equals("ClosedPopup", StringComparison.Ordinal))
				closedPopup = true;
		}

		private IEnumerator SpinAllSpotSlotMachines()
		{
			WaitForSeconds delayAfterActiveSecond = new WaitForSeconds(delayAfterActive.value);
			WaitForSeconds delayAfterSlotStopSecond = new WaitForSeconds(delayAfterSlotMachineStop.value);
			WaitForSeconds delayAfterWinAnim = new WaitForSeconds(delayAfterWinAnimation.value);
			WaitForSeconds spinDelay = new WaitForSeconds(delayAfterSpin.value);

			int jackpotResponseIndex = 0;
			// InitExpectation();
			for (int i = 0; i < spotSlotMachineList.value.Count; i++)
			{
				closedPopup = false;
				var jackpotMachineHitResponse = jackpotMachineBonusResponseList.value[i];
				var slotMachine = spotSlotMachineList.value[i].GetComponentInChildren<BaseSlotMachine>(true);
				var slotMachineAnimator = spotSlotMachineList.value[i].GetComponentInChildren<Animator>(true);
				slotMachine.CreateReel(0, 0, 0, 1, 1, 0);
				int slotMachineSize = slotMachineAnimator.GetInteger("Size");
				GlobalReelStrips.Instance.index = defaultSpotSlotReelTypeIndex.value + slotMachineSize - 2;
				slotMachine.InitializeSymbols();

				var slotMachineBB = spotSlotMachineList.value[i].GetComponent<Blackboard>();
				var orderCotroller = slotMachineBB.GetVariable<GameObject>("orderController");

				SortingGroup sortingGroupComponent =orderCotroller.value.GetComponent<SortingGroup>();
				Canvas slotMachineCanvas = orderCotroller.value.GetComponent<Canvas>();

				sortingGroupComponent.sortingOrder = baseSortingGroupOrder.value + sortingOrderIncreaseOffsetPerSlotMachineAppear.value * i;
				slotMachineCanvas.sortingOrder = baseCanvasSortingOrder.value + sortingOrderIncreaseOffsetPerSlotMachineAppear.value * i;

				bool isJackpot = jackpotMachineHitResponse.GetValue<bool>("isJackpot");
				int creditMultiplierIndex = jackpotMachineHitResponse.GetValue<int>("symbolTypeIndex");
				int symbolIndex = coinSymbolIndex.value;
				Blackboard targetBonusResponse = jackpotMachineHitResponse;
				if (isJackpot)
				{
					targetBonusResponse = jackpotResponseList.value[jackpotResponseIndex++];
					symbolIndex = GetSymbolIndexByJackpotIndex(targetBonusResponse.GetValue<int>("jackpotIndex"));
				}

				ReplaceReelStrip(symbolIndex, creditMultiplierIndex);
				var indices = new List<int>();
				indices.Add(0);
				var reel = slotMachine.reels[0];
				slotMachine.SetStripIndices(indices, 1);
				slotMachineAnimator.SetTrigger("Active");
				MessageDispatcher.Dispatch(ON_CONTENT_UI_DETAIL_EVENT, new EventData<int>("SBDeactiveJackpotSlotBonusSymbol", i));
				yield return delayAfterActiveSecond;

				slotMachineAnimator.SetTrigger("Active");
				GSManager.Instance.GetHandler("Wheel Spin Start").Play();
				GSManager.Instance.GetHandler("Wheel Spin Tick").Play();
				reel.movement.Spin();

				yield return spinDelay;

				reel.movement.Stop();

				yield return new WaitUntil(() => reel.movement.spinState == SpinState.Stopped);

				GSManager.Instance.GetHandler("Wheel Spin Tick").Stop();
				GSManager.Instance.GetHandler("Wheel Stop").Play();
				slotMachine.GetSymbol(0, 0).Play("Stop Effect");
				slotMachineAnimator.SetTrigger("Active");

				BeginBonus(targetBonusResponse);
				long earnCredit = BlackboardUtils.FindValue<long>("./bonus/response/earnCredit");
				MessageDispatcher.Dispatch(ON_CONTENT_UI_DETAIL_EVENT, new EventData<long>("UpdateSBJackpotCredit", earnCredit));

				yield return delayAfterSlotStopSecond;

				slotMachine.GetSymbol(0, 0).Play("Win");

				yield return delayAfterWinAnim;

				if (isJackpot)
				{
					bool claimEnded = false;
					string uid = targetBonusResponse.GetValue<string>("uid");
					MetaSystem.SlotClaimBonus(uid, 0, () => { claimEnded = true; }, null);
					var snapshot = GSManager.Instance.GetAudioMixerSnapshot("Content_Popup");
					snapshot.TransitionTo(0);
					OpenPopup(popupName);

					yield return new WaitUntil(() => claimEnded);

					// yield return delayAfterSlotMachineStop;
					int gameId = BlackboardUtils.FindValue<int>("./game/gameId");
					long betCredit = BlackboardUtils.FindValue<long>("./turn/totalBetCredit");
					int jackpotType = BlackboardUtils.FindValue<int>("./bonus/response/jackpotIndex");
					Analytics.jackpot(gameId, betCredit, earnCredit, jackpotType);
					yield return new WaitUntil(() => closedPopup);

					snapshot = GSManager.Instance.GetAudioMixerSnapshot("Content_Main");
					snapshot.TransitionTo(0);
				}

				var bonusCredit = BlackboardUtils.FindVariable<long>(null, "./bonus/response/earnCredit");
				var bonus = BlackboardUtils.FindVariable<Blackboard>(null, "./bonus").value;
				ContentBlackboardUtils.AddEarnCredit(bonus, bonusCredit.value);
				EndBonus();
				MessageDispatcher.Dispatch("OnContentUIDetailEvent", new EventData("StopSBJackpotCredit"));
			}

			MessageDispatcher.UnRegister("OnContentUIDetailEvent", OnClosedPopup);
			MessageDispatcher.UnRegister("OnContentEvent", OnLeaveGameDelegator);
			EndAction();
		}

		private int GetSymbolIndexByJackpotIndex(int jackpotIndex)
		{
			return miniJackpotSymbolIndex.value + jackpotIndex;
		}

		private void ReplaceReelStrip(int symbolIndex, int creditMultiplierIndex)
		{
			BaseReelStrip reelStrip = GlobalReelStrips.Instance.GetReelStrips().GetReelStrip(0);

			var repalceList = new List<SymbolInfo>();
			var newSymbolInfo = new SymbolInfo();
			newSymbolInfo.symbol = symbolIndex;
			if (symbolIndex == 18) newSymbolInfo.mask = SymbolAttribute.Scatter2;

			var customData = newSymbolInfo.customData;
			if (customData == null) newSymbolInfo.customData = new Dictionary<string, object>();
			if ((customData = newSymbolInfo.customData).ContainsKey("index")) customData.Add("index", 0);
			customData["index"] = creditMultiplierIndex;

			repalceList.Add(newSymbolInfo);
			reelStrip.ReplaceRange(0, repalceList);
		}

		private void OpenPopup(string popupName)
		{
			var prefab = AssetBundleManager.LoadAsset<GameObject>(BlackboardUtils.FindVariable<string>(null, "./game/gameTitle").value, popupName);
			GameObject go = GameObject.Instantiate(prefab) as GameObject;
			go.name = popupName;

			go.transform.SetParent(PopupManager.Instance.contents, false);
			PopupManager.Instance.Open(go);

			var bb = go.GetComponent<Blackboard>();
			if (bb != null)
			{
				var variable = BlackboardUtils.GetOrCreateVariable<GameObject>(bb, "caller");
				variable.value = agent.gameObject;
			}
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
}
