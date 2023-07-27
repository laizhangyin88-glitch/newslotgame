using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class EpicPassRewardIconController : MonoBehaviour
    {
        private bool isInit = false;

        private ContextElement rootElement;
        private ContextElement iconUnlock;
        private ContextElement iconRestart;

        private void Start()
        {
            InitProperty();
        }

        public void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(true);

            iconUnlock = ContextUtils.FindElement(rootElement, "Icon Unlock", ContextSearchingType.ChildrenSearch);
            iconRestart = ContextUtils.FindElement(rootElement, "Icon Restart", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetActive(iconUnlock, !EpicPassUtils.IsReset);
            MetaContextElementUtils.SetActive(iconRestart, EpicPassUtils.IsReset);

            isInit = true;
        }
    }
}