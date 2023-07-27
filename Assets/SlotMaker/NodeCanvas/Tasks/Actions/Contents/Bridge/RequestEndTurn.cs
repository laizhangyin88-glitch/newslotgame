using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;
using System.Collections.Generic;

namespace BagelCode.Tasks.Actions.ClientAPI
{
	[Category("★ BagelCode/ClientAPI")]
	public class RequestEndTurn : ActionTask
	{
	    protected override void OnExecute()
	    {
	    	MetaSystem.SlotEndTurn(null, null);
	        EndAction();
	    }
	}
}
