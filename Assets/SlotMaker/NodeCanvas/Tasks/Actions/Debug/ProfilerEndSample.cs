using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine.Profiling;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Debug")]
public class ProfilerEndSample : ActionTask
{
	protected override void OnExecute()
	{
#if DEV
		Profiler.EndSample();
#endif
		EndAction();
	}
}

}
