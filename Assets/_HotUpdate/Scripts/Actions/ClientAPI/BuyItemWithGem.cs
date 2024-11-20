using System.Collections.Generic;
using BagelCode.ClientModels;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class BuyItemWithGem : ActionTask <Blackboard>
{
    public BBParameter<string> valueA;
    public BBParameter<int> userGroupID;
    public BBParameter<int> metaGameEventID;
    public BBParameter<string> contextID;

    [BlackboardOnly]
    public BBParameter<bool> isSuccess;

    protected override string info
    {
        get
        {
            return string.Format("Request Buy Item {0} with Gem", valueA);
        }
    }

    protected override void OnExecute()
    {
        if(BlackboardQueryUtils.CheckPurchaseProhibitedRegion())
        {
            isSuccess.value = false;
            EndAction(true);
            return;
        }

        Purchase();
    }

    private void Purchase()
    {
        var productId = BlackboardUtils.FindVariable<int>(agent, string.Format("{0}/id", valueA.value) );

        var gemPrice = BlackboardUtils.FindVariable<long>(agent, string.Format("{0}/gemPrice", valueA.value) );
        var iamIDVariable = BlackboardUtils.FindVariable<int>(agent, "iamId");
        int iamID = iamIDVariable == null ? 0 : iamIDVariable.value;
        int userGroupID = this.userGroupID.value;
        int metaGameEventID = this.metaGameEventID.value;
        var iamTriggerTypeVaraible = BlackboardUtils.FindVariable<InAppMessageTriggerType>(agent, "triggerType");
        string iamTriggerType = iamTriggerTypeVaraible == null ? "" : iamTriggerTypeVaraible.value.ToString();

        if (productId != null)
        {
            RequestBuyItem( productId.value,
                            gemPrice.value,
                            iamID,
                            userGroupID,
                            metaGameEventID,
                            iamTriggerType);
        }
        else
        {
            Debug.LogError("No ItemId found." + agent.gameObject.name);
        }
    }

    private void RequestBuyItem(int productID,
                                long gemPrice,
                                int iamID,
                                int userGroupID,
                                int metaGameEventID,
                                string iamTriggerType)
    {
        BagelCodeClientAPI.BuyItemWithGem( productID, iamID, userGroupID, metaGameEventID, iamTriggerType, contextID.value,
        (response) =>
        {
            ClientItemAcquired();

            BlackboardUtils.DestroyBlackboard(MainBlackboard.Get(), "purchaseResponse");
            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "purchaseResponse");

            ClientAPI2Blackboard.Serialize(bb, response);
            BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
            BlackboardQueryUtils.SpentGems(gemPrice);
            BlackboardQueryUtils.ApplyPurchaseItems(response.itemUseResultList, response.userSyncInfo);

            BlackboardUtils.SetOrCreateValue<int>(bb, "productID", productID);

            if(agent != null)
            {
                isSuccess.value = true;
                EndAction(true);
            }
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.DAILY_BOOST_ALREADY_EXIST_ERROR:
                    {
                        bool stringError = false;
                        ErrorPopupInfo info = new ErrorPopupInfo();

                        info.type = ErrorPopupType.OK;
                        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DAILY_BOOST_ALREADY_EXIST", out stringError);
                        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                        ErrorPopupHandler.Instance.OpenError(info);

                        if(agent != null)
                        {
                            isSuccess.value = false;
                            EndAction(true);
                        }
                    }
                    break;
                case ClientModels.Error.ALREADY_USED_SUBSCRIPTION_ERROR:
                    {
                        bool stringError = false;
                        ErrorPopupInfo info = new ErrorPopupInfo();

                        info.type = ErrorPopupType.OK;
                        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_ALREADY_USED_SUBSCRIPTION_ERROR", out stringError);
                        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                        ErrorPopupHandler.Instance.OpenError(info);

                        if(agent != null)
                        {
                            isSuccess.value = false;
                            EndAction(true);
                        }
                    }
                    break;
                case ClientModels.Error.NOT_ENOUGH_GEM_ERROR:
                {

                }
                    break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }

    private void ClientItemAcquired()
    {
        if(agent == null) return;

        var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, valueA.value);

        BiEventUtils.ItemAcquired(productBB.value, contextID.value, false);
    }
}

}
