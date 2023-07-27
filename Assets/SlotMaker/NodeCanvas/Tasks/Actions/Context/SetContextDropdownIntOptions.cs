using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using TMPro;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextDropdownIntOptions : ActionTask<ContextElement> 
{
	public BBParameter<int> begin;
	public BBParameter<int> end;

	protected override string info
	{
		get { return string.Format("{0}({1}-{2})", agentInfo, begin, end); }
	}

	protected override void OnExecute()
	{
		ContextTextMeshProDropdown dropdownContext = agent as ContextTextMeshProDropdown;
		if (dropdownContext != null)
		{
			var options = new List<string>();
			for (int i = begin.value; i < end.value; ++i)
				options.Add(i.ToString());

			var dropdown = dropdownContext.dropdown;
			dropdown.ClearOptions();
			dropdown.AddOptions(options);
		}
		EndAction();
	}
}

}