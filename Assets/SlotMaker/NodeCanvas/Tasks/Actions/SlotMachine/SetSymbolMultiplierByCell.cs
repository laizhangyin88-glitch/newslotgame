using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
	[Category("★ BagelCode/SlotMachine")]
	public class SetSymbolMultiplierByCell : ActionTask<BaseSlotMachine>
	{
	    public BBParameter<Cell> cell;
		public BBParameter<int> multiplier;
		public OperationMethod Operation = OperationMethod.Set;

	    protected override string info
	    {
			get { return string.Format("Set Symbol Multiplier({0}, {1}, {2})", cell, multiplier, Operation); }
	    }

	    protected override void OnExecute()
	    {
	    	var symbolInfo = agent.GetSymbol(cell.value.column, cell.value.row).symbolInfo;
	    	symbolInfo.multiplier = OperationUtils.Operate(symbolInfo.multiplier, multiplier.value, Operation);
	        EndAction();
	    }
	}
}
