using NodeCanvas.Framework;
using System.Collections.Generic;

namespace BagelCode
{
    public class LobbyBannerGroupPreview : LobbyBannerGrorupBase
    {
        public override void UpdateBannerInfo(List<Blackboard> noticeList)
        {
            isPreview = true;
            base.UpdateBannerInfo(noticeList);
        }
    }
}
