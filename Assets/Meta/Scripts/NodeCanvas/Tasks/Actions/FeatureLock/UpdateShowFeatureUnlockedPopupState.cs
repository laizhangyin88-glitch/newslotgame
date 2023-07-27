using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Task.Action
{
    [Category("★ BagelCode/Feature Lock")]
    public class UpdateShowFeatureUnlockedPopupState : ActionTask
    {
        protected override string info
        {
            get { return "Update Show Feature Unlocked Popup State"; }
        }

        protected override void OnExecute()
        {
            var currentLevel = BlackboardUtils.GetOrCreateVariable<int>(null, "/userSyncInfo/level");

            LockedFeatureType[] featuresToCheck = 
            {
                LockedFeatureType.CHALLENGE,
                LockedFeatureType.TOURNAMENT,
                LockedFeatureType.CLUB,
                LockedFeatureType.EARLY_ACCESS
            };

            for (int i = 0; i < featuresToCheck.Length; i ++)
            {
                int unlockLevel = BlackboardQueryUtils.GetFeatureMinLevel(featuresToCheck[i]);

                if (unlockLevel == currentLevel.value)
                    PlayerPrefs.SetInt(string.Format("SHOW_FEATURE_UNLOCKED_POPUP_{0}", featuresToCheck[i].ToString()), 1);
            }
            
            EndAction();
        }
    }
}
