using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    public class SymbolController : MonoBehaviour
    {
        public Symbol symbol;

        public List<GameObject> cachingObjects;


        public FSMOwner owner;
        private Dictionary<Graph, Graph> graphInstances = new Dictionary<Graph, Graph>();

        private const string SKIP_ANIMATION_NAME = "Skip";

        protected Graph GetInstance(Graph originalGraph)
        {
            if (originalGraph == null)
                return null;

            //in editor the instance is always the original
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return originalGraph;
#endif

            //if its already an instance, return the instance
            if (graphInstances.ContainsValue(originalGraph))
                return originalGraph;

            Graph instance = null;

            //if it's not an instance but rather an asset reference which has been instantiated before, return the instance stored,
            //otherwise create and store a new instance.
            if (!graphInstances.TryGetValue(originalGraph, out instance))
            {
                instance = InstantiateGraph(originalGraph);
            }

            return instance;
        }

        [Button]
        public void StartBehaviour()
        {
            owner.graph = GetInstance(symbol.symbolAssets.GetGraph(symbol.symbolInfo.symbol));
            var fsm = (FSM)owner.graph;
            if (fsm != null)
            {
                fsm.isRunning = true;
                fsm.EnterState((FSMState)fsm.primeNode);
            }
        }

        public void EnterState(string stateName)
        {
            owner.TriggerState(stateName);
        }

        [Button]
        public void StopBehaviour()
        {
            if (owner.graph != null)
                owner.graph.isRunning = false;
            owner.graph = null;
            ClearCachingObject();
        }

        public void ClearCachingObject()
        {
            int cachingCount = cachingObjects.Count;
            for (int i = 0; i < cachingCount; ++i)
            {
                var cachingObject = cachingObjects[i];
                if (cachingObject != null)
                {
                    GameObjectId gameObjectId = null;
                    if (cachingObject.TryGetComponent(out gameObjectId))
                        Destroy(gameObjectId);

                    var pooledObject = cachingObject.GetComponent<PooledObject>();
                    if (pooledObject != null)
                        pooledObject.ReturnToPool();
                    else
                        Destroy(cachingObject);

                    cachingObjects[i] = null;
                }
            }
        }
        private Graph InstantiateGraph(Graph originalGraph)
        {
            Graph instance = Graph.Clone<Graph>(originalGraph, null);
            graphInstances[originalGraph] = instance;
            instance.UpdateReferencesFromOwner(owner);

            return instance;
        }

        public bool TryInstantiateGraph(Graph originalGraph)
        {
            if (!graphInstances.ContainsKey(originalGraph))
            {
                InstantiateGraph(originalGraph);
                return true;
            }
            return false;
        }

        public void Play(BaseSymbol symbol, string animationName)
        {
            EnterState(animationName);
        }

        public void Play(string animationName)
        {
            EnterState(animationName);
        }

        public void Skip(BaseSymbol symbol)
        {
            EnterState(SKIP_ANIMATION_NAME);
        }
    }
}
