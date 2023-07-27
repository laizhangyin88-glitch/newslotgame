using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using NodeCanvas.Framework;
using TMPro;
using System;

namespace GameStudio.Slot.FSF
{
    public class FSFCommunityMainSlotFrameWinHandler :FSFCommunityFrameWinHandler

    {
        [SerializeField]
        private TextMeshProUGUI spinCountUI;
        [SerializeField]
        private Animator spinCountAnimator;
        [SerializeField]
        private ObjectPool addedSpinFlyingObjPool;
        [SerializeField]
        private ObjectPool multiplierFlyingObjPool;
        [SerializeField]
        private ObjectPool winBoxMultiplierFlyingObjPool;

        public float frameMultiplierApplyingDuration;
        public float symbolWinDuration;
        public float lastSymbolWinWaitTime;
        public float extraSpinAnimDuration;

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override IEnumerator StartFrameWinFlow(Frame frame, long frameMultiplier)
        {
            var frameList = GetFrameList(winCalcTryCount);
            var hitSymbolSet = GetFrameHitSymbolList(frameList);

            List<BaseSymbol> extraSpinSymbolList = new List<BaseSymbol>();
            foreach(var symbol in hitSymbolSet)
            {
                if (symbol.symbolIndex == PLUS_SPIN_SYMBOL_INDEX)
                {
                    extraSpinSymbolList.Add(symbol);
                    symbol.Play("Win");
                }
            }

            if (extraSpinSymbolList.Count > 0)
            {
                yield return new WaitForSeconds(extraSpinAnimDuration);
                GSManager.Instance.GetHandler("FS Fly").Play();
            }

            BlackboardUtils.FindVariable<int>(null, "./bonus/totalSpinCount").value += extraSpinSymbolList.Count;
            var remainSpinCount = BlackboardUtils.FindVariable<int>(null, "./bonus/totalSpinCount").value - BlackboardUtils.FindVariable<int>(null, "./bonus/spinCount").value;

            FlyObjectToDest(addedSpinFlyingObjPool, spinCountUI.transform, extraSpinSymbolList);
            yield return StartCoroutine(FSFWinFrameController.CallActionAfterDelay(() =>
            {
                if (remainSpinCount > 1)
                    spinCountUI.text = string.Format(FSFSpinCountUIAdmin.EXTRA_SPINS_TEXT_FORMAT, remainSpinCount);
                else
                    spinCountUI.text = string.Format(FSFSpinCountUIAdmin.EXTRA_SPIN_TEXT_FORMAT, remainSpinCount);
                if(extraSpinSymbolList.Count > 0) spinCountAnimator.SetTrigger("Collect");
            },
                1f));


            var myFrameHitSymbolList = GetFrameHitSymbolList(frame);
            ApplyFrameMultiplierToSymbols(myFrameHitSymbolList, frameMultiplier);
            if(myFrameHitSymbolList.Count > 0) yield return new WaitForSeconds(frameMultiplierApplyingDuration);

            foreach (var symbol in hitSymbolSet)
            {
                if (symbol.symbolIndex == BLANK_SYMBOL_INDEX || symbol.symbolIndex == PLUS_SPIN_SYMBOL_INDEX) continue;
                symbol.Play("Win");

                long symbolMultiplier = MultiplierPerSymbolIndex[symbol.symbolIndex] ;
                if (myFrameHitSymbolList.Contains(symbol)) symbolMultiplier *= frameMultiplier;
                long newMultiplier = symbolMultiplier + accumulatedMultiplier;
                accumulatedMultiplier = newMultiplier;

                var flyingObj = multiplierFlyingObjPool.GetObject();
                var flyingPositionController = flyingObj.GetComponentInChildren<DirectionalWeightPositionController>(true);
                flyingPositionController.from = symbol.transform;
                flyingPositionController.to = MultiplierText.transform;
                flyingObj.gameObject.SetActive(true);

                GSManager.Instance.GetHandler("FS Fly").Play();
                StartCoroutine(FSFWinFrameController.CallActionAfterDelay(() => {
                    MultiplierText.text = string.Format(FSFSymbolEventHandler.MULTIPLIER_STRING_FORMAT, newMultiplier);
                    MultiplierAnimator.SetTrigger("Collect");
                    flyingObj.ReturnToPool();
                }, 0.7f));
                yield return new WaitForSeconds(symbolWinDuration);
            }
            if(hitSymbolSet.Count > 0) yield return new WaitForSeconds(lastSymbolWinWaitTime);
            winCalcTryCount++;

            yield break;
        }

