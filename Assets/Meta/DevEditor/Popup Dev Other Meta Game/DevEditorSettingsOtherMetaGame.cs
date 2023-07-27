using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using SlotMaker;

using static BagelCode.DevEditorSettingsOtherMetaGame.DebugButtonTypeOtherMetaGame;

namespace BagelCode
{
    public class DevEditorSettingsOtherMetaGame : DevEditorSettings<DevEditorSettingsOtherMetaGame.DebugButtonTypeOtherMetaGame>
    {
        protected override int Height() => HEIGHT;

        private const int HEIGHT = 1; // update menually

        public enum DebugButtonTypeOtherMetaGame
        {
            UNKNOWN = 0,

            VIP_EARN_BADGE,
            VIP_STATIC_BADGE_COUNT,
            VIP_LOUNGE_JACKPOT_SPIN,
            VIP_LOUNGE_JACKPOT_DEBUG,
        }

        protected override void SetButtons()
        {
            // Buttons here
            displayButtons = new DebugButtonTypeOtherMetaGame[HEIGHT, WIDTH]
            {
                { VIP_EARN_BADGE, VIP_STATIC_BADGE_COUNT, VIP_LOUNGE_JACKPOT_SPIN, VIP_LOUNGE_JACKPOT_DEBUG, UNKNOWN},
            };
        }

        protected override void SetButtonFunctions()
        {
            // Toggle
            elementNameDict.Add(VIP_EARN_BADGE, "Button Vip Earn Badge");
            buttonTextDict.Add(VIP_EARN_BADGE, new string[] { "Vip\nEarn Badge(Off)", "Vip\nEarn Badge(On)" });
            onClickMethodDict.Add(VIP_EARN_BADGE, () => OnToggle(VIP_EARN_BADGE));
            playerPrefsKeyDict.Add(VIP_EARN_BADGE, VipLounge.VipLounge.Defines.PLAYER_PREFS_DEBUG_EARN_BADGE);

            elementNameDict.Add(VIP_STATIC_BADGE_COUNT, "Button Vip Static Badge");
            buttonTextDict.Add(VIP_STATIC_BADGE_COUNT, new string[] { "Vip\nBadge\nDefault", "Vip\nBadge(0)", "Vip\nBadge(1)", "Vip\nBadge(2)" });
            onClickMethodDict.Add(VIP_STATIC_BADGE_COUNT, () => { OnToggle(VIP_STATIC_BADGE_COUNT); VipStaticBadge(); });
            playerPrefsKeyDict.Add(VIP_STATIC_BADGE_COUNT, VipLounge.VipLounge.Defines.PLAYER_PREFS_DEBUG_STATIC_BADGE);

            elementNameDict.Add(VIP_LOUNGE_JACKPOT_SPIN, "Button Vip Lounge Jackpot Spin");
            buttonTextDict.Add(VIP_LOUNGE_JACKPOT_SPIN, new string[] { "Jackpot Spin Debug" });
            onClickMethodDict.Add(VIP_LOUNGE_JACKPOT_SPIN, VipLoungeJackpotDebugSpin);

            elementNameDict.Add(VIP_LOUNGE_JACKPOT_DEBUG, "Button Vip Lounge Jackpot Debug");
            buttonTextDict.Add(VIP_LOUNGE_JACKPOT_DEBUG, new string[] { "Click Debug Spin(Off)", "Click Debug Spin(On)" });
            onClickMethodDict.Add(VIP_LOUNGE_JACKPOT_DEBUG, () => OnToggle(VIP_LOUNGE_JACKPOT_DEBUG));
            playerPrefsKeyDict.Add(VIP_LOUNGE_JACKPOT_DEBUG, VipLounge.VipLounge.Defines.PLAYER_PREFS_VIP_LOUNGE_JACKPOT_DEBUG);
            // Functions

        }

        private void VipStaticBadge()
        {
            int vipBadgePlayerPrefs = PlayerPrefs.GetInt(VipLounge.VipLounge.Defines.PLAYER_PREFS_DEBUG_STATIC_BADGE);
            if(vipBadgePlayerPrefs > 0)
            {
                BlackboardQueryUtils.SetBadgeCount(vipBadgePlayerPrefs - 1);
            }
        }

        private void VipLoungeJackpotDebugSpin()
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>("testsuite", "VIP Lounge Jackpot DebugSpin");
            var go = GameObject.Instantiate(prefab) as GameObject;
            go.name = "Meta DebugSpin";
            go.transform.SetParent(PopupManager.Instance.transform, false);
            OnClickClose();
        }
    }
}
