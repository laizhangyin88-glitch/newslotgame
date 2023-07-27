using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class BasePickMachine : MonoBehaviour, IPickMachine
    {
        [System.Serializable]
        public class PickCountWeight
        {
            public int pickCount;
            public int weight;
        }

        public FSMOwner owner;

        public int kind;
        public int countOfKind;

        public List<PickCountWeight> pickCountWeightList = new List<PickCountWeight>();

        public int resultIndex;
        public int withoutKindIndex;

        public List<BasePickSymbol> symbols = new List<BasePickSymbol>();
        public List<BasePickSymbol> remainSymbols = new List<BasePickSymbol>();
        public List<BasePickSymbol> pickSymbols = new List<BasePickSymbol>();
        public List<PickSymbolInfo> symbolInfos;

        public List<int> viewDeckList = new List<int>();

        public int totalCount;

        public int pivotPickIndex;
        public int pickCount;
        public int remainPickCount;
        
        [HideInInspector]
        public List<int> resultDeckList;
        
        public void Clear()
        {
            resultIndex = -1;
            withoutKindIndex = 0;
            pivotPickIndex = 0;
            pickCount = 0;
            remainPickCount = 0;

            if(symbolInfos != null)
                symbolInfos.Clear();
            if(viewDeckList != null)
                viewDeckList.Clear();
            if(pickSymbols != null)
                pickSymbols.Clear();
        }

        public void Initialize()
        {
            symbols = GetComponentsInChildren<BasePickSymbol>(true).ToList();

            for(int i=0; i<symbols.Count; ++i)
            {
                symbols[i].Initialize(this);
            }

            remainSymbols = new List<BasePickSymbol>();
            remainSymbols.AddRange(symbols);
            Shuffle(remainSymbols);
        }

        // resultIndoex : start 0, withoutMinKind : start 1
        public void InitDeck(int targetIndex, int withoutMinKind)
        {
            Clear();

            if(kind <= 0 || countOfKind <= 0 || targetIndex > kind || (withoutMinKind > 0 && targetIndex < withoutMinKind)) return;

            resultIndex = targetIndex;
            withoutKindIndex = withoutMinKind;
            totalCount = kind*countOfKind;
            if(symbols == null || symbols.Count != totalCount) return;

            if(countOfKind > 1)
            {
                InitMultipleDeck();
            }
            else
            {
                InitSingleDeck();
            }
        }

        public void BeginPick()
        {
            for(int i=0; i<symbols.Count; ++i)
            {
                symbols[i].Ready();
            }
        }

        public void EndPick()
        {
            // Win();
            OnEndPick();
        }

        public void OpenWithoutSymbols()
        {
            for(int i=0; i<pivotPickIndex; ++i)
            {
                remainSymbols[i].Opened(symbolInfos[i]);
            }

            remainSymbols.RemoveRange(0, pivotPickIndex);
        }

        public void Pick(BasePickSymbol symbol)
        {
            if(remainPickCount >0)
            {
                int currentIndex = pivotPickIndex + pickCount;
                ++pickCount;
                --remainPickCount;

                symbol.Open(symbolInfos[currentIndex]);

                OnPick(symbol);

                remainSymbols.Remove(symbol);
                pickSymbols.Add(symbol);

                if(!IsEnablePick())
                    EndPick();
            }
        }

        public void Win()
        {
            for(int i=0; i<pickSymbols.Count; ++i)
            {
                pickSymbols[i].Win();
            }

            OnWin();
        }

        public void OpenRemainSymbols()
        {
            for(int i=0; i<remainSymbols.Count; ++i)
            {
                int currentIndex = pivotPickIndex + pickCount + i;
                remainSymbols[i].Disable(symbolInfos[currentIndex]);
            }
        }

        public bool IsEnablePick()
        {
            if(remainPickCount >0) return true;
            return false;
        }

        private void InitSingleDeck()
        {
            // To do...
        }

        private void InitMultipleDeck()
        {
            // int pivotMin = System.Math.Min(minPickCount, totalCount);
            // int pivotMax = System.Math.Min(maxPickCount, totalCount);
            // int pivotResult = UnityEngine.Random.Range(pivotMin, pivotMax + 1) - countOfKind;
            // int fullPickCount = kind * (countOfKind-1) + 1;
            // int maxPickCount = fullPickCount - (withoutKindIndex * (countOfKind-1));
            // Debug.LogError(string.Format("FullPickCount : {0}", fullPickCount));
            // Debug.LogError(string.Format("MaxPickCount : {0}", maxPickCount));

            // Dictionary<int, int> pickCountSimulateResult = new Dictionary<int, int>();
            // for(int i=3; i<fullPickCount+1; ++i)
            // {
            //     pickCountSimulateResult[i] = 0;
            // }

            // for(int i=0; i<100000; ++i)
            // {
            //     int resultC = GetPickCount();

            //     if(pickCountSimulateResult.ContainsKey(resultC))
            //     {
            //         pickCountSimulateResult[resultC] += 1;
            //     }
            //     else
            //     {
            //         pickCountSimulateResult[resultC] = 1;
            //     }
            // }

            // foreach(KeyValuePair<int, int> data in pickCountSimulateResult)
            // {
            //     Debug.LogError(string.Format("{0} = {1}", data.Key, data.Value));
            // }

            int pivotResult = GetPickCount() - countOfKind;
            // Debug.LogError(string.Format("PickCount : {0}", pivotResult));

            List<int> openedDeckList = new List<int>();
            List<int> pickSeedList = new List<int>();
            List<int> remainDeckList = new List<int>();

            resultDeckList = new List<int>();
            symbolInfos = new List<PickSymbolInfo>();

            for(int i=0; i<kind; ++i)
            {
                if(i == resultIndex)
                {
                    // AddIndex(ref resultDeckList, i, countOfKind - 1);
                    continue;
                }
                else if(i >= withoutKindIndex)
                {
                    AddIndex(ref pickSeedList, i, countOfKind - 1);
                    AddIndex(ref remainDeckList, i, 1);
                }
                else
                {
                    AddIndex(ref openedDeckList, i, countOfKind);
                }
            }
            
            Shuffle(pickSeedList);
            Shuffle(remainDeckList);

            // Add Prev Indices
            if(pickSeedList.Count > pivotResult)
            {
                resultDeckList.AddRange(pickSeedList.GetRange(0, pivotResult));
                pickSeedList.RemoveRange(0, pivotResult);
                remainDeckList.AddRange(pickSeedList);
            }
            else
            {
                resultDeckList.AddRange(pickSeedList.GetRange(0, pickSeedList.Count));
                pickSeedList.RemoveRange(0, pickSeedList.Count);
            }

            // Add Result Indices
            AddIndex(ref resultDeckList, resultIndex, countOfKind - 1);
            Shuffle(resultDeckList);
            resultDeckList.Add(resultIndex);

            List<int> deckList = new List<int>();
            deckList.AddRange(openedDeckList);
            deckList.AddRange(resultDeckList);
            deckList.AddRange(remainDeckList);

            viewDeckList.AddRange(deckList);

            // Make Symbol Infos.
            for(int i=0; i<deckList.Count; ++i)
            {
                PickSymbolInfo info = new PickSymbolInfo();
                info.index = deckList[i];
                info.isWin = deckList[i] == resultIndex;
                symbolInfos.Add(info);
            }

            pivotPickIndex = openedDeckList.Count;
            remainPickCount = resultDeckList.Count;
        }

        private void AddIndex(ref List<int> deckList, int index, int count)
        {
            for(int i=0; i<count; ++i)
            {
                deckList.Add(index);
            }
        }

        private List<int> UniqueRandomList(int min, int max)
        {
            int count = max - min;
            List<int> result = new List<int>();
            for (int i = 0; i < count; ++i)
            {
                result.Add(min + i);
            }

            Shuffle(result);
            return result;
        }

        private void Shuffle<T>(List<T> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; ++i)
            {
                int k = UnityEngine.Random.Range(0, count);
                T temp = list[i];
                list[i] = list[k];
                list[k] = temp;
            }
        }

        private int GetPickCount()
        {
            int fullPickCount = kind * (countOfKind-1) + 1;
            int maxPickCount = fullPickCount - (withoutKindIndex * (countOfKind-1));
            int totalWeight = 0;
            List<int> weightList = new List<int>();

            for(int i=0; i<pickCountWeightList.Count; ++i)
            {
                if(maxPickCount >= pickCountWeightList[i].pickCount)
                {
                    totalWeight += pickCountWeightList[i].weight;
                    weightList.Add(totalWeight);
                }
            }

            int resultWeight = UnityEngine.Random.Range(0, totalWeight);

            int resultWeightIndex = 0;
            for(int i=0; i<weightList.Count; ++i)
            {
                if(resultWeight < weightList[i])
                    return pickCountWeightList[i].pickCount;
            }

            Debug.LogError(string.Format("Out of Range : {0}", resultWeight));
            return 0;
        }

        protected virtual void OnPick(BasePickSymbol symbol){}
        protected virtual void OnWin(){}
        protected virtual void OnEndPick(){}
    }

}