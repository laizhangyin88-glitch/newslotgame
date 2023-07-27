using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/MetaGames")]
    public class UpdateMetaIconRecentClientVersion : ActionTask<ContextElement>
    {
        public BBParameter<ContextElement> needUpdateElement;
        public BBParameter<ContextElement> saveAsFlippingTextElement;
        public BBParameter<string> saveAsAppDownloadUrl;

        private const string LAST_RECENT_NUDGE_VERSION_KEY = "LAST_RECENT_NUDGE_VERSION";

        protected override void OnExecute()
        {
            if (needUpdateElement == null || needUpdateElement.value == null)
            {
                EndAction();
                return;
            }

            int recentVersion = BlackboardUtils.FindVariable<int>(null, "/recentClientNumberVersion").value;
            int lastRecentVersion = PlayerPrefs.GetInt(LAST_RECENT_NUDGE_VERSION_KEY, 0);

            if (lastRecentVersion < recentVersion)
            {
                PlayerPrefs.SetInt(LAST_RECENT_NUDGE_VERSION_KEY, recentVersion);
            }

            saveAsAppDownloadUrl.value = BlackboardUtils.FindVariable<string>(null, "/appDownloadUrl").value;
            long rewardCoins = BlackboardUtils.FindVariable<long>(null, "/values/misc/VERSION_UPDATE_REWARD_CREDIT").value;

            Blackboard bb = agent.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue<string>(bb, "META_ICON_RECENT_FLIPPING_TEXT_0", StringTableUtils.GetString(StringTable.StringTableType.Global, "META_ICON_RECENT_FLIPPING_TEXT_0"));
            BlackboardUtils.SetOrCreateValue<string>(bb, "META_ICON_RECENT_FLIPPING_TEXT_1", StringTableUtils.GetString(StringTable.StringTableType.Global, "META_ICON_RECENT_FLIPPING_TEXT_1", rewardCoins));

            ContextElement needUpdateTextElement = ContextUtils.FindElement(needUpdateElement.value, "Text", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetText(needUpdateTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "META_ICON_RECENT_UPDATE_BUTTON"));

            saveAsFlippingTextElement.value = ContextUtils.FindElement(needUpdateElement.value, "Information Base", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetText(saveAsFlippingTextElement.value, BlackboardUtils.FindValue<string>(bb, "META_ICON_RECENT_FLIPPING_TEXT_1"));
            MetaContextElementUtils.SetClickable(needUpdateElement.value, "OnAppDownload", agent, null);

            EndAction();
        }
    }
}