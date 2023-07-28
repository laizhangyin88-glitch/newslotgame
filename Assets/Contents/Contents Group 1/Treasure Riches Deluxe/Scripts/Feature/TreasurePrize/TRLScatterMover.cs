using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameStudio.Slot.TRL.Utillity;
using UnityEngine;

namespace GameStudio.Slot.TRL
{
    public class TRLScatterMover : MonoBehaviour
    {
        [SerializeField] private List<TRLScatterMove> reelScatterMoveList = new List<TRLScatterMove>();

        public void DisableScatterAnimators()
        {
            foreach (var each in reelScatterMoveList) each.Disable();
        }

        public Coroutine MoveStackedScattersCoroutine(List<int> scatterStackColumns) => StartCoroutine(_MoveStackedScattersCoroutine(scatterStackColumns));
        private IEnumerator _MoveStackedScattersCoroutine(List<int> scatterStackColumns)
        {
            List<Animator> moveingAnimatorList = new List<Animator>();
            int consecutivelyCount = 0;

            for (int i = 0; i < scatterStackColumns.Count; i++)
            {
                if (scatterStackColumns[i] == i) consecutivelyCount++;

                TRLScatterMove each = reelScatterMoveList[scatterStackColumns[i]];
                each.Enable();
                moveingAnimatorList.Add(each.animator);
                each.animator.Update(Time.deltaTime);
            }
            yield return new WaitUntil(() => moveingAnimatorList.Count == 0 || moveingAnimatorList.All((each) => each.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f));

            for (int i = 0; i < scatterStackColumns.Count; i++)
            {
                if (i != scatterStackColumns[i])
                {
                    reelScatterMoveList[scatterStackColumns[i]].MoveCoroutine(scatterStackColumns[i] - i);
                    yield return new WaitForSeconds(0.3f);
                }
            }
            yield return new WaitUntil(() => moveingAnimatorList.Count == 0 || moveingAnimatorList.All((each) => each.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f));
        }


        public Coroutine WinAnimationCoroutine() => StartCoroutine(_WinAnimationCoroutine());
        private IEnumerator _WinAnimationCoroutine()
        {
            List<Animator> activeAnimatorList = new List<Animator>();
            foreach (var each in reelScatterMoveList)
            {
                Animator animator = each.animator;
                if (animator.gameObject.activeSelf == true)
                {
                    animator.SetTrigger("Win");
                    activeAnimatorList.Add(animator);
                    animator.Update(Time.deltaTime);
                }
            }
            TRLUtillity.PlaySound("Scatter Stack");
            yield return new WaitForSeconds(2f);
        }
    }
}