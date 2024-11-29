using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions {
    [Category("★ BagelCode/Utils")]
    public class OpenURL : ActionTask {
        public BBParameter<string> url;

        protected override string info {
            get { 
                return string.Format("Open URL {0}", url); 
            }
        }

        protected override void OnExecute() {
            if (url == null) {
            } else {
#if UNITY_WEBGL && !UNITY_EDITOR
                NativeHelper.Instance.OpenUrl(url.value);
#else
                Application.OpenURL(url.value);
#endif
            }
            EndAction();
        }
    }
}

