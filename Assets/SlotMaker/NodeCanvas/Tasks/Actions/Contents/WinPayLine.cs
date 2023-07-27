using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class WinPayLine : ActionTask
{
    public BBParameter<int> column;
    public BBParameter<int> row;

    public BBParameter<SymbolWin> win;
    public BBParameter<List<Blackboard>> payLines;
    public BBParameter<List<Animator>>   payLineObjs;

    public bool enable;

    public List<int> GetPayLine(int lineIndex)
    {
        return payLines.value[lineIndex].GetValue<List<int>>("value");
    }

    protected override void OnExecute()
    {
        if (win == null || win.value == null)
        {
            EndAction(false);
        }
        else
        {
            List<int> payLine = GetPayLine((int)win.value.lineIndex);
            for (int i = 0; i < column.value; ++i)
            {
                int index = payLine[i] * row.value + i;
                payLineObjs.value[index].SetBool("enable", enable);
            }
            EndAction();
        }
    }
}

}
