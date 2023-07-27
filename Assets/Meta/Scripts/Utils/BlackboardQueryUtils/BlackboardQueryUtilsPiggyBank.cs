using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{

public static partial class BlackboardQueryUtils
{
    const string potOfGoldProductListKey = "piggyBankProductList";

    public static void UpdatePiggyBankProduct(Product newProduct)
    {
        BlackboardUtils.DestroyBlackboard(MainBlackboard.Get(), "piggyBankProduct");
        var productBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "piggyBankProduct");
        ClientAPI2Blackboard.Serialize(productBB, newProduct);
    }

    public static void UpdatePiggyBankCoin(long piggyCoin)
    {
        var piggyCoinVar = BlackboardUtils.GetOrCreateVariable<long>( MainBlackboard.Get(), "me/piggyCredit");

        piggyCoinVar.value = piggyCoin;
    }

    public static void UpdatePotOfGoldProductList(List<Product> newProductList)
    {
        BlackboardUtils.SetOrCreateList<Product>(MainBlackboard.Get(), potOfGoldProductListKey, newProductList, ClientAPI2Blackboard.Serialize);
    }

    public static Blackboard GetPotOfGoldProduct(int index)
    {
        var productList = BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), potOfGoldProductListKey);

        if(productList != null && productList.Count > index)
            return productList[index];

        return null;
    }

    public static int GetPotOfGoldSalePercent(int index)
    {
        Blackboard productBB = GetPotOfGoldProduct(index);

        if(productBB != null)
        {
            return GetProductSalePercent(productBB);
            // double price = BlackboardUtils.FindVariable<double>(productBB, "price").value;
            // double origPrice = BlackboardUtils.FindVariable<double>(productBB, "originalPrice").value;

            // if(price < origPrice)
            //     return System.Convert.ToInt32(((1.0 -(price/origPrice)) * 100.0));
        }

        return 0;
    }
}

}

