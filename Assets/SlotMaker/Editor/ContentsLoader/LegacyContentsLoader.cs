using NodeCanvas.Framework;

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SlotMaker
{
    internal static class LegacyContentsLoader
    {
        public static void CreateSlotMachine()
        {
            var slotData = ContentCustomData.GetSlotData(0);
            int column = slotData.column;
            int row = slotData.row;

            CreateReelStripsManager(column, row);

            var slotMachine = GameObject.Find("Slot Machine").GetComponent<SlotMachine>();
            List<int> visibleCounts = slotData.visibleCounts;
            for (int i = 0; i < column; ++i)
            {
                int beginRow = row - visibleCounts[i];
                int endRow = beginRow + visibleCounts[i];
                slotMachine.CreateReel(i, beginRow, i + 1, endRow, 0);
            }
            slotMachine.Shuffle();

            // symbol controller version
            slotMachine.Visit((sb) =>
            {
                var image = sb.gameObject.GetComponent<Blackboard>().GetValue<SpriteRenderer>("baseImage");
                if (image != null)
                    ContentCustomData.Instance.symbolSprite.Apply(sb, image, 0);
            });
        }

        private static void CreateReelStripsManager(int column, int row)
        {
            var parent = ContentCustomData.Instance.transform;
            var go = new GameObject();
            go.name = "ReelStrips Manager";
            go.transform.SetParent(parent, false);

            var mgr = go.AddComponent<GlobalReelStrips>();
            mgr.stripsList = new List<ReelStrips>();

            var symbolMask = ContentCustomData.GetSlotData(0).symbolMask;
            int symbolCount = ContentCustomData.Instance.symbolCount;

            List<List<int>> reelSequenceList = new List<List<int>>();
            for (int colIndex = 0; colIndex < column; ++colIndex)
            {
                List<int> indexList = new List<int>();
                for (int i = 0; i < 20; ++i)
                    indexList.Add(UnityEngine.Random.Range(0, symbolCount));
                reelSequenceList.Add(indexList);
            }

            go = new GameObject();
            go.name = "ReelStrips";
            go.transform.SetParent(mgr.transform, false);

            var reelStrips = go.AddComponent<ReelStrips>();
            reelStrips.reelStrips = new List<BaseReelStrip>();

            for (int i = 0; i < reelSequenceList.Count; ++i)
            {
                go = new GameObject();
                go.name = "ReelStrip";
                go.transform.SetParent(reelStrips.transform, false);

                var reelStrip = go.AddComponent<ReelStrip>();
                reelStrip.stripIndex = i;
                reelStrip.strip = new List<SymbolInfo>();

                var indexList = reelSequenceList[i];
                for (int j = 0; j < indexList.Count; ++j)
                {
                    reelStrip.strip.Add(SlotUtils.CreateSymbolInfo(indexList[j], symbolMask));
                }
                reelStrips.reelStrips.Add(reelStrip);
            }
            mgr.stripsList.Add(reelStrips);
        }

        public static int LoadPaytable(string bundleName)
        {
            CreateContentLang(bundleName);
            CreateSymbolPay();

            var root = GameObject.Find("Popup Manager").transform;
            {
                var prefab = AssetBundleManager.LoadAsset<GameObject>(bundleName, "Paytable");
                GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                go.name = "Paytable";
                go.transform.SetParent(root, false);
                return go.GetComponent<Blackboard>().GetValue<int>("pageCount");
            }
        }

        public static void SelectPaytable(int pageIndex)
        {
            var paytable = GameObject.Find("Popup Manager/Paytable");
            if (paytable == null) return;
            int pageCount = paytable.GetComponent<Blackboard>().GetValue<int>("pageCount");
            pageIndex = Mathf.Min(pageIndex, pageCount - 1);
            var pages = GameObject.Find("Popup Manager/Paytable/Anchor/Pages");
            pages.GetComponent<PageScrollRect>().pageIndex = pageIndex;
            pages.GetComponent<PageScrollRect>().ForceUpdatePageIndex();
            Selection.activeGameObject = pages;
        }

        private static void CreateContentLang(string bundleName)
        {
            var contentBB = StringTable.Get(StringTable.StringTableType.Content);
            contentBB.variables.Clear();
            var stringTable = AssetBundleManager.LoadAsset<StringTableObject>(bundleName + "lang", "EN_Content").stringTable;
            foreach (var pair in stringTable)
            {
                var variable = contentBB.AddVariable(pair.key, typeof(string));
                variable.value = pair.value;
            }
        }

        private static void CreateSymbolPay()
        {
            var cb = ContentBlackboard.Get();

            var customData = cb.AddVariable("customData", typeof(Blackboard));
            customData.value = GameObject.Find("Global Blackboard/content/customData").GetComponent<Blackboard>();

            var game = BlackboardUtils.GetOrCreateBlackboard(cb, "game");
            var paytables = BlackboardUtils.GetOrCreateBlackboardList(game, "paytables");
            var child = BlackboardUtils.CreateBlackboard("List`1");
            BlackboardUtils.AddToBlackboardList(game, "paytables", child);
            BlackboardUtils.GetOrCreateBlackboardList(paytables[0], "value");

            // set dummy symbol pay
            int symbolCount = ContentCustomData.Instance.symbolCount;
            List<long> symbolPay = new List<long>();
            for (long j = 0; j < 10; ++j)
                symbolPay.Add((j + 1) * 30);

            for (int i = 0; i < symbolCount + 40; ++i)
            {
                var symbolPayBB = BlackboardUtils.CreateBlackboard("List`1");
                BlackboardUtils.SetOrCreateValue(symbolPayBB, "value", symbolPay);
                BlackboardUtils.AddToBlackboardList(paytables[0], "value", symbolPayBB);
            }

            // set normal path of scatter pay
            BlackboardUtils.SetOrCreateValue(game, "scatterPay", symbolPay);
            BlackboardUtils.SetOrCreateValue(game, "scatterPays", symbolPay);
        }
    }
}
