using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace BagelCode.Tasks.Actions.BI
{

    [Category("★ BagelCode/BI")]
    public class ClearVIPFunnelInfo : ActionTask<Blackboard>
    {
        protected override string info
        {
            get { return "Clear VIP Funnel Info"; }
        }
        
        protected override void OnExecute()
        {
            if (PlayerPrefs.HasKey("VIP_CLUB_FUNNEL_TYPE"))
                PlayerPrefs.DeleteKey("VIP_CLUB_FUNNEL_TYPE");

            if (PlayerPrefs.HasKey("VIP_CLUB_FUNNEL_CONTEXT_ID"))
                PlayerPrefs.DeleteKey("VIP_CLUB_FUNNEL_CONTEXT_ID");
            
            EndAction();
        }
    }

}
