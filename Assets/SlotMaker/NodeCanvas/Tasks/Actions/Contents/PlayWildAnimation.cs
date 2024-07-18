using BagelCode;
using NodeCanvas.Framework;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Tasks.Actions.Contents
{
    public class PlayWildAnimation : ActionTask
    {
        private float animationTime = 2;

        public BBParameter<int> slotIndex = 0;

        private float time = 0;

        protected List<GameObject> gameObjects = new List<GameObject>();

        private List<Animator> animations = new List<Animator>();
        protected override void OnExecute()
        { 
            gameObjects = new List<GameObject>();
            animations = new List<Animator>();
            GameObject slotMachine = ContentCustomData.GetSlotData(slotIndex.value).slotMachine;
            var baseSlotMachine = slotMachine.GetComponent<BaseSlotMachine>();
            List<Symbol> symbols = new List<Symbol>();
            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
            var deck = (Deck)slotData.deck.Clone();

            for (int i = 0; i < deck.deck.Count; i++)
            {
                var temp = deck.deck[i];
                for (int j = 0; j < temp.Count; j++)
                {
                    var symbol = temp[j];
                    if(symbol.symbol == 0)///水果派对，wild牌的值为 0
                    {
                        var symbolGo = baseSlotMachine.GetSymbol(i, j);
                        PlayAnimation(symbolGo);
                    }
                }
            }
            if(BlackboardUtils.GetOrCreateVariable<List<GameObject>>(BlackboardUtils.GetContentFSMBlackboard(), "_wildList").value == null)
            {
                BlackboardUtils.GetOrCreateVariable<List<GameObject>>(BlackboardUtils.GetContentFSMBlackboard(), "_wildList").value = new List<GameObject>();
            }
            BlackboardUtils.GetOrCreateVariable<List<GameObject>>(BlackboardUtils.GetContentFSMBlackboard(), "_wildList").value.AddRange(gameObjects);
             
            EndAction();
        }

        private void PlayAnimation(BaseSymbol symbol)
        {
            //GameObject prefab = AssetBundleManager.LoadAsset<GameObject>(BlackboardUtils.FindVariable<string>(null, "./game/gameTitle").value, symbolName);
            string bundleName = BlackboardUtils.FindVariable<string>(null, "./game/gameTitle").value;
            GameObject prefab = null;
            if (Mathf.Round(symbol.transform.localPosition.y) > 0)   ///在上面，从上往下运动
            {
                //MoveAnimation(1, symbol, symbol.transform.localPosition);
                prefab = AssetBundleManager.LoadAsset<GameObject>(bundleName, "zuanshi02");
            }
            else if(Math.Round(symbol.transform.localPosition.y) < 0)///在下面，从下往上运动
            {
                //MoveAnimation(0, symbol, symbol.transform.localPosition);
                prefab = AssetBundleManager.LoadAsset<GameObject>(bundleName, "zuanshi01");
            }
            else ///在中间
            {
                prefab = AssetBundleManager.LoadAsset<GameObject>(bundleName, "zuanshi03");
            }
            CreateWild(symbol);
            if (prefab != null)
            {
                GameObject go = GameObject.Instantiate(prefab);
                gameObjects.Add(go);

                go.transform.SetParent(symbol.transform.parent, false);
                go.transform.localScale = Vector3.one * 43;
                go.transform.localPosition = new Vector3(0, -42, 0);
                TimerExtensions.DelayAction(go, 1.2f, () => 
                {
                    for (global::System.Int32 i = 0; i < gameObjects.Count; i++)
                    {
                        if(i == gameObjects.Count - 1)
                        {
                            gameObjects[i].gameObject.SetActive(false);
                        }
                        else
                        {
                            gameObjects[i].gameObject.SetActive(true);
                        }
                    }
                    go.SetActive(false);
                });
            }
        }

        private void CreateWild(BaseSymbol baseSymbol)
        { 
            int y = 140;
            for (int i = 0; i < 3; i++)
            {
                GameObject game = loadAnimation();
                game.SetActive(false);
                game.transform.SetParent(baseSymbol.transform.parent, false);
                game.transform.localScale = Vector3.one;
                game.transform.localPosition = new Vector3(0, y - y * i, 0);
            }
        }

        private GameObject loadAnimation()
        {
            var symbolName = "A_frame_wild";//ContentCustomData.Instance.symbolName[index];

            GameObject prefab = AssetBundleManager.LoadAsset<GameObject>(BlackboardUtils.FindVariable<string>(null, "./game/gameTitle").value, symbolName);
            GameObject temp = GameObject.Instantiate(prefab);
            gameObjects.Add(temp);
            temp.name = symbolName; 
            var animation = temp.GetComponentInChildren<Animator>();
            animations.Add(animation);
            
            return temp;
        }
    }
}
