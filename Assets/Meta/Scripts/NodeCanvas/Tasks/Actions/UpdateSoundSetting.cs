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
            SlotMaker.GSManager.Instance.MusicVolume = (float)PlayerPrefs.GetInt("MUTE_MUSIC", 1);
            SlotMaker.GSManager.Instance.SfxVolume = (float)PlayerPrefs.GetInt("MUTE_SFX", 1);
            EndAction();
        }
    }
}