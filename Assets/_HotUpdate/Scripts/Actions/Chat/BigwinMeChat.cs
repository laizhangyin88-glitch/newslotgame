using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;

namespace SlotMaker.Tasks.Actions
{

[Category("★ BagelCode/Chat")]
public class BigwinMeChat : ActionTask<Blackboard>
{
    public BBParameter<int> winType;
    public BBParameter<string> earnCredit = "./spin/earnCredit";

    protected override string info { get { return "Send my bigwin to Chat"; } } 

    protected override void OnExecute()
    {
        // bool error = false;
        // var credit = BlackboardUtils.FindVariable<long>(agent, earnCredit.value);
        // var name = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/me/name");

        // var bb = BlackboardUtils.CreateBlackboard("chatBB");

        // bb.AddVariable("message", typeof(string));
        // bb.AddVariable("type", typeof(string));
            
        // bb.SetValue("type", "win");       

        // if (winType.value == 0)
        // {
        //     bb.SetValue("message", StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_BIGWIN", out error, name.value, credit.value));
        // }
        // else if (winType.value == 1)
        // {
        //     bb.SetValue("message", StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_SUPERBIGWIN", out error, name.value, credit.value));
        // }
        // else if (winType.value == 2)
        // {
        //     bb.SetValue("message", StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_MEGAWIN", out error, name.value, credit.value));
        // }
        // else if (winType.value == 3)
        // {
        //     bb.SetValue("message", StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_SUPERMEGAWIN", out error, name.value, credit.value));
        // }
        // else if (winType.value == 4)
        // {
        //     bb.SetValue("message", StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_EPICWIN", out error, name.value, credit.value));
        // }

        // BagelCode.BlackboardQueryUtils.UpdateChat(bb, false);

        // if (winType.value == 4)
        // {
        //     bool error = false;
        //     var credit = BlackboardUtils.FindVariable<long>(agent, earnCredit.value);
        //     var name = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/me/name");

        //     var bb = BlackboardUtils.CreateBlackboard("chatBB");

        //     bb.AddVariable("message", typeof(string));
        //     bb.AddVariable("type", typeof(string));
                
        //     bb.SetValue("type", "win");

        //     bb.SetValue("message", StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_EPICWIN", out error, name.value, credit.value));

        //     BagelCode.BlackboardQueryUtils.UpdateChat(bb, false);
        // }

        EndAction();
    }

}

}
