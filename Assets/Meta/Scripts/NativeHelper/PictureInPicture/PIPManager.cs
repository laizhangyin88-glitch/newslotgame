using System.Collections.Generic;
using SlotMaker;
using System.Linq;

namespace BagelCode
{
    public class PIPManager : MonoWeakSingleton<PIPManager>
    {
        // any state true to activate pip
        private Dictionary<string, bool> pipStateDict = new Dictionary<string, bool>();

        private void Start()
        {
            NativeHelper.Instance.RegistPipCallback(OnChangePipMode);
        }

        protected override void OnDestroy()
        {
            // NativeHelper.Instance.UnRegistPipCallback();
            base.OnDestroy();
        }

        public void SetPipState(string key, bool enable)
        {
            pipStateDict[key] = enable;
            UpdatePipState();
        }

        public void UpdatePipState()
        {
            // check anyone enabled. by autoSpin, another..?
            bool enable = pipStateDict.Any(s => s.Value);

            if (BlackboardQueryUtils.IsPipModeEnabled() &&
                BlackboardQueryUtils.IsPipModeSettingsEnabled())
            {
                NativeHelper.Instance.SetPIP(enable);
            }
            else
            {
                NativeHelper.Instance.SetPIP(false);
            }
        }

        private static void OnChangePipMode(bool inPictureInPictureMode)
        {
            SendChangePipMode(inPictureInPictureMode ? "pip" : "fullscreen");
        }

        private static void SendChangePipMode(string action)
        {
            var gameID = BlackboardUtils.FindVariable<int>(null, "/enterGameInfo/gameId");
            var totalBetCredit = BlackboardUtils.FindVariable<long>("./totalBetCredit");

            if(gameID == null || totalBetCredit == null) return;

            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["game_id"] = gameID.value;
            customData["bet"] = totalBetCredit.value;
            customData["action"] = action;
            customData["slot_enter_context_id"] = BiEventUtils.GetSlotEnterContextID();

            Analytics.CustomEvent("client_pip_mode", customData);
        }
    }
}
