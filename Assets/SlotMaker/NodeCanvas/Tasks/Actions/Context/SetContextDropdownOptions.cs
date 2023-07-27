using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using TMPro;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextDropdownStringOptions : ActionTask<ContextElement> 
{
	public BBParameter<List<string>> options;

	protected override string info
	{
		get { return string.Format("{0}({1})", agentInfo, options); }
	}

	protected override void OnExecute()
	{
		ContextTextMeshProDropdown dropdownContext = agent as ContextTextMeshProDropdown;
		if (dropdownContext != null)
		{
			var dropdown = dropdownContext.dropdown;
			dropdown.ClearOptions();
			dropdown.AddOptions(options.value);
		}
		EndAction();
	}
}

[Category("★ SlotMaker/Context")]
public class SetContextDropdownObjectOptions : ActionTask<ContextElement> 
{
	public BBParameter<IList> options;

	protected override string info
	{
		get { return string.Format("{0}({1})", agentInfo, options); }
	}

	protected override void OnExecute()
	{
		ContextTextMeshProDropdown dropdownContext = agent as ContextTextMeshProDropdown;
		if (dropdownContext != null)
		{
			var stringOptions = new List<string>();
			for (int i = 0; i < options.value.Count; ++i)
				stringOptions.Add(options.value[i].ToString());

			var dropdown = dropdownContext.dropdown;
			dropdown.ClearOptions();
			dropdown.AddOptions(stringOptions);
		}
		EndAction();
	}
}

[Category("★ SlotMaker/Context")]
public class SetContextDropdownCustomFormatOptions : ActionTask<ContextElement> 
{
	public BBParameter<IList> options;
	public BBParameter<string> format;

	protected override string info
	{
		get { return string.Format("{0}({1})", agentInfo, options); }
	}

	protected override void OnExecute()
	{
        if(format != null && !string.IsNullOrEmpty(format.value))
        {
			ContextTextMeshProDropdown dropdownContext = agent as ContextTextMeshProDropdown;
			if (dropdownContext != null)
			{
				var stringOptions = new List<string>();
				for (int i = 0; i < options.value.Count; ++i)
					stringOptions.Add(string.Format(format.value, options.value[i].ToString()));

				var dropdown = dropdownContext.dropdown;
				dropdown.ClearOptions();
				dropdown.AddOptions(stringOptions);
			}
        }

		EndAction();
	}
}

}