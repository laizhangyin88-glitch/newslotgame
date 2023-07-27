using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas;

namespace SlotMaker
{
	public class ActionListPlayerCollection : MonoBehaviour
	{
		public List<ActionListPlayer> actionListPlayers;

		private Dictionary<string, ActionListPlayer> actionListPlayerDict = new Dictionary<string, ActionListPlayer>();

		private void Awake()
		{
			int count = actionListPlayers.Count;
			for (int i = 0; i < count; ++i)
			{
				var player = actionListPlayers[i];
				actionListPlayerDict[player.gameObject.name] = player;
			}
		}

		public ActionListPlayer GetActionListPlayer(int actionIndex)
		{
			return actionListPlayers[actionIndex];
		}

		public ActionListPlayer FindActionListPlayer(string actionName)
		{
			ActionListPlayer player;
			if (actionListPlayerDict.TryGetValue(actionName, out player))
				return player;

			return null;
		}
	}
}
