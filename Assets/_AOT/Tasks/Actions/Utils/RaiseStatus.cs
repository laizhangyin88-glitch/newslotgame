using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System.Collections.Generic;

namespace NodeCanvas.Tasks.Actions{

    [Category("★ SlotMaker/Utility")]
    [Description("Raise Success or Failure status. Note that either Success or Failure must be used.")]
    public class RaiseStatus : ActionTask<GraphOwner> 
    {
        [RequiredField]
        public Status status;

        protected override string info{
            get {return string.Format("Raise {0}", status);}
        }

        protected override void OnExecute(){
            EndAction(status == Status.Success);
        }       
    }
}
