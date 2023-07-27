using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;

namespace BagelCode
{
    public class MetaOverrideRelativeSortingLayerAutoUpdaterBase : MonoBehaviour
    {
        protected bool isUpdate = true;

        private void Start()
        {
            isUpdate = true;
        }

        private void OnEnable()
        {
            if(isUpdate)
                StartCoroutine("SortLayer");
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator SortLayer()
        {
            yield return null;

            UpdateSortingLayer();

            isUpdate = false;
        }

        public void UpdateSortingLayer()
        {
            var sortingLayer = GetComponent<OverrideSortingLayer>();

            if(sortingLayer != null)
                sortingLayer.UpdateSortingLayer();
        }
    }
}