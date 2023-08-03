using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;

namespace BagelCode
{

    public static partial class BlackboardQueryUtils
    {
        public static void SaveWebSocketHost(string wsHost)
        {
            if (!string.IsNullOrEmpty(wsHost))
            {
                var webSocktBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "webSocket");
                BlackboardUtils.SetOrCreateValue(webSocktBB, "wsHost", wsHost);
            }
        }
    }

}

