using ParadoxNotion;
using SlotMaker;
using SlotMaker.Tasks.Actions;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using UnityEngine;

namespace BagelCode
{
    public class MachineTest : MonoBehaviour
    {

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {

                if (MachineSelectManager.Instance.isFreeGameTimeSelectPop()
                    || MachineSelectManager.Instance.isMiniGameSelectPop()
                    || MachineSelectManager.Instance.isGameConfigSelectPop())
                {
                    MachineSelectManager.Instance.NextSelectItem();
                }
                else
                    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_RIGHT));
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {


                if (MachineSelectManager.Instance.isFreeGameTimeSelectPop()
                    || MachineSelectManager.Instance.isMiniGameSelectPop()
                    || MachineSelectManager.Instance.isGameConfigSelectPop())
                {
                    MachineSelectManager.Instance.PreviousSelectItem();
                }
                else
                    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_LEFT));
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                if (ApplicationSettings.Instance.isMachine)
                    StartCoroutine(IOController.Instance.PurchaseCreditRequest(1, 100));
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                if (ApplicationSettings.Instance.isMachine)
                    StartCoroutine(IOController.Instance.PurchaseCreditRequest(2, 100));
            }


            if (Input.GetKeyDown(KeyCode.F1))
            {

                if (MachineSelectManager.Instance.isFreeGameTimeSelectPop())
                {
                    MachineSelectManager.Instance.ConfirmFreeGameSelectItem();
                }
                else if (MachineSelectManager.Instance.isGameConfigSelectPop())
                {
                    MachineSelectManager.Instance.ConfirmGameConfigSelectPop();
                }
                else if (PopupManager.Instance.popupCount > 0 || PopupManager.Instance.Exist())
                {
                    Debug.Log($"【machine】: Popup");

                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Return"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClose"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Finalize"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnCollect"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnPointerClick"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin")); // 多数游戏免费游戏的开始提示弹窗


                    //EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin"));

                    /* ID:39
                     * Big Win Text Event Mega Win
                     * Big Win Text Event Super Mega Win
                     * Big Win Text Event Big Win
                     */
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("MachineSpinClick"));


                    //ID:39
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartBigWheel"));//Big Wheel Trigger Popup
                                                                                                   //EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect"));//Big Wheel Result Popup


                    //ID:21
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("collectEvent")); //


                    //ID:93
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Clicked")); //Free Game Select Popup（免费游戏结算确认界面）


                    //ID:40
                    //EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin"));//Free Game Trigger Popup
                    //EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect"));//Free Game Result Popup


                    //ID:37
                    //EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect")); //Free Game Result Popup（免费游戏结算确认界面）

                    //ID:142
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnFinishBonus")); //Double Jackpot Major Popup FIJ（免费游戏结算确认界面）


                    //ID:149 - 白虎
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnButtonClicked")); //Double Jackpot Major Popup FIJ（免费游戏开始界面）


                    //ID:144 - 狮子
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClickButton")); //免费游戏开始界面、免费游戏结算界面

                }
                else if (MachineSelectManager.Instance.isMiniGameSelectPop())
                {
                    MachineSelectManager.Instance.ConfirmMiniGameSelectItem();
                }
                else if (MachineSelectManager.Instance.isMiniGamePop())
                {
                    MachineSelectManager.Instance.ConfirmMiniGame();
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
                Debug.Log($" @ 游戏状态  == IsSpin :{BlackboardQueryUtils.IsSpin()}   IsAutoSpin : {BlackboardQueryUtils.IsAutoSpin()}   IsIngame : {BlackboardQueryUtils.IsIngame()}");
            }
        }
    }
}
