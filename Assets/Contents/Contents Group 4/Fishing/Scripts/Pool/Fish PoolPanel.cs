using SlotMaker.Cards.Tasks.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishPoolPanel
    {
        private Transform mtrans;
        public List<GameObject> PoolList = new List<GameObject>();
        public FishShakeCamera ShakeCamera;

        public FishPoolPanel(GameObject gameObject)
        {
            InitView(gameObject);
        }

        private void InitView(GameObject gameObject)
        {
            mtrans = gameObject.transform;
            FindView(mtrans);
            GetFishRootObj();
        }

        private void FindView(Transform trans)
        {
            GameObject PoolObj = trans.Find("Pool_Fish").gameObject;
            PoolList.Add(PoolObj);
            PoolObj = trans.Find("Pool_Bullet").gameObject;
            PoolList.Add(PoolObj);
            PoolObj = trans.Find("Pool_Net").gameObject;
            PoolList.Add(PoolObj);
            PoolObj = trans.Find("Pool_Gold").gameObject;
            PoolList.Add(PoolObj);
            PoolObj = trans.Find("Pool_Score").gameObject;
            PoolList.Add(PoolObj);
            PoolObj = trans.Find("Pool_PlusTips").gameObject;
            PoolList.Add(PoolObj);
            PoolObj = trans.Find("Pool_Effect").gameObject;
            PoolList.Add(PoolObj);
            PoolObj = trans.Find("Pool_FishOutTips").gameObject;
            PoolList.Add(PoolObj);
            PoolObj = trans.Find("Pool_SpecialDeclare").gameObject;
            PoolList.Add(PoolObj);
            PoolObj = trans.Find("Pool_Lightning").gameObject;
            PoolList.Add(PoolObj);
            PoolObj = trans.Find("Pool_TipsContent").gameObject;
            PoolList.Add(PoolObj);
        }

        private void GetFishRootObj()
        {
            ShakeCamera = PoolList[0].GetComponent<FishShakeCamera>();
            if (ShakeCamera == null)
                ShakeCamera = PoolList[0].AddComponent<FishShakeCamera>();
            ShakeCamera.enabled = false;
        }

    }
}

