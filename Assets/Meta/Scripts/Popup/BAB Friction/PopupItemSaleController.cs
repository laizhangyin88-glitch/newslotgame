using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupItemSaleController : MonoBehaviour
    {
        public Blackboard rootBlackboard;

		public bool combineApplicationType;
        public string biContextID;

		private ContextElement shopListElement;
		private ContextElement tierIconElement;
		private List<ContextElement> shopItemElementList;

        private bool isInit = false;
        private bool isLoadedShopImage = false;
        private ContextElement shopImageElement;

		private Blackboard shopBB;

		private int meTier;

        public float defaultDelay = 0.5f;
        public float interval = 0.1f;

		private bool flipEffectEnalbed;

		private int shopID = 0;
		private bool isSendStoreBI = false;

		public void OnInitShop()
		{
			if (isInit) return;

			shopBB = BlackboardQueryUtils.GetShopBB(ShopType.GEM_BAB_PROMOTION);

			if (shopBB != null)
				shopID = shopBB.GetValue<int>("id");

			rootBlackboard = gameObject.GetComponent<Blackboard>();

			//Init context.
			var rootElement = gameObject.GetComponent<ContextElement>();
			rootElement.UpdateContext(false);

			shopListElement = ContextUtils.FindElement(rootElement, "Shop List", ContextSearchingType.ChildrenSearch);

			var closeButtonElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);
			MetaContextElementUtils.SetClickable(closeButtonElement, "OnCloseShop", rootElement, null);

            shopImageElement = ContextUtils.FindElement(rootElement, "Background Image", ContextSearchingType.ChildrenSearch);

			shopItemElementList = new List<ContextElement>();

			for (int i = 0; i < 3; ++i)
			{
                shopItemElementList.Add( ContextUtils.FindElement(shopListElement, string.Format("Shop Item {0}", i+1), ContextSearchingType.ChildrenSearch));
                MetaContextElementUtils.SetBlackboardValue<string>(shopItemElementList[i], "_biContextID", GetBIContextID());
			}

			isInit = true;
		}

		public void OnUpdateVaribales()
		{
			UpdateShopContents();

            MetaContextElementUtils.SetBlackboardValue(tierIconElement, "isRefresh", true);

			if (!isLoadedShopImage)
			{
				var imageUrlVariable = shopBB.GetVariable<string>("imageUrl");
				if (imageUrlVariable != null && !string.IsNullOrEmpty(imageUrlVariable.value))
				{
					string imageURL = shopBB.GetValue<string>("imageUrl");

					IContextImage imageElement = shopImageElement as IContextImage;

					imageElement.SetHash(imageURL.GetHashCode().ToString());

					WebImageDownloader.Instance.LoadWebImage(
						imageURL,
						CacheType.FileCache,
						true,
						ShopImageLoadSuccess,
						delegate(Sprite img)
						{
							if (img != null)
							{
								if (imageElement.CheckHash(imageURL.GetHashCode().ToString()))
								{
									imageElement.SetSprite(img);
								}
							}
						},
						null,
						ShopImageLoadFailed
					);
				}
			}
		}

		private void UpdateShop()
		{
			if (shopBB != null)
			{
				var productGroupList = shopBB.GetValue<List<Blackboard>>("productGroupList");

				for(int i = 0; i < shopItemElementList.Count; ++i)
				{
					shopItemElementList[i].gameObject.SetActive(true);

					var productList = BlackboardQueryUtils.GetProductList(productGroupList[i]);
					MetaContextElementUtils.SetBlackboardValue<List<Blackboard>>(shopItemElementList[i], "productList", productList);

					MetaContextElementUtils.SetBlackboardValue<GameObject>(shopItemElementList[i], "parentShop", gameObject);

                    shopItemElementList[i].gameObject.GetComponent<GraphOwner>().StartBehaviour();
                    MetaContextElementUtils.SendEvent(shopItemElementList[i], "OnUpdate", null, null);
                }
			}
		}

        public string InitBIContextID(string contextID)
        {
            if(string.IsNullOrEmpty(contextID))
                contextID = BiEventUtils.GenerateContextID();

            biContextID = contextID;
            return biContextID;
        }

        private string GetBIContextID()
        {
            if(string.IsNullOrEmpty(biContextID))
                biContextID = BiEventUtils.GenerateContextID();

            return biContextID;
        }

        private void UpdateShopContents()
        {
            UpdateShop();
        }
        
        private void ShopImageLoadSuccess(string url)
        {
            // loading False. 
            // Set Image. 
            isLoadedShopImage = true;
        }

        private void ShopImageLoadFailed(WebImageDownloader.WebImageDownloadError error)
        {
            // Refresh??
        }

        private string GetBundleName(string bundleName)
        {
            return combineApplicationType ? ApplicationSettings.MakeApplicationBundleName(bundleName) : bundleName;
        }
    }
}
