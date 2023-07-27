using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class ContextTierSetter : MonoBehaviour
    {
        public Blackboard bb;
        public string bbValueKey = "tierGroup";
        
        public Animator animator;
        public string keyName = "Tier";

        protected Variable<int> tierGroup;
        public int TierGroup
        {
            get { return tierGroup.value; }
        }

        private void Awake()
        {
            if(bb == null)
                bb = GetComponent<Blackboard>();

            if(animator == null)
                animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            if (bb == null) return;
            if (animator == null) return;

            StartCoroutine("UpdateTierGroup");
        }

        private void OnDisable()
        {
            StopAllCoroutines();

            if(tierGroup != null)
                tierGroup.onValueChanged -= OnUpdateTierGroup;
        }

        private IEnumerator UpdateTierGroup()
        {
            if(tierGroup == null)
            {
                while(true)
                {
                    if(bb == null) break;

                    tierGroup = BlackboardUtils.FindVariable<int>(bb, bbValueKey);

                    if (tierGroup != null)
                    {
                        break;
                    }

                    yield return new WaitForSeconds(0.1f);
                }
            }

            tierGroup.onValueChanged += OnUpdateTierGroup;

            OnUpdateTierGroup(null, null);
        }

        private void OnUpdateTierGroup(string name, object value)
        {
            if(animator != null)
                animator.SetInteger(keyName, TierGroup);
        }
    }

}
