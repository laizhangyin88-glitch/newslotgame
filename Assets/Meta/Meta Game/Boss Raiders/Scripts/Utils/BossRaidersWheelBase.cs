using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode.BossRaiders
{
    public abstract class BossRaidersWheelBase
    {
        protected Animator rootAnimator;
        protected ContextElement rootElement;

        protected ContextElement[] wheelElements;
        protected ContextElement highlightElement;
        protected ContextElement wheelInactiveElement;

        protected int wheelItemCount = 12;

        public abstract void SetWheelData(long multi);

        public virtual void OnInit(ContextElement root, Animator animator, int wheelCount)
        {
            rootElement = root;
            rootAnimator = animator;
            wheelItemCount = wheelCount;
        }

        public ContextElement GetHighlightElement()
        {
            return highlightElement;
        }

        public void InitInactiveElement(ContextElement inactiveElement)
        {
            wheelInactiveElement = inactiveElement;
        }

        public void SetAnimation(string key, bool isActive)
        {
            rootAnimator?.SetBool(key, isActive);
        }

        public void SetInactiveObject(bool isActive)
        {
            wheelInactiveElement?.gameObject.SetActive(isActive);
        }

        public Animator GetHighlightAnimator(string cellName)
        {
            return ContextUtils.FindElement(highlightElement, cellName, ContextSearchingType.ChildrenSearch)?.GetComponent<Animator>() ?? null;
        }
    }
}