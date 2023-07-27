using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/Utils")]
public class SetIndexToCell : ActionTask
{
    public BBParameter<int> totalColumn;
    public BBParameter<int> totalRow;

    public BBParameter<int> index;

    public BBParameter<Cell> saveAs;

	protected override string info
	{
		get { return string.Format("SetIndexToCell({0}) to {1}", index, saveAs); }
	}

	protected override void OnExecute()
	{
        int column = index.value % totalColumn.value;
        int row    = index.value / totalColumn.value;
        saveAs.value = new Cell(column,row);
		EndAction();
	}
}

}
