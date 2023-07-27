using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;

namespace BagelCode.Task.Action
{
    [Category("★ BagelCode/Feature Lock")]
    public class GetFeatureUnlockLevel : ActionTask
    {
        public BBParameter<LockedFeatureType> featureType;
        public BBParameter<int> unlockLevel;

        protected override string info
        {
            get {return string.Format("{0} = Get Geature Unlock Level({1})", unlockLevel, featureType);}
        }

        protected override void OnExecute()
        {
            unlockLevel.value = BlackboardQueryUtils.GetFeatureMinLevel(featureType.value);
        
            EndAction();
        }
    }
}
