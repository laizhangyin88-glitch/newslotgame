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
public class FilterColorTag : ActionTask<ContextElement>
{
    public BBParameter<string> target;
    public BBParameter<string> toColor; // #000000 etc...

    protected override string info
    {
        get { return string.Format("filter color tag to {0}", toColor.value); }
    }

    protected override void OnExecute()
    {
        if (string.IsNullOrEmpty(toColor.value))
        {
            target.value = Regex.Replace(target.value, "(?:<color=)(.*?)(?:>)", "");
            target.value = Regex.Replace(target.value, "</color>", "");
        }
        else
        {
            string replaceColor = string.Format("<color={0}>", toColor.value);
            target.value = Regex.Replace(target.value, "(?:<color=)(.*?)(?:>)", replaceColor);
        }

        EndAction();
    }
}

}
