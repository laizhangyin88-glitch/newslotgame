using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

namespace BagelCode.GemJackpot
{
    public class GemJackpotWinRewardObjectController : MonoBehaviour
    {
        public ContextElement rootElement;
        public Animator rootAnimator;
        public IContextText creditOnText;
        public IContextText creditOffText;

        private bool isInit = false;
        private bool _isActiveSelf = false;
        public bool isActiveSelf
        {
            get { return _isActiveSelf; }
        }

        private long multiplyCredit;

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();

            ContextElement onTextElement = ContextUtils.FindElement(rootElement, "Base On/Text", ContextSearchingType.FullNameSearch);
            ContextElement offTextElement = ContextUtils.FindElement(rootElement, "Base Off/Text", ContextSearchingType.FullNameSearch);

            creditOnText = onTextElement as IContextText;
            creditOffText = offTextElement as IContextText;
            isInit = true;
        }

        public void OnInit()
        {
            InitProperty();

            //rootAnimator.SetBool("Active", true);
        }

        public void SetMultiplyCredit(long multyply)
        {
            multiplyCredit = multyply;
        }

        public void SetWinRewardText(long reward)
        {
            string strReward = StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_CREDIT", reward);
            if (creditOnText != null)
                creditOnText.SetText(strReward);

            if (creditOffText != null)
                creditOffText.SetText(strReward);
        }

        public void SetWinRewardActive(bool isActive)
        {
            if (rootAnimator != null)
            {
                if (isActive != _isActiveSelf && isActive) GSManager.Instance.GetHandler("Meta_Gemjackpot_Wingage").Play();
                _isActiveSelf = isActive;
                rootAnimator.SetBool("isAcitve", isActive);
            }
        }

        public void SetChangeAnimationEvent()
        {
            SetWinRewardText(multiplyCredit);
        }
    }
}
