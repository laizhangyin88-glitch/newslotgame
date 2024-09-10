using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Utils")]
    public class UpdateSoundSetting : ActionTask
    {
        protected override string info
        {
            get
            {
                return string.Format("Update Sound Setting");
            }
        }

        protected override void OnExecute()
        {
            SlotMaker.GSManager.Instance.MusicVolume = PlayerPrefs.GetFloat("MUTE_MUSIC", 1);
            SlotMaker.GSManager.Instance.SfxVolume = PlayerPrefs.GetFloat("MUTE_SFX", 1);
            EndAction();
        }
    }
}
