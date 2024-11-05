using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/NativeHelper")]
    public class UnSubscribeBackButtonEvent : ActionTask
    {
        protected override string info
        {
            get 
            {
                return string.Format("UnSubscribe BackButton Event");
            }
        }

        protected override void OnExecute()
        {
            GraphOwner owner = agent.GetComponent<GraphOwner>();
            MetaSystem.UnSubscribeBackButton(owner.GetHashCode());        

            EndAction();                                             
        }
    }
}
