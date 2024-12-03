using BagelCode;
using InnerKeyboard;
using NodeCanvas.Framework;
using SimpleJSON;
using SlotMaker;
using SpringGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BussinessItemData
{
    public long change_credit;
    public string user_id;
    public string source;
    public long after_credit;
    public long before_credit;
    public string agent_id;
    public string change_time;
    public long id;
    public int game_id;
}


public class BussinessRecordController : MonoBehaviour
{
    private GameObject Item;

    private Button ButtonNext;
    private Button ButtonPrevious;
    private Button ButtonInput;
    private TextMeshProUGUI PageIndexTxt;
    private Button ButtonClose;
    [SerializeField]
    private int one_page_count = 6;

    private List<BussinessRecordItemController> itemList;
    private int totalPage;
    private int currentPage;

    private Calendar _Calendar;
    private Button StartButton;
    private Button EndButton;
    private TextMeshProUGUI StartTxt;
    private TextMeshProUGUI EndTxt;
    private Button SearchButton;



    private bool isSetStartTime = true;

    public static Dictionary<int, string> gameNameDicti;
    private KeyboardParam KeyboardPara = new KeyboardParam("");  //键盘参数

    private GameListContent _GameListContent;
    private Button GameButton;
    private int searchGameId;
    private string searchGameName;

    private bool isChangeSearch = false;

    private Button ClearStart;
    private Button ClearEnd;
    private Button AllBtn;
    private TextMeshProUGUI searchGameNameTxt;

    private DetailInfoView _DetailInfoView;

    private List<BussinessItemData> _BussinessItemDataList = new List<BussinessItemData>();
    private void Start()
    {
        if (OrientationUtils.Instance.contentOrientation == ScreenOrientation.LandscapeLeft)
        {
            one_page_count = 6;
        }
        else
        {
            one_page_count = 18;
        }
        searchGameId = -100;
        ClearStart = transform.Find("content/Selecter/ClearStart").GetComponent<Button>();
        ClearStart.onClick.AddListener(() => { StartTxt.text = ""; });
        ClearEnd = transform.Find("content/Selecter/ClearEnd").GetComponent<Button>();
        ClearEnd.onClick.AddListener(() => { EndTxt.text = ""; });
        AllBtn = transform.Find("content/Selecter/AllBtn").GetComponent<Button>();
        AllBtn.onClick.AddListener(() => { searchGameNameTxt.text = "All"; searchGameId = -100; });
        Item = transform.Find("content/Item").gameObject;
        ButtonNext = transform.Find("content/Info/bg/ButtonNext").GetComponent<Button>();
        ButtonClose = transform.Find("content/ButtonClose").GetComponent<Button>();
        ButtonPrevious = transform.Find("content/Info/bg/ButtonPrevious").GetComponent<Button>();
        ButtonInput = transform.Find("content/Info/bg/ButtonInput").GetComponent<Button>();
        ButtonNext.onClick.AddListener(OnClickBtnNext);
        ButtonPrevious.onClick.AddListener(OnClickBtnPrevious);
        ButtonInput.onClick.AddListener(OnClickBtnInput);
        ButtonClose.onClick.AddListener(OnClickBtnClose);
        PageIndexTxt = ButtonInput.transform.Find("PageIndex").GetComponent<TextMeshProUGUI>();
        itemList = new List<BussinessRecordItemController>();
        InitItemList();
        _Calendar = transform.Find("content/Calendar").GetComponent<Calendar>();
        _Calendar.gameObject.SetActive(false);
        _Calendar.OnClickConfirmEvent += OnCalendarConfirmEvent;
        StartButton = transform.Find("content/Selecter/StartButton").GetComponent<Button>();
        StartButton.onClick.AddListener(OnClickStartBtn);
        EndButton = transform.Find("content/Selecter/EndButton").GetComponent<Button>();
        EndButton.onClick.AddListener(OnClickEndBtn);
        SearchButton = transform.Find("content/Selecter/SearchButton").GetComponent<Button>();
        SearchButton.onClick.AddListener(OnClickSearchButton);
        StartTxt = StartButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        EndTxt = EndButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        currentPage = 1;
        GetPageData(currentPage);
    }
    private void InitItemList()
    {
        Transform Content = transform.Find("content/ScrollViewRecord/Viewport/Content");
        for (int i = 0; i < one_page_count; i++)
        {
            GameObject temp = Instantiate(Item);
            temp.transform.SetParent(Content, false);
            BussinessRecordItemController item = temp.GetComponent<BussinessRecordItemController>();
            item.controller = this;
            itemList.Add(item);
        }
    }

    private void OnCalendarConfirmEvent()
    {
        if (isSetStartTime)
        {
            StartTxt.text = _Calendar.GetCalendarValue();
        }
        else
        {
            EndTxt.text = _Calendar.GetCalendarValue();
        }
    }

    private void OnClickSearchButton()
    {
        GetRecordData(0, true);
    }

    private void GetRecordData(int currentPage, bool isSendTotalPage = false)
    {
        if (!string.IsNullOrEmpty(StartTxt.text) && !string.IsNullOrEmpty(EndTxt.text))
        {
            GetPageData(currentPage, searchGameId, StartTxt.text + ":00", EndTxt.text + ":00");
        }
        else if (!string.IsNullOrEmpty(EndTxt.text))
        {
            GetPageData(currentPage, searchGameId, "", EndTxt.text + ":00");
        }
        else if (!string.IsNullOrEmpty(StartTxt.text))
        {
            GetPageData(currentPage, searchGameId, StartTxt.text + ":00");
        }
        else
        {
            GetPageData(currentPage, searchGameId);
        }

    }


