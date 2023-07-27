using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine.Profiling;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Debug")]
public class ProfilerBeginSample : ActionTask
{
	public string sampleName;

	protected override void OnExecute()
	{
#if DEV
		Profiler.BeginSample(sampleName);
#endif
		EndAction();
	}
}

}
