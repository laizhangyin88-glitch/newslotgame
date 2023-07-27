using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelCode.ClientModels;
using SlotMaker;
using NodeCanvas.Framework;
using UnityEngine.UI;
using ParadoxNotion.Services;
using ParadoxNotion;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsWildPuzzleController : MonoBehaviour
    {
        private const float MIN_GAUGE_VALUE = 0.05f;

        private ContextElement root;
        private Animator anim;
        private Blackboard bb;

        private bool isInit = false;

        private float timer = 0;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private List<Animator> depots = new List<Animator>();

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
            var closeButtonElement = ContextUtils.FindElement(root, "Anchor/Button Close", FULL);
            MetaContextElementUtils.SetClickable(closeButtonElement,
                () => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLOSE));

            var count = VegasDreams.Utils.WildPuzzleCount;
            var max = VegasDreams.Utils.WildPuzzleCountMax;

            var isCollectable = count == max;

            float gaugeValue = count > 0 ? Mathf.Lerp(MIN_GAUGE_VALUE, 1, (float)count / max) : 0f;
            MetaContextElementUtils.SimpleSetActive(root, "Anchor/Contents/Progress Bar/Handle", count > 0, FULL);
            MetaContextElementUtils.SimpleSetActive(root, "Anchor/Contents/Progress Bar/Handle/Glow", count != max, FULL);
            MetaContextElementUtils.SimpleSetActive(root, "Anchor/Contents/Progress Bar/Handle/Particle Flare", count != max, FULL);

            MetaContextElementUtils.SimpleSetFloatProperty(root, "Anchor/Contents/Progress Bar", gaugeValue, FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Anchor/Contents/Text Contents", "VEGAS_DREAMS_WILD_PUZZLE_TEXT", FULL, VegasDreams.Utils.WildDepotExp);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Anchor/Bottom Area/Button Collect/Text", "BUTTON_COLLECT", FULL);
            MetaContextElementUtils.SimpleSetActive(root, "Anchor/Contents/Progress Bar/End Gage", isCollectable, FULL);

            anim.SetBool("isFull", isCollectable);
            if (isCollectable)
                MetaContextElementUtils.SimpleSetText(root, "Anchor/Contents/Progress Bar/Text Progress Bar","MAX", FULL);
            else
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Anchor/Contents/Progress Bar/Text Progress Bar","A_PER_B", FULL, count, max);
            
            var collectButtonElement = ContextUtils.FindElement(root, "Anchor/Bottom Area/Button Collect", FULL);
            collectButtonElement.GetComponent<PIDButton>().interactable = isCollectable;
            MetaContextElementUtils.SetClickable(collectButtonElement,
                () => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_COLLLECT));
        }

        public void SendCalleeCallback()
        {
            var caller = BlackboardUtils.FindVariable<GameObject>(bb, "caller");

            if (VegasDreams.Utils.GurusBuilding != null)
            {
                EventSender.SendEvent(caller.value, MessageRouter.ON_CUSTOM_EVENT, new EventData("OnCollectWildGurus"));
            }
            else
            {
                EventSender.SendEvent(caller.value, MessageRouter.ON_CUSTOM_EVENT, new EventData("OnCollectWildSelectPopup"));
            }
        }
    }
}
