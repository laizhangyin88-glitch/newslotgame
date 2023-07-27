using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Utils")]
    public class SetEnableBackButton : ActionTask
    {
        public BBParameter<bool> isEnable;

        protected override string info
        {
            get
            {
                return string.Format("Enable Back Button {0}", isEnable);
            }
        }

        protected override void OnExecute()
        {
            MetaObjectUtils.EnableBackButton(isEnable.value);
            EndAction();
        }
    }
}