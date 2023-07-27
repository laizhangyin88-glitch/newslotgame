using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using BagelCode.ClientModels;
using BagelCode.Protobuf;
using System.Collections.Generic;

namespace BagelCode
{
    public static class MetaBlackboardUtils
    {
        public static void ClearBlackboardList(IBlackboard bb, string key)
        {
            if (bb == null) return;

            var variable = bb.RemoveVariable(key);
            if (variable != null)
            {
                var listTransform = ((Blackboard)bb).transform.Find(key);
                if (listTransform != null)
                {

                    for (int i = 0; i < listTransform.childCount; ++i)
                    {
                        var child = listTransform.GetChild(i);
                        GameObject.Destroy(child.gameObject);
                    }
                }
            }
        }

        public static void SerializeList<T>(IBlackboard bb, List<T> serializableList,
            System.Action<IBlackboard, T> serializeFunc, string listVariableName)
        {
            if (bb == null || serializableList == null || serializableList.Count == 0 ||
                serializeFunc == null || string.IsNullOrEmpty(listVariableName)) return;

            foreach (var serializable in serializableList)
            {
                var listElementBB = BlackboardUtils.CreateBlackboard("element");

                serializeFunc(listElementBB, serializable);
                BlackboardUtils.AddToBlackboardList(bb, listVariableName, listElementBB);
            }
        }

        public static Variable<T> FindVariable<T>(IBlackboard bb, params string[] names)
        {
            if(names.Length == 0) return null;

            foreach(var name in names)
            {
                string variableName = null;
                bb = BlackboardUtils.FindBlackboard(bb, name, ref variableName);
                if (bb == null) return null;

                var variable = bb.GetVariable<T>(variableName);
                if (variable != null) return variable;
            }

            return null;
        }

        // return copy
        public static Blackboard AddCopyToBlackboardList(IBlackboard bb, string key, Blackboard target)
        {
            var newBB = (Blackboard)BlackboardUtils.CreateBlackboard("element");
            BlackboardUtils.CopyBlackboard(target, newBB);

            var bbList = BlackboardUtils.GetOrCreateBlackboardList(bb, key);
            bbList.Add(newBB);

            newBB.transform.parent = ((Blackboard)bb).GetComponent<Transform>().Find(key);
            newBB.transform.SetAsLastSibling();

            return newBB;
        }
    }
}
