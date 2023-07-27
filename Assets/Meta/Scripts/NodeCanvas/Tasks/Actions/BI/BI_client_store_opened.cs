using UnityEngine;
using System.Text;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_store_opened : ActionTask<Blackboard>
{
    public BBParameter<string> shopTypeValue;
    public BBParameter<string> shopIDValue;

    public BBParameter<string> contextID;
    public BBParameter<string> typeValue;

    private const string SHOP_OPENED_FROM_TYPE = "SHOP_OPENED_FROM_TYPE";
    private const string TYPE_DEFAULT = "default";

    protected override void OnExecute()
    {
        if(string.IsNullOrEmpty(contextID.value))
            contextID.value = BiEventUtils.GenerateContextID();

        var shopType = BlackboardUtils.FindVariable<ShopType>(agent, shopTypeValue.value);
        var shopID = BlackboardUtils.FindVariable<int>(agent, shopIDValue.value);

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["shop_type"] = shopType.value.ToString();
        customData["shop_id"] = shopID.value;
        customData["context_id"] = contextID.value;

        var eventInfo = PassiveEventUtils.GetPassiveEvent(shopType.value);
        customData["shop_event_flag"] = eventInfo == null ? false : true;

        if(PlayerPrefs.HasKey(SHOP_OPENED_FROM_TYPE))
        {
            string openType = PlayerPrefs.GetString(SHOP_OPENED_FROM_TYPE);

            if(!string.IsNullOrEmpty(openType))
                customData["open_type"] = openType;
            else
                customData["open_type"] = TYPE_DEFAULT;

            PlayerPrefs.DeleteKey(SHOP_OPENED_FROM_TYPE);
        }
        else
        {
            customData["open_type"] = TYPE_DEFAULT;
        }
        BiEventUtils.AppendLevelMultiplierEventData(customData, (typeValue == null || string.IsNullOrEmpty(typeValue.value)) ? "coin" : typeValue.value);
        Analytics.CustomEvent("client_store_opened", customData);
        AdjustManager.Instance.SendEvent("store_opened");

        EndAction();
    }
}

}
