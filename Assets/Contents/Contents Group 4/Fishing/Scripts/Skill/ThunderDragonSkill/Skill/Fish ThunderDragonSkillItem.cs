using ParadoxNotion;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class FishThunderDragonSkillItem : FishSkillItemBase
    {
        private Animator bombAnim;
        private Animator mulBombAnim;
        private Animator lightningAnim;
        private Animator ballAnim;

        private bool isShow = false;

        protected override void InitAnitmatorView()
        {
            bombAnim = skillTrans.Find("BombAnimator").GetComponent<Animator>();
            mulBombAnim = skillTrans.Find("MulBombAnimator").GetComponent<Animator>();
            lightningAnim = skillTrans.Find("LightningAnimator").GetComponent<Animator>();
            ballAnim = skillTrans.Find("BallAnimator").GetComponent<Animator>();
        }

        protected override void ShowBornAnim()
        {
            isShow = false;
            float tweenDuring = 1f;
            float delayTime = 0.4f;
            mulBombAnim.gameObject.SetActive(false);
            lightningAnim.gameObject.SetActive(false);
            bombAnim.gameObject.SetActive(false);
            ballAnim.gameObject.SetActive(true);
            bombAnim.Play("ThunderDragonBall", 0, 0);
            numText.gameObject.SetActive(true);
            ShowLightAnimate(FishThunderDragonSkillManager.Instance);
            AsyncActionUtils.DelayedAction(FishThunderDragonSkillManager.Instance, delayTime, () =>
            {
                AsyncActionUtils.ApplyScaling(FishThunderDragonSkillManager.Instance, skillTrans, Vector3.zero, Vector3.one, tweenDuring, TweenUtils.VectorTweenLinear);
                AsyncActionUtils.ApplyLocalMovement(FishThunderDragonSkillManager.Instance, skillTrans, skillTrans.localPosition, Vector3.zero, tweenDuring, TweenUtils.VectorTweenLinear, tweenDuring);
                AsyncActionUtils.ApplyScaling(FishThunderDragonSkillManager.Instance, numText.transform, Vector3.zero, Vector3.one, tweenDuring, TweenUtils.VectorTweenLinear, tweenDuring + 1, () =>
                {
                    curSkillState = SkillState.Idel;
                });
            });
        }

        public void OnBombMsg()
        {
            SetNumText(skillVo.BombCount, FishThunderHammerSkillManager.Instance);
            curSkillState = SkillState.play;
            ShowBombAnim();
        }

        private void ShowBombAnim()
        {
            if (isShow)
                return;
            isShow = true;
            bombAnim.gameObject.SetActive(true);
            AsyncActionUtils.DelayedAction(FishThunderHammerSkillManager.Instance, 5.2f, () => { bombAnim.gameObject.SetActive(false); });
            lightningAnim.gameObject.SetActive(true);
            AsyncActionUtils.DelayedAction(FishThunderHammerSkillManager.Instance, 1f, () => { lightningAnim.gameObject.SetActive(false); });

            ballAnim.transform.localScale = Vector3.one * 1.5f;
            AsyncActionUtils.ApplyScaling(FishThunderDragonSkillManager.Instance, ballAnim.transform, Vector3.one * 1.5f, Vector3.one, 1, TweenUtils.VectorTweenLinear);
            for (int i = 1; i < 4; i++)
            {
                AsyncActionUtils.DelayedAction(FishThunderHammerSkillManager.Instance, i, () =>
                {
                    ballAnim.transform.localScale = Vector3.one * 1.5f;
                    AsyncActionUtils.ApplyScaling(FishThunderDragonSkillManager.Instance, ballAnim.transform, Vector3.one * 1.5f, Vector3.one, 1, TweenUtils.VectorTweenLinear, 0, () =>
                    {
                        if (i == 3)
                        {
                            ballAnim.gameObject.SetActive(false);
                            lightningAnim.gameObject.SetActive(true);
                            mulBombAnim.gameObject.SetActive(true);
                            AsyncActionUtils.DelayedAction(FishThunderHammerSkillManager.Instance, 2f, () =>
                            {
                                MessageDispatcher.Dispatch("FishBombEnd", new EventData<int>("MainFishUID", skillVo.killFishUID));
                            });
                        }
                    });
                });
            }
        }
    }
}
