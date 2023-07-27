using System.Collections;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using BagelCode.OSA_Scroll;
using System.Collections.Generic;
using BagelCode.ClientModels;

namespace BagelCode.VipLounge
{
    public class PopupVipLoungeJackpotResultBase : MonoBehaviour
    {
        protected ContextElement root = null;
        protected Blackboard rootBB = null;
        protected Animator anim = null;

        protected ContextElement scoreTextElement = null;
        protected ContextElement collectButtonElement = null;

        protected long winCredit = 0L;
        protected LoungeJackpotWinType winType = LoungeJackpotWinType.UNKNOWN;

        private Coroutine closeCoroutine = null;

        public void OnInit()
        {
            root = GetComponent<ContextElement>();
            rootBB = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext();

            winCredit = BlackboardUtils.FindVariable<long>(rootBB, "winCredit")?.value ?? 10000L;
            winType = BlackboardUtils.FindVariable<LoungeJackpotWinType>(rootBB, "winType")?.value ?? LoungeJackpotWinType.UNKNOWN;

            InitProperty();
            PlaySound();
        }

        protected virtual void InitProperty() { }
        protected virtual void PlaySound() { }
        protected void OnClose()
        {
            if (closeCoroutine != null)
                return;
            closeCoroutine = StartCoroutine(OnCloseCoroutine());
        }

        protected void SetScoreText(string text)
        {
            if (scoreTextElement != null)
                MetaContextElementUtils.SetText(scoreTextElement, text);
        }

        protected void SendCallback()
        {
            EventSender.SendCalleeCallback(gameObject);
        }

        protected virtual IEnumerator OnCloseCoroutine()
        {
            yield return null;
        }
    }
}