using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using TMPro;

namespace BagelCode
{
    public class TestSuiteCheckThumbnailPopup : MonoBehaviour
    {
        public GameObject origTextObj;
        public GameObject contentsObj;

        public GameObject scrollObj;

        public List<GameObject> noproblemObjs;

        // gameInfo, target, cause
        private const string errorTextFormat = "<color=#00ff00>{0}</color> : <color=#FFFF00>{1}</color>";

        public const string checkAssetName = "slotthumb1";

        public const string checkSlotImageBigFormat = "Slot Image Big {0}";
        public const string checkSlotImageSmallFormat = "Slot Image Small {0}";
        public const string checkSlotThumbnailFormat = "Slot Thumbnail {0}";
        public const string checkSlotThumbnailImageFormat = "Slot Thumbnail Image {0}";

        private int problemCount;

        private void Awake()
        {
            scrollObj.SetActive(false);

            foreach(GameObject obj in noproblemObjs)
            {
                obj.SetActive(false);
            }
        }

        private void Start()
        {
            problemCount = 0;
            // Text List. 
            var gameInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/gameInfoList");

            // Check slot big, small images
            // Check thumbnail prefabs
            // Check thumbnail images

            for(int i=0; i < gameInfoList.value.Count; ++i)
            {
                // Check slot big, small prefabs
                var gameTitle = gameInfoList.value[i].GetValue<string>("gameTitle").ToUpper();
                var gameID = gameInfoList.value[i].GetValue<int>("gameId");

                string assetName = string.Format(checkSlotImageBigFormat, gameTitle);
                if(CheckPrefab(assetName, gameTitle, gameID))
                    ++problemCount;

                assetName = string.Format(checkSlotImageSmallFormat, gameTitle);
                if(CheckPrefab(assetName, gameTitle, gameID))
                    ++problemCount;
                
                assetName = string.Format(checkSlotThumbnailFormat, gameTitle);
                if(CheckPrefab(assetName, gameTitle, gameID))
                    ++problemCount;

                assetName = string.Format(checkSlotThumbnailImageFormat, gameTitle);
                if(CheckSprite(assetName, gameTitle, gameID))
                    ++problemCount;
            }

            OpenProblem(problemCount != 0);
        }

        private bool CheckPrefab(string assetName, string gameTitle, int gameID)
        {
            var prefabAsset = AssetBundleManager.LoadAsset<GameObject>(checkAssetName, assetName);
            if (prefabAsset == null)
            {
                // Make Log. Check File or tag
                MakeLogText(errorTextFormat, string.Format("{0}({1})", assetName, gameID), "check prefab or tag.");
                return true;
            }

            var image = prefabAsset.GetComponent<Image>();
            if(image == null)
            {
                MakeLogText(errorTextFormat, string.Format("{0}({1})", assetName, gameID), "image component is null.");
                return true;
            }

            if(image.sprite == null)
            {
                MakeLogText(errorTextFormat, string.Format("{0}({1})", assetName, gameID), "check bind image is null.");
                return true;
            }

            var splitNames = image.sprite.name.Split(' ');
            if(splitNames.Length > 0 && gameTitle != splitNames[splitNames.Length-1])
            {
                MakeLogText(errorTextFormat, string.Format("{0}({1})", assetName, gameID), "check bind image.");
                return true;
            }

            return false;
        }

        private bool CheckSprite(string assetName, string gameTitle, int gameID)
        {
#if UNITY_EDITOR
            var spriteAsset = AssetBundleManager.LoadAsset<Texture2D>(checkAssetName, assetName);
#else
            var spriteAsset = AssetBundleManager.LoadAsset<Sprite>(checkAssetName, assetName);
#endif
            if (spriteAsset == null)
            {
                MakeLogText(errorTextFormat, string.Format("{0}({1})", assetName, gameID), "check png or tag.");
                return true;
            }

            return false;
        }

        private void MakeLogText(string textFormat, params object[] args)
        {
            string logText = string.Format(textFormat, args);

            var go = GameObject.Instantiate(origTextObj) as GameObject;
            go.transform.SetParent(contentsObj.transform, false);
            var meshPro = go.GetComponent<TextMeshProUGUI>();
            meshPro.text = logText;

            go.SetActive(true);
        }

        // public void OnClickTestButton(int index)
        // {
        //     bool toggle = false;
        //     switch (index)
        //     {
        //     case 0:
        //         toggle = string.IsNullOrEmpty(testCase);
        //         testCase = toggle ? "CC01" : null;
        //         break;
        //     case 1:
        //         toggle = !UserDataToBoolean(forceGuestMode);
        //         forceGuestMode = toggle.ToString();
        //         break;
        //     case 2:
        //         toggle = !UserDataToBoolean(quit);
        //         quit = toggle.ToString();
        //         break;
        //     }
        //     buttons[index].GetComponent<Image>().color = toggle ? TestSuiteManager.BUTTON2_COLOR : TestSuiteManager.BUTTON1_COLOR;

        //     dirty = true;
        // }

        // public void OnClickLogFilter(int index)
        // {
        //     LogFilter mask = (LogFilter)(1 << index);
        //     LogFilter logFilter = ApplicationSettings.Instance.logFilter;

        //     if ((logFilter & mask) == mask)
        //     {
        //         ApplicationSettings.Instance.logFilter &= ~mask;
        //         logFilters[index].GetComponent<Image>().color = TestSuiteManager.BUTTON1_COLOR;
        //     }
        //     else
        //     {
        //         ApplicationSettings.Instance.logFilter |= mask;
        //         logFilters[index].GetComponent<Image>().color = TestSuiteManager.BUTTON2_COLOR;
        //     }
        // }

        // private bool UserDataToBoolean(string userData)
        // {
        //     if (string.IsNullOrEmpty(userData))
        //         return false;

        //     return Convert.ToBoolean(userData);
        // }

        public void OpenProblem(bool isProblem)
        {
            scrollObj.SetActive(isProblem);

            foreach(GameObject obj in noproblemObjs)
            {
                obj.SetActive(!isProblem);
            }
        }

        public void Close()
        {
            GameObject.Destroy(gameObject);
        }
    }
}
