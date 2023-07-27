using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using UnityEngine.UI;

namespace BagelCode.EpicPass
{
    public class EpicPassLoadingSceneController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Animator rootAnimator;
        private Blackboard rootBB;

        private ContextElement bigIconElement;
        private ContextElement backgroundWebImageAreaElement;
        private ContextElement backgroundImageElement;
        private ContextElement defaultBackgroundElement;

        private ContextElement progressBar;
        private ContextElement progressIconAreaElement;
        private ContextElement progressIconElement;
        private ContextElement progressIconImageElement;

        private bool isInit = false;

        private Blackboard _metaEnterInfoBB;
        private Blackboard MetaEnterInfoBB
        {
            get
            {
                if(_metaEnterInfoBB == null)
                    _metaEnterInfoBB = BlackboardQueryUtils.GetMetaGameEnterInfo();
                return _metaEnterInfoBB;
            }
        }

        private void InitProperty()
        {
            if(isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();
            rootBB = gameObject.GetComponent<Blackboard>();

            bigIconElement = ContextUtils.FindElement(rootElement, "Epic Pass Icon/Icon", ContextSearchingType.FullNameSearch);
            backgroundWebImageAreaElement = ContextUtils.FindElement(rootElement, "Background Server Area/Epic Pass Background Web Image", ContextSearchingType.FullNameSearch);
            backgroundImageElement = ContextUtils.FindElement(backgroundWebImageAreaElement, "Anchor/Background", ContextSearchingType.FullNameSearch);
            defaultBackgroundElement = ContextUtils.FindElement(rootElement, "Background Default Area/Epic Pass Background Default", ContextSearchingType.FullNameSearch);

            progressBar = ContextUtils.FindElement(rootElement, "Progress Bar", ContextSearchingType.ChildrenSearch);
            progressIconAreaElement = ContextUtils.FindElement(rootElement, "Progress Bar Icon Area", ContextSearchingType.ChildrenSearch);
            progressIconElement = ContextUtils.FindElement(progressIconAreaElement, "Epic Pass Progress Bar Icon", ContextSearchingType.ChildrenSearch);
            progressIconImageElement = ContextUtils.FindElement(progressIconElement, "Web Image", ContextSearchingType.ChildrenSearch);
            progressBar.gameObject.GetComponent<Slider>().handleRect = progressIconAreaElement.gameObject.GetComponent<RectTransform>();

            // Disable interactable
            MetaContextElementUtils.SetBooleanProperty(progressBar, false);

            isInit = true;
        }

        public void OnInit()
        {
            InitProperty();

            var isEnter = BlackboardUtils.GetOrCreateVariable<bool>(rootBB, "isEnter");

            if(MetaEnterInfoBB != null && isEnter.value)
            {
                string pointIconImageURL = BlackboardUtils.GetOrCreateVariable<string>(MetaEnterInfoBB, "pointIconImageUrl").value;
                string iconBigImageURL = BlackboardUtils.GetOrCreateVariable<string>(MetaEnterInfoBB, "iconBigImageUrl").value;
                string backgroundImageURL = BlackboardUtils.GetOrCreateVariable<string>(MetaEnterInfoBB, "backgroundImageUrl").value;

                if(MetaContextElementUtils.SetWebImage(progressIconImageElement, pointIconImageURL))
                {
                    // Active
                }
                else
                {
                    // Deactive
                }

                if(MetaContextElementUtils.SetWebImage(bigIconElement, iconBigImageURL))
                {
                    // Active
                }
                else
                {
                    // Deactive
                }

                if(MetaContextElementUtils.SetWebImage(backgroundImageElement, backgroundImageURL))
                {
                    // Active
                    defaultBackgroundElement.gameObject.SetActive(false);
                }
                else
                {
                    // Deactive
                    defaultBackgroundElement.gameObject.SetActive(true);
                }
            }

            rootAnimator.SetBool("MetaGameLogo", isEnter.value);

            // progreaaBar Update.

            rootAnimator.SetBool("Active", true);
        }
    }
}