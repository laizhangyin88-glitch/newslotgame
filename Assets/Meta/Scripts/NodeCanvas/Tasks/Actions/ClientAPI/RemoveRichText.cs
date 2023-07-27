using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using System.Text.RegularExpressions;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RemoveRichText : ActionTask
{
	public BBParameter<string> text;

	protected override void OnExecute()
	{
        string pattern = "\\<.*?\\>";

        if (Regex.IsMatch(text.value, pattern))
        {
            text.value = Regex.Replace(text.value, pattern, "", RegexOptions.None);
        }

        EndAction();
    }
}

}
