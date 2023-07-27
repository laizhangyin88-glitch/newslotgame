using UnityEngine;
using System.Collections;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using BagelCode;

namespace SlotMaker.Task.Actions
{

// todo. full intergration?
[Category("★ SlotMaker/Popup")]
public class OpenCommonErrorPopup : ActionTask<Transform>
{
    public BBParameter<string> textString;
    public BBParameter<string> titleString;
    public BBParameter<string> buttonString;
    public BBParameter<bool> useXbutton;


    protected override string info
    {
        get { return "Open Common Error Popup"; }
    }

    protected override void OnExecute()
    {
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.OK;
        info.title = titleString.value;
        info.text = textString.value;
        info.buttonText1 = buttonString.value;
        info.useXButton = useXbutton.value;
        info.callback1 = CallbackErrorPopup;

        ErrorPopupHandler.Instance.OpenError(info);
    }

    private void CallbackErrorPopup()
    {
        EndAction();
    }
}

}
