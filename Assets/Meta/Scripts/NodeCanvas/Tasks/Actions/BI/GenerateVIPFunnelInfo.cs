using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace BagelCode.Tasks.Actions.BI
{

    [Category("★ BagelCode/BI")]
    [Description("存储到PlayerPrefs\nVIP_CLUB_FUNNEL_TYPE <- type(int)\nVIP_CLUB_FUNNEL_CONTEXT_ID <- new GUID")]
    public class GenerateVIPFunnelInfo : ActionTask<Blackboard>
    {
        public BBParameter<BI_client_vip_club_funnel.BIVIPClubFunnelType> type;

        protected override string info
        {
            get { return "Generate VIP Funnel Info : " + type; }
        }
        
        protected override void OnExecute()
        {
            PlayerPrefs.SetInt("VIP_CLUB_FUNNEL_TYPE", (int)type.value);
            PlayerPrefs.SetString("VIP_CLUB_FUNNEL_CONTEXT_ID", BiEventUtils.GenerateContextID());
            
            EndAction();
        }
    }

}
