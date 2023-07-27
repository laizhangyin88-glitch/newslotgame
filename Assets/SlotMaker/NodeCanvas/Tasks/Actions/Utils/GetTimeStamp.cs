using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions 
{
    public enum TimeStampType
    {
        GetTimeStamp,
        GetLocalTimeStamp,
        GetSessionPlayTime,
        GetTotalSessionPlayTime
    };

    [Category("★ BagelCode/TimeUtils")]
    public class GetTimeStamp : ActionTask 
    {
        public TimeStampType timeStampType;
        [BlackboardOnly]
        public BBParameter<long> saveAs;

        protected override string info 
        {
            get 
            { 
                switch (timeStampType) 
                {
                case TimeStampType.GetTimeStamp:
                    return string.Format("Get Time Stamp as {0}", saveAs); 
                case TimeStampType.GetLocalTimeStamp:
                    return string.Format("Get Local Time Stamp. as {0}", saveAs); 
                case TimeStampType.GetSessionPlayTime:
                    return string.Format("Get Session Play Time as {0}", saveAs); 
                case TimeStampType.GetTotalSessionPlayTime:
                    return string.Format("Get Total Session Play Time as {0}", saveAs); 
                }
                
                return string.Format("Get Time Stamp as {0}", saveAs); 
            }
        }

        protected override void OnExecute() 
        {
            switch (timeStampType) 
            {
            case TimeStampType.GetTimeStamp:
                saveAs.value = MetaSystem.GetTimeStamp();
                break;
            case TimeStampType.GetLocalTimeStamp:
                saveAs.value = MetaSystem.GetLocalTimeStamp();
                break;
            case TimeStampType.GetSessionPlayTime:
                saveAs.value = MetaSystem.GetSessionTimeStamp();
                break;
            case TimeStampType.GetTotalSessionPlayTime:
                saveAs.value = MetaSystem.GetTotalSessionTimeStamp();
                break;
            }
            
            EndAction();
        }
    }
}

