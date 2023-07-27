using SlotMaker;
using UnityEngine;

namespace BagelCode.EpicAlbum
{
    public class PageChangeFinished : StateMachineBehaviour
    {
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (animatorStateInfo.normalizedTime >= 1f)
            {
                var pageController = animator.GetComponent<IEpicAlbumPageElement>();
                if(pageController != null)
                {
                    pageController.PageChangeFinished();
                }

                animator.SetInteger("Page", 0);
            }
            
        }
    }

}
