using BagelCode;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishSpineEffectItem : FishEffectItemBase
    {
        public SkeletonAnimation SkeletonAnimation;
        public string AnimName;
        public FishSpineEffectItem(GameObject obj): base(obj)
        {
            AnimName = "die";
            FindView();
        }

        public override void FindView()
        {
            SkeletonAnimation = gameObject.GetComponent<SkeletonAnimation>();
        }

        public override void PlayAnim()
        {
            gameObject.transform.position = beginPos;
            if (SkeletonAnimation != null)
                SkeletonAnimation.state.SetAnimation(0, AnimName, false);

            if (string.IsNullOrEmpty(EffectVo.EffectAudio))
                FishAudioManager.Instance.PlayNormalAudio(int.Parse(EffectVo.EffectAudio));
        }
    }
}
