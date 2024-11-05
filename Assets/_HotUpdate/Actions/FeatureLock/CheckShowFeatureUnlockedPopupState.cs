using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace BagelCode.Task.Action
{
    [Category("★ BagelCode/Feature Lock")]
    public class CheckShowFeatureUnlockedPopupState : ConditionTask
    {
        public BBParameter<List<LockedFeatureType>> featuresToCheck;
        public BBParameter<LockedFeatureType> featureType;
        
        protected override string info
        {
            get
            {
                string features = "";
                if (featuresToCheck.value != null)
                {
                    for (int i = 0; i < featuresToCheck.value.Count; i++)
                        features += (i == 0 ? featuresToCheck.value[i].ToString() : ", " + featuresToCheck.value[i].ToString());
                }
                
                return string.Format("Check Show Feature Unlocked Popup State for {0}", features);
            }
        }

        protected override bool OnCheck()
        {
            if (featuresToCheck.value != null)
            {
                for (int i = 0; i < featuresToCheck.value.Count; i++)
                {
                    int show = PlayerPrefs.GetInt(string.Format("SHOW_FEATURE_UNLOCKED_POPUP_{0}", featuresToCheck.value[i].ToString()), 0);

                    if (show == 1)
                    {
                        featureType.value = featuresToCheck.value[i];
                        PlayerPrefs.SetInt(string.Format("SHOW_FEATURE_UNLOCKED_POPUP_{0}", featuresToCheck.value[i].ToString()), 0);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}