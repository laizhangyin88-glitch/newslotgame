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
        titleList.Reverse();
        content = winTips.transform.Find("content").GetComponent<TextMeshProUGUI>();
        MessageDispatcher.Register("SetJackpot", OnSetJackpot);
        MessageDispatcher.Register("UpdateJackpot", OnUpdateJackpot);
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
    }

    private void OnSetJackpot(EventData data)
    {
        var dataList = data.value as List<int>;
        List<Jackpot> jackpots = new List<Jackpot>();
        for (int i = 0; i < dataList.Count; i++)
            jackpots.Add(new Jackpot { total_bonus_count = dataList[i] }); 
        for (int i = 0; i < jackpotViews.Count; i++)
            jackpotViews[i].SetJackpot(jackpots[i].total_bonus_count);
    }

    private void OnUpdateJackpot(EventData data)
    {
        if (!canUpdate) return;
        var jsonData = data.value as JSONNode;

        List<int> jacks = new List<int>();
        for (int i = 0; i < jsonData["remain_jackpot_list"].Count; i++)
        {
            var temp = (float)jsonData["remain_jackpot_list"][i];
            temp *= 100;
            var str = temp.ToString();
            str = str.Split('.')[0];
            jacks.Add(int.Parse(str));
        }

        List<Jackpot> jackpots = new List<Jackpot>();
        jacks.ForEach(j =>
        {
            jackpots.Add(new Jackpot { total_bonus_count = j });
        });
        jackpots.Reverse();

        UpdateJackpot(jackpots);

        if (jsonData.HasKey("winner_user_id"))
        {
            string userId = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/userId").value;
            var winnerId = jsonData["winner_user_id"];
            WinResult winResult = new WinResult
            {
                user_id = winnerId.ToString(),
                nick_name = jsonData["winner_nick_name"],
                single_reward = (int) ((float)jsonData["earn_money"] * 100 ),
                bonus_id = jsonData["jackpot_id"]
            };

            if (winnerId != 0)
            {

                tempJakcpot = jackpots;
                ShowEffect();
                if (winnerId == userId)
                    ShowWinJackpot(winResult);
                else
                    DispatchWinJackpot(winResult);
            }
        }
    }

    private void UpdateJackpot(List<Jackpot> jackpots)
    {
        for (int i = 0; i < jackpotViews.Count; i++)
            jackpotViews[i].ScrollTo(jackpots[i].total_bonus_count);
    }

    private void ShowWinJackpot(WinResult winResult)
    {
        var mgr = GSManager.Instance;
        if (mgr != null) mgr.GetHandler("SFX_Coin_Drop").Play();
        var jackpotView = jackpotViews[winResult.bonus_id];
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
        if (ApplicationSettings.Instance.isMachine)
        {
            var mgr = GSManager.Instance;
            if (mgr != null) mgr.GetHandler("SFX_Coin_Drop").Play();
        }
        var jackpotView = jackpotViews[winResult.bonus_id];
        jackpotView.jackpot = jackpotView.jackpot - winResult.single_reward > 0 ? jackpotView.jackpot - winResult.single_reward : 0;
        jackpotView.SetJackpot(jackpotView.jackpot);
        ShowAnnounce(winResult);
    }

    private void ShowAnnounce(WinResult winResult)
    {
        string titleStr = "";
        int index = winResult.bonus_id;
        switch (index)
        {
            case 3: titleStr = "grand"; break;
            case 2: titleStr = "mega"; break;
            case 1: titleStr = "minor"; break;
            case 0: titleStr = "mini"; break;
        }
        string str = $"{winResult.nick_name} win {titleStr} jackpot $";
        str += GetNumStr(winResult.single_reward);
        announceView.AddMessage(str);
    }

    private void ShowWinTips(WinResult winResult)
    {
        titleList.ForEach(t => t.SetActive(false));
        int index = winResult.bonus_id;
        winTips.SetActive(true);
        titleList[index].SetActive(true);
        string titleStr = "";
        switch (index)
        {
            case 3: titleStr = "grand"; break;
            case 2: titleStr = "mega"; break;
            case 1: titleStr = "minor"; break;
            case 0: titleStr = "mini"; break;
        }
        content.text = $"{winResult.nick_name} win {titleStr} jackpot ";
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

    private string GetNumStr(int num)
    {
        string str = "$";
        string temp = (num % 10).ToString();
        num /= 10;
        temp = (num % 10).ToString() + temp;
        num /= 10;
        if (num > 999)
        {
            str += num / 1000;
            str += ",";
            num %= 1000;
            for (int i = 0; i < 3 - num.ToString().Length; i++)
                str += '0';
            str += num;
            str += ".";
            str += temp;
        }
        else
        {
            str += num % 1000;
            str += ".";
            str += temp;
        }
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
        yield return new WaitForSeconds(10);
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

    public override string ToString()
    {
        return $"user_id : {user_id}; nick_name : {nick_name}; single_reward : {single_reward}; bonus_id : {bonus_id};";
    }
}
