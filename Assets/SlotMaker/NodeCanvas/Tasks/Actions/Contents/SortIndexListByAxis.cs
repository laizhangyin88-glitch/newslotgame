using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
	[Category("★ BagelCode/Contents")]
	public class SortIndexListByAxis : ActionTask
	{
	    public BBParameter<List<int>> indices;
	    public BBParameter<int> column;
	    public BBParameter<int> row;

	    public enum StartAxis
	    {
	    	Column,
	    	Row
	    }
	    public StartAxis fromAxis = StartAxis.Column;
	    public StartAxis toAxis = StartAxis.Row;

	    public enum SortOrder
	    {
	    	Accending,
	    	Descending
	    }
	    public SortOrder order = SortOrder.Accending;

	    public BBParameter<List<int>> saveAs;

		protected override string info
		{
	        get { return string.Format("{0} = Sort({1}, {2}, {3}, {4})", saveAs, indices, fromAxis, toAxis, order); }
		}

		protected override void OnExecute()
		{
			saveAs.value = new List<int>(indices.value);
			saveAs.value.Sort((lhv, rhv) => {
				if (fromAxis == StartAxis.Column && toAxis == StartAxis.Row)
				{
					lhv = (lhv % column.value) * row.value + (lhv / column.value);
					rhv = (rhv % column.value) * row.value + (rhv / column.value);
				}
				else if (fromAxis == StartAxis.Row && toAxis == StartAxis.Column)
				{
					lhv = (lhv % row.value) * column.value + (lhv / row.value);
					rhv = (rhv % row.value) * column.value + (rhv / row.value);
				}
				return (lhv - rhv) * (order == SortOrder.Accending ? 1 : -1);
			});

			EndAction();
		}
	}
}
