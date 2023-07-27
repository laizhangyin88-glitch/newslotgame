using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class ChangeSymbolSortingOrder : ActionTask
{
    public BBParameter<int> symbolIndex;
    public BBParameter<int> sortingOrder;

    protected override string info { get { return string.Format("ChangeSortingOrder({0}, {1})", symbolIndex, sortingOrder); } }

    protected override void OnExecute()
    {
        ContentCustomData.Instance.symbolSortingOrder.orders[symbolIndex.value] = sortingOrder.value;

        EndAction();
    }
}

}
