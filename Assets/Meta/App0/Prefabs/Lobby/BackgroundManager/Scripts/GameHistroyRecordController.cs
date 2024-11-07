using InnerKeyboard;
using NodeCanvas.Framework;
using SimpleJSON;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHistroyRecordController : MonoBehaviour
{
    private Button ButtonNext;
    private Button ButtonPrevious;
    private Button ButtonInput;
    private TextMeshProUGUI PageIndexTxt;
    private Button ButtonClose;
    [SerializeField]
    private int one_page_count = 40;

    private GameHistroyRecordScrollView ScrollView;
    private List<GameHistroyRecordItemData> itemDataList;
    private int totalPage;
    private int currentPage;

    public static Dictionary<int, string> gameNameDicti;
    private KeyboardParam KeyboardPara = new KeyboardParam("");  //键盘参数
    private CustomScrollRect customScrollRect;
    private void Start()
    {
        InitGameNameDicti();

        ButtonNext = transform.Find("content/Info/bg/ButtonNext").GetComponent<Button>();
        ButtonClose = transform.Find("content/ButtonClose").GetComponent<Button>();
        ButtonPrevious = transform.Find("content/Info/bg/ButtonPrevious").GetComponent<Button>();
        ButtonInput = transform.Find("content/Info/bg/ButtonInput").GetComponent<Button>();
        ButtonNext.onClick.AddListener(OnClickBtnNext);
        ButtonPrevious.onClick.AddListener(OnClickBtnPrevious);
        ButtonInput.onClick.AddListener(OnClickBtnInput);
        ButtonClose.onClick.AddListener(OnClickBtnClose);
        PageIndexTxt = ButtonInput.transform.Find("PageIndex").GetComponent<TextMeshProUGUI>();
        ScrollView = transform.Find("content/ScrollViewRecord").GetComponent<GameHistroyRecordScrollView>();
        customScrollRect = ScrollView.transform.GetComponent<CustomScrollRect>();
        itemDataList = new List<GameHistroyRecordItemData>();
        GetTotalPageCount();
        currentPage = 0;
        GetPageData(currentPage);
        //this.DelayAction(1, () =>
        //{
        //    customScrollRect.enabled = true;
        //});
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
        PageIndexTxt.text = index + "/" + totalPage;
    }

    private void OnClickBtnNext()
    {
        if (currentPage >= totalPage - 1) return;
        GetPageData(++currentPage);
        SetPageIndex(currentPage + 1);
    }

    private void OnClickBtnPrevious()
    {
        if (currentPage <= 0) return;
        GetPageData(--currentPage);
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
    private void GetTotalPageCount()
    {
        Dictionary<string, object> req1 = new Dictionary<string, object>
                {
                    {"one_page_count", one_page_count},
                };
        NetManager.Instance.Post(RPCName.agent_query_round_log_page_count, req1, (res) =>
        {
            totalPage = res["page_count"];
            SetPageIndex(1);
        },
        (error) =>
        {

        });
    }

    private void GetPageData(int pageIndex)
    {
        Dictionary<string, object> req = new Dictionary<string, object>
                {
                    {"page_num", pageIndex},
                    {"one_page_count", one_page_count },
                };
        NetManager.Instance.Post(RPCName.agent_query_round_log_page_info, req, (res) =>
        {
            JSONNode node = res["page_info_list"];
            itemDataList.Clear();
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

            ScrollView.itemDataList = itemDataList;
            ScrollView.ResetItems(itemDataList.Count, true, true);
            ScrollView.SmoothScrollTo(0, 0.1f);
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
