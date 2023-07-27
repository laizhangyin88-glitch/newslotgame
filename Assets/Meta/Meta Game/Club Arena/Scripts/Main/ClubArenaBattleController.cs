using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode.ClubArena
{
    public class ClubArenaBattleController : MonoBehaviour
    {
        public float effectMovementTime = 0.75f;
        public Transform particleCollider;

        private ContextElement rootElement;
        private Animator rootAnimator;

        private ContextElement callerElement;

        private ContextElement myPositionElement;
        private ContextElement opponentPositionElement;
        private ContextElement damageTextElement;

        private ClubArenaChestController myChestController = null;
        private ClubArenaChestController opponentChestController = null;

        private ClubArenaWheelRewardItemController rewardItemController = null;

        private bool isInit = false;
        private bool isRevenge = false;

        private List<string> myChestNameList;
        private List<string> opponentChestNameList;

        private UnityAction<string> ownerCallback;

        protected const int CHEST_COUNT = 3;

        public void OnInit(ContextElement caller)
        {
            if (isInit) return;

            callerElement = caller;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();

            ContextElement playerAreaElement = ContextUtils.FindElement(rootElement, "Player Area", ContextSearchingType.ChildrenSearch);
            myPositionElement = ContextUtils.FindElement(playerAreaElement, "Start Position", ContextSearchingType.ChildrenSearch);
            myChestController = myPositionElement.GetComponent<ClubArenaChestController>();
            ContextElement opponentAreaElement = ContextUtils.FindElement(rootElement, "Opponent Player Area", ContextSearchingType.ChildrenSearch);
            opponentPositionElement = ContextUtils.FindElement(opponentAreaElement, "Start Position", ContextSearchingType.ChildrenSearch);
            opponentChestController = opponentPositionElement.GetComponent<ClubArenaChestController>();

            ContextElement damageAreaElement = ContextUtils.FindElement(rootElement, "Damage Area", ContextSearchingType.ChildrenSearch);
            ContextElement damageElement = ContextUtils.FindElement(damageAreaElement, "Damage", ContextSearchingType.ChildrenSearch);
            damageTextElement = ContextUtils.FindElement(damageElement, "Text", ContextSearchingType.ChildrenSearch);

            // Init Data
            myChestNameList = new List<string>();
            opponentChestNameList = new List<string>();

            for (int i = 0; i < CHEST_COUNT; ++i)
            {
                myChestNameList.Add(string.Format("My Club Chest Lv {0}", i + 1));
                opponentChestNameList.Add(string.Format("Opponent Club Chest Lv {0}", i + 1));
            }

            isInit = true;
        }

        public void OnCreateMyChest()
        {
            CreateMyChest();
        }

        public void OnCreateOpponentChest()
        {
            CreateOpponentChest();
        }

        public bool CheckChangedMyChestLevel()
        {
            return myChestController.CheckChangedChestLevel();
        }

        private void ClearRewardEnergyItem()
        {
            if (rewardItemController != null)
            {
                Destroy(rewardItemController);
                rewardItemController = null;
            }
        }

        private bool CreateMyChest()
        {
            if (myChestController != null)
            {
                myChestController.OnInit(false, particleCollider);
                myChestController.SetCallback(InvokeCallback);
                return true;
            }
            return false;
        }

        private bool CreateOpponentChest()
        {
            if (opponentChestController != null)
            {
                opponentChestController.OnInit(true, particleCollider);
                opponentChestController.SetCallback(InvokeCallback);
                GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_MATCH_END).Play();
                return true;
            }

            return false;
        }

        public Transform GetMyChestTransform()
        {
            return myChestController.transform;
        }

        public bool GetOpponentAlive()
        {
            return ClubArenaUtils.OpponentState.GetValue<long>("point") > 0;
        }

        private long GetOpponentShield()
        {
            return ClubArenaUtils.OpponentState.GetValue<long>("shield");
        }

        public Transform GetShieldTransform()
        {
            return myChestController.GetShieldTransform();
        }

        private void InvokeCallback(string key)
        {
            if (ownerCallback != null)
                ownerCallback.Invoke(key);
        }

        public void SetCallback(UnityAction<string> callback)
        {
            ownerCallback = callback;
        }

        public void SetAnimation(string key)
        {
            if (rootAnimator != null)
                rootAnimator.SetTrigger(key);
        }

        public void SetChestActive(bool isMy, bool isActive)
        {
            if (isMy)
                myChestController.SetChestActive("Active", isActive);
            else
            {
                opponentChestController.SetChestActive("Appear", isActive);
                SetOpponentActiveRevenge();
            }
        }

        public void SetTextDamage(long damage)
        {
            if (damageTextElement != null)
                MetaContextElementUtils.SetTextGlobal(damageTextElement, "CLUB_ARENA_DAMAGE_TEXT", damage);
        }

        public void OnUpdateMyReward(ClubArenaDebugSpinResultType resultType)
        {
            Blackboard bb = ClubArenaUtils.WheelResultInfo;
            if (resultType == ClubArenaDebugSpinResultType.POINT)
            {
                myChestController.SetInfoAppearPoint(bb.GetValue<long>("point"));
                myChestController.SetInfoAnimation("Point");
                ClubArenaUtils.MyState.SetValue("point", ClubArenaUtils.MyState.GetValue<long>("point") + bb.GetValue<long>("point"));
                //GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_GET_POINTS).Play();
            }
            else if (resultType == ClubArenaDebugSpinResultType.SHIELD)
            {
                myChestController.SetInfoAppearShield(bb.GetValue<long>("shield"));
                myChestController.SetInfoAnimation("Shield");
                ClubArenaUtils.MyState.SetValue("shield", ClubArenaUtils.MyState.GetValue<long>("shield") + bb.GetValue<long>("shield"));
                myChestController.SetInfoActiveShieldItem(ClubArenaUtils.MyState.GetValue<long>("shield"));
            }
            else if (resultType == ClubArenaDebugSpinResultType.STEAL)
            {
                myChestController.SetInfoAppearPoint(ClubArenaUtils.AddedPoint);
                myChestController.SetInfoAnimation("Point");
                ClubArenaUtils.MyState.SetValue("point", ClubArenaUtils.MyState.GetValue<long>("point") + ClubArenaUtils.AddedPoint);
                //GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_GET_POINTS).Play();
            }
            else if (resultType == ClubArenaDebugSpinResultType.ATTACK)
            {
                myChestController.SetInfoAppearPoint(ClubArenaUtils.AddedPoint);
                myChestController.SetInfoAnimation("Point");
                ClubArenaUtils.MyState.SetValue("point", ClubArenaUtils.MyState.GetValue<long>("point") + ClubArenaUtils.AddedPoint);
                //GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_GET_POINTS).Play();
            }
            UpdateMyChestInfo();
        }

        private void CompletedRewardEnergyItem()
        {
            InvokeCallback(ClubArenaUtils.BATTLE_ANIMATION_REWARD_ENERGY);
            ClearRewardEnergyItem();
        }

        public void CreateRewardEnergyItem(string bundleName, string assetName, Transform toTransform)
        {
            // Energy fly : My chest -> Spin Button
            ClearRewardEnergyItem();

            GameObject effectGO = MetaObjectUtils.MakePrefab(bundleName, assetName, toTransform);
            Vector3 from = myChestController.GetShieldTransform().position;
            Vector3 to = toTransform.position;
            rewardItemController = effectGO.GetComponent<ClubArenaWheelRewardItemController>();
            rewardItemController?.OnInit(ClubArenaSpinResultType.BONUS);

            //GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_POINTS_FLY).Play();

            AsyncActionUtils.ApplyMovement(this, effectGO.transform, from, to, effectMovementTime, TweenUtils.VectorTweenCollectMove,
                0f, CompletedRewardEnergyItem);
        }

        public void ChangedMyChest()
        {
            myChestController.ChangedChest(true);
            GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_UPGRADE_START).Play();
        }

        public void SetRevenge(bool _isRevenge)
        {
            isRevenge = _isRevenge;
        }

        public void SetOpponentActiveRevenge()
        {
            opponentChestController.SetInfoActiveRevenge(isRevenge);
        }

        public void BattleAttack(int index)
        {
            //SetAnimation("myClub");
            myChestController.SetChestAnimationTrigger("Attack Step " + index.ToString());
            //GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_BATTLE_ATTACK).Play();
            GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_ATTACK_SUCCESS).Play();
            BattleOpponentAttack();
        }

        public void BattleOpponentAttack()
        {
            if (ClubArenaUtils.OpponentHasShield)
            {
                //SetAnimation("opponentClub");
                opponentChestController.SetChestAnimationTrigger("Shield FX");
            }
            else
                BattleOpponentHit();
        }

        public void BattleOpponentDead()
        {
            opponentChestController.SetChestAnimationBool("Appear", false);
            opponentChestController.SetChestAnimationTrigger("Dead");
            myChestController.SetChestAnimationTrigger("Win");
        }

        public void BattleOpponentHit()
        {
            opponentChestController.SetChestAnimationTrigger("Hit");
            SetTextDamage(ClubArenaUtils.GetOpponentBasePoint());
            //GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_BATTLE_INJURED).Play();
        }

        public void BattleSteal(int index)
        {
            //SetAnimation("myClub");
            myChestController.SetChestAnimationTrigger("Steal Step " + index.ToString());
            SetTextDamage(ClubArenaUtils.GetOpponentBasePoint());
            GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_STEAL_START).Play();
        }

        public void UpdateMyChestInfo()
        {
            myChestController.UpdateInfo();
        }

        public void UpdateOpponentChestInfo()
        {
            opponentChestController.SetInfoAppearPoint(ClubArenaUtils.GetOpponentBasePoint() * -1);
            opponentChestController.SetInfoAnimation("Point");
            opponentChestController.UpdateInfo();
        }
    }
}