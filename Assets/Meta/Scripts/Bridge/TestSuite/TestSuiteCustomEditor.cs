using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NodeCanvas.Framework;
using SlotMaker;
using SlotMaker.TestSuite;

namespace BagelCode
{
	public class TestSuiteCustomEditor : MonoBehaviour
	{
        public Sprite[] purchaseModes = new Sprite[2];
        public Sprite[] purchaseCrashModes = new Sprite[2];

        public Image purchaseModeSprite;
        public Image purchaseCrashModeSprite;

        private const string DEBUG_PURCHASE = "DEBUG_PURCHASE";
        private const string DEBUG_PURCHASE_CRASH = "DEBUG_PURCHASE_CRASH";

        private static GameObject safeAreaObj = null;

        public void Awake()
        {
            int debugPurchaseIndex = PlayerPrefs.GetInt(DEBUG_PURCHASE, 1);
            int debugPurchaseCrashIndex = PlayerPrefs.GetInt(DEBUG_PURCHASE_CRASH, 1);

            purchaseModeSprite.sprite = purchaseModes[debugPurchaseIndex];
            purchaseCrashModeSprite.sprite = purchaseCrashModes[debugPurchaseCrashIndex];
        }

		public void PlayVideoAds()
        {
            var placementKey = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "videoAdsPlacementNames/timebonus").value;
            if(BagelCode.VideoAdsController.Instance.IsVideoAdsAvailable(placementKey))
            {
                BagelCode.VideoAdsController.Instance.ShowRewardedVideo(placementKey, null);
            }
            else
            {
                Debug.LogError(string.Format("Video is not available.({0})", placementKey));
            }
        }

        public void TogglePurchaseMode()
        {
            // 1 == dev purchase
            int index = PlayerPrefs.GetInt(DEBUG_PURCHASE, 1);
            PlayerPrefs.SetInt(DEBUG_PURCHASE, index == 1 ? 0 : 1);
            purchaseModeSprite.sprite = purchaseModes[index == 1 ? 0 : 1];
        }

        public void TogglePurchaseCrashMode()
        {
            // 1 == crash off
            int index = PlayerPrefs.GetInt(DEBUG_PURCHASE_CRASH, 1);
            PlayerPrefs.SetInt(DEBUG_PURCHASE_CRASH, index == 1 ? 0 : 1);
            purchaseCrashModeSprite.sprite = purchaseCrashModes[index == 1 ? 0 : 1];
        }

        public void CheckSlotImages()
        {
            var gameInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/gameInfoList");
            if(gameInfoList != null && gameInfoList.value.Count > 0)
            {
                TestSuiteManager.Instance.ClosePopup();
                TestSuiteManager.Instance.LoadGameObject("Check Thumbnail Popup");
            }
            else
            {
                Debug.LogError("Retry. from lobby.");
            }
        }

        public void ToggleSafeArea()
        {
            if(safeAreaObj == null)
            {
                safeAreaObj = MetaObjectUtils.MakePrefab("testsuite", "Dev Safe Area", null, "Main Canvas");
            }
            else
            {
                GameObject.Destroy(safeAreaObj);
                safeAreaObj = null;
            }
        }

        public void OpenDevPopup()
        {
            var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>("testsuite", "Popup Dev Editor Scene").GetSceneInfo();
            GameObject go = SceneManager.LoadScene(PopupManager.Instance.transform, sceneInfo);
            go.name = "Popup Dev Editor";

            PopupManager.Instance.Open(go);
            TestSuiteManager.Instance.ClosePopup();
        }

        public void OpenScratcherSymbolListPopup()
        {
            var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>("testsuite", "Popup Dev Scratcher Scene").GetSceneInfo();
            GameObject go = SceneManager.LoadScene(PopupManager.Instance.transform, sceneInfo);
            go.name = "Popup Dev Scratcher";

            PopupManager.Instance.Open(go);
            TestSuiteManager.Instance.ClosePopup();
        }

        public void OpenMetaDebugSpin()
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>("testsuite", "Meta DebugSpin");
            var go = GameObject.Instantiate(prefab) as GameObject;
            go.name = "Meta DebugSpin";
            go.transform.SetParent(PopupManager.Instance.transform, false);

            TestSuiteManager.Instance.ClosePopup();
        }

        public void OpenGemJackpotDebug()
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>("testsuite", "GemJackpot Debug");
            var go = GameObject.Instantiate(prefab) as GameObject;
            go.name = "GemJackpot Debug";
            go.transform.SetParent(PopupManager.Instance.transform, false);

            TestSuiteManager.Instance.ClosePopup();
        }

        public void OpenHiddenObjectDebug()
        {
            var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>("testsuite", "Popup Dev Editor Hidden Objects Scene").GetSceneInfo();
            GameObject go = SceneManager.LoadScene(PopupManager.Instance.transform, sceneInfo);
            go.name = "Popup Dev Editor Hidden Objects";

            PopupManager.Instance.Open(go);
            TestSuiteManager.Instance.ClosePopup();
        }

        public void OpenOtherMetaGame()
        {
            var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>("testsuite", "Popup Dev Editor Other Meta Game Scene").GetSceneInfo();
            GameObject go = SceneManager.LoadScene(PopupManager.Instance.transform, sceneInfo);
            go.name = "Popup Dev Editor Other Meta Game";

            PopupManager.Instance.Open(go);
            TestSuiteManager.Instance.ClosePopup();
        }

        public void OpenBossRaidersDealDebug()
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>("testsuite", "BossRaiders Deal DebugSpin");
            var go = GameObject.Instantiate(prefab) as GameObject;
            go.name = "BossRaiders Deal DebugSpin";
            go.transform.SetParent(PopupManager.Instance.transform, false);

            TestSuiteManager.Instance.ClosePopup();
        }
    }
}
