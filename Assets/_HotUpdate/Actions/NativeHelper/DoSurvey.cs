using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

    [Category("★ BagelCode/NativeHelper")]
    public class DoSurvey : ActionTask<ContextElement>
    {
        public BBParameter<string> hash;
        public BBParameter<string> userId;
        
        protected override string info
        {
            get
            {
                return string.Format("Do survey");
            }
        }

        protected override void OnExecute()
        {
#if !UNITY_EDITOR
        NativeHelper.Instance.DoSurvey(hash.value, userId.value, () => { EndAction(true); });
#else
            EndAction(true);
#endif
        }
    }

}
