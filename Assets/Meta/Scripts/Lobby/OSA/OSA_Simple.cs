using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;
using frame8.Logic.Misc.Other;
using frame8.Logic.Misc.Other.Extensions;
using frame8.Logic.Misc.Visual.UI;
using Com.ForbiddenByte.OSA.Core;

namespace BagelCode.OSA_Scroll
{
    public class OSA_Simple : OSA<BaseParams, BaseItemViewsHolder>
    {
        public SceneInfoObject scene;
        public GameObject prefab;
        public List<Blackboard> data;
        public string fieldName;

        protected GameObject Prefab
        {
            get 
            {
                if (prefab == null)
                {
                    prefab = SceneManager.LoadScene(transform, scene.GetSceneInfo());
                    prefab.SetActive(false);
                }
                return prefab;
            }
        }

        protected override void Start()
        {
            base.Start();

            ResetItems(data.Count);
        }

        protected override BaseItemViewsHolder CreateViewsHolder(int itemIndex)
        {
            var item = new BaseItemViewsHolder();
            item.Init(Prefab, _Params.Content, itemIndex);
            return item;
        }

        protected override void UpdateViewsHolder(BaseItemViewsHolder newOrRecycled)
        {
            newOrRecycled.root.GetComponent<Blackboard>().SetValue(fieldName, data[newOrRecycled.ItemIndex]);
            newOrRecycled.root.GetComponent<GraphOwner>().StartBehaviour();
        }
    }
}
