using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace BagelCode
{
    public class FishComboFish : FishFishBase
    {
        public List<string> AnimParams;
        public int ComboFishCount;
        public SpriteRenderer[] BGGroupRenderList;
        public List<GameObject> FishGroupList;
        public List<Transform> SubScaleFishList;
        public List<FishFishBase> ChildFishInsList;
        public Vector3 dieMoveToTargetPos;
        public FishComboFish()
        {
            Init();
        }

        private void Init()
        {
            InitData();
        }

        private void InitData()
        {
            AnimParams = new List<string>() { "Fish_Move", "Fish_Catch" };
            ComboFishCount = 1;
            FishGroupList = new List<GameObject>();
            SubScaleFishList = new List<Transform>();
            ChildFishInsList = new List<FishFishBase>();
        }

        public override void BuildFish(FishVo vo, GameObject obj)
        {
            InitBaseFish(vo, obj);
            CaculateComboFishCount();
            FindView();
            BuildChildFish();
            SetAllChildFishPosition();
        }

        public override void ResetFishState(FishVo vo)
        {
            ResetBaseFishStateData(vo);
            BuildChildFish();
            SetAllChildFishPosition();
        }

        public void FindView()
        {
            Transform trans = gameObject.transform;
            FindBGComboFishRenderView(trans);
            FindComboFishNodeView(trans);
            FindComboSubFishScale(trans);
        }

        public void CaculateComboFishCount()
        {
            ComboFishCount = fishVo.FishKindGroup.Count;
        }

        public void FindBGComboFishRenderView(Transform trans)
        {
            GameObject tempBGGroup = trans.Find("Bone/BGGroup").gameObject;
            if (tempBGGroup != null)
            {
                BGGroupRenderList = tempBGGroup.GetComponentsInChildren<SpriteRenderer>();
            }
        }

        public void FindComboFishNodeView(Transform trans)
        {
            Transform tempFishGroup = trans.Find("Bone/FishGroup");
            if (tempFishGroup != null)
            {
                int childCount = tempFishGroup.childCount;
                GameObject tempChild;
                if (childCount > 0)
                {
                    for (int i = 1; i < childCount + 1; i++)
                    {
                        tempChild = tempFishGroup.Find("Fish" + i).gameObject;
                        if (tempChild != null)
                        {
                            FishGroupList.Add(tempChild);
                        }
                    }
                }
            }
        }

        public void FindComboSubFishScale(Transform trans)
        {
            Transform tempFishScale = trans.Find("Scale");
            if (tempFishScale != null)
            {
                int childCount = tempFishScale.childCount;
                Transform tempChild;
                if (childCount > 0)
                {
                    for (int i = 1; i < childCount + 1; i++)
                    {
                        tempChild = tempFishScale.Find("Scale" + i);
                        if (tempChild != null)
                        {
                            SubScaleFishList.Add(tempChild);
                        }
                    }
                }
            }
        }

        public void BuildChildFish()
        {
            if (fishVo.FishKindGroup != null)
            {
                if (fishVo.FishKindGroup.Count > 0)
                {
                    for (int i = 0; i < fishVo.FishKindGroup.Count; i++)
                    {
                        int tempFishId = (int)fishVo.FishKindGroup[i];
                        FishFishBase childFish = FishFishManager.Instance.GetChildFish(tempFishId);
                        if (childFish != null)
                        {
                            int fishRuleType = FishFishManager.Instance.GetFishRuleType(fishVo.fishId);
                            childFish.IsEnableBoxcollider(false);
                            if (fishRuleType == 2)
                                childFish.IsShowFishLight(true);
                            ChildFishInsList.Add(childFish);
                        }
                    }
                }
            }
        }

        public void RemoveChildFish()
        {
            if (ChildFishInsList != null)
            {
                if (ChildFishInsList.Count > 0)
                {
                    for (int i = 0; i < ChildFishInsList.Count; i++)
                    {
                        FishGameObjectPoolManager.Instance.SetPoolParent(ChildFishInsList[i].gameObject, PoolType.FishPool);
                        FishFishManager.Instance.AddChildFishToAllUsedFishList(ChildFishInsList[i]);
                    }
                }
                ChildFishInsList.Clear();
            }
        }

        public void SetAllChildFishPosition()
        {
            if (ChildFishInsList != null)
            {
                if (ChildFishInsList.Count > 0)
                {
                    for (int i = 0; i < ChildFishInsList.Count; i++)
                    {
                        if (ChildFishInsList[i].fishVo.fishId <= 11)
                        {
                            Vector3 scale = SubScaleFishList[ChildFishInsList[i].fishVo.fishId - 1].localScale;
                            gameObject.transform.Find("Bone/BGGroup").localScale = scale;
                        }
                        ChildFishInsList[i].SetFishParent(FishGroupList[i].transform);
                    }
                }
            }
        }

        public void PlayChildMoveAnim()
        {
            if (ChildFishInsList != null)
            {
                for (int i = 0; i < ChildFishInsList.Count; i++)
                {
                    int fishRuleType = FishFishManager.Instance.GetFishRuleType(fishVo.fishId);
                    if (fishRuleType == 2)
                        ChildFishInsList[i].IsShowFishLight(true);
                    ChildFishInsList[i].PlayMoveAnim();
                }
            }
        }

        public void PlayChildDieAnim()
        {
            if (ChildFishInsList != null)
            {
                for (int i = 0; i < ChildFishInsList.Count; i++)
                {
                    ChildFishInsList[i].PlayDieAnim(false);
                }
            }
        }

        public override void SetMainFishOrder(int orderIndex)
        {
            SetChildFishSpriteRenderOrder(orderIndex);
            SetBGSpriteRenderOrder(orderIndex);
        }

        public override void SetHitFlyDirection(Vector3 direction)
        {
            hitFlyDirection = direction;
        }

        public override void SetBeHitColor()
        {
            if (!isHit)
            {
                SetChildFishColor(beHitColor);
                SetBGSpriteColor(beHitColor);
                isHit = true;
            }
        }

        public override void ResetNormalColor()
        {
            SetChildFishColor(NormalColor);
            SetBGSpriteColor(NormalColor);
            isHit = false;
            currentHitTime = 0;
        }

        public void SetChildFishColor(Color color)
        {
            if (ChildFishInsList != null)
            {
                for (int i = 0; i < ChildFishInsList.Count; i++)
                {
                    ChildFishInsList[i].SetMainFishColor(color);
                }
            }
        }

        public void SetBGSpriteColor(Color color)
        {
            if (BGGroupRenderList != null)
            {
                for (int i = 0; i < BGGroupRenderList.Length; i++)
                {
                    BGGroupRenderList[i].color = color;
                }
            }   
        }

        public void SetChildFishSpriteRenderOrder(int orderIndex)
        {
            if (ChildFishInsList != null)
            {
                for (int i = 0; i < ChildFishInsList.Count; i++)
                {
                    ChildFishInsList[i].SetMainFishOrder(orderIndex + (i - 1));
                }
            }
        }

        public void SetBGSpriteRenderOrder(int orderIndex)
        {
            if (BGGroupRenderList != null)
            {
                for (int i = 0; i < BGGroupRenderList.Length; i++)
                {
                    BGGroupRenderList[i].sortingOrder = orderIndex - 2 - (BGGroupRenderList.Length - (i + 1));
                }
            }
        }

        public override void PlayMoveAnim()
        {
            
        }

        public override void PlayMoveAnim(bool isLoop)
        {
            
        }

        public override void PlayDieAnim(bool isLoop)
        {
            PlayChildDieAnim();
        }

        public override void SetMainFishColor(Color color)
        {
            throw new System.NotImplementedException();
        }

        public override void Destroy()
        {
            RemoveChildFish();
            ResetNormalColor();
            BaseDestroy();
        }

        public override List<Vector3> GetEffectPoint(int partId = 0)
        {
            return null;
        }

        public override void RemoveFishPart(int id)
        {
        }
    }
}
