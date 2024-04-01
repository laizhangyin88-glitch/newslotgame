using ParadoxNotion;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class MachineTest : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_RIGHT));
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_LEFT));
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                if(ApplicationSettings.Instance.isMachine)
                    StartCoroutine(IOController.Instance.PurchaseCreditRequest(1, 100));
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                if (ApplicationSettings.Instance.isMachine)
                    StartCoroutine(IOController.Instance.PurchaseCreditRequest(2, 100));
            }


            if (Input.GetKeyDown(KeyCode.F1))
            {

                if (PopupManager.Instance.popupCount > 0 || PopupManager.Instance.Exist())
                {
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Return"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClose"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Finalize"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnCollect"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnPointerClick"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin"));
                }
                else
                    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 1));


            }

            if ((Input.GetKeyDown(KeyCode.E)))
            {
                globalStore.test_is_free_spin = 1;
            }


            if ((Input.GetKeyDown(KeyCode.D)))
            {
                globalStore.test_is_free_spin = 0;
            }

            if ((Input.GetKeyDown(KeyCode.R)))
            {
                //
                EventSender.SendGlobalEvent("OnLobby");  //测试：退出问题
            }

            if (Input.GetKeyUp(KeyCode.F1))
            {
                if (PopupManager.Instance.popupCount == 0)
                    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 0));
            }



            if (Input.GetKeyUp(KeyCode.H))
            {
                /*
                 BlackboardQueryUtils.IsIngame()
                BlackboardQueryUtils.IsSpin()
                BlackboardQueryUtils.IsAutoSpin();
                //BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "isSpin").value
            */
                Debug.Log($" @ 游戏状态  == IsSpin :{BlackboardQueryUtils.IsSpin()}   IsAutoSpin : {BlackboardQueryUtils.IsAutoSpin()}   IsIngame : {BlackboardQueryUtils.IsIngame()}" );
            }
        }
    }
}
