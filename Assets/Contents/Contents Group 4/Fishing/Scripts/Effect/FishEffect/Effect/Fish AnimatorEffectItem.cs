using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishAnimatorEffectItem : FishEffectItemBase
    {
        public Animator Anim;
        public FishAnimatorEffectItem(GameObject obj):base(obj)
        {
            FindView();
        }

        public override void FindView()
        {
            Anim = gameObject.transform.GetComponent<Animator>();
        }

        public override void PlayAnim()
        {
            gameObject.transform.position = beginPos;
            if (Anim != null)
                Anim.Play(EffectVo.EffectName, 0, 0);
            if (!string.IsNullOrEmpty(EffectVo.EffectAudio))
                FishAudioManager.Instance.PlayNormalAudio(int.Parse(EffectVo.EffectAudio));
        }
    }
}
