using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/Utils")]
public class SetColumnRowToCell : ActionTask
{
    public BBParameter<int> column;
    public BBParameter<int> row;

    public BBParameter<Cell> saveAs;

	protected override string info
	{
		get { return string.Format("SetColumnRowToCell({0},{1}) to {2}", column,row,saveAs); }
	}

	protected override void OnExecute()
	{
        saveAs.value = new Cell(column.value, row.value);
		EndAction();
	}
}

}
