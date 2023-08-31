using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishTipsFish : FishFishBase
    {
        List<string> AnimParams;
        SkeletonAnimation SpineAnim;
        MeshRenderer SpineMeshRenderer;
        public FishTipsFish()
        {
            Init();
        }

        private void Init()
        {
            InitData();
        }

        private void InitData()
        {
            AnimParams = new List<string> { "Fish_Move" };
        }

        public override void BuildFish(FishVo vo, GameObject obj)
        {
            base.BuildFish(vo, obj); 
            FindView();
            InitViewData();
        }

        public override void ResetFishState(FishVo vo)
        {
            base.ResetFishState(vo);
            PlayAnim(1, false);
        }

        public void FindView()
        {
            Transform trans = gameObject.transform;
            SpineAnim = trans.Find("Bone/Fish").GetComponent<SkeletonAnimation>();
            SpineMeshRenderer = trans.Find("Bone/Fish").GetComponent<MeshRenderer>();
        }

        public void InitViewData()
        {
            PlayAnim(1, false);
        }

        public void PlayAnim(int index, bool isLoop)
        {
            if (SpineAnim != null)
            {
                string animName = AnimParams[index];
                if (!string.IsNullOrEmpty(animName))
                {
                    SpineAnim.loop = isLoop;
                    SpineAnim.state.SetAnimation(0, animName, isLoop);
                }
            }
        }

        public override void SetMainFishOrder(int orderIndex)
        {
            if (SpineMeshRenderer)
            {
                SpineMeshRenderer.sortingOrder = orderIndex;
            }
        }

        public override void PlayDieAnim()
        {
            
        }

        public override void SetMainFishColor(Color color)
        {
             
        }

        public override void Destroy()
        {
            BaseDestroy();
        }

        public override void ResetNormalColor()
        {
             
        }

        public override List<Vector3> GetEffectPoint(int partId = 0)
        {
            return null;
        }

    }
}
