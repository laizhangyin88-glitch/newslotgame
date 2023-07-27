using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;

namespace BagelCode
{
    public class DevScratcherController : MonoBehaviour
    {
        private ContextElement tabElement;
        private ContextElement scrollElement;
        private OSA_DevScratcherCoverList osaScratcherScrollView;

        private bool isInit = false;

        public void Init()
        {
            if(isInit) return;

            ContextElement agent = GetComponent<ContextElement>();
            agent.UpdateContext(false);

            tabElement = ContextUtils.FindElement(agent, "Dev Scratcher Tab", ContextSearchingType.ChildrenSearch);
            scrollElement = ContextUtils.FindElement(agent, "Dev Scratcher Cover Scroll Rect", ContextSearchingType.ChildrenSearch);
            osaScratcherScrollView = scrollElement.gameObject.GetComponent<OSA_DevScratcherCoverList>();

            MetaContextElementUtils.SetClickable(
                tabElement,
                "OnSelectTab",
                agent,
                null
            );

            MetaContextElementUtils.SetIntProperty(tabElement, 0);
            SelectTab();

            isInit = true;
        }

        private void TabScratcher()
        {
            MetaContextElementUtils.SetIntProperty(tabElement, 0);
            osaScratcherScrollView.UpdateSymbolItemList();
        }

        private void TabCover()
        {
            MetaContextElementUtils.SetIntProperty(tabElement, 1);
            osaScratcherScrollView.UpdateCoverItemList();
        }

        public void SelectTab()
        {
            OnSelectTab(MetaContextElementUtils.GetIntProperty(tabElement, 0));
        }

        private void OnSelectTab(int index)
        {
            if(index == 0)
                TabScratcher();
            else
                TabCover();
        }
    }
}