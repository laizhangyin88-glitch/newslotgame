using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Utils")]
public class GetPlatformString : ActionTask
{
	public BBParameter<string> saveAs;

	protected override string info
	{
		get{ return string.Format("{0} = PlatformName", saveAs); }
	}

	protected override void OnExecute()
	{
		string platformName = ApplicationSettings.GetPlatformName().ToUpper();

		if(platformName == null)
		{
		    Debug.LogError("Can not detect platformDevice");
            EndAction(false);
		}
		else
		{
			saveAs.value = platformName;
			EndAction();
		}
	}
}	

}

