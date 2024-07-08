using BagelCode;
using PlayFab;
using SlotMaker.Json;
using SlotMaker;
using SlotMaker.Slots.Tasks.Actions.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BagelCode;
using ParadoxNotion;
using BagelCode.ClientModels;
using System;

public class DoorSelectItem : MonoBehaviour
{
    public Sprite[] sprites;

    private int _index = 0;

    [NonSerialized]
    public DoorController _DoorController;

    private FrameAnimator _frameAnimator;

    private Button btn;


    public void SetIndex(int index)
    {
        this._index = index;
        if (name_image != null)
        {
            name_image.sprite = sprites[index];
        }
    }

    private Image name_image;


    private void Awake()
    {
        name_image = transform.Find("name").GetComponent<Image>();
        btn = transform.Find("Image").GetComponent<Button>();
        _frameAnimator = transform.Find("animation").GetComponent<FrameAnimator>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClickBtn);
        _frameAnimator.gameObject.SetActive(false);
        _frameAnimator.FinishEvent += AnimationFinish;
    }

    private void AnimationFinish()
    {
        _frameAnimator.FinishEvent -= AnimationFinish;
        _frameAnimator.Stop();
        _frameAnimator.gameObject.SetActive(false);
        if (_DoorController != null)
        {
            _DoorController.SetClickReward();
            this.DelayAction(2, () =>
            {
                _DoorController.PlayMiniGame(_index);
            });
        }
    }

    public void OnClickBtn()
    {
        long betCredit = BlackboardUtils.FindVariable<long>("./betCredit").value;
        long extraBetCredit = BlackboardUtils.FindVariable<long>("./extraBetCredit").value;
        this._DoorController.isSelect = true;
        GSManager.Instance.GetHandler("door_select_E01").Play();
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"bet",betCredit},
            {"extra_bet",extraBetCredit },
            {"jackpot_game_index", _index},
        };
        NetManager.Instance.Post(RPCName.newSlotSpin, req, (res) =>
        {
            Debug.LogError("免费游戏的全部数据......................" + res.ToString());
            switch(_index)
            {
                case 0:
                    MiniGameDataManagers.Instance.FillGame1Data(res["game_result"]["jackpot_game_result"]);
                    break;
                case 1:
                    MiniGameDataManagers.Instance.FillGame2Data(res["game_result"]["jackpot_game_result"]);
                    break;
                case 2:
                    MiniGameDataManagers.Instance.FillGame3Data(res["game_result"]["jackpot_game_result"]);
                    break;
            }
            
            this.btn.gameObject.SetActive(false);
            _frameAnimator.gameObject.SetActive(true);
            _frameAnimator.Reset();
            _frameAnimator.Play();
        },
        (error) =>
        {
            CommonError(error);
        });
    }
    private void CommonError(BagelCodeHTTPError error)
    {
        Debug.LogWarning("V3MetaSystem.CommonError invoked!!");
        Debug.LogError(SlotSimpleJson.SerializeObject(error));

        switch (error.errorCode)
        {
            case Error.NOT_IN_ROOM_ERROR:
                {
                    bool stringError = false;
                    ErrorPopupInfo info = new ErrorPopupInfo();
                    info.type = ErrorPopupType.OK;
                    info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_NOT_EXIST_ROOM", out stringError);
                    info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                    info.callback1 = delegate
                    {
                        MessageDispatcher.Dispatch("OnContentEvent", new EventData("LeaveGame"));
                    };

                    ErrorPopupHandler.Instance.OpenError(info);
                }
                break;

            default:
                GlobalErrorHandler.GlobalError(error);
                break;
        }
    }

}
