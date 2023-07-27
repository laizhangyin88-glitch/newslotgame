using GameStudio.Slot.IIP.Utility;
using UnityEngine;
namespace GameStudio.Slot.IIP.Feature
{
    public class IIPCommunityGamePenguin : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        public bool isAppeared = false;

        private void OnEnable()
        {
            isAppeared = false;
        }

        public void Rescue()
        {
            animator.SetBool("Rescue", true);
        }

        public void Arrive()
        {
            animator.SetBool("Arrive", true);
            IIPUtility.PlaySound("Penguin Return");
        }

        public void SetDirection(int direction)
        {
            animator.SetInteger("Direction", direction);
        }

        public void SetIsFront(bool isFront)
        {
            animator.SetBool("Front", isFront);
        }
        public void Jump()
        {
            animator.SetTrigger("Jump");
        }

        public void Appear()
        {
            animator.SetTrigger("Appear");
            isAppeared = true;
        }

        public void SetIsWalking(bool isWalking)
        {
            animator.SetBool("Walking", isWalking);
        }
    }
}