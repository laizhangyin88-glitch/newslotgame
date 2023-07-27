using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using System.Collections;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class LevelUpController : EventMonoSingleton<LevelUpController>
    {
        private Blackboard bb;

        private void Start()
        {
            bb = GetComponent<Blackboard>();
        }

//        public IEnumerator DebugInterruptingShopCoroutine()
//        {
//#if DEV
//            bool isActive = PlayerPrefs.GetInt("META_INTERRUPTING_SHOP", 0) == 1;
//            if (!isActive) yield break;

//            GameObject loadingObj = null;
//            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
//                (GameObject popupObj) => loadingObj = popupObj));

//            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
//            string asset = "Shop Scene";
//            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

//            GameObject shopObj = null;
//            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
//                (SceneLoadOperation sceneLoadOperation) => shopObj = sceneLoadOperation.GetScene()));

//            var shopBB = shopObj.GetComponent<Blackboard>();
//            BlackboardUtils.SetOrCreateValue(shopBB, "coinShopType", ShopType.COIN);
//            BlackboardUtils.SetOrCreateValue(shopBB, "gemShopType", ShopType.GEM);
//            BlackboardUtils.SetOrCreateValue(shopBB, "_openTabIndex", 0);

//            MetaPopupUtils.OpenPopup(shopObj);

//            MetaObjectUtils.SetCalleeCaller(shopObj, gameObject);

//            MetaPopupUtils.ClosePopup(loadingObj);

//            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
//            yield return new WaitUntilTrigger(callbackTrigger);
//#endif
//            yield break;
//        }

        public IEnumerator MetaFeatureUnlockCoroutine()
        {
            int currentLevel = BlackboardUtils.FindValue<int>(bb, "_currentLevel");
            int prevLevel = currentLevel - 1;

            // Check HOG
            bool isHogEnabled = BlackboardQueryUtils.IsHiddenObjectsActive();
            int minLevel = BlackboardQueryUtils.GetFeatureMinLevel(ClientModels.LockedFeatureType.HIDDEN_UNIVERSE);
            if (isHogEnabled && prevLevel < minLevel && minLevel <= currentLevel)
            {
                yield return StartCoroutine(ShowHiddenObjectsFeatureUnlockCoroutine());
            }
        }

        private IEnumerator ShowHiddenObjectsFeatureUnlockCoroutine()
        {
            string bundle = HiddenObjects.HiddenObjects.Defines.COMMON_BUNDLE;
            string asset = "Hidden Objects Chatting Unlocked";

            var inGameObj = GameObject.Find("In Game");
            var inGameElement = inGameObj.GetComponent<ContextElement>();
            var chattingMiniAreaElement = ContextUtils.FindElement(inGameElement, "In Game Bottom/Bet/Button Plus/Chatting Mini Area", ContextSearchingType.FullNameSearch);
            Transform parent = chattingMiniAreaElement.GetComponent<Transform>();

            var unlockObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            var unlockAnim = unlockObj.GetComponent<Animator>();
            unlockAnim.SetTrigger("Appear");

            yield return new WaitForSeconds(5.5f);

            Destroy(unlockObj);
        }

        public Blackboard GetMysteryGiftInfo()
        {
            return BlackboardQueryUtils.GetMysteryGiftInfo();
        }

        public void ClearMysteryGiftInfo()
        {
            BlackboardQueryUtils.ClearMysteryGiftInfo();
        }

        [Button]
        private void TestHiddenObjectsFeatureUnlock()
        {
            StartCoroutine(ShowHiddenObjectsFeatureUnlockCoroutine());
        }
    }
}
