using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class ClearReelSpinOrders : ActionTask
{
    public BBParameter<int> column;
    public BBParameter<int> row;
    public BBParameter<bool> direction = true;

    public BBParameter<List<int>> saveAs;

    protected override string info { get { return string.Format("ClearReelSpinOrders"); } }

    protected int GetDirectionalColumn(int column)
    {
        return direction.value ? column : (this.column.value - 1) - column;
    }

    protected override void OnExecute()
    {
        var orderList = new List<int>();

        for (int i = 0; i < column.value; ++i)
        {
            orderList.Add(i);
        }

        saveAs.value = orderList;

        EndAction();
    }
}

}
