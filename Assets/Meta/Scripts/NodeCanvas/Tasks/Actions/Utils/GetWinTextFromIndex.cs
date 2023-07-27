using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Action
{

[Category("★ BagelCode/Utils")]
public class GetWinTextFromIndex : ActionTask
{
    // 0 : big , 1 : super big, 2 : mega, 3 : super mega, 4 : epic
    public BBParameter<int> winType;
    public BBParameter<bool> toUpper;

    public BBParameter<string> saveAs;

    private static readonly string[] BIG_WIN_TYPES = new string[]{ "WIN_TEXT_BIG", "WIN_TEXT_SUPER_BIG", "WIN_TEXT_MEGA", "WIN_TEXT_SUPER_MEGA", "WIN_TEXT_EPIC" };
    private const string WIN_TEXT_FORMAT = "WIN_TEXT_FORMAT";

    protected override string info
    {
        get
        {
            return string.Format("{0} = Get Text from index({1})", saveAs, winType);
        }
    }

    protected override void OnExecute()
    {
        if(winType != null)
        {
            if(winType.value < 0)
                winType.value = 0;
            if(winType.value > BIG_WIN_TYPES.Length)
                winType.value = BIG_WIN_TYPES.Length;

            saveAs.value = StringTableUtils.GetString(StringTable.StringTableType.Global, BIG_WIN_TYPES[winType.value]);
            if(!string.IsNullOrEmpty(saveAs.value) && toUpper.value)
                saveAs.value = saveAs.value.ToUpper();
        }
        else
        {
            saveAs.value = "";
        }
        

        EndAction();
    }
}

}
