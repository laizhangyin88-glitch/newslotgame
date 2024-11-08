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


public class GameHistroyRecordItemData
{
    public int sn;
    public long bet;
    public long total_win;
    public long end_cent;
    public long init_cent;
    public long start_time;
    public long game_time;
    public int win_line_count;
    public long total_bet;
    public int game_id;
}


public class GameHistroyRecordController : MonoBehaviour
{
    private GameObject Item;

    private Button ButtonNext;
    private Button ButtonPrevious;
    private Button ButtonInput;
    private TextMeshProUGUI PageIndexTxt;
    private Button ButtonClose;
    [SerializeField]
    private int one_page_count = 6;

    private List<GameHistroyRecordItemData> itemDataList;
    private List<GameHistroyRecordItem> itemList;
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
        InitGameNameDicti();

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
        itemDataList = new List<GameHistroyRecordItemData>();
        itemList = new List<GameHistroyRecordItem>();
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
        GameButton = transform.Find("content/Selecter/GameButton").GetComponent<Button>();
        GameButton.onClick.AddListener(OnClickGameBtn);
        searchGameNameTxt = GameButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        StartTxt = StartButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        EndTxt = EndButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        GetTotalPageCount();
        currentPage = 0;
        GetPageData(currentPage);
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
            if (isSendTotalPage)
                GetTotalPageCount(searchGameId, StartTxt.text + ":00", EndTxt.text + ":00");
            GetPageData(currentPage, searchGameId, StartTxt.text + ":00", EndTxt.text + ":00");
        }
        else if (!string.IsNullOrEmpty(EndTxt.text))
        {
            if (isSendTotalPage)
                GetTotalPageCount(searchGameId, "", EndTxt.text + ":00");
            GetPageData(currentPage, searchGameId, "", EndTxt.text + ":00");
        }
        else if (!string.IsNullOrEmpty(StartTxt.text))
        {
            if (isSendTotalPage)
                GetTotalPageCount(searchGameId, StartTxt.text + ":00");
            GetPageData(currentPage, searchGameId, StartTxt.text + ":00");
        }
        else
        {
            if (isSendTotalPage)
                GetTotalPageCount(searchGameId);
            GetPageData(currentPage, searchGameId);
        }

    }

    private void OnClickGameBtn()
    {
        if (_GameListContent == null)
        {
            GameObject GameList = transform.Find("content/GameList").gameObject;
            _GameListContent = new GameListContent(GameList);
            _GameListContent.controller = this;
        }
        _GameListContent.gameObject.SetActive(true);
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

    private void InitItemList()
    {
        Transform Content = transform.Find("content/ScrollViewRecord/Viewport/Content");
        for (int i = 0; i < one_page_count; i++)
        {
            GameObject temp = Instantiate(Item);
            temp.transform.SetParent(Content, false);
            GameHistroyRecordItem item = temp.GetComponent<GameHistroyRecordItem>();
            item.controller = this;
            itemList.Add(item);
        }
    }

    private void HideAllItem()
    {
        if(itemList.Count > 0)
        {
            for (global::System.Int32 i = 0; i < itemList.Count; i++)
            {
                itemList[i].gameObject.SetActive(false);
            }
        }
    }

    private void InitGameNameDicti()
    {
        if(gameNameDicti == null)
        {
            gameNameDicti = new Dictionary<int, string>();
            var gameInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/gameInfoList");
            foreach (var item in gameInfoList.value)
            {
                int id = item.GetValue<int>("gameId");
                string name = item.GetValue<string>("enumId").Replace('_', ' ');
                if (id > 0 && !string.IsNullOrEmpty(name))
                {
                    gameNameDicti.Add(id, name);
                }
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
        if (currentPage >= totalPage - 1) return;
        GetRecordData(++currentPage);
        SetPageIndex(currentPage + 1);
    }

    private void OnClickBtnPrevious()
    {
        if (currentPage <= 0) return;
        GetRecordData(--currentPage);
        SetPageIndex(currentPage + 1);
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
        SetPageIndex(currentPage + 1);
    }

    private void OnClickEnterBtn()
    {
        if(int.TryParse(PageIndexTxt.text, out int pageIndex))
        {
            if(pageIndex <= totalPage && pageIndex > 0)
            {
                GetPageData(pageIndex - 1);
                SetPageIndex(pageIndex);
                currentPage = pageIndex - 1;
            }
            else
            {
                SetPageIndex(currentPage + 1);
            }
        }
        else
        {
            SetPageIndex(currentPage + 1);
        }
        Keyboard.Instance.DestroySelf();
    }

    void EditCallBack(KeyboardParam kbpara)
    {
        PageIndexTxt.text = kbpara.OutputStr;
    }

    private void GetTotalPageCount(int gameId = 0, string start = "", string end = "")
    {
        Dictionary<string, object> req = new Dictionary<string, object>
                {
                    {"one_page_count", one_page_count},
                };
        if (gameId > 0)
        {
            req.Add("game_id", gameId);
        }
        if (!string.IsNullOrEmpty(start))
        {
            req.Add("start_time", start);
        }
        if (!string.IsNullOrEmpty(end))
        {
            req.Add("end_time", end);
        }
        NetManager.Instance.Post(RPCName.agent_query_round_log_page_count, req, (res) =>
        {
            totalPage = res["page_count"];
            currentPage = 0;
            SetPageIndex(1);
        },
        (error) =>
        {

        });
    }


    public void ShowMoreView(GameHistroyRecordItemData data)
    {
        if(_DetailInfoView == null)
        {
            GameObject view = transform.Find("AllInfoContent").gameObject;
            _DetailInfoView = new DetailInfoView(view);
        }
        _DetailInfoView.SetText(data);
        _DetailInfoView.gameObject.SetActive(true);
    }

    private void GetPageData(int pageIndex, int gameId = 0, string start = "", string end = "")
    {
        Dictionary<string, object> req = new Dictionary<string, object>
                {
                    {"page_num", pageIndex},
                    {"one_page_count", one_page_count },
                };
        if (gameId > 0)
        {
            req.Add("game_id", gameId);
        }
        if (!string.IsNullOrEmpty(start))
        {
            req.Add("start_time", start);
        }
        if (!string.IsNullOrEmpty(end))
        {
            req.Add("end_time", end);
        }
        NetManager.Instance.Post(RPCName.agent_query_round_log_page_info, req, (res) =>
        {
            JSONNode node = res["page_info_list"];
            itemDataList.Clear();
            HideAllItem();
            for (global::System.Int32 i = 0; i < node.Count; i++)
            {
                var temp = node[i];
                GameHistroyRecordItemData item = new GameHistroyRecordItemData();
                item.sn = temp["sn"].AsInt;
                item.game_id = temp["game_id"].AsInt;
                item.bet = temp["bet"].AsLong;
                item.total_win = temp["total_win"].AsLong;
                item.init_cent = temp["init_cent"].AsLong;
                item.end_cent = temp["end_cent"].AsLong;
                item.total_bet = temp["total_bet"].AsLong;
                item.win_line_count = temp["win_line_count"].AsInt;
                item.game_time = temp["game_time"].AsLong;
                item.start_time = temp["start_time"].AsLong;
                itemDataList.Add(item);
            }
            for (global::System.Int32 i = 0; i < itemDataList.Count; i++)
            {
                itemList[i].gameObject.SetActive(true);
                itemList[i].UpdateView(itemDataList[i], i);
            }
        },
        (error) =>
        {

        });
    }

    private void OnDestroy()
    {
        gameNameDicti.Clear();
        gameNameDicti = null;
    }
}
