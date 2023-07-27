using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/Utils")]
public class SetCellToIndex : ActionTask
{
    public BBParameter<int> totalColumn;
    public BBParameter<int> totalRow;

    public BBParameter<Cell> cell;

    public BBParameter<int> saveAs;

	protected override string info
	{
		get { return string.Format("SetCellToIndex({0}) to {1}", cell, saveAs); }
	}

	protected override void OnExecute()
	{
        saveAs.value = cell.value.row * totalColumn.value + cell.value.column;
		EndAction();
	}
}

}
