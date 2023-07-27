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
    public class ClubArenaChestBaseController : MonoBehaviour
    {
        protected ContextElement rootElement;
        protected Animator rootAnimator;

        protected UnityAction<string> ownerAnimationEventString;

        public List<ParticleSystem> particleList;

        public int chestIndex = 0;

        protected virtual void InitProperty()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();
        }

        protected virtual void InitData()
        {
        }

        public void OnInit()
        {
            InitProperty();
            InitData();
        }

        private void AnimationEventString(string key)
        {
            // Animation : Event string send
            if (ownerAnimationEventString != null)
                ownerAnimationEventString.Invoke(key);
        }

        public void SetAnimationEventString(UnityAction<string> callback)
        {
            ownerAnimationEventString = callback;
        }

        public void SetAnimatorBool(string key, bool isActive)
        {
            if (rootAnimator != null)
                rootAnimator.SetBool(key, isActive);
        }

        public void SetAnimatorTrigger(string key)
        {
            if (rootAnimator != null)
                rootAnimator.SetTrigger(key);
        }

        public void SetParticleCollider(Transform collider)
        {
            if (particleList == null || collider == null)
                return;

            for (int i = 0; i < particleList.Count; ++i)
            {
                var collision = particleList[i].collision;
                collision.SetPlane(0, collider);
            }
        }

        protected void OnPlaySound(string soundKey)
        {
            GSManager.Instance.GetHandler(soundKey).Play();
        }
    }
}