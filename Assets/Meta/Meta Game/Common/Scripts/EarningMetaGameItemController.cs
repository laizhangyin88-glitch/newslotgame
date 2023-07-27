using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class EarningMetaGameItemController : MonoBehaviour
    {
        protected ContextElement root;
        protected Blackboard bb;
        protected Animator anim;

        public float flyingTime;

        protected virtual void Start()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            if(root != null) root.UpdateContext(false);

            StartCoroutine(DisappearCoroutine());
        }

        private IEnumerator DisappearCoroutine()
        {
            yield return new WaitForSeconds(flyingTime);
            if(anim != null) anim.SetTrigger("Collect");
        }
    }
}
