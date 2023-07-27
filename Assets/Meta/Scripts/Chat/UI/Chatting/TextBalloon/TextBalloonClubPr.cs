using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Chat
{
    public class TextBalloonClubPr : TextBalloonBase
    {
        [SerializeField]
        private ContextText clubNameContext;
        [SerializeField]
        private ContextText clubContentContext;
        [SerializeField]
        private ContextElement clubSymbolArea;

        private ContextButton viewButton;

        private GameObject clubSymbol;


        public override void Init(ChattingController _owner)
        {
            base.Init(_owner);
        }

        public override void Refresh(ChatMessageData _chatData)
        {
            base.Refresh(_chatData);

            var data = (_chatData.chatPoll.data as ChatDataClubPr);
            
            if(!IsMe())
            {

                if(viewButton == null)
                {
                    var buttonArea = mContext.Find("Button Area");
                    viewButton = MetaObjectUtils.MakePrefab<ContextButton>("Button Primary", buttonArea.transform);
                    viewButton.UpdateContext();
                    viewButton.FindElement<ContextTextMeshProUGUI>("Text").SetGlobalText("CHAT_GLOBAL_CLUB_PR_VIEW_BUTTON");
                }

                viewButton.RemoveAllListener();
                viewButton.AddListenerOnClick(
                    (context) => 
                    {
                        var clubMemberObj = MetaObjectUtils.MakeScene("Popup Club Member Scene", PopupManager.Instance.transform.Find("Area"));
                        var clubMemberScene = clubMemberObj.GetComponent<Blackboard>();
                        PopupManager.Instance.Open(clubMemberObj);
                        clubMemberScene.AddVariable("clubID", data.clubId);
                    }
                );

                viewButton.gameObject.SetActive(true);
            }
            else
            {
                if(viewButton != null)
                    viewButton.gameObject.SetActive(false);
            }

            clubNameContext.SetText(data.clubName);
            clubContentContext.SetGlobalText("CHAT_GLOBAL_CLUB_PR_CONTENT_TEXT", data.clubName);

            if(clubSymbol != null)
                Destroy(clubSymbol);

            if (!string.IsNullOrEmpty(data.clubSymbol))
            {
                clubSymbol = MetaIconUtils.MakeClubSymbolIconObject(data.clubSymbol, clubSymbolArea.transform, null);
            }
        }

    }
}

