using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas;

namespace SlotMaker
{
    public class SymbolDelegator : MonoBehaviour
    {
        public GameObject baseAnimator;
        public List<ActionListPlayer> actionList;
        public List<SymbolDelegatorCondition> conditionList;

        private GameObject _delegator;
        public GameObject delegator
        {
            get { return _delegator; }
            set { Clear(ref _delegator); _delegator = value; }
        }

        private GameObject _decorator;
        public GameObject decorator
        {
            get { return _decorator; }
            set { Clear(ref _decorator); _decorator = value; }
        }

        private Dictionary<string, ActionListPlayer> actionDict = null;
        private Dictionary<string, SymbolDelegatorCondition> conditionDict = null;
    	private ActionListPlayer activePlayer = null;

    	private const string skipAnimationName = "Skip";

        private void Awake()
    	{
    		actionDict = new Dictionary<string, ActionListPlayer>();
    		int count = actionList.Count;
    		for (int i = 0; i < count; ++i)
    		{
    			actionDict[actionList[i].gameObject.name] = actionList[i];
    		}

            conditionDict = new Dictionary<string, SymbolDelegatorCondition>();
            count = conditionList.Count;
            for (int i = 0; i < count; ++i)
            {
                conditionDict[conditionList[i].gameObject.name] = conditionList[i];
            }
    	}

        public void Clear()
        {
            baseAnimator.SetActive(true);

            Clear(ref _delegator);
            Clear(ref _decorator);
        }

        private void Clear(ref GameObject go)
        {
            if (go == null)
                return;

            var po = go.GetComponent<PooledObject>();
            if (po == null)
                UnityEngine.Object.Destroy(go);
            else
                po.ReturnToPool();

            go = null;
        }

        public void Play(BaseSymbol symbol, string animationName)
        {
            ActionListPlayer player = null;
            SymbolDelegatorCondition condition;

            if (conditionDict.TryGetValue(animationName, out condition))
                player = condition.FindPlayer(symbol, animationName);

            if (player == null && !actionDict.TryGetValue(animationName + symbol.symbolInfo.symbol, out player))
                actionDict.TryGetValue(animationName, out player);

    		if (player != null)
    		{
    			StopPlayer();
    			Play(player);
    		}
        }

        public void Skip(BaseSymbol symbol)
        {
            Play(symbol, skipAnimationName);
        }

        private void Play(ActionListPlayer player)
    	{
    		activePlayer = player;
    		activePlayer.Play(OnFinishPlayer);
    	}

    	private void StopPlayer()
    	{
    		if (activePlayer != null)
    		{
    			activePlayer.actionList.EndAction();
    			activePlayer = null;
    		}
    	}

    	private void OnFinishPlayer(bool success)
    	{
    		activePlayer = null;
    	}
    }
}
