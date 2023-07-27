using UnityEngine;
using System.Collections;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

	[Category("★ BagelCode/Events")]
	public class SendSpinButtonEvent : ActionTask {

		private const string ON_SPINBUTTON_EVENT = "OnSpinButtonEvent";

		protected override string info{
			get{ return "★ Global Send SpinButtonEvent"; }
		}

		protected override void OnUpdate(){
            var e = new EventData(ON_SPINBUTTON_EVENT);
            MessageDispatcher.Dispatch(ON_SPINBUTTON_EVENT, e);
            EndAction();
		}
	}
}
