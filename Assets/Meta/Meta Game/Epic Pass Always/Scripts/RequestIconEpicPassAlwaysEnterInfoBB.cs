using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/EpicPass Always")]
    public class RequestIconEpicPassAlwaysEnterInfoBB : ActionTask<Blackboard>
    {
        public BBParameter<List<string>> saveAsWebImageUrlList;
        public BBParameter<string> saveAsBundleName;

        protected override string info
        {
            get { return string.Format("Request EpicPass Always Info(Icon only)"); }
        }

        protected override void OnExecute()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS_V2);
            saveAsWebImageUrlList.value = null;

            if (eventInfo != null)
            {
                var epicPassInfoBB = EpicPassUtilsV2.EpicPassInfo;
                if (epicPassInfoBB == null || eventInfo.id != EpicPassUtilsV2.EventId)
                {
                    BagelCodeClientAPI.RequestSeasonPassEnterInfoV2(BlackboardQueryUtils.GetIngameID(),
                        (response) =>
                        {
                            if (agent != null)
                            {
                                BlackboardQueryUtils.UpdateSeasonPassEnterInfo(response.seasonPassEnterInfo);
                                saveAsWebImageUrlList.value = GetEpicPassIconWebImage();
                                saveAsBundleName.value = GetEpicPassBundleName();
                                EpicPassUtilsV2.EventId = eventInfo.id;
                                EndAction();
                            }
                        },
                        (error) =>
                        {
                            if (agent != null)
                            {
                                GlobalErrorHandler.GlobalError(error);
                                EndAction(false);
                            }
                        });
                }
                else
                {
                    saveAsWebImageUrlList.value = GetEpicPassIconWebImage();
                    saveAsBundleName.value = GetEpicPassBundleName();
                    EndAction(true);
                }
            }
            else
            {
                saveAsWebImageUrlList.value = GetEpicPassIconWebImage();
                saveAsBundleName.value = GetEpicPassBundleName();
                EndAction(true);
            }
        }

        private List<string> GetEpicPassIconWebImage()
        {
            List<string> imageList = new List<string>();

            string iconTabImageURL = EpicPassUtilsV2.TabIconImageUrl;
            string pointIconImageURL = EpicPassUtilsV2.PointIconImageUrl;
            string bigIconImageURL = EpicPassUtilsV2.BigIconImageUrl;

            if (!string.IsNullOrEmpty(iconTabImageURL))
                imageList.Add(iconTabImageURL);
            if (!string.IsNullOrEmpty(pointIconImageURL))
                imageList.Add(pointIconImageURL);
            if (!string.IsNullOrEmpty(bigIconImageURL))
                imageList.Add(bigIconImageURL);

            if (imageList.Count > 0)
                return imageList;
            return null;
        }

        private string GetEpicPassBundleName()
        {
            return MetaStringDefine.LOBBY_BUNDLE_NAME;
        }
    }
}