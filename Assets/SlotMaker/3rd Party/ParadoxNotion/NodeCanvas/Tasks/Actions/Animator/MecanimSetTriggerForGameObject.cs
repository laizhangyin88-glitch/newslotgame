using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

   // [Name("Set Parameter Trigger")]
    [Category("Animator")]
    [Description("You can either use a parameter name OR hashID. Leave the parameter name empty or none to use hashID instead.")]
    public class MecanimSetTriggerForGameObject : ActionTask<GameObject>   //<Animator>
    {
        public BBParameter<GameObject> gameObject;
        public BBParameter<string> nodePath;
        public BBParameter<string> parameter;
        public BBParameter<int> parameterHashID;

        protected override string info
        {
            get { return string.Format("Mec.SetTrigger {0} for {1}", string.IsNullOrEmpty(parameter.value) && !parameter.useBlackboard ? parameterHashID.ToString() : parameter.ToString() , gameObject); }
        }

        protected override void OnExecute()
        {

            GameObject gb = null;
            if (gameObject == null || gameObject.value == null)
            {
                gb = agent;
            }
            else
            {
                gb = gameObject.value;
            }


            string path = nodePath.value;
            Animator node = null;
            if (string.IsNullOrEmpty(path))
            {
                node = gb.GetComponent<Animator>();
            }
            else{
                node = gb.transform.Find(path).GetComponent<Animator>();
            }

            if (!string.IsNullOrEmpty(parameter.value))
            {
                node.SetTrigger(parameter.value);
            }
            else
            {
                node.SetTrigger(parameterHashID.value);
            }
            EndAction();
        }
    }
}
