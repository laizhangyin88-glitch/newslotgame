using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishParticaleEffectItem : FishEffectItemBase
    {
        public ParticleSystem[] particleSystemList;

        public FishParticaleEffectItem(GameObject obj) : base(obj)
        {
            FindView();
        }

        public override void FindView()
        {
            particleSystemList = gameObject.transform.GetComponentsInChildren<ParticleSystem>();
        }

        public override void PlayAnim()
        {
            gameObject.transform.position = beginPos;
            for (int i = 0; i < particleSystemList.Length; i++)
                particleSystemList[i].Play();

            if (!string.IsNullOrEmpty(EffectVo.EffectAudio))
                FishAudioManager.Instance.PlayNormalAudio(int.Parse(EffectVo.EffectAudio));
        }


    }
}
