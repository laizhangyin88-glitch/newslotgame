using BagelCode;
using SlotMaker.Json;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ParadoxNotion;
using BagelCode.ClientModels;

public class CommonMiniGameController : MonoBehaviour
{
    private List<CommonMiniGameItemController> commonMiniGameItemControllers = new List<CommonMiniGameItemController>();
    private Button redBtn;
    private Button blueBtn;
    private Image BlackImage;
    private Image red_Image;
    private Image blue_Image;
    public GameObject item;
    private Transform itemParent;
    
    private int CurrentIndex = 0;

    private bool isCanClick = false;

    private void Start()
    {
        CurrentIndex = 0;
        isCanClick = true; 

        redBtn = transform.Find("red_Button").GetComponent<Button>();
        blueBtn = transform.Find("blue_Button").GetComponent<Button>();
        red_Image = transform.Find("Item/red_Image").GetComponent<Image>();
        blue_Image = transform.Find("Item/blue_Image").GetComponent<Image>();
        BlackImage = transform.Find("Item/BlackImage").GetComponent<Image>();
        itemParent = transform.Find("ScrollView/Viewport/Content").transform;

        redBtn.onClick.RemoveAllListeners();
        blueBtn.onClick.RemoveAllListeners();

        red_Image.gameObject.SetActive(false);
        blue_Image.gameObject.SetActive(false);
        BlackImage.gameObject.SetActive(true);

        redBtn.onClick.AddListener(OnClickRedBtn);
        blueBtn.onClick.AddListener(OnClickBlueBtn);

        InitList();
    }

    private void OnClickRedBtn()
    {
        if(isCanClick)
        {
            isCanClick = false;
            SendValue(1);
        }
    }

    private void OnClickBlueBtn()
    {
        if (isCanClick)
        {
            isCanClick = false;
            SendValue(0);
        }
    }

    private void SendValue(int value)
    {
        int gameId = BlackboardUtils.FindValue<int>("./game/gameId");
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            { "game_id", gameId},
            { "option", value },
        };
        NetManager.Instance.Post(RPCName.new_high_low_game, req, (res) =>
        {
            Debug.LogError("比大小，游戏下发结果....." + res.ToString());
            //"比大小，游戏下发结果.....{"high_low_game_result":{"earn_credit":0,"result":1,"option":0},"err":0,"seq_id":4}"
            var result = res["high_low_game_result"]["result"];///服务器下发的结果
            var option = res["high_low_game_result"]["option"];///我的发送的操作
            ShowResult(result == 1); ///展示服务器下发的结果
            this.commonMiniGameItemControllers[this.CurrentIndex].ShowImage(result == 1);
            CurrentIndex++;
            if(result == option) ///赢了
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
            }
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

    private void ShowResult(bool isShowRed)
    {
        AsyncActionUtils.ApplyRotation(this, BlackImage.transform, Vector3.zero, new Vector3(0, 90, 0), 0.5f, TweenUtils.VectorTweenLinear, 0, () =>
        {
            AsyncActionUtils.ApplyRotation(this, BlackImage.transform, new Vector3(0, 90, 0), new Vector3(0, 180, 0), 0.5f, TweenUtils.VectorTweenLinear, 0, () =>
            {
                BlackImage.gameObject.SetActive(false);
                red_Image.gameObject.SetActive(isShowRed);
                blue_Image.gameObject.SetActive(!isShowRed);
            });
        });
    } 

    private void Reset()
    {
        BlackImage.transform.rotation = Quaternion.identity;
        BlackImage.gameObject.SetActive(true);
        
        red_Image.gameObject.SetActive(false);
        blue_Image.gameObject.SetActive(false);
    }

    private void InitList()
    {
        if(item != null)
        {
            for (int i = 0; i < 11; i++)
            {
                GameObject temp = Instantiate(item) as GameObject;
                temp.transform.SetParent(itemParent.transform, false);
                CommonMiniGameItemController controller = temp.AddComponent<CommonMiniGameItemController>();
                controller.OnInit();
                commonMiniGameItemControllers.Add(controller);
            }
        }
    }

    private void Clear()
    {
        for (int i = 0; i < commonMiniGameItemControllers.Count; i++)
        {
            Destroy(commonMiniGameItemControllers[i].gameObject);
        }
        commonMiniGameItemControllers.Clear();
        Destroy(this.gameObject);
    }
}
