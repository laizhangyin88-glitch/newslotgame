using Spine;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public abstract class FishSkillItemBase
    {
        protected enum SkillState
        {
            born = 1,
            Idel = 2,
            play = 3,
        }
        private Vector3 beginPos;

        protected SkillState curSkillState;
        protected Action callBack;
        protected GameObject skillObj;
        protected Transform skillTrans;
        protected Text numText;

        public string objResName;
        public bool isCanDestroy;
        public SkillVo skillVo;


        protected virtual void InitTextView()
        {
            numText = skillTrans.Find("num").GetComponent<Text>();
        }

        public void ResetSkillVo(SkillVo vo)
        {
            skillVo = vo;
            isCanDestroy = false;
        }

        public void ResetSkillState(Vector3 beginPos, float skillTime, Action callBack = null)
        {
            isCanDestroy = false;
            this.callBack = callBack;
            this.beginPos = beginPos;
            GetCurSkillStep(skillTime);
        }

        private void GetCurSkillStep(float skillTime)
        {
            if (skillTime < 0.1f)
                curSkillState = SkillState.born;
            else
                curSkillState = SkillState.play;
        }

        public void ShowSkill()
        {
            switch (curSkillState)
            {
                case SkillState.born:
                    GetSkillResourceItem();
                    ShowBornAnim();
                    break;
                case SkillState.Idel:
                    break;
                case SkillState.play:
                    break;
                default:
                    break;
            }
        }

        private void GetSkillResourceItem()
        {
            skillObj = FishGameObjectPoolManager.Instance.GetGameObject(objResName, PoolType.EffectPool);
            skillTrans = skillObj.transform;
            skillTrans.position = beginPos;
            skillTrans.localRotation = Quaternion.identity;
            skillTrans.localScale = Vector3.zero;
            InitTextView();
            InitAnitmatorView();
        }

        protected abstract void InitAnitmatorView();

        protected abstract void ShowBornAnim();

        protected void ShowLightAnimate(MonoBehaviour agent)
        {
            var lightObj = FishGameObjectPoolManager.Instance.GetGameObject("DeclareLight", PoolType.SpecialDeclarePool);
            lightObj.transform.position = beginPos;
            lightObj.GetComponent<Animator>().Play("DeclareLight", 0, 0);
            AsyncActionUtils.DelayedAction(agent, 1.27f, () =>
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(lightObj, PoolType.SpecialDeclarePool);
            });
        }

        protected void SetNumText(int num, MonoBehaviour agent)
        {
            numText.text = num.ToString();
            AsyncActionUtils.ApplyScaling(agent, numText.transform, Vector3.one, Vector3.one * 1.5f, 0.75f, TweenUtils.VectorTweenLinear);
            AsyncActionUtils.ApplyScaling(agent, numText.transform, Vector3.one * 1.5f, Vector3.one, 0.75f, TweenUtils.VectorTweenLinear, 0.75f);
        }

        public void DelaySkillDestroy()
        {
            isCanDestroy = true;
        }

        public virtual void Destroy()
        {
            if (skillObj != null)
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(skillObj, PoolType.EffectPool);
            skillObj = null;

            isCanDestroy = false;
            callBack = null;
        }
    }
}
