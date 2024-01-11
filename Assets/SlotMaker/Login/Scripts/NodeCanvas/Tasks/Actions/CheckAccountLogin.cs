using BagelCode;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;


namespace NodeCanvas.Tasks.Actions
{

    [Category("★ BagelCode/AccountLogin")]
    public class CheckAccountLogin : ActionTask
    {
        protected override void OnExecute()
        {
            if (ApplicationSettings.Instance.accountLogin)
            {
                EventSender.SendGlobalEvent("OnAccountLogin");
            }
            else
            {
                EventSender.SendGlobalEvent("OnDefaultLogin");
            }
                
        }
    }
}
