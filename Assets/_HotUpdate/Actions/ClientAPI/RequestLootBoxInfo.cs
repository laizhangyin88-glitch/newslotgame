using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class RequestLootBoxInfo : ActionTask<Blackboard>
    {
        public BBParameter<ProbType> probType;
        public BBParameter<int> productID;
        public BBParameter<int> passiveEventID;

        public BBParameter<string> saveAsTitle;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        protected override string info
        {
            get
            {
                return string.Format("Request Loot Box Info ({0})", probType.value);
            }
        }
        protected override void OnExecute()
        {
            BagelCodeClientAPI.LootBoxInfo(probType.value, passiveEventID.value, productID.value,
            (response) =>
            {
                if (agent != null)
                {
                    for (int i = 0; i < response.probsInfo.probList.Count; ++i)
                    {
                        response.probsInfo.probList[i].prob *= 100.0;

                        // if(probType.value == ProbType.DAILY_BONUS_WHEEL
                        // || probType.value == ProbType.DAILY_MEGA_WHEEL)
                        //     response.probsInfo.probList[i].value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_LOOT_BOX_TEXT_COIN_VALUE", response.probsInfo.probList[i].value);
                    }

                    ClientAPI2Blackboard.Serialize(agent, response);

                    string title = "";
                    switch (probType.value)
                    {
                        case ProbType.DAILY_BONUS_WHEEL:
                            title = StringTableUtils.GetString(GLOBAL, "POPUP_LOOT_BOX_DAILY_SPIN_TITLE");
                            break;
                        case ProbType.CREDIT_MULTIPLIER_WHEEL:
                        case ProbType.GEM_BOOSTER:
                            title = StringTableUtils.GetString(GLOBAL, "POPUP_LOOT_BOX_COIN_MULTIPLIER_TITLE");
                            break;
                        case ProbType.DAILY_MEGA_WHEEL:
                            title = StringTableUtils.GetString(GLOBAL, "POPUP_LOOT_BOX_MEGA_WHEEL_TITLE");
                            break;
                        case ProbType.SPIN_BOOST:
                            title = StringTableUtils.GetString(GLOBAL, "POPUP_LOOT_BOX_SPIN_BOOST_TITLE");
                            break;
                        default:
                            saveAsTitle.value = "";
                            break;
                    }

                    if (!string.IsNullOrEmpty(title))
                    {
                        if (string.IsNullOrEmpty(response.probsInfo.message))
                        {
                            saveAsTitle.value = StringTableUtils.GetString(GLOBAL, "POPUP_LOOT_BOX_TITLE_DEFAULT_FORMAT", title);
                        }
                        else
                        {
                            saveAsTitle.value = StringTableUtils.GetString(GLOBAL, "POPUP_LOOT_BOX_TITLE_MESSAGE_FORMAT", title, response.probsInfo.message);
                        }
                    }
                    else
                    {
                        saveAsTitle.value = "";
                    }

                    EndAction(true);
                }
            },
            (error) =>
            {
                GlobalErrorHandler.GlobalError(error);
            });
        }
    }
}
