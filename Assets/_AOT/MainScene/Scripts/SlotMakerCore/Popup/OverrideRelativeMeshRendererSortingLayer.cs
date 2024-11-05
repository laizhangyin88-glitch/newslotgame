using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace SlotMaker
{
    [RequireComponent(typeof(MeshRenderer))]
    public class OverrideRelativeMeshRendererSortingLayer : OverrideSortingLayer
    {
        public int relativeSortingOrder;

        public override void UpdateSortingLayer()
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            var rootCanvas = transform.parent._GetComponentInParent<Canvas>();

            meshRenderer.sortingLayerID = rootCanvas.sortingLayerID;
            meshRenderer.sortingOrder = rootCanvas.sortingOrder + relativeSortingOrder;
        }
    }
}
