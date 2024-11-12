using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using SlotMaker;

namespace BagelCode.Task.Condition
{

[Category("★ BagelCode/Events")]
[Description("Send a graph event. If global is true, all graph owners in scene will receive this event. Use along with the 'Check Event' Condition")]
public class SendPassiveEvent : ActionTask<GraphOwner> {

	[RequiredField]
	public BBParameter<string> eventName;
	public BBParameter<float> delay;

	private const string ON_PASSIVE_EVENT = "OnPassiveEvent";

	protected override string info{
		get{ return "★ Send Event [" + eventName + "]" + (delay.value > 0? " after " + delay + " sec." : "" );}
	}

	protected override void OnUpdate(){
		if (elapsedTime >= delay.value){
			var e = new EventData(eventName.value);
			MessageDispatcher.Dispatch(ON_PASSIVE_EVENT, e);
			EndAction();
		}
	}
}


[Category("★ BagelCode/Events")]
[Description("Send a graph event with T value. If global is true, all graph owners in scene will receive this event. Use along with the 'Check Event' Condition")]
public class SendPassiveEvent<T> : ActionTask<GraphOwner> {

	[RequiredField]
	public BBParameter<string> eventName;
	public BBParameter<T> eventValue;
	public BBParameter<float> delay;

	private const string ON_PASSIVE_EVENT = "OnPassiveEvent";

	protected override string info{
		get {return string.Format("★ Event [{0}] ({1}){2}", eventName, eventValue, (delay.value > 0? " after " + delay + " sec." : "")  );}
	}

	protected override void OnUpdate(){
		if (elapsedTime >= delay.value){
			var e = new EventData<T>(eventName.value, eventValue.value);
			MessageDispatcher.Dispatch(ON_PASSIVE_EVENT, e);
			EndAction();
		}
	}
}

}
