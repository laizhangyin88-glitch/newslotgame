using System.Collections;
using System.Collections.Generic;
using BagelCode.Tasks.Actions.Contents;
using GameStudio.Slot;
using GameStudio.Slot.IIP.Utility;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.IIP.Feature
{
    public class IIPWildCollectAdmin : FeatureModule
    {
        public ObjectPool objectPool;
        public Transform target;
        public SlotMachine slotMachine;
        [SerializeField]
        private List<Transform> listAnchors = new List<Transform>();
        private void Awake()
        {
            RegisterEvent("IIPCollectWild", CollectAllWilds);
        }



        public void CollectAllWilds(EventData eventData)
        {
            StartCoroutine(CollectAllWildCoroutine());
        }

        public IEnumerator CollectAllWildCoroutine()
        {
            BeginBonus beginBonus = new BeginBonus();
            beginBonus.bonusId = 21500;
            IIPUtility.ExecuteAction(beginBonus, this);

            Blackboard bonusBB = BlackboardUtils.FindVariable<Blackboard>("./bonus/response").value;
            List<Blackboard> cellList = bonusBB.GetVariable<List<Blackboard>>("wildList").value;
            int count = 0;
            foreach (var each in cellList)
            {
                if (count == 0) IIPUtility.PlaySound("Wild Appear 1");
                if (count > 0) IIPUtility.PlaySound("Wild Appear 2");
                count++;
                BaseSymbol symbol = slotMachine.GetSymbol(each.GetVariable<int>("x").value, each.GetVariable<int>("y").value);
                Vector3 fromPosition = symbol.transform.position;

                GameObject wildFlying = objectPool.GetObject(true).gameObject;
                wildFlying.transform.SetParent(wildFlying.transform);
                wildFlying.SetActive(false);
                wildFlying.transform.position = fromPosition;

                Variable<Transform> from = BlackboardUtils.FindVariable<Transform>(wildFlying.GetComponent<Blackboard>(), "from");
                from.SetValue(listAnchors[symbol.column].GetChild(symbol.row));
                Variable<Transform> to = BlackboardUtils.FindVariable<Transform>(wildFlying.GetComponent<Blackboard>(), "to");
                to.SetValue(target);
                wildFlying.SetActive(true);
                yield return new WaitForSeconds(0.15f);
            }

            EndBonus endBonus = new EndBonus();
            IIPUtility.ExecuteAction(endBonus, this);

            yield return new WaitForSeconds(0.66f);


            ContentEvent.SendEvent("IIPEndCollectWild");
        }
    }
}