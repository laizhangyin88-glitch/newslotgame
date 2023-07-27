using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/Utils")]
public class SetCellToColumnRow : ActionTask
{
	public BBParameter<Cell> cell;

	public BBParameter<int> column;
	public BBParameter<int> row;

	protected override string info
	{
		get { return string.Format("{0}, {1} = {2}", column, row, cell); }
	}

    protected override void OnExecute()
    {
		if (!column.isNone)
            column.value = cell.value.column;
		if (!row.isNone)
            row.value = cell.value.row;
		EndAction();
	}
}

}
