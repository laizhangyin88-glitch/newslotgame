using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

namespace BagelCode.GemJackpot
{
    public class GemJackpotClosePopupController : MonoBehaviour
    {
        private bool isInit = false;
        private Blackboard bb => GemJackpotUtils.GemJackpotInfo;

        public enum nodealAction
        {
            None,
            Spin,
            Collect
        }

        public void Update()
        {
            if (isInit == false)
            {
                isInit = true;
                Initialize();
            }
        }

        public void Initialize()
        {
            ContextElement rootElement = GetComponent<ContextElement>();
            rootElement.UpdateContext();

            int freeSpinCount = bb.GetValue<int>("freeSpinCount");
            if (freeSpinCount == 0)
            {
                ContextTextMeshProUGUI textDesc = ContextUtils.FindElement(rootElement, "Contents Area/Text Desc", ContextSearchingType.FullNameSearch).GetComponent<ContextTextMeshProUGUI>();
                textDesc.gameObject.SetActive(false);
            }

            ContextButton spinButton = ContextUtils.FindElement(rootElement, "Button Spin", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            spinButton.AddListenerOnClick(
                context =>
                {
                    closePopup(nodealAction.Spin);
                }
            );

            ContextButton collectButton = ContextUtils.FindElement(rootElement, "Button Collect", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            collectButton.AddListenerOnClick(
                context =>
                {
                    closePopup(nodealAction.Collect);
                    BlackboardUtils.SetOrCreateValue(bb, "reserveClose", true);
                }
            );

            int nextProgress = bb.GetValue<int>("nextProgress");
            if (nextProgress <= 0)
            {
                collectButton.gameObject.SetActive(false);
            }

            ContextButton closeButton = ContextUtils.FindElement(rootElement, "Button Close Area/Button Close", ContextSearchingType.FullNameSearch).GetComponent<ContextButton>();
            closeButton.AddListenerOnClick(
                context =>
                {
                    closePopup(nodealAction.Spin);
                }
            );


        }

        public void closePopup(nodealAction result)
        {
            BlackboardUtils.SetOrCreateValue(bb, "noDealAction", (int)result);
        }
    }
}
