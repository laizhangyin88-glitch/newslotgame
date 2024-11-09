using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameListContent
{

    public GameObject gameObject;
    private Transform transform;

    private Button buttonClose;
    private Button GameNameBtn;
    private Transform List;
    private Button BtnPrevious;
    private Button BtnNext;
    private Button BtnClose;
    private TextMeshProUGUI pageTxt;
    private int totatlPage;
    private int currentPage;

    public GameHistroyRecordController controller;

    private List<GameObject> btnList;

    private List<string> nameList;
    private List<int> idList;

    private int currentIndex = 0;
    public GameListContent(GameObject gameObject)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        Init();
    }

    private void Init()
    {
        buttonClose = transform.Find("ButtonClose").GetComponent<Button>();
        GameNameBtn = transform.Find("GameNameBtn").GetComponent<Button>();
        List = transform.Find("List");
        BtnClose = transform.Find("ButtonClose").GetComponent<Button>(); 
        pageTxt = transform.Find("PageTxt").GetComponent<TextMeshProUGUI>();
        BtnPrevious = transform.Find("BtnPrevious").GetComponent<Button>();
        BtnNext = transform.Find("BtnNext").GetComponent<Button>();
        BtnPrevious.onClick.AddListener(OnClickBtnPrevious);
        BtnNext.onClick.AddListener(OnClickBtnNext);
        InitGameNameBtnList();
        BtnClose.onClick.AddListener(()=> { gameObject.SetActive(false); });
    }

    private void OnClickBtnPrevious()
    {
        if (currentPage > 1)
        {
            CreateBtn(currentIndex - 40, currentIndex - 20);
            currentIndex -= 20;
            SetPageInfo(--currentPage);
        }
    }

    private void OnClickBtnNext()
    {
        if (currentPage < totatlPage)
        {
            CreateBtn(currentIndex, currentIndex + 20); 
            currentIndex += 20;
            SetPageInfo(++currentPage);
        }
    }

    private void InitGameNameBtnList()
    {
        btnList = new List<GameObject>();
        nameList = new List<string>();
        idList = new List<int>();
        var dicti = GameHistroyRecordController.gameNameDicti.OrderBy(kvp => kvp.Value);
        foreach (var item in dicti)
        {
            nameList.Add(item.Value);
            idList.Add(item.Key);
        }
        CreateBtn(currentIndex, currentIndex + 20);
        currentIndex += 20;
        double temp = GameHistroyRecordController.gameNameDicti.Count / 20.0f;
        Debug.LogError(temp);
        totatlPage = (int)Math.Ceiling(temp);
        currentPage = 1;
        SetPageInfo(currentPage);
    }

    private void SetPageInfo(int page)
    {
        pageTxt.text = string.Format("{0}/{1}", page, totatlPage);
    }

    private void CreateBtn(int start, int end)
    {
        if(end > nameList.Count)
        {
            end = nameList.Count;
        }
        if(start <= 0)
        {
            start = 0;
        }
        if (btnList.Count > 0)
        {
            HideAllBtn();
        }
        for (int i = start; i < end; i++)
        {
            GameObject go = null;
            if (i > btnList.Count - 1)
            {
                GameObject temp = GameObject.Instantiate(GameNameBtn.gameObject);
                temp.transform.SetParent(List.transform, false);
                btnList.Add(temp);
                go = temp;
            }
            else
            {
                go = btnList[i];
            }
            Button btn = go.GetComponent<Button>();
            go.SetActive(true);
            btn.onClick.RemoveAllListeners();
            TextMeshProUGUI text = go.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            text.text = nameList[i];
            int index = i;
            btn.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                Debug.LogError(nameList[index] + idList[index].ToString());
                controller.SetSearchGameInfo(nameList[index], idList[index]);
            });
        }
    }

    private void HideAllBtn()
    {
        for (int i = 0; i < btnList.Count; i++)
        {
            btnList[i].gameObject.SetActive(false);
        }
    }
}
