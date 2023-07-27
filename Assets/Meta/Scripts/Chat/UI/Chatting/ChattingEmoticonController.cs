using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class ChattingEmoticonController : MonoBehaviour
    {
        private ContextElement buttonElement;
        private Animator anim;

        private void Awake()
        {
            buttonElement = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();

            MetaContextElementUtils.SetClickable(buttonElement, OnClick);
        }

        private void OnClick()
        {
            anim.SetBool("isActive", true);
        }

        private void Play()
        {
        }
    }
}
