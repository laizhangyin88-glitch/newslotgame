using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using ParadoxNotion;
using BagelCode.ClientModels;
using BagelCode.Protobuf;

namespace BagelCode
{
    public class PassiveEventUtils
    {
        public static EventInfo GetPassiveEvent(ShopType shopType)
        {
            EventInfo eventInfo = null;

            switch(shopType)
            {
                case ShopType.COIN:
                case ShopType.COIN_WITH_META_GAME:
                case ShopType.COIN_WITH_BOSS_RAIDERS:
                case ShopType.COIN_WITH_CLUB_ARENA:
                    {
                        eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.COIN_SHOP_EVENT_MULTIPLY);
                    }
                    break;
                case ShopType.GEM:
                case ShopType.GEM_WITH_META_GAME:
                case ShopType.GEM_WITH_BOSS_RAIDERS:
                case ShopType.GEM_WITH_CLUB_ARENA:
                    {
                        eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_SHOP_EVENT_MULTIPLY);
                    }
                    break;
                case ShopType.DAILY_BOOST:
                    break;
                case ShopType.PIGGY_BANK:
                    break;
                case ShopType.DAILY_BONUS:
                    break;
                case ShopType.TIER_BOOST:
                    break;
                case ShopType.TIER_UP:
                    {
                        eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.TIER_UP_SHOP_EVENT_MULTIPLY);
                    }
                    break;
                case ShopType.VOUCHER:
                    break;
                case ShopType.ACTION:
                    break;
                case ShopType.ALL_IN_BONUS:
                    break;
                case ShopType.MEGA_WHEEL:
                    break;
                case ShopType.VIP_DEAL:
                    break;
                case ShopType.POG_BOOSTER:
                    break;
                case ShopType.COIN_BOOSTER:
                    break;
                case ShopType.GEM_BAB:
                    break;
                case ShopType.GEM_BOOSTER:
                    break;
                case ShopType.GEM_BAB_PROMOTION:
                    break;
            }

            return eventInfo;
        }
    }
}
