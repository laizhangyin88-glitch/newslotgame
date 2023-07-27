using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode
{

public static partial class BlackboardQueryUtils
{
    public static Blackboard FindShopFromBBList(List<Blackboard> shopListBB, string shopName)
    {
        for(int i = 0; i < shopListBB.Count; ++i)
        {
            if(shopName == shopListBB[i].GetValue<string>("name"))
            {
                return shopListBB[i];
            }
        }

        return null;
    }

    public static Blackboard FindItemFromBBList(List<Blackboard> itemListBB, string itemID)
    {
        for(int i = 0; i < itemListBB.Count; ++i)
        {
            Blackboard itemBB = itemListBB[i];
            if(itemID == itemBB.GetValue<string>("itemId"))
            {
                return itemBB;
            }
        }

        return null;
    }
}

}

