using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using TMPro;

namespace BagelCode.GemJackpot
{
    public class GemJackpotSymbol : MonoBehaviour
    {
		public Animator animator = null;
        public List<GameObject> customObjects;
		public GameObject cachingObject;
		public Animator stopEffectAnimator;

		private readonly int checkRow = 0;

		private readonly string[] META_GEMJACKPOT_SYMBOLSTOP =
        {
			"Meta_Gemjackpot_Coinstop",
			"Meta_Gemjackpot_Reelstop",
			"Meta_Gemjackpot_Bonusstop"
		};

		public void Apply(BaseSymbol symbol)
        {
			// index type : GemJackpotSlotSymbolType
			int reelIndex = symbol.reel.reelIndex;
			int symbolData = symbol.symbolInfo.symbol;

			int customType = GetCustomType(reelIndex, symbolData, symbol);
			ToggleCustomObject(customType);
		}

		private int GetCustomType(int reelIndex, int symbolData, BaseSymbol symbol)
		{
			switch (symbolData)
			{
				case 1: // Coin
					symbol.symbolInfo.mask = SymbolAttribute.Blank;	// SymbolAttribute.Scatter1;
					return 0;
				case 3:	// BONUS
					int value = 1;
					if (reelIndex >= 0 && reelIndex < 5)
						value = value + reelIndex;
					symbol.symbolInfo.mask = SymbolAttribute.Scatter2;
					return value;
			}
			symbol.symbolInfo.mask = SymbolAttribute.Blank;
			return -1;	// Empty
		}

		private void ToggleCustomObject(int customType)
		{
			int count = customObjects.Count;
			for (int i = 0; i < count; ++i)
			{
				bool isActive = i == customType;
				customObjects[i].SetActive(isActive);
				if (isActive == true)
					animator = customObjects[i].GetComponent<Animator>();
			}
		}

		public void ClearCachingObject()
        {
			if (cachingObject != null)
            {
				var pooledObject = cachingObject.GetComponent<PooledObject>();
				if (pooledObject != null)
					pooledObject.ReturnToPool();
				else
					Destroy(cachingObject);

				cachingObject = null;
			}
        }

		protected void PlayAnimation(BaseSymbol symbol)
        {
			if (animator == null) return;

			switch(symbol.symbolIndex)
            {
				case 1:
					animator.SetBool("isLight", true);
					break;
				case 3:
					if (GemJackpotUtils.CheckConsecutiveBonusSymbol(symbol.reel.reelIndex))
						animator.SetBool("isLight", true);
					break;
            }
        }

		public void Stop(BaseSymbol symbol)
		{
			if (symbol.row == checkRow)
            {
				PlayAnimation(symbol);
			}
        }

		public void Play(BaseSymbol symbol, string animationName)
        {
            // Symbol win(animationName = Win)
            //Debug.Log("Symbol Play : " + symbol.reel.reelIndex + " / " + symbol.row + " / " + symbol.symbolIndex + " / " + animationName);
            if (animator != null && symbol.row == checkRow)
			{
				if (animationName.CompareTo("PrepareStop") == 0)
                {
					if (stopEffectAnimator != null)
						stopEffectAnimator.SetTrigger("isActive");

					PlayAnimation(symbol);

					if (symbol.symbolIndex == 1 || symbol.symbolIndex == 2)
						GSManager.Instance.GetHandler(META_GEMJACKPOT_SYMBOLSTOP[symbol.symbolIndex - 1]).Play();
					else if (symbol.symbolIndex == 3)
					{
						if (GemJackpotUtils.CheckConsecutiveBonusSymbol(symbol.reel.reelIndex))
							GSManager.Instance.GetHandler(META_GEMJACKPOT_SYMBOLSTOP[symbol.symbolIndex - 1] + symbol.reel.reelIndex.ToString()).Play();
						else
							GSManager.Instance.GetHandler(META_GEMJACKPOT_SYMBOLSTOP[1]).Play();
					}
				}
				else if (animationName.CompareTo("Win") == 0)
				{
					animator.SetBool("isActive", true);
					if (symbol.symbolIndex == 1) MoveToCoin(symbol);
					else if (symbol.symbolIndex == 3 && symbol.reel.reelIndex == 0)
						GSManager.Instance.GetHandler("Meta_Gemjackpot_Bonussymbol").Play();
				}
			}
        }

		protected void MoveToCoin(BaseSymbol symbol)
        {
			// Slot coin move to guage
			Blackboard bb = GemJackpotUtils.GemJackpotInfo;
			GameObject toObject = BlackboardUtils.FindValue<GameObject>(bb, "coinSymbolMoveArea");
			GameObject poolObject = BlackboardUtils.FindValue<GameObject>(bb, "coinSymbolMovePool");

			if (toObject != null && poolObject != null && poolObject.GetComponent<ObjectPool>() != null)
			{
				Vector3 toPosition = toObject.transform.position;
				Vector3 fromPosition = this.transform.position;
				ObjectPool objectPool = poolObject.GetComponent<ObjectPool>();

				GameObject go = GetCoinPooledObject(objectPool);
				go.transform.position = fromPosition;

				GemJackpotSymbolCoinController controller = go.GetComponent<GemJackpotSymbolCoinController>();
				if (controller != null)
                {
					controller.SetMoveTransform(transform, toObject.transform);
					go.SetActive(true);
					//animator.gameObject.SetActive(false);
					controller.MoveCoin(animator.gameObject);
                }
			}
        }

		protected GameObject GetCoinPooledObject(ObjectPool objectPool)
		{
			var po = objectPool.GetObject(true);
			var go = po.gameObject;
			go.transform.SetParent(objectPool.transform, false);
			go.SetActive(false);

			return go;
		}
	}
}