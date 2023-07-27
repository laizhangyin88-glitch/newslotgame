using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Task.Actions
{

[Category("★ SlotMaker/Popup")]
public class OpenCommonPopup : ActionTask<Transform>
{
    public BBParameter<GameObject> popup;

    public BBParameter<string> yesButtonEvent;
    public BBParameter<string> noButtonEvent;
    public BBParameter<string> xButtonEvent;
    public BBParameter<string> BackButtonEvent;

    public BBParameter<string> title;
    public BBParameter<string> text;

    public BBParameter<string> yesButtonText;
    public BBParameter<string> noButtonText;

    public BBParameter<bool> autoCloseYes;
    public BBParameter<bool> autoCloseNo;
    public BBParameter<bool> autoCloseX;

    public BBParameter<bool> useXButton;
    public BBParameter<bool> useBackButton;

    private const string OWNER = "owner";
    private const string EVENT_BUTTON_YES = "eventButtonYes";
    private const string EVENT_BUTTON_NO = "eventButtonNo";
    private const string EVENT_BUTTON_X = "eventButtonX";
    private const string EVENT_BUTTON_BACK = "eventButtonBack";
    private const string TITLE = "title";
    private const string TEXT = "text";
    private const string BUTTON_YES_TEXT = "buttonYesText";
    private const string BUTTON_NO_TEXT = "buttonNoText";
    private const string AUTO_CLOSE_YES = "autoCloseYes";
    private const string AUTO_CLOSE_NO = "autoCloseNo";
    private const string AUTO_CLOSE_X = "autoCloseX";
    private const string USE_CLOSE_BUTTON = "useCloseButton";
    private const string USE_BACK_BUTTON = "useBackButton";

    protected override string info
    {
        get { return "Open " + popup + " Popup"; }
    }

    protected override void OnExecute()
    {
        if (popup.value == null) return;

        // Debug.Log( string.Format("OpenCommonPopup Title : {0}, Text : {1}", title.value, text.value) );

        // BB Data Setting.
        Blackboard bb = popup.value.GetComponent<Blackboard>();

        bb.SetValue(OWNER, agent.transform);

        bb.SetValue(EVENT_BUTTON_YES, yesButtonEvent.value);
        bb.SetValue(EVENT_BUTTON_NO, noButtonEvent.value);
        bb.SetValue(EVENT_BUTTON_X, xButtonEvent.value);
        bb.SetValue(EVENT_BUTTON_BACK, BackButtonEvent.value);

        bb.SetValue(TITLE, title.value);
        bb.SetValue(TEXT, text.value);

        bb.SetValue(BUTTON_YES_TEXT, yesButtonText.value);
        bb.SetValue(BUTTON_NO_TEXT, noButtonText.value);

        bb.SetValue(AUTO_CLOSE_YES, autoCloseYes.value);
        bb.SetValue(AUTO_CLOSE_NO, autoCloseNo.value);
        bb.SetValue(AUTO_CLOSE_X, autoCloseX.value);

        bb.SetValue(USE_CLOSE_BUTTON, useXButton.value);
        bb.SetValue(USE_BACK_BUTTON, useBackButton.value);

        PopupManager.Instance.Open(popup.value);

        EndAction();
    }
}

}
