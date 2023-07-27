using System.Collections;
using System.Collections.Generic;
using GameStudio.Slot;
using GameStudio.Slot.IIP.Utility;
using SlotMaker;
using UnityEngine;

namespace GameStudio.Slot.IIP.Feature
{
    public class IIPWildExpectation : FeatureController
    {
        [SerializeField] private Animator animator;
        [SerializeField] private List<Transform> parentList = new List<Transform>();
        [SerializeField] private List<ObjectPool> expectationPoolList = new List<ObjectPool>();

        protected override string ON_FEATURE_BEGIN_EVENT => "IIPWildExpectation";

        protected override string ON_FEATURE_END_EVENT => "IIPEndWildExpectation";

        protected override IEnumerator OnPlayCoroutine()
        {
            const int row = 3;
            const int column = 5;
            const int effectCount = 20;
            const float effectDelay = 0.1f;

            int randomRow = Random.Range(0, row);
            int randomColumn = Random.Range(0, column);
            int lastRow = Random.Range(0, row);
            int lastColumn = Random.Range(0, column);

            animator.SetBool("isActive", true);
            IIPUtility.PlaySound("Wild Expectation");
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < effectCount; i++)
            {
                while (randomRow == lastRow) randomRow = Random.Range(0, row); ;
                while (randomColumn == lastColumn) randomColumn = Random.Range(0, column);

                lastColumn = randomColumn;
                lastRow = randomRow;

                PooledObject instance = expectationPoolList[randomRow].GetObject(true);
                instance.transform.SetParent(parentList[randomColumn]);
                instance.transform.localPosition = Vector3.zero;

                yield return new WaitForSeconds(effectDelay);
            }
            yield return new WaitForSeconds(1f);
            animator.SetBool("isActive", false);
        }
    }
}