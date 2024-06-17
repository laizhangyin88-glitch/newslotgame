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
        public BBParameter<int> slotIndex = 0;

        private float time = 0;

        protected List<GameObject> gameObjects = new List<GameObject>();

        private List<Animator> animations = new List<Animator>();
        protected override void OnExecute()
        { 
            gameObjects = new List<GameObject>();
            animations = new List<Animator>();
            Debug.LogError("播放动画...............................");
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
            BlackboardUtils.GetOrCreateVariable<List<GameObject>>(BlackboardUtils.GetContentFSMBlackboard(), "_wildList").value = gameObjects;
            EndAction();
        }

        private void PlayAnimation(BaseSymbol symbol)
        {
            if(Mathf.Round(symbol.transform.localPosition.y) > 0)   ///在上面，从上往下运动
            {
                MoveAnimation(1, symbol, symbol.transform.localPosition);
            }
            else if(Math.Round(symbol.transform.localPosition.y) < 0)///在下面，从下往上运动
            {
                MoveAnimation(0, symbol, symbol.transform.localPosition);
            }
            else ///在中间
            {
                MoveAnimation(2, symbol, symbol.transform.localPosition);
            }
        }

        private void MoveAnimation(int isDown, BaseSymbol baseSymbol, Vector3 from)
        {
            int count = 0;
            var wild = loadAnimation(baseSymbol.symbolInfo.symbol);
            wild.transform.SetParent(baseSymbol.transform, false);
            gameObjects.Add(wild.gameObject);
            for (int i = 0; i < 2; i++)
            {
                GameObject go = GameObject.Instantiate(baseSymbol.gameObject); 
                //go.transform.GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
                go.name = "TTTTTTTTTTTTT";
                var temp = loadAnimation(baseSymbol.symbolInfo.symbol);
                temp.transform.SetParent(go.transform, false);
                go.transform.SetParent(baseSymbol.transform.parent);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = from;
                gameObjects.Add(go);
                if (isDown == 1)    ///在上面，从上往下运动
                {
                    if (count == 0)
                    {
                        AsyncActionUtils.ApplyLocalMovement(go.GetComponent<BaseSymbol>(), go.transform, from, Vector3.zero, 3, TweenUtils.VectorTweenLinear);
                    } 
                    else
                    {
                        AsyncActionUtils.ApplyLocalMovement(go.GetComponent<BaseSymbol>(), go.transform, from, new Vector3(0, -140,0), 3, TweenUtils.VectorTweenLinear);
                    }
                }
                else if(isDown == 0)///在下面，从下往上运动
                {
                    if (count == 0)
                    {
                        AsyncActionUtils.ApplyLocalMovement(go.GetComponent<BaseSymbol>(), go.transform, from, Vector3.zero, 3, TweenUtils.VectorTweenLinear);
                    }
                    else
                    {
                        AsyncActionUtils.ApplyLocalMovement(go.GetComponent<BaseSymbol>(), go.transform, from, new Vector3(0, 140, 0), 3, TweenUtils.VectorTweenLinear);
                    }
                }
                else
                {
                    if (count == 0)
                    {
                        AsyncActionUtils.ApplyLocalMovement(go.GetComponent<BaseSymbol>(), go.transform, from, new Vector3(0, 140, 0), 3, TweenUtils.VectorTweenLinear);
                    }
                    else
                    {
                        AsyncActionUtils.ApplyLocalMovement(go.GetComponent<BaseSymbol>(), go.transform, from, new Vector3(0, -140, 0), 3, TweenUtils.VectorTweenLinear);
                    }
                }
                count++;
            }
        }


        private GameObject loadAnimation(int index)
        {
            var symbolName = "A_frame_wild";//ContentCustomData.Instance.symbolName[index];

            GameObject prefab = AssetBundleManager.LoadAsset<GameObject>(BlackboardUtils.FindVariable<string>(null, "./game/gameTitle").value, symbolName);
            GameObject temp = GameObject.Instantiate(prefab);
            temp.name = symbolName;
            var animation = temp.GetComponentInChildren<Animator>();
            animations.Add(animation);
            animation.Play("Default");
            //animation.GetCurrentAnimatorStateInfo(0) = true;
            return temp;
        }

        protected override void OnUpdate()
        {
            Debug.LogError("@@@@@@@@@@@@@@@@@@@@");
            if(animations.Count > 0)
            {
                if((time+=Time.deltaTime) > 1.938)
                {
                    for (global::System.Int32 i = 0; i < animations.Count; i++)
                    {
                        animations[i].Play("Default");
                    }
                    time = 0;
                }
            }
        }
    }
}
