using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    // [Name("Set Parameter Trigger")]
    [Category("Animator")]
    [Description("You can either use a parameter name OR hashID. Leave the parameter name empty or none to use hashID instead.")]
    public class MecanimPlayStateForGameObject : ActionTask<GameObject>   //<Animator>
    {
        public BBParameter<GameObject> gameObject;
        public BBParameter<string> nodePath;
        public BBParameter<string> parameter;

        protected override string info
        {
            get { return string.Format("Mec.Play State {0} for {1}",  parameter.ToString(), gameObject); }
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
            else
            {
                node = gb.transform.Find(path).GetComponent<Animator>();
            }

            if (!string.IsNullOrEmpty(parameter.value) && node != null)
            {
                node.Play(parameter.value);
            }

            EndAction();
        }
    }
}
