using UnityEngine;
using System.Collections;
using SlotMaker;

namespace BagelCode
{
    public abstract class ProfileEventGroup
    {
        public bool isDisplay = false;

        protected virtual float DisplayTime => 0f;
        protected ContextElement root;

        protected ContextElement eventTagAreaElement;

        protected GameObject tagObj = null;
        protected Animator anim;

        protected TimerTrigger timerTrigger;

        private Coroutine displayCoroutine = null;

        protected bool isInit = false;

        public ProfileEventGroup(ContextElement _root)
        {
            root = _root;

            root.UpdateContext(false);

            eventTagAreaElement = ContextUtils.FindElement(root, "Event Tag Area", ContextSearchingType.ChildrenSearch);

            timerTrigger = new TimerTrigger(DisplayTime);
        }

        public virtual void UpdateTagState()
        {

        }

        protected virtual void Initialize()
        {
            isInit = true;

            UpdateTagState();
        }

        //

        public abstract bool IsAvailable();

        public void Appear()
        {
            if (IsAvailable())
            {
                displayCoroutine = root.StartCoroutine(DisplayCoroutine());
                isDisplay = true;
            }
        }

        private IEnumerator DisplayCoroutine()
        {
            if (!isInit) Initialize();
            if (!isInit) yield break; // initializing failure

            if (anim != null && anim.isActiveAndEnabled)
            {
                anim.SetBool("Appear", true);
                OnAppearTag();
            }

            // Elapsed or Closed
            timerTrigger = new TimerTrigger(DisplayTime);

            var trigger = new WaitUntilTrigger(TrueCase.ANY_TRUE, timerTrigger);
            trigger.ignoreLog = true; // for dev
            yield return trigger;

            isDisplay = false;
            // Disappear
        }

        protected virtual void OnAppearTag()
        {

        }

        protected virtual void OnDisappearTag()
        {

        }

        public virtual void Disappear()
        {
            isDisplay = false;
            OnDisappearTag();

            if (displayCoroutine != null)
            {
                root.StopCoroutine(displayCoroutine);
                displayCoroutine = null;
            }

            if (anim != null)
                anim.SetBool("Appear", false);
        }
    }
}
