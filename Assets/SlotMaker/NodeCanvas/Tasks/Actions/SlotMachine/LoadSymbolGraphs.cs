using System;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using AsyncTasks = System.Threading.Tasks;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/SlotMachine")]
    public class LoadSymbolGraphs : ActionTask<Transform>
    {
        public BBParameter<SymbolAssets> symbolAssets;
        public BBParameter<float> graphLoadInterval = 0.05f;

        protected override void OnExecute()
        {
            var slotMachine = agent.GetComponent<SlotMachine>();
            HashSet<Graph> requiredGraph = new HashSet<Graph>();

            for (int i = 0; i < symbolAssets.value.Count; ++i)
            {
                if (symbolAssets.value.GetGraph(i))
                    requiredGraph.Add(symbolAssets.value.GetGraph(i));
            }

            LoadSequenceAsync(slotMachine, requiredGraph);
            EndAction();
        }

#if DEV
        private float beginTime;
        private float endTime;

        private void BeginSample()
        {
            beginTime = Time.realtimeSinceStartup;
        }

        private void EndSample()
        {
            endTime = Time.realtimeSinceStartup;
            if (ApplicationSettings.LogTest())
               Debug.Log($"Symbol Graph PreLoad - Total Load Time : {endTime - beginTime}s");
        }
#endif

        private async AsyncTasks.Task LoadSequenceAsync(BaseSlotMachine slotMachine, HashSet<Graph> requiredGraph)
        {
#if DEV
            BeginSample();
#endif
            for (int i = 0; i < slotMachine.symbolPool.availiableObjects.Count; i++)
            {
                var child = slotMachine.symbolPool.transform.GetChild(i);
                var symbolController = child.GetComponent<SymbolController>();
                await LoadGraphAsync(symbolController, requiredGraph);
            }
            foreach (var each in slotMachine.GetSymbols())
            {
                var child = each.transform;
                var symbolController = child.GetComponent<SymbolController>();
                await LoadGraphAsync(symbolController, requiredGraph);
            }
#if DEV
            EndSample();
#endif
        }

        private async AsyncTasks.Task LoadGraphAsync(SymbolController symbolController, HashSet<Graph> requiredGraph)
        {
            foreach (var graphAsset in requiredGraph)
            {
                symbolController.TryInstantiateGraph(graphAsset);
                await AsyncTasks.Task.Delay(TimeSpan.FromSeconds(graphLoadInterval.value));
            }

        }
    }
}