using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/Contents")]
    public class InstantiatePooledSymbolsGraph : ActionTask
    {
        public BBParameter<ObjectPool> symbolPool = null;
        public BBParameter<GameObject> localSymbolAssets = null;

        protected override string info
        {
            get
            {
                bool useLocalSymbolAssets = !localSymbolAssets.isNull || localSymbolAssets.isDefined;
                string symbolAssetsInfo = (useLocalSymbolAssets) ? string.Format("SymbolAssets {0}", localSymbolAssets) : "GlobalSymbolAssets";
                
                return "Instantiate Pooled Symbols Graph with " + symbolAssetsInfo;
            }
        }

        protected override void OnExecute()
        {
            var symbols = symbolPool.value.availiableObjects;
            var symbolAssets = localSymbolAssets.value?.GetComponent<SymbolAssets>() ?? GlobalSymbolAssets.Instance;

            for (int i = 0; i < symbols.Count; ++i)
            {
                var symbolController = symbols[i].GetComponent<SymbolController>();

                int assetCount = symbolAssets.assets.Count;
                for (int j = 0; j < assetCount; j++)
                {
                    Graph srcGraph = symbolAssets.GetGraph(j);
                    symbolController.TryInstantiateGraph(srcGraph);
                }
            }

            EndAction();
        }
    }

}
