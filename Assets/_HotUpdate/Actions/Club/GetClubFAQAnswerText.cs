using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Club")]

public class GetClubFAQAnswerText : ActionTask<Blackboard>
{
    public BBParameter<int> index;

    public BBParameter<string> saveText;

    private const string faqFormat = "CLUB_FAQ_ANSWER_{0}";

    protected override string info
    {
        get { return "Get Club FAQ Answer Text"; }
    }

    protected override void OnExecute()
    {
        bool error = false;
        string textKey = string.Format(faqFormat, index.value);

        if(index.value == 2)
        {
            var creationCost = BlackboardUtils.FindVariable(null, "/values/club/CREATION_COST");
            saveText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, textKey, creationCost.value, out error);
        }
        else if(index.value == 4)
        {
            var maxLevel = BlackboardUtils.FindVariable(null, "/values/club/LEVEL/MAX_LEVEL");
            saveText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, textKey, maxLevel.value, out error);
        }
        else
        {
            saveText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, textKey, out error);
        }

        

        EndAction();
    }
}

}
