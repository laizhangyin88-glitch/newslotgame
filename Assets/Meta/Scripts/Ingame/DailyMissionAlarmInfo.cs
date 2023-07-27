using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{

public enum DailyMissionAlarmType
{
    Progress,
    Complete,
    AllComplete,

    UNKNOWN
}

public class DailyMissionAlarmInfo
{
    public DailyMissionAlarmType alarmType;
    public List<string> textList = new List<string>();
}

}
