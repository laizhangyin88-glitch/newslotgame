using System.Collections;
using GameStudio.Slot.TRL.Utillity;
using UnityEngine;

namespace GameStudio.Slot.TRL
{
    public class TRLScatterMove : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        public Animator animator { get { return _animator; } }

        public Coroutine MoveCoroutine(int moveDistance) => StartCoroutine(_MoveCoroutine(moveDistance));
        IEnumerator _MoveCoroutine(int moveDistance)
        {
            animator.SetInteger("Position", moveDistance);
            animator.Update(Time.deltaTime);
            TRLUtillity.PlaySound("Scatter Move");

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        }

        public void Enable()
        {
            animator.gameObject.SetActive(true);
        }
        public void Disable()
        {
            animator.gameObject.SetActive(false);
        }
    }
}