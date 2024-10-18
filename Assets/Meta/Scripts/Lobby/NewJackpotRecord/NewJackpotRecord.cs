using BagelCode;
using ParadoxNotion;
using SlotMaker;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewJackpotRecord : MonoBehaviour
{
    private ContextElement root;
    private List<Toggle> toggles = new List<Toggle>();

    void Start()
    {
        root = GetComponent<ContextElement>();
        root.UpdateContext(false);
        InitContext();
        InitClickEvents();
        GetJackpotRecord(4);
    }

    private void InitContext()
    {
        var toggelGroup = transform.Find("Anchors/Toggle Group");
        for (int i = 0; i < toggelGroup.childCount; i++)
        {
            toggles.Add(toggelGroup.GetChild(i).GetComponent<Toggle>());
            toggles[i].onValueChanged.AddListener(OnToggleValueChange);
        }
        toggles.Reverse();
    }

    private void OnToggleValueChange(bool value)
    {
        if (value)
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                if (toggles[i].isOn)
                {
                    GetJackpotRecord(i);
                    break;
                }
            }
        }
    }

    private void GetJackpotRecord(int type)
    {
        gameObject.GetComponent<GameSoundPlayer>().PlayGameSound("UI_Button_Normal");
        string bbTimerStr = $"jackpotRecordTimer{type}";
        string bbJackpotRecordStr = $"jackpotRecordType{type}";
        long timeStamp = MainBlackboard.Get().GetVariable<long>(bbTimerStr) != null ? MainBlackboard.Get().GetValue<long>(bbTimerStr) : 0;
        var curTimeStamp = MetaSystem.GetTimeStamp();
        if (curTimeStamp - timeStamp > 60000)
        {
            NetManager.Instance.SendMsg(RPCName.queryJackpotRanking, new Dictionary<string, object> {
                {"jackpot_id",type == 4 ? 0 : type},
                { "is_all", type == 4 ? 1 : 0}
            });
        }
        else
        {
            var eventData = MainBlackboard.Get().GetValue<EventData>(bbJackpotRecordStr);
            MessageDispatcher.Dispatch(RPCName.queryJackpotRanking, eventData);
        }
        MainBlackboard.Get().SetValue(bbTimerStr, MetaSystem.GetTimeStamp());
    }

    private void InitClickEvents()
    {
        MetaContextElementUtils.SimpleSetClickable(root, "CloseBtn",
            () =>
            {
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, new EventData(MetaEventDefine.ON_LEAVE_JACKPOT_RECORD));
                MetaPopupUtils.ClosePopup(gameObject);
            }, true, ContextSearchingType.ChildrenSearch);
        
    }

}
