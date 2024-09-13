using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/Symbol")]
    public class SymbolGetGameObject : ActionTask<BaseSymbol>
    {
        public BBParameter<int> prefabId;
        public BBParameter<int> cachingId;

        protected override string info { get { return string.Format("Cache({0}) = GetGameObject({1})", cachingId, prefabId); } }

        protected override void OnExecute()
        {
            var symbolController = agent.GetComponent<SymbolController>();
            var go = symbolController.cachingObjects[cachingId.value];

            if (go != null)
            {
                var goID = go.GetComponent<GameObjectId>();
                if (goID != null && goID.id != prefabId.value)
                {
                    GameObject.Destroy(go);
                    go = null;
                }
            }

            if (go == null)
            {
                go = agent.symbolAssets.GetGameObject(agent.symbolInfo.symbol, prefabId.value);
                if (go != null)
                {
                    go.AddComponent<GameObjectId>().id = prefabId.value;
                    go.transform.SetParent(agent.anchor, false);
                    symbolController.cachingObjects[cachingId.value] = go;
                }
            }
            else
            {
                if (go.activeSelf)
                    go.SetActive(false);
                go.SetActive(true);
            }

            EndAction();
        }
    }
}
