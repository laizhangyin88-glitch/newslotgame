using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsInGameTextController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private bool isStartingText = false;
        private long penaltyDuration = 0L;

        private long remaining;
        private int iRemaining;

        private bool isFinish = false;
        private bool isInit = false;

        private const long ZERO_DISPLAYING_TIME = 200L;

        private void Start()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            anim.SetBool("Active", true);

            isStartingText = bb.GetValue<bool>("isStartingText");
            penaltyDuration = bb.GetValue<long>("penaltyDuration");
            remaining = penaltyDuration;
            iRemaining = (int)(remaining / 1000L) + 1;

            if (isStartingText)
                MetaContextElementUtils.SetClickable(root, Close);

            UpdateText();

            isInit = true;
        }

        private void Update()
        {
            if (!isStartingText) // Penalty
            {
                remaining -= (long)(Time.deltaTime * 1000f);
                int _iRemaining = (int)(remaining / 1000L) + 1;
                if (_iRemaining < iRemaining)
                {
                    iRemaining = _iRemaining;
                    UpdateText();
                }

                if (remaining < -ZERO_DISPLAYING_TIME)
                {
                    Close();
                }
            }
        }

        private void UpdateText()
        {
            if (isStartingText)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Text",
                    "HIDDEN_OBJECTS_IN_GAME_STARTING", ContextSearchingType.ChildrenSearch);
            }
            else // Penalty
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Text",
                    "HIDDEN_OBJECTS_IN_GAME_WARNING", ContextSearchingType.ChildrenSearch, iRemaining);
            }
        }

        private void Close()
        {
            if (isFinish) return;

            EventSender.SendCalleeCallback(gameObject);
            PopupManager.Instance.Close(gameObject);
            anim.SetTrigger("Close");

            isFinish = true;
        }
    }
}
