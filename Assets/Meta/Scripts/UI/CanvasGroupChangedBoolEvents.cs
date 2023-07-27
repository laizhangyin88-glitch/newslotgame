using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;

namespace BagelCode
{

[RequireComponent(typeof(ScrollRect))]
public class CanvasGroupChangedBoolEvents : UIBehaviour
{
    public UnityBoolEvent onInteractionChanged;

    private readonly List<CanvasGroup> canvasGroupCache = new List<CanvasGroup>();
    protected override void OnCanvasGroupChanged()
    {
        bool _groupAllowInteraction = true;
        Transform t = transform;
        while (t != null)
        {
            t.GetComponents(canvasGroupCache);
            bool shouldBreak = false;
            for (int i = 0; i < canvasGroupCache.Count; ++i)
            {
                if (!canvasGroupCache[i].interactable)
                {
                    _groupAllowInteraction = false;
                    shouldBreak = true;
                }

                if (canvasGroupCache[i].ignoreParentGroups)
                    shouldBreak = true;
            }
            if (shouldBreak)
                break;

            t = t.parent;
        }

        onInteractionChanged.Invoke(_groupAllowInteraction);
    }
}

}