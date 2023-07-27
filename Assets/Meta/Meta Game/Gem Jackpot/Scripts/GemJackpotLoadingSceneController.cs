using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

namespace BagelCode.GemJackpot
{
    public class GemJackpotLoadingSceneController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Animator rootAnimator;
        private Blackboard rootBB;

        private ContextElement contentsElement;
        private ContextElement jackpotTextElement;
        private ContextElement progressBar;

        private bool isInit = false;

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();
            rootBB = gameObject.GetComponent<Blackboard>();

            contentsElement = ContextUtils.FindElement(rootElement, "Contents", ContextSearchingType.ChildrenSearch);
            jackpotTextElement = ContextUtils.FindElement(contentsElement, "Text", ContextSearchingType.ChildrenSearch);

            progressBar = ContextUtils.FindElement(contentsElement, "Progress Bar", ContextSearchingType.ChildrenSearch);

            BlackboardUtils.SetOrCreateValue(rootBB, "_progressBar", progressBar);

            ContextElement topIconElement = ContextUtils.FindElement(contentsElement, "Icon Rotate Area", ContextSearchingType.ChildrenSearch);
            CheckScreenScale();
            if (topIconElement != null)
            {
#if (UNITY_WEBGL || UNITY_WSA) && !UNITY_EDITOR
                topIconElement.gameObject.SetActive(false);
#endif
            }

            isInit = true;
        }

        public void OnInit()
        {
            InitProperty();

            rootAnimator.SetBool("MetaGameLogo", BlackboardUtils.GetOrCreateVariable<bool>(rootBB, "isEnter").value);
            rootAnimator.SetBool("Active", true);
        }

        public void SetOrientationBB(GameObject target, Orientation orientation)
        {
            Blackboard bb = target.GetComponent<Blackboard>();
            BlackboardQueryUtils.SetOrientationBB(bb, "prevOrientation", orientation);
        }

        public void SetLobbyBGMPlay(bool isPlay)
        {
            //BlackboardQueryUtils.SetBGMPlay(isPlay);
            if (isPlay)
                GSManager.Instance.MusicVolume = (float)PlayerPrefs.GetInt("MUTE_MUSIC", 1);
            else
                GSManager.Instance.MusicVolume = 0.0f;
        }

        private void CheckScreenScale()
        {
            //GemJackpotUtils.CheckScreenScale(contentsElement, new Vector3(1.2f, 1.2f, 1.0f));
        }
    }
}
