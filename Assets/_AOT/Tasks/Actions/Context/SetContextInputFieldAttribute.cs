using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using TMPro;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextInputFieldAttribute : ActionTask<ContextElement>
{
    public BBParameter<int> characterLimit;
    public InputField.ContentType contentType;

    protected override string info
    {
        get { return string.Format("{0}.chracterLimit = {1}; .contentType = {2}", agentInfo, characterLimit, contentType); }
    }

    protected override void OnExecute()
    {
        bool error = true;
        IContextInputField textElement = agent as IContextInputField;
        if(textElement != null) {
            if (characterLimit != null && characterLimit.value > 0) {
                textElement.characterLimit = characterLimit.value;
            }
            textElement.contentType = contentType;
            error = false;
        }
        EndAction(!error);
    }
}

}
