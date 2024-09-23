using BagelCode;
using SlotMaker;
using UnityEngine;

public class LobbyTimeBonus : MonoBehaviour
{
    private ContextButton contextButton => GetComponent<ContextButton>();

    void Start()
    {
        contextButton.UpdateContext();

        contextButton.AddListenerOnClick((context) => EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_JACKPOT_DIALOG));
    }
}
