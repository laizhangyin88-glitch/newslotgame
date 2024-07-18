using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Tasks.Conditions
{

[Category("★ SlotMaker/SlotMachine")]
public class CheckSymbolMask : ConditionTask
{
	public BBParameter<int> valueA;
    public SymbolAttribute valueB;

	protected override string info { get { return string.Format("CheckSymbolMask({0}, {1})", valueA, valueB); } }

    protected override bool OnCheck()
    {
		return SymbolMask.HasAttribute((SymbolAttribute)valueA.value, valueB);
    }
}

}
