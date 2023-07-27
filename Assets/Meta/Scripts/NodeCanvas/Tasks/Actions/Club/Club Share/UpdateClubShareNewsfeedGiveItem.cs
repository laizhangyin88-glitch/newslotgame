using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
 {
     
     [Category("★ BagelCode/Club")]
     public class UpdateClubShareNewsfeedGiveItem : ActionTask<Blackboard>
     {
        public BBParameter<Blackboard> feedInfoBB;

        public BBParameter<Blackboard> ownerInfoBB;
        public BBParameter<EventInfoType> mgEventType;
        public BBParameter<int> itemID;
        public BBParameter<int> subtractCount;

        public BBParameter<float> saveAsFromGauge;
        public BBParameter<float> saveAsTargetGauge;

        protected override string info
        {
            get { return "Update Club Share Newsfeed give item"; }
        }

        protected override void OnExecute()
        {
            var like = BlackboardUtils.FindVariable<int>(feedInfoBB.value, "like");
            var capacity = BlackboardUtils.FindVariable<int>(feedInfoBB.value, "capacity");
            saveAsFromGauge.value = (float)like.value/(float)capacity.value;
            like.value += 1;
            saveAsTargetGauge.value = (float)like.value/(float)capacity.value;
            
            var feedListInfoBB = BlackboardUtils.FindVariable<List<Blackboard>>(ownerInfoBB.value, "clubNewsFeedResponse/feedList").value;

            for(int i=0; i < feedListInfoBB.Count; ++i)
            {
                ClubFeedType feedType = feedListInfoBB[i].GetValue<ClubFeedType>("type");
                if(feedType == ClubFeedType.COLLECTING_GAME_REQUEST)
                {
                    switch(mgEventType.value)
                    {
                        case EventInfoType.COLLECTING_GAME:
                            {

                                var pieceID = feedListInfoBB[i].GetValue<int>("pieceId");

                                if(pieceID == itemID.value)
                                {
                                    var possessions = feedListInfoBB[i].GetVariable<int>("possessions");
                                    possessions.value -= subtractCount.value;
                                }
                            }
                            break;
                    }
                }
            }

            var memberListInfoBB = BlackboardUtils.FindVariable<List<Blackboard>>(ownerInfoBB.value, "clubInfoResponse/clubMemberList").value;

            var meID = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/userId");

            for(int i=0; i < memberListInfoBB.Count; ++i)
            {
                var userID = memberListInfoBB[i].GetValue<string>("userId");
                if(userID == meID.value)
                {
                    var clubGiving = BlackboardUtils.FindVariable<long>(memberListInfoBB[i], "clubGiving");
                    if(clubGiving != null)
                        clubGiving.value += 1;

                    break;
                }
            }

            EndAction();
        }
     }
 }