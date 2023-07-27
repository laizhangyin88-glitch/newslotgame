using System;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/StatusMatch")]

public class GetStatusMatchResultBB : ActionTask<Blackboard>
{
    public BBParameter<string> titleText;
    public BBParameter<string> contentText;
    public BBParameter<string> buttonText;

    protected override string info
    {
        get { return "Get StatusMatch Result BB"; }
    }

    protected override void OnExecute()
    {
        StatusMatchResult result = BlackboardUtils.FindVariable<StatusMatchResult>(null, "/statusMatchResult").value;
        int tier = BlackboardUtils.FindVariable<int>(null, "/me/tier").value;
        switch (result)
        {
            case StatusMatchResult.NEWLY_OFFERED:
            break;
            case StatusMatchResult.PASS:
                titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_RESULT_POPUP_PASS_TITLE_TEXT");
                buttonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_RESULT_POPUP_PASS_BUTTON_TEXT");
                contentText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_RESULT_POPUP_PASS_CONTENT_TEXT", tier);
            break;
            case StatusMatchResult.FAIL:
                titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_RESULT_POPUP_FAIL_TITLE_TEXT");
                buttonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_RESULT_POPUP_FAIL_BUTTON_TEXT");
                contentText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_RESULT_POPUP_FAIL_CONTENT_TEXT");
            break;
            case StatusMatchResult.FAIL_ID:
                titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_RESULT_POPUP_FAIL_TITLE_TEXT");
                buttonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_RESULT_POPUP_FAIL_BUTTON_TEXT");
                contentText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_RESULT_POPUP_FAIL_ID_CONTENT_TEXT");
            break;
            case StatusMatchResult.FAIL_IMAGE:
                titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_RESULT_POPUP_FAIL_TITLE_TEXT");
                buttonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_RESULT_POPUP_FAIL_BUTTON_TEXT");
                contentText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_RESULT_POPUP_FAIL_IMAGE_CONTENT_TEXT");
            break;

            default: // no_result or unknown
            break;
        }

        EndAction();
    }

}

}
