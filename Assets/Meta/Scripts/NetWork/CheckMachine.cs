using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine.SceneManagement;

namespace SlotMaker.Tasks.Condition
{

    [Category("★ SlotMaker/Utility")]
    public class CheckMachine : ConditionTask
    {
       // public TargetPlatform platform;

        protected override string info { get { return "Check Machine"; } }

        protected override bool OnCheck()
        {

            //Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            GameObject.Find("IO System").SetActive(ApplicationSettings.Instance.isMachine);
            if (ApplicationSettings.Instance.isMachine)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
