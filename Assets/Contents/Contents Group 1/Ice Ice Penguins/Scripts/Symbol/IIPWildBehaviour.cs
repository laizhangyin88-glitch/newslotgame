using System.Collections.Generic;
using GameStudio.Slot.IIP.Utility;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace GameStudio.Slot.IIP.Symbol
{
    public class IIPWildBehaviour : SymbolBehaviour
    {
        private List<Transform> fromAnchorList;
        private Transform targetAnchor;
        private ObjectPool wildFlyingPool;

        public override void StartBehaviour(SymbolEventHandler eventHandler)
        {
            base.StartBehaviour(eventHandler);
            if (fromAnchorList == null)
                fromAnchorList = BlackboardUtils.FindVariable<GameObject>("./wildFlyAnchor").value.GetComponent<Blackboard>().GetVariable<List<Transform>>("anchorList").value;
            if (targetAnchor == null)
                targetAnchor = BlackboardUtils.FindVariable<GameObject>("./moon").value.transform;
            if (wildFlyingPool == null)
                wildFlyingPool = BlackboardUtils.FindVariable<GameObject>("./wildFlyPool").value.GetComponent<ObjectPool>();
        }

        public override void OnEntry()
        {
            animator.SetBool("Text", false);
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
            if (gameObject.activeInHierarchy)
                PlayAnimation("Idle");
            GetCachedObject(0).SetActive(false);
            GetCachedObject(1).SetActive(false);
        }

        public override void OnStopEffect()
        {
            PlayAnimation("Deactive");
            GetCachedObject(0).SetActive(false);
            GetCachedObject(1).SetActive(true);

            Transform fromAnchor = fromAnchorList[symbol.column].GetChild(symbol.row);

            GameObject wildFlying = wildFlyingPool.GetObject(true).gameObject;
            wildFlying.transform.SetParent(wildFlying.transform);
            wildFlying.SetActive(false);
            wildFlying.transform.position = fromAnchor.position;

            //Set from anchor and target anchor in wild flying instance 
            Blackboard wildFlyingBB = wildFlying.GetComponent<Blackboard>();
            Variable<Transform> from = wildFlyingBB.GetVariable<Transform>("from");
            Variable<Transform> to = wildFlyingBB.GetVariable<Transform>("to");
            from.SetValue(fromAnchor);
            to.SetValue(targetAnchor);

            wildFlying.SetActive(true);
        }

        public override void OnWin()
        {
            PlayAnimation("Deactive");
            GetCachedObject(0).SetActive(true);
            GetCachedObject(1).SetActive(false);
        }
    }
}
