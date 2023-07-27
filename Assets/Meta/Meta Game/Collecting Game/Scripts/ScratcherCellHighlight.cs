using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;

namespace BagelCode.Scratcher
{
    public class ScratcherCellHighlight
    {
        public List<ScratcherCellController> highlightCells;
        public List<GameObject> gameObjectsToActivate;
        public bool isHit;

        public ScratcherCellHighlight(Blackboard symbolBB)
        {
            highlightCells = new List<ScratcherCellController>();
            gameObjectsToActivate = new List<GameObject>();
            if (symbolBB != null)
                isHit = symbolBB.GetVariable<bool>("isHit")?.value ?? false;
            else isHit = false;
        }
    }
}