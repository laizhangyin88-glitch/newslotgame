using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Globalization;
using SlotMaker;
using SimpleJSON;
using GameUtil;

public class FruitPartyMiniGameController3 : MonoBehaviour
{
    public static FruitPartyMiniGameController3 Instance;

    public Sprite[] sprites;

    public GameObject RewardScoreItem;
    public GameObject SelectStarItem;

    public Sprite ExitSprite;

    public Sprite BonusSprite;

    private Transform TargetList;
    private Transform SelectList;
    private Text BetTxt;

    private Text TotalWinTxt;

    private Text CountDownTxt;

    public long totalScore = 0;

    private Button MaskButton;

    private Text BonusText;

    private int bonusValue;

    private long currentBet;

    private int dataIndex = 0;

    private List<Game3Data> game3Datas;

    private int[] spriteIndexs;

    private LoopTimer _loopTimer;

    private int currentCountDown;

    private RewardScoreItemController[] rewardScoreItemControllers = new RewardScoreItemController[5];
    private SelectStarItemController[] selectStarItemControllers = new SelectStarItemController[6];

    private Transform point;


    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    public void OnStart()
    {
        dataIndex = 0;
        game3Datas = MiniGameDataManagers.Instance.game3Datas;
        currentBet = BlackboardUtils.FindVariable<long>("./betCredit").value;

        TargetList = transform.Find("TargetList");
        SelectList = transform.Find("SelectList");
        point = SelectList.transform.Find("point");
        BetTxt = transform.Find("Image/Bet").GetComponent<Text>();
        TotalWinTxt = transform.Find("Image/TotalWin").GetComponent<Text>();
        CountDownTxt = transform.Find("Image/CountDown").GetComponent<Text>();
        MaskButton = transform.Find("MaskButton").GetComponent<Button>();
        BonusText = transform.Find("Image/BonusText").GetComponent<Text>();
        MaskButton.gameObject.SetActive(false);

        BetTxt.text = "BET \n" + currentBet.ToString("N0");

        TotalWinTxt.text = "0";
        BonusText.text = "0";

        InitRewardScoreList();
        InitSelectList();

        UpdateRewarScoreList();
        this.DelayAction(0.5f, () =>
        {
            foreach (var item in selectStarItemControllers)
            {
                item.PlayStartAnimation();
            }
        });
        StartCountDown();
    }
    private void StartCountDown()
    {
        _loopTimer?.Cancel();
        currentCountDown = 5;
        CountDownTxt.text = currentCountDown.ToString();
        _loopTimer = this.LoopAction(1, (count) =>
        {
            currentCountDown--;
            CountDownTxt.text = currentCountDown.ToString();
            if (currentCountDown <= 0)
            {
                _loopTimer?.Cancel();
                AutoSelected();
                CountDownTxt.text = "";
            }
        });
    }

    private void AutoSelected()
    {
        int index = Random.Range(0, selectStarItemControllers.Length);
        selectStarItemControllers[index].OnClickButton();
    }
    public int getSpriteIndex()
    {
        int result = 0;
        if (game3Datas != null && game3Datas.Count > 0 && dataIndex < game3Datas.Count)
        {
            result = game3Datas[dataIndex].card_index - 1;
        }
        return result;
    }

    public void SetBonusValue()
    {
        BonusText.text = game3Datas[dataIndex].bonus.ToString();
        bonusValue = game3Datas[dataIndex].bonus;
        dataIndex++;
    }

    public void UpdateRewarScoreList()
    {
        var node = BlackboardUtils.GetOrCreateVariable<JSONNode>(ContentBlackboard.Get(), "MiniGameData");
        var mutiple_dict = node.value["star"]["mutiple_dict"];
        int[] scores = new int[mutiple_dict.Count];
        spriteIndexs = new int[mutiple_dict.Count];
        int index = 0;
        foreach (var item in mutiple_dict)
        {
            scores[index] = item.Value;
            if (int.Parse(item.Key) == 100)///退出的图标序号是 9 
            {
                spriteIndexs[index] = 9; 
            } 
            else
            {
                spriteIndexs[index] = int.Parse(item.Key) - 1;
            }
            index++;
        }
        index = 0;
        foreach (var item in rewardScoreItemControllers)
        {
            item.SetRewardScoreData(spriteIndexs[index], scores[index]);
            index++;
        }
    }