    public void SetSearchGameInfo(string name, int id)
    {
        searchGameId = id;
        searchGameName = name;
        searchGameNameTxt.text = searchGameName;
    }

    private void OnClickStartBtn()
    {
        isSetStartTime = true;
        _Calendar.gameObject.SetActive(true);
    }

    private void OnClickEndBtn()
    {
        isSetStartTime = false;
        _Calendar.gameObject.SetActive(true);
    }


    private void HideAllItem()
    {
        if (itemList.Count > 0)
        {
            for (global::System.Int32 i = 0; i < itemList.Count; i++)
            {
                itemList[i].gameObject.SetActive(false);
            }
        }
    }


    private void OnClickBtnClose()
    {
        Destroy(gameObject);
    }

    private void SetPageIndex(int index)
    {
        if (totalPage == 0)
        {
            PageIndexTxt.text = "0/0";
        }
        else
        {
            PageIndexTxt.text = index + "/" + totalPage;
        }
    }

    private void OnClickBtnNext()
    {
        if (currentPage >= totalPage) return;
        GetRecordData(++currentPage);
        //SetPageIndex(currentPage + 1);
    }

    private void OnClickBtnPrevious()
    {
        if (currentPage <= 0) return;
        GetRecordData(--currentPage);
        //SetPageIndex(currentPage + 1);
    }

    private void OnClickBtnInput()
    {
        OpenBackgroundManager.OpenView("lobby0", "KeyBoard", transform);
        this.DelayAction(Time.deltaTime, () =>
        {
            if (Keyboard.Instance)
            {
                Keyboard.Instance.ShowKeyboard(KeyboardPara, EditCallBack);
                Keyboard.Instance.CancelBtnEvent += OnClickCancelBtn;
                Keyboard.Instance.ConfirmBtnEvent += OnClickEnterBtn;
            }
        });
    }

    private void OnClickCancelBtn()
    {
        Keyboard.Instance.DestroySelf();
        SetPageIndex(currentPage);
    }

    private void OnClickEnterBtn()
    {
        if (int.TryParse(PageIndexTxt.text, out int pageIndex))
        {
            if (pageIndex <= totalPage && pageIndex > 0)
            {
                GetPageData(pageIndex);
                SetPageIndex(pageIndex);
                currentPage = pageIndex;
            }
            else
            {
                SetPageIndex(currentPage);
            }
        }
        else
        {
            SetPageIndex(currentPage);
        }
        Keyboard.Instance.DestroySelf();
    }

    void EditCallBack(KeyboardParam kbpara)
    {
        PageIndexTxt.text = kbpara.OutputStr;
    }

    private void GetTotalPageCount(int gameId = 0, string start = "", string end = "")
    {
    }


    public void ShowMoreView(BussinessItemData data)
    {
        if (_DetailInfoView == null)
        {
            GameObject view = transform.Find("AllInfoContent").gameObject;
            _DetailInfoView = new DetailInfoView(view);
        }
        _DetailInfoView.SetText(data);
        _DetailInfoView.gameObject.SetActive(true);
    }

    private void GetPageData(int pageIndex, int gameId = 0, string start = "", string end = "")
    {
        JSONNode data = JSONNode.Parse("{}");
        data["method"] = "getAccountsHistory";
        JSONNode param = JSONNode.Parse("{}");
        param["token"] = NumericKeypadController.token;
        param["start_time"] = start;
        param["end_time"] = end;
        param["page"] = pageIndex;
        param["per_page"] = 6;
        Debug.LogError(NetData_Login.Instance.NetData_UserId);
        param["user_id"] = NetData_Login.Instance.NetData_UserId; 
        data["params"] = param;
        NetManager.Instance.Post(RPCName.user_php_interface, data, (res) =>
        {
            JSONNode node = res["page_info_list"];
            HideAllItem();
            totalPage = res["response_data"]["last_page"].AsInt;
            currentPage = res["response_data"]["current_page"].AsInt;
            SetPageIndex(currentPage);
            JSONNode data = res["response_data"]["data"];
            _BussinessItemDataList.Clear();
            for (int i = 0; i < data.Count; i++)
            {
                var temp = data[i];
                BussinessItemData item = new BussinessItemData();
                item.id = temp["id"].AsLong;
                item.game_id = temp["game_id"].AsInt;
                item.agent_id = temp["agent_id"];
                item.change_time = temp["change_time"];
                item.user_id = temp["user_id"];
                item.source = temp["source"];
                item.after_credit = temp["after_credit"].AsLong;
                item.before_credit = temp["before_credit"].AsLong;
                item.change_credit = temp["change_credit"].AsLong;
                _BussinessItemDataList.Add(item);
            }
            for (int i = 0; i < _BussinessItemDataList.Count; i++)
            {
                itemList[i].gameObject.SetActive(true);
                itemList[i].UpdateView(_BussinessItemDataList[i], i);
            }
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });
    }

    private void OnDestroy()
    {
        _BussinessItemDataList.Clear();
    }
}
