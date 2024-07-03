using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;
using SlotMaker;
using Sirenix.OdinInspector;

namespace BagelCode
{

    public class ContentUIActive : MonoBehaviour
    {
        public Animator animator;

        // private Dictionary<string, MessageDispatcher.EventDelegate> contentUIDelegates = new Dictionary<string, MessageDispatcher.EventDelegate>();

        private bool forceMode;

        private MessageDelegates delegates;

        private int ANIMATOR_ACTIVE = Animator.StringToHash("Active");

        private void Awake()
        {
            delegates = new MessageDelegates
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                { "ShowUI", ShowUI },
                { "HideUI", HideUI }
                }
            );
        }

        private void Start()
        {
            BlackboardUtils.GetOrCreateVariable<bool>(null, "/inGame").value = true;
        }

        private void OnEnable()
        {
            MessageDispatcher.Register("OnContentUIEvent", delegates.Delegate);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister("OnContentUIEvent", delegates.Delegate);
        }

        private void ShowUI(EventData eventData)
        {
#if DEV
        if (eventData.value != null)
        {
            animator.SetBool(ANIMATOR_ACTIVE, true);
            forceMode = false;
            return;
        }
#endif
            if (!forceMode)
                animator.SetBool(ANIMATOR_ACTIVE, true);
        }

        private void HideUI(EventData eventData)
        {
#if DEV
        if (eventData.value != null)
        {
            animator.SetBool(ANIMATOR_ACTIVE, false);
            forceMode = true;
            return;
        }
#endif
            if (!forceMode)
                animator.SetBool(ANIMATOR_ACTIVE, false);
        }

        [Button]
        public void test_HideUI()
        {
            animator.SetBool(ANIMATOR_ACTIVE, false);
        }
        [Button]
        public void test_ShowUI()
        {
            animator.SetBool(ANIMATOR_ACTIVE, true);
        }
    }  
    
}
