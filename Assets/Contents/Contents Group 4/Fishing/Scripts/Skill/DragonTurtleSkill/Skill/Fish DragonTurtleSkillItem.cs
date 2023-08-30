using ParadoxNotion;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class FishDragonTurtleSkillItem : FishSkillItemBase
    {
        Animator enviorAnim;
        Animator rollAnim;
        Animator bombAnim;
        RectTransform rollImageRect;

        protected override void InitAnitmatorView()
        {
            enviorAnim = skillTrans.Find("EnviorAnimator").GetComponent<Animator>();
            rollAnim = skillTrans.Find("RollAnimator").GetComponent<Animator>();
            bombAnim = skillTrans.Find("BombAnimator").GetComponent<Animator>();
            rollImageRect = rollAnim.transform.Find("Image").GetComponent<RectTransform>();
        }

        protected override void ShowBornAnim()
        {
            float tweenDuring = 1f;
            float delayTime = 0.4f;
            enviorAnim.gameObject.SetActive(false);
            bombAnim.gameObject.SetActive(false);
            rollAnim.gameObject.SetActive(true);
            rollImageRect.localRotation = Quaternion.identity;
            rollAnim.Play("DragonTurtleRoll", 0, 0);
            numText.gameObject.SetActive(true);
            ShowLightAnimate(FishDragonTurtleSkillManager.Instance);
            AsyncActionUtils.DelayedAction(FishDragonTurtleSkillManager.Instance, delayTime, () =>
            {
                AsyncActionUtils.ApplyScaling(FishDragonTurtleSkillManager.Instance, skillTrans, Vector3.zero, Vector3.one, tweenDuring, TweenUtils.VectorTweenLinear);
                AsyncActionUtils.ApplyLocalMovement(FishDragonTurtleSkillManager.Instance, skillTrans, skillTrans.localPosition, Vector3.zero, tweenDuring, TweenUtils.VectorTweenLinear, tweenDuring);
                AsyncActionUtils.ApplyScaling(FishDragonTurtleSkillManager.Instance, numText.transform, Vector3.zero, Vector3.one, tweenDuring, TweenUtils.VectorTweenLinear, tweenDuring + 1, () =>
                {
                    curSkillState = SkillState.Idel;
                });
            });
        }

        public void OnBomBMsg()
        {
            SetNumText(skillVo.BombCount, FishDragonTurtleSkillManager.Instance);
            curSkillState = SkillState.play;
            ShowBombAnim();
        }

        private void ShowBombAnim()
        {
            rollAnim.SetTrigger("bomb");
            bombAnim.gameObject.SetActive(true);
            bombAnim.Play("DragonTurtleBomb", 0, 0);
            AsyncActionUtils.DelayedAction(FishDragonTurtleSkillManager.Instance, 1.15f, () =>
            {
                enviorAnim.gameObject.SetActive(true);
                enviorAnim.Play("DragonTurtleEnvior", 0, 0);
                MessageDispatcher.Dispatch("FishBombEnd", new EventData<int>("MainFishUID", skillVo.killFishUID));
            });
        }
    }
}

