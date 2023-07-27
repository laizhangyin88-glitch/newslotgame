using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Services;
using SlotMaker;
using SlotMaker.Json;

namespace BagelCode
{
    public class ThirdPartyAnalyticsManager
    {
        private static List<int> revenueSection = new List<int>() {1, 3, 5, 6, 7, 8};

        public static void UpdateRevenueSection(List<int> updateRevenueSection)
        {
            if(updateRevenueSection == null) return;
            revenueSection = updateRevenueSection;
        }

        public static void SendPurchaseEvent(Blackboard productBB, Blackboard purchaseResponseBB)
        {
            AdjustManager.Instance.SendPurchaseEvent(productBB, purchaseResponseBB);

            double productPrice = productBB.GetValue<double>("price");
            double lifetimeSpend = purchaseResponseBB.GetValue<double>("lifetimeSpend");

            SendConversionValueEvent(productPrice, lifetimeSpend);
        }

        private static void SendConversionValueEvent(double revenue, double lifetimeSpend)
        {
            // revenue is price
            // lifetimeSpend is user total purchases

            int revenueNum = (int)(revenue+0.5);
            int lifetimeSpendNum = (int)(lifetimeSpend+0.5);
            int prevLifetimeSpendNum = lifetimeSpendNum - revenueNum;

            if(revenueSection.Count <= 0 || revenueSection[revenueSection.Count-1] <= prevLifetimeSpendNum)
            {
                // conversion value over.
                return;
            }

            int section = 0;
            int prevSection = 0;
            for(int i=0; i < revenueSection.Count; ++i)
            {
                if(prevLifetimeSpendNum >= revenueSection[i])
                    prevSection = i;

                if(lifetimeSpendNum >= revenueSection[i])
                    section = i;
                else
                    break;
            }

            // ignore first purchase.
            if(prevLifetimeSpendNum == 0 || section > prevSection)
            {
                string eventID = string.Format("purchase_{0}", section+1);
                AdjustManager.Instance.SendEvent(eventID);
            }
            else
            {
                // same prev conversion section
            }

        }
    }
}