        protected void ApplyFrameMultiplierToSymbols(List<BaseSymbol> hitSymbolList, long multiplier)
        {
            if (hitSymbolList.Count > 0)
            {
                WinFrame.GetComponent<Animator>().SetBool("Skip Effect", false);
                WinFrame.GetComponent<Animator>().SetTrigger("Multiply");
                GSManager.Instance.GetHandler("Multiplier Change").Play();
            }

            var pooledFlyingMultiplierList = new List<PooledObject>();
            foreach (var symbol in hitSymbolList)
            {
                if (symbol.symbolIndex != BLANK_SYMBOL_INDEX && symbol.symbolIndex != PLUS_SPIN_SYMBOL_INDEX)
                {
                    long newMultiplier = MultiplierPerSymbolIndex[symbol.symbolIndex] * multiplier;

                    var winBoxMultiplierFlyingObj = winBoxMultiplierFlyingObjPool.GetObject();
                    pooledFlyingMultiplierList.Add(winBoxMultiplierFlyingObj);
                    winBoxMultiplierFlyingObj.GetComponentInChildren<TextMeshProUGUI>().text = string.Format(FSFSymbolEventHandler.MULTIPLIER_STRING_FORMAT, multiplier); ;
                    var positionController = winBoxMultiplierFlyingObj.GetComponentInChildren<DirectionalWeightPositionController>();
                    positionController.from = WinFrame.GetComponentInChildren<TextMeshProUGUI>().transform;

                    bool isHighMultiplier = symbol.symbolIndex == 15;
                    var symbolEventHandler = symbol.GetComponent<FSFSymbolEventHandler>();
                    positionController.to =isHighMultiplier ? symbolEventHandler.multiplierHighAnchor :symbolEventHandler.multiplierNormalAnchor;

                    StartCoroutine(FSFWinFrameController.CallActionAfterDelay(() =>
                    {
                        symbol.Play("OnMultiply");
                        var winPrefab = symbol.GetComponent<SymbolEventHandler>().GetCachedObject(1);
                        var prefabMultiplierText = winPrefab.GetComponent<Blackboard>().GetValue<TextMeshProUGUI>("increasingMultiplierText");
                        prefabMultiplierText.text = string.Format(FSFSymbolEventHandler.MULTIPLIER_STRING_FORMAT, newMultiplier);
                        var winBoxMultiplier = winPrefab.GetComponent<Blackboard>().GetValue<TextMeshProUGUI>("winBoxMultiplier");
                        winBoxMultiplier.text = string.Format(FSFSymbolEventHandler.MULTIPLIER_STRING_FORMAT, multiplier);
                    }, 1f));

                    StartCoroutine(FSFWinFrameController.CallActionAfterDelay(() =>
                    {
                        var creditText = symbol.GetComponent<FSFSymbolEventHandler>().creditText;
                        creditText.text = string.Format(FSFSymbolEventHandler.MULTIPLIER_STRING_FORMAT, newMultiplier);
                    }, 2f));
                }
            }

            StartCoroutine(FSFWinFrameController.CallActionAfterDelay(() =>
            {
                pooledFlyingMultiplierList.ForEach((pooledObj) => { pooledObj.ReturnToPool(); });
            }, 1f));
        }

        private List<Frame> GetFrameList(int spinIndex)
        {
            var frameList = new List<Frame>();
            foreach (var communityResult in CommunityUserResultList)
            {
                var frame = FSFWinFrameController.GetFrameFromBlackboard(communityResult.GetValue<List<Blackboard>>("frameInfoPerSpin")[spinIndex]);
                frameList.Add(frame);
            }

            return frameList;
        }

        public HashSet<BaseSymbol> GetFrameHitSymbolList(List<Frame> frameList)
        {
            List<BaseSymbol> symbolList = new List<BaseSymbol>();
            foreach (var frame in frameList)
                symbolList.AddRange(GetFrameHitSymbolList(frame));

            symbolList.Sort(new SymbolSorter());
            HashSet<BaseSymbol> hitSymbolSet = new HashSet<BaseSymbol>();
            foreach (var hitSymbol in symbolList) hitSymbolSet.Add(hitSymbol);

            return hitSymbolSet;
        }

        public List<BaseSymbol> GetFrameHitSymbolList(Frame frame)
        {
            List<BaseSymbol> symbolList = new List<BaseSymbol>();
            for (int colIndex = frame.column; colIndex < frame.column + frame.width; colIndex++)
            {
                for (int rowIndex = frame.row; rowIndex > frame.row - frame.height; rowIndex--)
                {
                    var symbol = SlotMachine.GetSymbol(colIndex, rowIndex);
                    if (symbol.symbolIndex != BLANK_SYMBOL_INDEX) symbolList.Add(symbol);
                }
            }

            return symbolList;
        }

        public List<BaseSymbol> GetSymbolList(int startRow, int endRow, int startCol, int endCol)
        {
            List<BaseSymbol> symbolList = new List<BaseSymbol>();
            for (int i = startCol; i < endCol; i++)
            {
                for (int j = startRow; j < endRow; j++)
                {
                    symbolList.Add(SlotMachine.GetSymbol(i, j));
                }
            }

            return symbolList;
        }

        private void FlyObjectToDest(ObjectPool positionControllerObjectPool, Transform flyingDest, List<BaseSymbol> flyingStartSymbolList)
        {
            foreach (var symbol in flyingStartSymbolList) FlyObjectToDest(positionControllerObjectPool, flyingDest, symbol);
        }

        private void FlyObjectToDest(ObjectPool positionControllerObjectPool, Transform flyingDest, BaseSymbol flyingStartSymbol)
        {
            var pooledObj = positionControllerObjectPool.GetObject();
            var positionController = pooledObj.GetComponentInChildren<DirectionalWeightPositionController>();
            positionController.from = flyingStartSymbol.transform;
            positionController.to = flyingDest;
            pooledObj.gameObject.SetActive(true);
            StartCoroutine(FSFWinFrameController.CallActionAfterDelay(() => { pooledObj.gameObject.SetActive(false); pooledObj.ReturnToPool(); }, 1.25f));
        }
    }

    public class SymbolSorter : IComparer<BaseSymbol>
    {
        public int Compare(BaseSymbol comparer0, BaseSymbol comparer1)
        {
            if (comparer0.column == comparer1.column) return comparer0.row - comparer1.row;
            return comparer0.column - comparer1.column;
        }
    }
}
