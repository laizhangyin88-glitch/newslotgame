using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using SlotMaker;

namespace BagelCode.HiddenObjects
{
    [System.Serializable]
    public class InGameBGMData
    {
        public string symbol;
        public List<BundleTitleName> stageBgmList;
    }

    [System.Serializable]
    public struct BundleTitleName
    {
        public string bundlePostfix;
        public string bgmTitle;
    }

    [CreateAssetMenu(fileName = "HiddenObjectsInGameBGMData", menuName = "Meta/ScriptableObject/HiddenObjectsInGameBGMData")]
    public class HiddenObjectsInGameBGMData : ScriptableObject
    {
#if UNITY_EDITOR
        [PropertyOrder(0)]
        [Button(ButtonSizes.Medium, Name = "Set Stage BGMs")]
        private void SetupStagesBGM(string targetSymbol = "")
        {
            for (int i = 0; i < bgmDataList.Count; ++i)
            {
                for (int j = 0; j < bgmDataList[i].stageBgmList.Count; ++j)
                {
                    if (!string.IsNullOrEmpty(targetSymbol) &&
                        targetSymbol != bgmDataList[i].symbol) continue;

                    string bundle = string.Format("mghiddenobjects{0}st{1}", bgmDataList[i].symbol, j + 1).ToLower();
                    string asset = string.Format("{0} Stage {1}", bgmDataList[i].symbol, j + 1);
                    var stagePrefab = AssetBundleManager.LoadAsset<GameObject>(bundle, asset);
                    if (stagePrefab != null)
                    {
                        var stageController = stagePrefab.GetComponent<HiddenObjectsInGameStageController>();
                        if (stageController is null)
                        {
                            stageController = stagePrefab.AddComponent<HiddenObjectsInGameStageController>();
                        }

                        var title = bgmDataList[i].stageBgmList[j].bgmTitle;
                        string bgmName = "HOG_BGM_" + title;

                        var bgmBundleKey = bgmDataList[i].stageBgmList[j].bundlePostfix;
                        string bgmBundle = string.Format("mghiddenobjectsbgm{0}", bgmBundleKey).ToLower();
                        var clip = AssetBundleManager.LoadAsset<AudioClip>(bgmBundle, bgmName);

                        stageController.bgmName = bgmName;
                        stageController.clip = clip;

                        UnityEditor.EditorUtility.SetDirty(stagePrefab); // Make Diff
                        Debug.Log("Complete:" + asset);
                    }
                }
            }
        }
#endif
        [PropertyOrder(1)]
        public List<InGameBGMData> bgmDataList;
    }
}
