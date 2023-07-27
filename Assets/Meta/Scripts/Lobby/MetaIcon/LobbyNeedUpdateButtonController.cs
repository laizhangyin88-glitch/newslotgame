using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class LobbyNeedUpdateButtonController : MonoBehaviour
    {
        private ContextElement rootElement;

        private List<string> updateRecentFilppingTextList = new List<string>();
        private string appDownloadUrl;

        private const string LAST_RECENT_NUDGE_VERSION_KEY = "LAST_RECENT_NUDGE_VERSION";

        void Start()
        {
            OnInit();
        }

        private void OnInit()
        {
            rootElement = GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            int recentVersion = BlackboardUtils.FindVariable<int>(null, "/recentClientNumberVersion").value;
            int lastRecentVersion = PlayerPrefs.GetInt(LAST_RECENT_NUDGE_VERSION_KEY, 0);

            if (lastRecentVersion < recentVersion)
            {
                PlayerPrefs.SetInt(LAST_RECENT_NUDGE_VERSION_KEY, recentVersion);
            }

            appDownloadUrl = BlackboardUtils.FindVariable<string>(null, "/appDownloadUrl").value;
            long rewardCoins = BlackboardUtils.FindVariable<long>(null, "/values/misc/VERSION_UPDATE_REWARD_CREDIT").value;

            updateRecentFilppingTextList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "META_ICON_RECENT_FLIPPING_TEXT_0"));
            updateRecentFilppingTextList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "META_ICON_RECENT_FLIPPING_TEXT_1", rewardCoins));

            ContextElement anchorElement = ContextUtils.FindElement(rootElement, "Anchor", ContextSearchingType.ChildrenSearch);

            ContextElement needUpdateTextElement = ContextUtils.FindElement(anchorElement, "Text", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetText(needUpdateTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "META_ICON_RECENT_UPDATE_BUTTON"));

            ContextElement flippingTextElement = ContextUtils.FindElement(anchorElement, "Information Base", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetText(flippingTextElement, updateRecentFilppingTextList[0]);
            MetaContextElementUtils.SetClickable(anchorElement, OnClickAppDownload);

            MetaContextElementUtils.SetFlippingText(flippingTextElement, updateRecentFilppingTextList[0], updateRecentFilppingTextList, true, 3.0f);
        }

        private void OnClickAppDownload()
        {
            // Clicked Update
            Debug.Log("On Clicked Update");
            BIClientClickUpdateMetaIcon(false);
            if (appDownloadUrl != null)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                NativeHelper.Instance.OpenUrl(appDownloadUrl);
#else
                Application.OpenURL(appDownloadUrl);
#endif
            }
        }

        private void BIClientClickUpdateMetaIcon(bool isInGame)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["type"] = isInGame ? "in_game" : "lobby";
            Analytics.CustomEvent("client_click_update_meta_icon", customData);
        }
    }
}