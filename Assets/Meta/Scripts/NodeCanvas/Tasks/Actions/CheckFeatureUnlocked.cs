using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace UnityEngine
{
    [Category("★ BagelCode/Utils")]
    public class CheckFeatureUnlocked : ConditionTask
    {
        public BBParameter<List<LockedFeatureType>> unlockFeatureList;
        public BBParameter<LockedFeatureType> unlockFeatureType;
        
        protected override string info
        {
            get { return string.Format("Check Feature Unlocked : {0}", unlockFeatureType.value); }
        }

        protected override bool OnCheck() 
        {
            for (int i = 0; i < unlockFeatureList.value.Count; i++)
            {
                if (unlockFeatureType.value == unlockFeatureList.value[i])
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}