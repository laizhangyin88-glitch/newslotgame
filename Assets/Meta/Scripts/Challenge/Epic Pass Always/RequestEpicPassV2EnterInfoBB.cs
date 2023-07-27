using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/EpicPass Always")]
    public class RequestEpicPassV2EnterInfoBB : ActionTask<Blackboard>
    {
        public BBParameter<List<string>> saveAsWebImageUrlList;

        protected override string info
        {
            get { return string.Format("Request Epic Pass V2 Enter Info"); }
        }

        protected override void OnExecute()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS_V2);
            saveAsWebImageUrlList.value = null;

            if (eventInfo != null)
            {
                var metaEnterInfoBB = EpicPassUtilsV2.EpicPassInfo;

                if (metaEnterInfoBB == null || string.IsNullOrEmpty(EpicPassUtilsV2.TabIconImageUrl))
                {
                    // todo : data update
                    //BagelCodeClientAPI.RequestMetaEnterGameInfo(BlackboardQueryUtils.GetIngameID(),
                    //(response) =>
                    //{
                    //    if (agent != null)
                    //    {
                    //        BlackboardQueryUtils.UpdateMetaGameEnterInfo(response.metaGameEnterInfo);
                    //        saveAsWebImageUrlList.value = BlackboardQueryUtils.GetMetaGameCommonWebImageList(eventInfo);
                    //        EndAction();
                    //    }
                    //},
                    //(error) =>
                    //{
                    //    if (agent != null)
                    //    {
                    //        GlobalErrorHandler.GlobalError(error);
                    //        EndAction(false);
                    //    }
                    //});
                    saveAsWebImageUrlList.value = BlackboardQueryUtils.GetMetaGameCommonWebImageList(eventInfo);
                    EndAction(true);
                }
                else
                {
                    saveAsWebImageUrlList.value = BlackboardQueryUtils.GetMetaGameCommonWebImageList(eventInfo);
                    EndAction(true);
                }
            }
            else
            {
                EndAction(true);
            }
        }
    }
}