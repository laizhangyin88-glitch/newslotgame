using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupWinLobbyJackpot : MonoBehaviour
{
    private TextMeshProUGUI content;
    private List<GameObject> titleList = new List<GameObject>();
    private Transform anchor;
    private Transform panel;
    private Button closeBtn;
    private bool canClose = false;

    void Start()
    {
        panel = transform.Find("panel");
        anchor = panel.Find("Anchor");
        var title = anchor.Find("Title");
        for (int i = 0; i < title.childCount; i++)
            titleList.Add(title.GetChild(i).gameObject);
        titleList.Reverse();
        content = anchor.Find("content").GetComponent<TextMeshProUGUI>();
        closeBtn = transform.GetComponent<Button>();
        closeBtn.onClick.AddListener(OnClickCloseBtnHandle);
        
        if (LobbyJackpotManager.Instance.IsWinLobbyJackpot == false)
        {
            Debug.LogError("不存在彩金数据");
            return;
        }

        var winResult = LobbyJackpotManager.Instance.GetNextJackpot();
        OnShow(winResult);
    }

    public void OnShow(WinResult winResult)
    {
        if(winResult == null)
        {
            Debug.Log("彩金数据异常");
            return;
        }

        canClose = false;

        panel.gameObject.SetActive(false);
        panel.gameObject.SetActive(true);
        
        ShowWinTips(winResult);
        StartCoroutine(DelayClose(6f));
    }

    private IEnumerator DelayClose(float delay)
    {
        yield return new WaitForSeconds(delay / 3f);

        canClose = true;

        yield return new WaitForSeconds(delay / 3f * 2f);

        if (LobbyJackpotManager.Instance.IsWinLobbyJackpot)
        {
            OnShow(LobbyJackpotManager.Instance.GetNextJackpot());
        }
        else
        {
            PopupManager.Instance.Close(gameObject);
            Destroy(gameObject);
        }
    }

    private void OnClickCloseBtnHandle()
    {
        if (canClose == false)
            return;

        StopAllCoroutines();

        if (LobbyJackpotManager.Instance.IsWinLobbyJackpot)
        {
            OnShow(LobbyJackpotManager.Instance.GetNextJackpot());
        }
        else
        {
            PopupManager.Instance.Close(gameObject);
            Destroy(gameObject);
        }
    }

    private void ShowWinTips(WinResult winResult)
    {
        titleList.ForEach(t => t.SetActive(false));
        titleList[winResult.bonus_id].SetActive(true);
        string titleStr = GetTitleStr(winResult.bonus_id);
        
        content.text = $"{winResult.nick_name} win {titleStr} jackpot $";
        content.text += GetNumStr(winResult.single_reward);
    }

    private string GetTitleStr(int bonus_id)
    {
        string titleStr = "";
        switch (bonus_id)
        {
            case 3: titleStr = "grand"; break;
            case 2: titleStr = "mega"; break;
            case 1: titleStr = "minor"; break;
            case 0: titleStr = "mini"; break;
            default: titleStr = ""; break;
        }

        return titleStr;
    }
    private string GetNumStr(int value)
    {
        string str;
        int tempValue = value % 100;
        string point;
        if (tempValue < 10)
            point = "0" + tempValue;
        else
            point = tempValue.ToString();
        value /= 100;
        if (value > 1000)
        {
            str = $"{(value / 1000)},";
            str += value % 1000;
        }
        else
            str = value.ToString();
        str += $".{point}";
        return str;
    }
}
