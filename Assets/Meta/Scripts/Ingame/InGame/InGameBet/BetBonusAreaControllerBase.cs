using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public abstract class BetBonusAreaControllerBase : MonoBehaviour
    {
        protected BetBonusControllerBase betBonusController;

        [Unity.Collections.ReadOnly]
        public string bundleName;
        [Unity.Collections.ReadOnly]
        public string assetName;

        private List<string> imageUrlList;
        private int requests;

        // protected

        protected abstract void GetPrefabName();

        protected void Start()
        {
            StartCoroutine(MakeControllerCoroutine());
        }

        // private

        private IEnumerator MakeControllerCoroutine()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();

            if (!IsCorrectEvent(eventInfo))
                yield break;

            var metaEnterInfoBB = BlackboardQueryUtils.GetMetaGameEnterInfo();
            if (metaEnterInfoBB != null)
                imageUrlList = BlackboardQueryUtils.GetMetaGameCommonWebImageList(eventInfo);
            else yield break;

            if (imageUrlList == null)
                yield break;

            // settings about controller
            bundleName = BlackboardQueryUtils.GetMetaBundleName(eventInfo);
            GetPrefabName();

            // download web image
            requests = imageUrlList.Count;
            for (int i = 0; i < imageUrlList.Count; ++i)
            {
                if (!string.IsNullOrEmpty(imageUrlList[i]))
                {
                    WebImageDownloader.Instance.LoadWebImage(
                        imageUrlList[i],
                        CacheType.FileCache,
                        true,
                        Succeed,
                        null,
                        null,
                        Failed
                    );
                }
            }
        }

        private bool IsCorrectEvent(EventInfo eventInfo)
        {
            if (eventInfo == null)
                return false;

            switch (eventInfo.type)
            {
                case EventInfoType.SEASON_PASS:
                    return true;
            }

            return false;
        }

        private void Succeed(string url)
        {
            CountRequests();
        }

        private void Failed(WebImageDownloader.WebImageDownloadError error)
        {
            CountRequests();
        }

        private void CountRequests()
        {
            if (--requests == 0)
                MakeController();
        }

        private void MakeController()
        {
            GameObject controllerGO = MetaObjectUtils.MakePrefab(bundleName, assetName, transform);

            if (controllerGO != null)
                betBonusController = controllerGO.GetComponent<BetBonusControllerBase>();

            if (betBonusController != null)
                betBonusController.betBonusAreaController = this;
            else Debug.LogWarning(controllerGO + " has not <BetBonusControllerBase> component.");
        }
    }
}