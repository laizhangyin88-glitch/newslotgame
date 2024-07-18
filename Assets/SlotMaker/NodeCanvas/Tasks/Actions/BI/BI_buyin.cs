using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

using SlotMaker.Json;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_buyin : ActionTask<Blackboard>
    {
        public BBParameter<string> product;

        private List<string> productIDList = new List<string>();

        private const string dataKey = "buyin_product_list";

        protected override void OnExecute()
        {
            var game = BlackboardUtils.FindVariable<Blackboard>(agent, "./game");

            if(product.value != null)
            {
                if( PlayerPrefs.HasKey(dataKey) )
                {
                    string loadProductIDSet = PlayerPrefs.GetString(dataKey);
                    productIDList = SlotSimpleJson.DeserializeObject<List<string>>(loadProductIDSet);
                }

                var id = BlackboardUtils.FindVariable(agent, product.value + "/id");
                productIDList.Add(id.value.ToString());

                string serializedProductIDListSet = SlotSimpleJson.SerializeObject(productIDList);
                PlayerPrefs.SetString(dataKey, serializedProductIDListSet);
            }
            else
            {
                if( PlayerPrefs.HasKey(dataKey) )
                {
                    string loadProductIDSet = PlayerPrefs.GetString(dataKey);
                    productIDList = SlotSimpleJson.DeserializeObject<List<string>>(loadProductIDSet);

                    int gameId = game.value.GetValue<int>("gameId");

                    for (int i = 0; i < productIDList.Count; ++i)
                    {
                        Analytics.buyin(gameId, productIDList[i]);
                    }

                    PlayerPrefs.DeleteKey(dataKey);
                }
            }
            
            EndAction();
        }
    }
}
