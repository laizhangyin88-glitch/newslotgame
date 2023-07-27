using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class ChangePayLinesSortingLayer : ActionTask<Transform>
{
    public BBParameter<string> sortingLayerName;
    public BBParameter<int> orderInLayer;

    protected override string info { get { return string.Format("Change Pay Lines Sorting Order({0}, {1})", sortingLayerName, orderInLayer); } }

    protected override void OnExecute()
    {
        // SortingLayer.NameToID returns 0 when the sorting layer of given name doesn't exist
        if (SortingLayer.NameToID(sortingLayerName.value) == 0) 
        {
            Debug.LogErrorFormat("[SlotMachine] '{0}' is an invalid sorting layer name.", sortingLayerName.value);
            EndAction();
            return;
        }

        List<PayLines> payLinesList;
        // This action takes the Transform of the GO as an agent which has any payline related components.
        var payLinesHandler = agent.gameObject.GetComponent<PayLinesHandler>();
        var slotMachineEventForwarderLine = agent.gameObject.GetComponent<SlotMachineEventForwarder_Line>();
        var slotMachineEventForwarderSpotLine = agent.gameObject.GetComponent<SlotMachineEventForwarder_SpotLine>();
        
        if (payLinesHandler != null)
        {
            payLinesList = payLinesHandler.payLinesList;
        }
        else if (slotMachineEventForwarderLine != null)
        {
            payLinesList = slotMachineEventForwarderLine.payLinesList;
        }
        else if (slotMachineEventForwarderSpotLine != null)
        {
            payLinesList = slotMachineEventForwarderSpotLine.payLinesList;
        }
        else
        {
            Debug.LogErrorFormat("[SlotMachine] Agent '{0}' does not have any payline related components.", agent.name);
            EndAction();
            return;
        }

        for (int i = 0; i < payLinesList.Count; i++)
        {
            var lines = payLinesList[i].lines;
            for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
            {
                var lineRenderer = lines[lineIndex];
                lineRenderer.sortingLayerName = sortingLayerName.value;
                lineRenderer.sortingOrder = orderInLayer.value;
            }
        }

        EndAction();
    }
}

}
