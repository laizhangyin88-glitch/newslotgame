using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Utils")]
    public class UpdateFPSSetting : ActionTask
    {
        private static int FRAME_RATE_30 = 30;
        private static int FRAME_RATE_60 = 60;

        protected override string info
        {
            get
            {
                return string.Format("Update FPS Setting");
            }
        }

        protected override void OnExecute()
        {
#if !UNITY_WEBGL
            QualitySettings.vSyncCount = 0;

            if (PlayerPrefs.HasKey("fps"))
            {
                Application.targetFrameRate = PlayerPrefs.GetInt("fps") > 0 ? FRAME_RATE_30 : FRAME_RATE_60;
            }
            else
            {
                Application.targetFrameRate = FRAME_RATE_60;
                PlayerPrefs.SetInt("fps", 0);
            }
#endif
            EndAction();
        }
    }
}