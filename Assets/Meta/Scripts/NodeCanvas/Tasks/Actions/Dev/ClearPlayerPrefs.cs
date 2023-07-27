using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{

    [Category("★ BagelCode/Dev")]
    
    public class ClearPlayerPrefs : ActionTask<Blackboard>
    {
        protected override string info
        {
            get { return string.Format("Clear Player Preferences"); }
        }
    
        protected override void OnExecute()
        {
            PlayerPrefs.DeleteAll();
            EndAction();
        }
    }

}
