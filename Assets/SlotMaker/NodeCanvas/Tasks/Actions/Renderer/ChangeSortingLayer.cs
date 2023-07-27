using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Renderer")]
public class ChangeSortingLayer : ActionTask<Transform>
{
    public BBParameter<string> sortingLayerName;
    public BBParameter<int> sortingOrder;
    public BBParameter<bool> includeAllChildren;

    private bool changeOrder;

    protected override void OnExecute()
    {
        changeOrder = !sortingOrder.isNone && !sortingOrder.isNull;

        if (!includeAllChildren.value)
        {
            var renderer = agent.GetComponent<Renderer>();
            Change(renderer);
        }
        else
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(agent);

            while (queue.Count != 0)
            {
                Transform child = queue.Dequeue();
                for(int i = 0; i < child.childCount; ++i)
                {
                    Transform subChild = child.GetChild(i);
                    Renderer renderer  = subChild.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        Change(renderer);
                    }

                    if (subChild.childCount > 0)
                    {
                        queue.Enqueue(subChild);
                    }
                }
            }
        }

        EndAction();
    }

    private void Change(Renderer renderer)
    {
        renderer.sortingLayerName = sortingLayerName.value;
        if (changeOrder)
            renderer.sortingOrder = sortingOrder.value;
    }
}

}
