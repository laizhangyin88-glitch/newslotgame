using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;
using System.Text.RegularExpressions;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Utils")]
public class GetTruncatedString : ActionTask
{
    public BBParameter<string> valueA;
    public BBParameter<int>    length;
    public BBParameter<int>    dotCount = 3;

    protected override string info
    {
        get { return string.Format("Get Truncated String {0} to {1}", valueA, length); }
    }

    protected override void OnExecute()
    {
        if (valueA.value.Length > length.value)
        {
            string formatString = "{0}"; 
            for (int i = 0; i < dotCount.value; ++i)
            {
                formatString += ".";
            }
            valueA.value = string.Format(formatString, valueA.value.Substring(0, length.value-1));
        }

        EndAction();
    }
}

}
