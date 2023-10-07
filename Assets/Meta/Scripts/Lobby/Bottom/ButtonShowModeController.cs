using BagelCode.ClientModels;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class ButtonShowModeController : MonoBehaviour
    {
        public GameType gameType;
        private ContextButton contextButton => GetComponent<ContextButton>();
        void Start()
        {
            contextButton.UpdateContext();

            contextButton.AddListenerOnClick(
                (context) =>
                {
                    BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "curShowGameType", gameType);
                    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new ParadoxNotion.EventData("OnChangeGameListShowMode"));
                });
        }

    }
}
