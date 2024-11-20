using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Notice")]
public class GetNoticeListFromType : ActionTask<Blackboard>  
{
    [BlackboardOnly]
    public BBParameter<List<Blackboard>> saveAs;

    public NoticeTypes noticeType;

    protected override string info
    {
        get { return string.Format("{0} = GetNotice({1})", saveAs, noticeType); }
    }

    protected override void OnExecute()
    {
        saveAs.value = GetActiveMaxExposureNoticeList();
        EndAction();
    }

    private List<Blackboard> GetActiveMaxExposureNoticeList()
    {
        List<Blackboard> itemList = BlackboardQueryUtils.GetNoticeListFromType(noticeType);
        if (itemList == null)
            return null;

        List<Blackboard> activeItemList = new List<Blackboard>();
        long currentTimeStamp = TimeUtils.GetTimeStamp();
        for (int i = 0; i < itemList.Count; ++i)
        {
            Blackboard noticeInfo = itemList[i];
            int maxExposureCount = BlackboardUtils.FindValue<int>(noticeInfo, "constraints/maxExposureCount");
            if (maxExposureCount > 0)
            {
                int noticeId = BlackboardUtils.FindValue<int>(noticeInfo, "id");
                string triggeredMaxExposureKey = string.Format(BlackboardQueryUtils.noticeExposureCountKey, noticeId);
                int prefsMaxExposureCount = PlayerPrefs.GetInt(triggeredMaxExposureKey, 0);

                if (prefsMaxExposureCount < maxExposureCount)
                {
                    long endTimestamp = BlackboardQueryUtils.GetNoticeEndTimestamp(noticeInfo);
                    if (endTimestamp == 0 || endTimestamp > currentTimeStamp)
                        activeItemList.Add(noticeInfo);
                }
            }
            else
                activeItemList.Add(noticeInfo);
        }

        return activeItemList;
    }
}

}
