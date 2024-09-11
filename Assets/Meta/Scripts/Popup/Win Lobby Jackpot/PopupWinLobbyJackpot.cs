using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupWinLobbyJackpot : MonoBehaviour
{
    private TextMeshProUGUI content;
    private List<GameObject> titleList = new List<GameObject>();

    void Start()
    {
        var title = transform.Find("Title");
        for (int i = 0; i < title.childCount; i++)
            titleList.Add(title.GetChild(i).gameObject);
        content = transform.Find("content").GetComponent<TextMeshProUGUI>();

        var winResult = BlackboardUtils.FindVariable<WinResult>(MainBlackboard.Get(), "winLobbyJackpotResult").value;
        ShowWinTips(winResult);
        StartCoroutine(DelayClose());
    }

    private IEnumerator DelayClose()
    {
        yield return new WaitForSeconds(5);
        PopupManager.Instance.Close(gameObject);
        Destroy(gameObject);
    }

    private void ShowWinTips(WinResult winResult)
    {
        titleList.ForEach(t => t.SetActive(false));
        int index = winResult.bonus_id - 1;
        titleList[index].SetActive(true);
        string titleStr = "";
        switch (index)
        {
            case 0: titleStr = "grand"; break;
            case 1: titleStr = "mega"; break;
            case 2: titleStr = "minor"; break;
            case 3: titleStr = "mini"; break;
        }
        content.text = $"{winResult.nick_name} win {titleStr} jackpot $";
        content.text += GetNumStr(winResult.single_reward);
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
