using SlotMaker;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishTipsContentManager : MonoSingleton<FishTipsContentManager>
    {
        List<FishTipsContentItem> AllUsedTipsInsList;
        Dictionary<int, FishTipsContentItem> CurrentUseTipsList;
        int UID;
        string TipsContentName;

        private void Awake()
        {
            AllUsedTipsInsList = new List<FishTipsContentItem>();
            CurrentUseTipsList = new Dictionary<int, FishTipsContentItem>();
            UID = 0;
            TipsContentName = "TipsContent";
        }

        public int GetUID()
        {
            UID += 1;
            if (UID > 200)
                UID = 1;
            return UID;
        }

        public void SetShowTipsContent(FishFishBase fishIns)
        {
            GetTipsContent(fishIns);
        }

        public FishTipsContentItem GetTipsContent(FishFishBase fishIns)
        {
            if (AllUsedTipsInsList != null && AllUsedTipsInsList.Count > 0)
            {
                FishTipsContentItem tipsContent = AllUsedTipsInsList[0];
                AllUsedTipsInsList.RemoveAt(0);
                if (tipsContent != null)
                {
                    int UID = GetUID();
                    CurrentUseTipsList[UID] = tipsContent;
                    tipsContent.ResetState(fishIns, UID);
                    return tipsContent;
                }
                else
                {
                    Debug.LogError("Failed to retrieve TipsContentItem instance from pool");
                    return null;
                }
            }
            else
            {
                GameObject tipsContentObj = FishGameObjectPoolManager.Instance.GetGameObject(TipsContentName, PoolType.TipsContent);
                if (tipsContentObj != null)
                {
                    FishTipsContentItem tipsContent = new FishTipsContentItem(tipsContentObj);
                    if (tipsContent != null)
                    {
                        int UID = GetUID();
                        CurrentUseTipsList[UID] = tipsContent;
                        tipsContent.ResetState(fishIns, UID);
                        return tipsContent;
                    }
                    else
                    {
                        Debug.LogError("Failed to create TipsContentItem instance");
                        FishGameObjectPoolManager.Instance.ReCycleToGameObject(tipsContentObj, PoolType.TipsContent);
                        return null;
                    }
                }
                else
                {
                    Debug.LogError("Failed to retrieve TipsContentItem prefab from pool: " + TipsContentName);
                    return null;
                }
            }
        }

        public void RemoveTipsContent(FishTipsContentItem tipsContent)
        {
            FishTipsContentItem tempTips = CurrentUseTipsList[tipsContent.UID];
            if (tempTips != null)
            {
                if (AllUsedTipsInsList == null)
                {
                    AllUsedTipsInsList = new List<FishTipsContentItem> ();
                }
                AllUsedTipsInsList.Add(tipsContent);
                CurrentUseTipsList.Remove(tipsContent.UID);
            }
            else
            {
                Debug.LogError("TipsContentUID to be removed is null: " + tipsContent.UID);
            }
        }

        public void ClearAllUsingTipsContent()
        {
            if (CurrentUseTipsList != null)
            {
                foreach (FishTipsContentItem tipsContent in CurrentUseTipsList.Values)
                {
                    Destroy(tipsContent.gameObject);
                    RemoveTipsContent(tipsContent);
                }
            }
            CurrentUseTipsList = new Dictionary<int, FishTipsContentItem>();
        }

        public void UpdateTipsContentAutoDestory()
        {
            List<int> removeKeysCatch = new List<int>();
            if (CurrentUseTipsList != null && CurrentUseTipsList.Count > 0)
            {
                foreach (var item in CurrentUseTipsList)
                {
                    if (item.Value.isCanDestroy)
                    {
                        item.Value.Destroy();
                        removeKeysCatch.Add(item.Key);
                    }
                }

                for (int i = 0; i < removeKeysCatch.Count; i++)
                    RemoveTipsContent(CurrentUseTipsList[removeKeysCatch[i]]);
            }
        }

        private void Update()
        {
            foreach (var item in CurrentUseTipsList.Values)
                item.Update();
            UpdateTipsContentAutoDestory();
        }
    }
}
