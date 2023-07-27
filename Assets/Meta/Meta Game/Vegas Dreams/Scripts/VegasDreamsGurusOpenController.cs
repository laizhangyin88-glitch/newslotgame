using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelCode.ClientModels;
using SlotMaker;
using NodeCanvas.Framework;
using UnityEngine.UI;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsGurusOpenController : MonoBehaviour
    {
        private ContextElement root;
        private Animator anim;
        private Blackboard bb;

        private bool isInit = false;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            InitContents();

            isInit = true;
        }

        private void InitContents()
        {
            var closeButtonElement = ContextUtils.FindElement(root, "Button Close", CHILDREN);
            MetaContextElementUtils.SetClickable(closeButtonElement,
                () => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLOSE));

            var collectButtonElement = ContextUtils.FindElement(root, "Button Collect", CHILDREN);
            MetaContextElementUtils.SetClickable(collectButtonElement,
                () => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLOSE));
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Collect/Text", "VEGAS_DREAMS_GURUS_OPEN_BUTTON", FULL);

            BlackboardQueryUtils.UpdateGurusBuilding(VegasDreams.Utils.CreatedGurusBuilding);
            BlackboardQueryUtils.UpdateGurusBuildingPreset(VegasDreams.Utils.NewGurusBuildingPreset);
            EventSender.SendGlobalEvent(VegasDreams.Events.ON_OPEN_GURUS_BUILDING);
        }
    }
}
