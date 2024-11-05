using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using System.Text.RegularExpressions;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Chat")]
public class GetChatMessage : ActionTask<Blackboard>
{
    protected override string info
    {
        get { return "Get Chat Mini Message"; }
    }

    private const string colotTagFront = "(?:<color=)(.*?)(?:>)";
    private const string colotTagEnd   = "</color>";
    private const string shortenForm   = "{0}..";    

    public BBParameter<int> length;
    [BlackboardOnly]
    public BBParameter<Blackboard> chatBB;
    [BlackboardOnly]
    public BBParameter<string> saveAs;
    private const string constMessage = "message";
    private const string constType = "type";

    protected override void OnExecute()
    {
        var message = chatBB.value.GetVariable<string>(constMessage);
        var chatType = chatBB.value.GetVariable<string>(constType);
        
        if (message != null && message.value != null)
        {
            string miniMessage = message.value;
            
            miniMessage = Regex.Replace(miniMessage, colotTagFront, string.Empty);
            miniMessage = Regex.Replace(miniMessage, colotTagEnd, string.Empty);

            if (miniMessage.Length > length.value)
            {
                miniMessage = string.Format(shortenForm, miniMessage.Substring(0, length.value-1));
            }

            if(chatType.value == "otherClubChat")
            {
                bool error = false;
                miniMessage = StringTableUtils.GetString(StringTable.StringTableType.Global, "CONTENT_CHAT_MINI_CLUB_TEXT", out error, miniMessage);
            }

            saveAs.value = miniMessage;           
        }

        EndAction();
    }
}

}
