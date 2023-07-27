using System.Collections;
using UnityEngine;
using SlotMaker;

namespace BagelCode.LevelUpDash
{
    public class LevelUpDashExpiredTooltipController : MonoBehaviour
    {
        private ContextElement root;
        private Animator anim;

        private float remaining = 3f;
        private bool isClose = false;

        private void Start()
        {
            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text The End", "LEVEL_UP_DASH_EXPIRED_TOOLTIP_END", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Subtitle", "LEVEL_UP_DASH_EXPIRED_TOOLTIP_TEXT", ContextSearchingType.ChildrenSearch);
        }

        private void OnDisable()
        {
            isClose = true;
            Close();
        }

        private void Update()
        {
            if (remaining > 0)
            {
                remaining -= Time.deltaTime;
            }
            else if(!isClose)
            {
                isClose = true;
                Close();
            }
        }

        private void Close()
        {
            if(anim != null) anim.SetTrigger("Close");
        }
    }
}
