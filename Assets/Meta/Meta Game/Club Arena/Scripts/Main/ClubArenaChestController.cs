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
    public class ClubArenaChestController : MonoBehaviour
    {
        private List<ClubArenaChestBaseController> chestList;
        public ClubArenaChestInfoController infoController;

        public int chestIndex = 0;

        private UnityAction<string> ownerCallback;

        public void OnInit(bool isOpponent, Transform particleCollider)
        {
            chestList = new List<ClubArenaChestBaseController>(transform.GetComponentsInChildren<ClubArenaChestBaseController>(true));
            infoController = transform.GetComponentInChildren<ClubArenaChestInfoController>(true);

            chestIndex = GetChestLevel(isOpponent ? BlackboardUtils.FindValue<long>(ClubArenaUtils.OpponentState, "point") :
                                                        BlackboardUtils.FindValue<long>(ClubArenaUtils.MyState, "point"));

            infoController.OnInit(isOpponent);

            for (int i = 0; i < chestList.Count; ++i)
            {
                chestList[i].OnInit();
                chestList[i].SetParticleCollider(particleCollider);
                chestList[i].SetAnimationEventString(CallbackChestAnimationEventString);
                chestList[i].gameObject.SetActive(i == chestIndex);
            }
            infoController.gameObject.SetActive(true);
        }

        private int GetChestLevel(long point = 0)
        {
            int chestLevel = 0;
            Blackboard bb = ClubArenaUtils.ChestChangePoint;

            if (point < bb.GetValue<long>("middle"))
                chestLevel = 0;
            else if (point < bb.GetValue<long>("high"))
                chestLevel = 1;
            else
                chestLevel = 2;

            return chestLevel;
        }

        public Transform GetShieldTransform()
        {
            return infoController.GetShieldTransform();
        }

        private void CallbackChestAnimationEventString(string key)
        {
            if (string.Equals(key, ClubArenaUtils.BATTLE_ANIMATION_CHANGED_CHEST))
            {
                // Chest Animation : Upgrade Chest -> Finished call
                chestList[chestIndex].gameObject.SetActive(false);

                chestIndex = GetChestLevel(infoController.GetInfoPoint());
                chestList[chestIndex].SetAnimatorBool("Active", true);
                chestList[chestIndex].gameObject.SetActive(true);
                GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_UPGRADE_END).Play();
            }

            if (ownerCallback != null)
                ownerCallback.Invoke(key);
        }

        public void ChangedChest(bool isTrigger)
        {
            SetChestAnimationBool("Active", false);
            if (isTrigger)
                SetChestAnimationTrigger("Upgrade Chest");
        }

        public bool CheckChangedChestLevel()
        {
            int chestLevel = GetChestLevel(infoController.GetInfoPoint());
            return chestLevel != chestIndex;
        }

        public void SetCallback(UnityAction<string> callback)
        {
            ownerCallback = callback;
        }

        public void SetChestActive(string key, bool isActive)
        {
            if (isActive)
            {
                infoController.OnUpdateData();
                chestIndex = GetChestLevel(infoController.GetInfoPoint());
            }

            SetChestAnimationBool(key, isActive);
            chestList[chestIndex].gameObject.SetActive(isActive);

            infoController.gameObject.SetActive(isActive);
        }

        public void SetChestAnimationBool(string key, bool isActive)
        {
            chestList[chestIndex].SetAnimatorBool(key, isActive);
        }

        public void SetChestAnimationTrigger(string key)
        {
            chestList[chestIndex].SetAnimatorTrigger(key);
        }

        public void SetInfoActiveShield(bool isActive)
        {
            infoController?.SetActiveShield(isActive);
        }

        public void SetInfoActiveShieldItem(long index)
        {
            infoController.SetActiveShieldItem(index);
        }

        public void SetInfoActiveRevenge(bool isActive)
        {
            infoController?.SetActiveRevenge(isActive);
        }

        public void SetInfoAnimation(string key)
        {
            infoController?.SetAnimationTrigger(key);
        }

        public void SetInfoAppearShield(long shield)
        {
            infoController?.SetAppearShield(shield);
        }

        public void SetInfoAppearPoint(long point)
        {
            infoController?.SetAppearPoint(point);
        }

        public void UpdateInfo()
        {
            infoController?.OnUpdateStatus();
        }
    }
}