    private void InitRewardScoreList()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject temp = Instantiate(RewardScoreItem);
            temp.transform.parent = TargetList;
            temp.transform.localScale = Vector3.one;
            temp.transform.localRotation = Quaternion.identity;
            RewardScoreItemController controller = temp.GetComponent<RewardScoreItemController>();
            controller.SetIndex(i);
            rewardScoreItemControllers[i] = controller;
        }
    }

    private void InitSelectList()
    {
        for (int i = 0; i < 6; i++)
        {
            Transform parent = SelectList.GetChild(i);
            GameObject temp = Instantiate(SelectStarItem);
            temp.gameObject.SetActive(false);
            temp.transform.SetParent(parent);
            temp.transform.localScale = Vector3.one;
            temp.transform.localRotation = Quaternion.identity;
            temp.transform.localPosition = Vector3.zero;
            SelectStarItemController controller = temp.GetComponent<SelectStarItemController>();
            controller.endPosition = point.position;
            selectStarItemControllers[i] = controller;
        }
    }

    public void ClickStarItem(int spriteIndex)
    {
        MaskButton.gameObject.SetActive(true);
        _loopTimer?.Cancel();
        foreach (var item in rewardScoreItemControllers)
        {
            if (spriteIndex == item.spriteIndex) ///选中了，加分
            {
                totalScore += item.score * currentBet;
                TotalWinTxt.text = "Total Win \n" + totalScore.ToString("N0");
            }
        }
        if(spriteIndex == 100)///选中bonus，计算得分
        {
            totalScore += bonusValue * currentBet;
            TotalWinTxt.text = "Total Win \n" + totalScore.ToString("N0");
        }
        foreach (var item in selectStarItemControllers)
        {
            item.SetItemColor(Color.gray);
        }
        this.DelayAction(3, () =>
        {
            foreach (var item in selectStarItemControllers)
            {
                if (!item.isClick)
                {
                    int index = Random.Range(0, spriteIndexs.Length - 1);
                    item.UpdateStarItem(spriteIndexs[index]);
                    item.PlayAnimation();
                }
            }
            int temp = Random.Range(0, selectStarItemControllers.Length);
            if (spriteIndex != 9)///如果点击的是退出图标，这里就不再设置退出图标了
            {
                while (selectStarItemControllers[temp].isClick)
                {
                    temp = Random.Range(0, selectStarItemControllers.Length);
                }
                selectStarItemControllers[temp].SetExit(ExitSprite);
            }
            if (spriteIndex != 100)///如果点击的是bonus图标，这里就不再设置bonus图标了
            {
                while (selectStarItemControllers[temp].isExit || selectStarItemControllers[temp].isClick)
                {
                    temp = Random.Range(0, selectStarItemControllers.Length);
                }
                selectStarItemControllers[temp].UpdateStarItem(BonusSprite);
            }
        });
        if (spriteIndex != 9)
        {
            this.DelayAction(5, () =>
            {
                foreach (var item in selectStarItemControllers)
                {
                    item.Reset();
                }
                MaskButton.gameObject.SetActive(false);
                StartCountDown();
            });
        }
        else
        {
            this.DelayAction(6, () =>
            {
                gameObject.SetActive(false);
                Clear();
            });
        }
    }

    private void Clear()
    {
        if(selectStarItemControllers != null && selectStarItemControllers.Length > 0)
        {
            for (global::System.Int32 i = 0; i < selectStarItemControllers.Length; i++)
            {
                Destroy(selectStarItemControllers[i].gameObject);
                selectStarItemControllers[i] = null;
            }
        }
        if(rewardScoreItemControllers != null && rewardScoreItemControllers.Length > 0)
        {
            for (global::System.Int32 i = 0; i < rewardScoreItemControllers.Length; i++)
            {
                Destroy(rewardScoreItemControllers[i].gameObject);
                rewardScoreItemControllers[i] = null;
            }
        }
        MiniGameDataManagers.Instance.ResetAutoSpint();
        MiniGameDataManagers.Instance.ShowGameReward(totalScore);
    }
}




