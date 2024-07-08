using BagelCode;
using ParadoxNotion;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public enum GuessSizeState
{
    None = -1,
    GuessSize,
    ShowResult,
    Over,
}

public class MiniGameGuessSize : MonoBehaviour
{
    GuessSizeState state = GuessSizeState.None;
    Transform root;
    Animator ani_Man,ani_Coin1, ani_Coin2;
    Button btn_Coin1, btn_Coin2;
    void Start()
    {
        root = transform.Find("Animator/Anchor");
        ani_Man = root.Find("man/Animator").GetComponent<Animator>();
        ani_Coin1 = root.Find("coins/coin1").GetComponent<Animator>();
        ani_Coin2 = root.Find("coins/coin2").GetComponent<Animator>();

        btn_Coin1 = ani_Coin1.GetComponent<Button>();
        btn_Coin2 = ani_Coin2.GetComponent<Button>();

    }

    void OnEnable()
    {
        //开始猜
        state = GuessSizeState.GuessSize;

    }


    void AddEvent()
    {
        btn_Coin1.onClick.AddListener(ClickCoin1);
        btn_Coin2.onClick.AddListener(ClickCoin2);

    }

    void ClickCoin1() { OnClickCoin(0);}
    void ClickCoin2() { OnClickCoin(1);}

    void RemoveEvent()
    {
        btn_Coin1.onClick.RemoveListener(ClickCoin1);
        btn_Coin2.onClick.RemoveListener(ClickCoin2);
    }

    void OnClickCoin(int index)
    {
        if (state != GuessSizeState.GuessSize)
            return;
        state = GuessSizeState.ShowResult;

        int gameId = BlackboardUtils.FindValue<int>("./game/gameId");
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            { "game_id", gameId},
            { "option", index },
        };

        NetManager.Instance.Post(RPCName.new_high_low_game, req,
        (res) =>
        {
            Debug.LogError("比大小，游戏下发结果....." + res.ToString());
            //"比大小，游戏下发结果.....{"high_low_game_result":{"earn_credit":0,"result":1,"option":0},"err":0,"seq_id":4}"
            var result = res["high_low_game_result"]["result"];///服务器下发的结果
            var option = res["high_low_game_result"]["option"];///我的发送的操作

            if (result == option) ///赢了
            {
                state = GuessSizeState.GuessSize;
            }
            else
            {
                state = GuessSizeState.Over;


                MessageDispatcher.Dispatch("OnContentUIEvent", new EventData("BSTMiniGameGuessSizeFinish")); //发给脚本
                EventSender.SendGlobalEvent(new EventData("BSTMiniGameGuessSizeFinish")); //发给NodeCanvas （类型：OnCustomEvent）

            }

            /*ShowResult(result == 1); ///展示服务器下发的结果
            this.commonMiniGameItemControllers[this.CurrentIndex].ShowImage(result == 1);
            CurrentIndex++;
            if (result == option) ///赢了
            {
                Debug.LogError(" win ..................................");
                this.DelayAction(5, () =>
                {
                    isCanClick = true;
                    Reset();
                });
            }
            else
            {
                Debug.LogError("lose ............................................");
                this.DelayAction(5, () =>
                {
                    isCanClick = true;
                    Clear();
                });
            }*/
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });



    }
}
