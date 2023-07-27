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
    public class ClubArenaChestInfoController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Animator rootAnimator;

        private ContextElement pointTextElement;
        private ContextElement appearPointTextElement;
        private ContextElement shieldAnchorElement;
        private ContextElement shieldTextElement;
        private ContextElement appearShieldTextElement;
        private ContextElement usernameTextElement;
        private ContextElement revengeBedgeElement;
        private ContextElement[] shieldImageElements;

        private ContextElement myProfileAreaElement;

        private GameObject profilePicture;
        private Blackboard profileBB;

        private bool isOpponent = false;
        private long currentShieldCount = 0;

        private readonly int MAX_SHIELD_COUNT = 3;

        private void InitProperty()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();

            pointTextElement = ContextUtils.FindElement(rootElement, "Text Point", ContextSearchingType.ChildrenSearch);
            appearPointTextElement = ContextUtils.FindElement(rootElement, "Text Appear Point", ContextSearchingType.ChildrenSearch);

            shieldAnchorElement = ContextUtils.FindElement(rootElement, "Shield Progress Anchor", ContextSearchingType.ChildrenSearch);
            shieldImageElements = new ContextElement[MAX_SHIELD_COUNT];
            for (int i = 0; i < MAX_SHIELD_COUNT; ++i)
                shieldImageElements[i] = ContextUtils.FindElement(shieldAnchorElement, string.Format("Image Shield {0}", i + 1), ContextSearchingType.ChildrenSearch);

            shieldTextElement = ContextUtils.FindElement(rootElement, "Text Shield", ContextSearchingType.ChildrenSearch);
            appearShieldTextElement = ContextUtils.FindElement(rootElement, "Text Appear Shield", ContextSearchingType.ChildrenSearch);
            usernameTextElement = ContextUtils.FindElement(rootElement, "Text Username", ContextSearchingType.ChildrenSearch);

            revengeBedgeElement = ContextUtils.FindElement(rootElement, "Revenge Bedge", ContextSearchingType.ChildrenSearch);

            myProfileAreaElement = ContextUtils.FindElement(rootElement, "My Profile Area", ContextSearchingType.ChildrenSearch);
            profileBB = myProfileAreaElement.GetComponent<Blackboard>();
        }

        private void InitData(bool _isOpponent)
        {
            isOpponent = _isOpponent;
            OnUpdateData();
            if (!isOpponent)
            {
                currentShieldCount = ClubArenaUtils.MyState.GetValue<long>("shield");
                currentShieldCount = currentShieldCount > MAX_SHIELD_COUNT ? MAX_SHIELD_COUNT : currentShieldCount;
            }
            SetActiveShieldItem(currentShieldCount);
        }

        public void OnUpdateStatus()
        {
            if (isOpponent)
            {
                // Opponent Chest
                ClubArenaPersonalSimpleWithProfile opponentStateClass = ClubArenaUtils.OpponentStateClass;
                SetTextPoint(opponentStateClass.point);
                SetTextShield(opponentStateClass.shield);
            }
            else
            {
                // My Chest
                ClubArenaPersonal myStateClass = ClubArenaUtils.MyStateClass;
                SetTextPoint(myStateClass.point);
                SetTextShield(myStateClass.shield);
                //SetActiveShieldItem(currentShieldCount);
            }
        }

        public void OnInit(bool isOpponent)
        {
            InitProperty();
            InitData(isOpponent);
        }

        public void OnUpdateData()
        {
            Blackboard userBB = isOpponent ? ClubArenaUtils.OpponentState : BlackboardUtils.FindVariable<Blackboard>(null, "/me").value;

            SetActiveShield(!isOpponent);
            SetTextUserName(BlackboardUtils.FindValue<string>(userBB, "name"));
            SetUserProfile(userBB);
            OnUpdateStatus();
        }

        private Blackboard GetInfoBB()
        {
            return isOpponent ? ClubArenaUtils.OpponentState : ClubArenaUtils.MyState;
        }

        public long GetInfoPoint()
        {
            return GetInfoBB().GetValue<long>("point");
        }

        public Transform GetShieldTransform()
        {
            return shieldAnchorElement.transform;
        }

        public void SetActiveShield(bool isActive)
        {
            shieldAnchorElement?.gameObject.SetActive(isActive);
        }

        public void SetActiveShieldItem(long index)
        {
            if (index < 0)
                return;
            else if (index > MAX_SHIELD_COUNT)
                index = MAX_SHIELD_COUNT;

            for (int i = 0; i < MAX_SHIELD_COUNT; ++i)
                shieldImageElements[i].gameObject.SetActive(i < index);

            if (index == MAX_SHIELD_COUNT)
                SetAnimationTrigger("ShieldFull");

            currentShieldCount = index;
        }

        public void SetActiveRevenge(bool isActive)
        {
            revengeBedgeElement?.gameObject.SetActive(isActive);
        }

        public void SetAnimationTrigger(string key)
        {
            if (rootAnimator != null)
                rootAnimator.SetTrigger(key);
        }

        public void SetAppearShield(long shield)
        {
            SetTextAppearShield(shield);
        }

        public void SetAppearPoint(long point)
        {
            SetTextAppearPoint(point);
        }

        private void SetTextAppearShield(long shield)
        {
            if (appearShieldTextElement != null)
            {
                string message = shield > 0 ? "+" : "";
                message += StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_CHEST_SHIELD_APPEAR_TEXT", shield);
                MetaContextElementUtils.SetText(appearShieldTextElement, message);
            }
        }

        private void SetTextAppearPoint(long point)
        {
            if (appearPointTextElement != null)
            {
                string message = point > 0 ? "+" : "";
                message += StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_CHEST_POINT_APPEAR_TEXT", point);
                MetaContextElementUtils.SetText(appearPointTextElement, message);
            }
        }

        public void SetTextShield(long shield)
        {
            if (shieldTextElement != null)
                MetaContextElementUtils.SetTextGlobal(shieldTextElement, "CLUB_ARENA_CHEST_SHIELD_TEXT", shield, ClubArenaUtils.CLUB_ARENA_MAX_SHIELD);
        }

        public void SetTextPoint(long point)
        {
            if (pointTextElement != null)
                MetaContextElementUtils.SetText(pointTextElement, ClubArenaUtils.InfoPointNumberFormat(point));
        }

        public void SetTextUserName(string name)
        {
            if (usernameTextElement != null)
                MetaContextElementUtils.SetText(usernameTextElement, name);
        }

        protected void SetUserProfile(Blackboard bb)
        {
            BlackboardUtils.SetOrCreateValue(profileBB, "userInfo", bb);
            BlackboardUtils.SetOrCreateValue(profileBB, "updateProfile", true);
        }
    }
}