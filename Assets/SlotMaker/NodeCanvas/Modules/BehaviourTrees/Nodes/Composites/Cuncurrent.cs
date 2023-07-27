using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees{

	[Category("Composites/BagelCode")]
	[Icon("Parallel")]
	[Color("ff1493")]
	public class Cuncurrent : BTComposite{

		protected override Status OnExecute(Component agent, IBlackboard blackboard){

			var defferedStatus = Status.Resting;
			for (int i = 0; i < outConnections.Count; ++i){
				status = outConnections[i].Execute(agent, blackboard);

				if (defferedStatus == Status.Resting){
					if (status == Status.Failure){
						defferedStatus = Status.Failure;
					}
				}

				if (status == Status.Success || status == Status.Optional){
					outConnections[i].Reset();
				}
			}

			if (defferedStatus != Status.Resting){
				ResetRunning();
				return defferedStatus;
			}

			return Status.Running;
		}

		void ResetRunning(){
			for (var i = 0; i < outConnections.Count; i++){
				if (outConnections[i].status == Status.Running){
					outConnections[i].Reset();
				}
			}
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnNodeGUI(){
			GUILayout.Label("First Failure");
		}

		#endif
	}
}
