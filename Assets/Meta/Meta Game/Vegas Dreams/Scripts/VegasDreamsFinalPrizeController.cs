using UnityEngine;
using SlotMaker;
using System.Collections.Generic;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsFinalPrizeController : EventMonoBehaviour
    {
        private ContextElement root;
        private Animator anim;

        private bool isInit = false;

        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            GSManager.Instance.GetHandler(VegasDreams.Defines.FINAL_PRIZE).Play();

            // Init Elements
            InitContents();

            isInit = true;
        }

        private void InitContents()
        {
            var credit = VegasDreams.Utils.FinalRewardCredit;
            var gem = VegasDreams.Utils.FinalRewardGem;
            MetaContextElementUtils.SimpleSetActive(root, "Coin Effect", credit > 0);
            MetaContextElementUtils.SimpleSetActive(root, "Text Coin", credit > 0);
            MetaContextElementUtils.SimpleSetActive(root, "Gem Effect", gem > 0);
            MetaContextElementUtils.SimpleSetActive(root, "Text Gem", gem > 0);

            BlackboardQueryUtils.AddCoins(credit);
            BlackboardQueryUtils.AddGems(gem);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Coin", "COMMA_STYLE_COIN", CHILDREN, credit);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Gem", "COMMA_STYLE_GEM", CHILDREN, gem);

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Collect/Text", "BUTTON_COLLECT", FULL);
            MetaContextElementUtils.SimpleSetClickable(root, "Button Collect", () => 
            {
                EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLOSE);
            });
        }
    }
}
