using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class GetHitCells : ActionTask<Blackboard>
{
	public BBParameter<List<SymbolWin>> winList;

	public BBParameter<List<Cell>> saveAs;

	protected override void OnExecute()
	{
		saveAs.value = new List<Cell>();
		var hashSet = new HashSet<int>();

		int winCount = winList.value.Count;
		for (int i = 0; i < winCount; ++i)
		{
			var win = winList.value[i];
			int cellCount = win.cells.Count;
			for (int j = 0; j < cellCount; ++j)
			{
				var cell = win.cells[j];
				int hashCode = cell.GetHashCode();
				if (!hashSet.Contains(hashCode))
				{
					hashSet.Add(hashCode);
					saveAs.value.Add(cell);
				}
			}
		}

        saveAs.value.Sort((c1, c2) =>
        {
            int ret = c1.column.CompareTo(c2.column);
            if (ret != 0)
                return ret;

            return c1.row.CompareTo(c2.row);
        });

		EndAction();
	}
}

}
