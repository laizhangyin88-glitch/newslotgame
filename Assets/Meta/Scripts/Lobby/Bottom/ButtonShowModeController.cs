using BagelCode.ClientModels;
using ParadoxNotion;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class ButtonShowModeController : MonoBehaviour
    {
        public GameFilter gameType;
        private ContextButton contextButton => GetComponent<ContextButton>();
        private GameObject selectedObj;
        private Color normalColor = Color.white;
        private Color selectedColor = new Color(0,1,1);
        private TextMeshProUGUI text;
        void Start()
        {
            selectedObj = transform.Find("Anchor/Base/selected").gameObject;
            text = transform.Find("Anchor/Base/Text (TMP)").GetComponent<TextMeshProUGUI>();

            contextButton.UpdateContext();

            contextButton.AddListenerOnClick(
                (context) =>
                {
                    BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "curShowGameType", gameType);
                    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData("OnChangeGameListShowMode"));
                });

        }

        public void ChangeBtnState()
        {
            var temp = MainBlackboard.Get().GetValue<GameFilter>("curShowGameType");
            var selected = temp == gameType;
            selectedObj.SetActive(selected);
            text.color = selected ? selectedColor : normalColor;
        }

    }
}
