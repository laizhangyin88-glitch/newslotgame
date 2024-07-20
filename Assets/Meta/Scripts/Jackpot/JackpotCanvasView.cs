using Newtonsoft.Json;
using NodeCanvas.Framework;
using ParadoxNotion;
using SimpleJSON;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JackpotCanvasView : MonoBehaviour
{
    private List<JackpotView> jackpotViews = new List<JackpotView>();
    private bool canUpdate = true;
    private GameObject effect;
    private Coroutine effectCorountine;
    private GameObject winTips;
    private List<GameObject> titleList = new List<GameObject>();
    private TextMeshProUGUI content;
    private List<Jackpot> tempJakcpot = new List<Jackpot>();
    private AnnounceView announceView;

    private AssetBundleLoadAssetOperation loadSceneInfoOperation = null;
    private SceneLoadOperation sceneLoadOperation = null;
    private List<string> assetName;

    private void Awake()
    {
        var trans = transform.Find("Anchor/JackpotParent");
        for (int i = 0; i < trans.childCount; i++)
            jackpotViews.Add(trans.GetChild(i).GetComponent<JackpotView>());
        effect = transform.Find("Anchor/Effect").gameObject;
        winTips = transform.Find("Anchor/WinTips").gameObject;
        announceView = transform.Find("Anchor/Announce").GetComponent<AnnounceView>();
        var titleTrans = winTips.transform.Find("Title");
        for (int i = 0; i < titleTrans.childCount; i++)
            titleList.Add(titleTrans.GetChild(i).gameObject);
        content = winTips.transform.Find("content").GetComponent<TextMeshProUGUI>();
        MessageDispatcher.Register("SetJackpot", OnSetJackpot);
        MessageDispatcher.Register("UpdateJackpot", OnUpdateJackpot);
        MessageDispatcher.Register(RPCName.winGameBonus, OnWinGameBounus);
        assetName = new List<string>
        {
            "Grand Jackpot Trigger Popup Scene",
            "Major Jackpot Trigger Popup Scene",
            "Minor Jackpot Trigger Popup Scene",
            "Mini Jackpot Trigger Popup Scene",
        };
    }

    private void OnDestroy()
    {
        MessageDispatcher.UnRegister("SetJackpot", OnSetJackpot);
        MessageDispatcher.UnRegister("UpdateJackpot", OnUpdateJackpot);
        MessageDispatcher.UnRegister(RPCName.winGameBonus, OnWinGameBounus);
    }

    private void OnSetJackpot(EventData data)
    {
        List<Jackpot> jackpots = JsonConvert.DeserializeObject<List<Jackpot>>(data.value.ToString());
        for (int i = 0; i < jackpotViews.Count; i++)
            jackpotViews[i].SetJackpot(jackpots[i].total_bonus_count);
    }

    private void OnUpdateJackpot(EventData data)
    {
        if (!canUpdate) return;
        List<Jackpot> jackpots = JsonConvert.DeserializeObject<List<Jackpot>>(data.value.ToString());
        UpdateJackpot(jackpots);
    }

    private void UpdateJackpot(List<Jackpot> jackpots)
    {
        for (int i = 0; i < jackpotViews.Count; i++)
            jackpotViews[i].ScrollTo(jackpots[i].total_bonus_count);
    }

    private void OnWinGameBounus(EventData eventData)
    {
        var data = eventData.value as JSONNode;
        if(!data.HasKey("win_result_list")) return;
        canUpdate = false;
        var winResultList = JsonConvert.DeserializeObject<List<WinResult>>(data["win_result_list"].ToString());
        tempJakcpot = JsonConvert.DeserializeObject<List<Jackpot>>(data["bonus_list"].ToString());
        string userId = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/userId").value;
        ShowEffect();
        for (int i = 0; i < winResultList.Count; i++)
        {
            var winResult = winResultList[i];
            if (winResult.user_id == userId)
                ShowWinJackpot(winResult);
            else
                DispatchWinJackpot(winResult);
        }
    }

    private void ShowWinJackpot(WinResult winResult)
    {
        var jackpotView = jackpotViews[winResult.bonus_id - 1];
        jackpotView.jackpot = jackpotView.jackpot - winResult.single_reward > 0 ? jackpotView.jackpot - winResult.single_reward : 0;
        jackpotView.SetJackpot(jackpotView.jackpot);
        ShowAnnounce(winResult);
        ShowWinTips(winResult);
        //StartCoroutine(LoadOrignalJackpot(winResult.bonus_id - 1, (long)winResult.single_reward));
    }

    private IEnumerator LoadOrignalJackpot(int index, long target)
    {
        if (loadSceneInfoOperation == null)
            loadSceneInfoOperation = AssetBundleManager.LoadAssetAsync<SceneInfoObject>(ApplicationSettings.MakeApplicationBundleName("lobby"), assetName[index]);
        while (!loadSceneInfoOperation.IsDone())
        { yield return new WaitForEndOfFrame(); }
        if (loadSceneInfoOperation.IsDone())
        {
            if (sceneLoadOperation == null)
            {
                Transform root = transform;
                var go = GameObject.Find("Popup Manager/Contents");
                if (go != null)
                    root = go.transform;

                var sceneInfo = loadSceneInfoOperation.GetAsset<SceneInfoObject>().GetSceneInfo();

                sceneLoadOperation = SceneManager.LoadSceneAsync(root, sceneInfo, true);
            }
            while (!sceneLoadOperation.IsDone())
            { yield return new WaitForEndOfFrame(); }
            if (sceneLoadOperation.IsDone())
            {
                var obj = sceneLoadOperation.GetScene();
                long orignal = 0;
                var contentBB = ContentBlackboard.Get();
                BlackboardUtils.SetOrCreateValue(contentBB, "totalBetCredit", orignal);
                var bonusBB = BlackboardUtils.GetOrCreateBlackboard(contentBB, "bonus");
                var responeseBB = BlackboardUtils.GetOrCreateBlackboard(bonusBB, "response");
                responeseBB.SetValue("earnCredit", target);
                PopupManager.Instance.Open(obj);
                obj.SetActive(true);
            }
        }
    }

    private void DispatchWinJackpot(WinResult winResult)
    {
        var jackpotView = jackpotViews[winResult.bonus_id - 1];
        jackpotView.jackpot = jackpotView.jackpot - winResult.single_reward > 0 ? jackpotView.jackpot - winResult.single_reward : 0;
        jackpotView.SetJackpot(jackpotView.jackpot);
        ShowAnnounce(winResult);
    }

    private void ShowAnnounce(WinResult winResult)
    {
        string titleStr = "";
        int index = winResult.bonus_id - 1;
        switch (index)
        {
            case 0: titleStr = "grand"; break;
            case 1: titleStr = "mega"; break;
            case 2: titleStr = "minor"; break;
            case 3: titleStr = "mini"; break;
        }
        string str = $"{winResult.nick_name} win {titleStr} jackpot $";
        str += GetNumStr(winResult.single_reward);
        announceView.AddMessage(str);
    }

    private void ShowWinTips(WinResult winResult)
    {
        int index = winResult.bonus_id - 1;
        winTips.SetActive(true);
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

    private void CreateFadeJackpot()
    {

        var spin = ContentBlackboard.Get().GetValue<Blackboard>("spin");
        var bonusResult = BlackboardUtils.GetOrCreateBlackboard(spin, "response");
        Blackboard blackboard = new Blackboard();
        blackboard.SetValue("bonusId", 2101);
        blackboard.SetValue("type", 1);
        blackboard.SetValue("earnCredit", 48300);
        BlackboardUtils.AddToBlackboardList(bonusResult, "bonusResult", blackboard);
    }

    private void HideWinTips()
    {
        winTips.SetActive(false);
        titleList.ForEach(t => t.SetActive(false));
        content.text = "";
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

    private void ShowEffect()
    {
        if (effectCorountine != null)
            StopCoroutine(effectCorountine);
        effectCorountine = StartCoroutine(ShowEffectEnumerator());
    }

    private IEnumerator ShowEffectEnumerator()
    {
        canUpdate = false;
        effect.SetActive(true);
        yield return new WaitForSeconds(5);
        effect.SetActive(false);
        HideWinTips();
        canUpdate = true;
        UpdateJackpot(tempJakcpot);
        tempJakcpot.Clear();
    }

    
}

public class Jackpot
{
    public int total_bonus_count; 
}

public class WinResult
{
    public string user_id;
    public string nick_name;
    public int single_reward;
    public int bonus_id;
}
