using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ BagelCode/Chat")]
public class EnterRoomUserChat : ActionTask<Blackboard>
{
    public BBParameter<string> userId;
    public BBParameter<string> userName;
    public BBParameter<string> userClubId;

    protected override string info { get { return "Send enter user info to Chat"; } } 

    protected override void OnExecute()
    {
        // var varUserId   = BlackboardUtils.FindVariable<string>(agent, userId.value);
        // var varUserName = BlackboardUtils.FindVariable<string>(agent, userName.value);
        // var varUserClubId = BlackboardUtils.FindVariable<long>(agent, userClubId.value);

        // if (BagelCode.BlackboardQueryUtils.IsIgnoredUser(varUserId.value))
        // {
        //     EndAction();
        //     return;
        // }

        // bool error = false;

        // var bb = BlackboardUtils.CreateBlackboard("chatBB");

        // bb.AddVariable("message", typeof(string));
        // bb.AddVariable("type", typeof(string));
            
        // bb.SetValue("type", "system");       

        // if (BagelCode.BlackboardQueryUtils.IsMyClubMember(varUserClubId.value))
        // {
        //     bb.SetValue("message", StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_CLUB_MEMBER_JOIN", out error, varUserName.value));
        // }
        // else
        // {
        //     if (BagelCode.BlackboardQueryUtils.IsMyFriend(varUserId.value))
        //     {
        //         bb.SetValue("message", StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_FRIEND_JOIN", out error, varUserName.value));
        //     }
        //     else
        //     {
        //         bb.SetValue("message", StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_USER_JOIN", out error, varUserName.value));
        //     }
        // }
        
        // BagelCode.BlackboardQueryUtils.UpdateChat(bb, false);


        EndAction();
    }

}

}
