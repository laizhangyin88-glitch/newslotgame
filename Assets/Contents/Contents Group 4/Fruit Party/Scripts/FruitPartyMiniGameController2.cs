using GameUtil;
using SimpleJSON;
using SlotMaker;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class FruitPartyMiniGameController2 : MonoBehaviour
{
    public Transform point;

    public static FruitPartyMiniGameController2 Instance;

    public GameObject TargetScoreItem;

    public GameObject SelectItem;

    public Sprite[] sprites;

    private Transform TargetList;

    private Transform SelectList;

    private Text BetTxt;

    private Text TotalWinTxt;

    private Text CountDownTxt;

    public int currentClickIndex = 0;

    private int dataIndex = 0;

    private long CurrentTotalScore = 0;

    private Button MaskButton;

    private List<Game2Data> game2Datas;

    private int[] spriteIndexs;

    public long currentBet;

    private TargetScoreItemController[] targetScoreItemControllers = new TargetScoreItemController[4];
    private SelectItemController[] selectItemControllers = new SelectItemController[10];

    private LoopTimer _loopTimer;

    private int currentCountDown = 0;

    private bool isGameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    public void OnStart()
    {
        currentClickIndex = 0;
        dataIndex = 0;
        game2Datas = MiniGameDataManagers.Instance.game2Datas;
        currentBet = BlackboardUtils.FindVariable<long>("./betCredit").value;
        CurrentTotalScore = 0;
        isGameOver = false;
        

        TargetList = transform.Find("TargetList");
        SelectList = transform.Find("SelectList");
        BetTxt = transform.Find("Image/Bet").GetComponent<Text>();
        TotalWinTxt = transform.Find("Image/TotalWin").GetComponent<Text>();
        CountDownTxt = transform.Find("Image/CountDown").GetComponent<Text>();
        MaskButton = transform.Find("MaskButton").GetComponent<Button>();

        BetTxt.text = "BET \n" + currentBet.ToString("N0");

        MaskButton.gameObject.SetActive(false);
        InitTargetScore();
        InitSelectList();

        UpdateTargetScore();
        SetTotalScore(0);
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
            CountDownTxt.text =  currentCountDown.ToString();
            if(currentCountDown <= 0)
            {
                _loopTimer?.Cancel();
                AutoSelected();
                CountDownTxt.text = "";
            }
        });
    }

    private void AutoSelected()
    {
        if (!isGameOver)
        {
            int index = Random.Range(0, selectItemControllers.Length);
            while (selectItemControllers[index].isSelected)
            {
                index = Random.Range(0, selectItemControllers.Length);
            }
            selectItemControllers[index].OnClickButton();
        }
    }

    public int getClickSpriteIndex()
    {
        int result = 0;
        if(game2Datas != null && game2Datas.Count > 0 && dataIndex < game2Datas.Count)
        {
            result = game2Datas[dataIndex].card_index - 1;
        }
        return result; 
    }

    public void UpdateTargetScore()
    {
        var node = BlackboardUtils.GetOrCreateVariable<JSONNode>(ContentBlackboard.Get(), "MiniGameData");
        var mutiple_dict = node.value["ball"]["mutiple_dict"];
        var extern_mutiple_dict = node.value["ball"]["extern_mutiple_dict"];
        int[] totalScore = new int[mutiple_dict.Count];
        int[] score = new int[mutiple_dict.Count];
        spriteIndexs = new int[mutiple_dict.Count];
        int index = 0;
        foreach (var item in mutiple_dict)
        {
            spriteIndexs[index] = int.Parse(item.Key) - 1;
            score[index] = (item.Value);
            index++;
        }
        index = 0;
        foreach (var item in extern_mutiple_dict)
        {
            totalScore[index] = item.Value;
            index++;
        }
        index = 0;
        foreach (var item in targetScoreItemControllers)
        {
            item.SetItemData(totalScore[index], spriteIndexs[index], score[index]);
            index++;
        }
    }

    private void InitTargetScore()
    {
        for (int i = 0; i < 4; i++)
        {
            if (TargetScoreItem != null)
            {
                GameObject temp = Instantiate(TargetScoreItem);
                temp.transform.parent = TargetList;
                temp.transform.localScale = Vector3.one;
                temp.transform.localRotation = Quaternion.identity;
                TargetScoreItemController controller = temp.GetComponent<TargetScoreItemController>();
                targetScoreItemControllers[i] = controller;
            }
        }
    }

    private void InitSelectList()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject temp = Instantiate(SelectItem);
            temp.gameObject.SetActive(false);
            temp.transform.SetParent(SelectList);
            temp.transform.localScale = Vector3.one;
            temp.transform.localRotation = Quaternion.identity;
            SelectItemController controller = temp.GetComponent<SelectItemController>();
            controller.SelectIndex = i;
            controller.endPosition = SelectList.transform.GetChild(i).localPosition;
            selectItemControllers[i] = controller;
        }
        //int index = 0;
        //foreach (var item in selectItemControllers)
        //{
        //    this.DelayAction(index * 0.2f, () =>
        //    {
        //        item.PlayMoveAnamtion();
        //    });
        //    index++;
        //}
        PlaySelectsAnimation();
    }

    private void PlaySelectsAnimation()
    {
        foreach (var item in selectItemControllers)
        {
            item.gameObject.SetActive(false);
            item.Reset();
        }
        int index = 4;   
        for (int i = 0; i < 5; i++)
        {
            this.DelayAction(i * 0.2f, () =>
            {
                selectItemControllers[index].PlayMoveAnamtion();
                index--;
            });
        }
        int count = 5;
        for (int i = 5; i < 10; i++)
        {
            int time = i - 5;
            this.DelayAction(time * 0.2f, () =>
            {
                selectItemControllers[count].PlayMoveAnamtion();
                count++;
            });
        }
    }

    public void ClickSelectItem(int spriteIndex)
    {
        _loopTimer?.Cancel();
        currentClickIndex++;
        dataIndex++;
        foreach (var item in targetScoreItemControllers)
        {
            item.CompareSpriteIndex(spriteIndex);
        }
        if (currentClickIndex < 3)
        {
            StartCountDown(); 
        }
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void FinishGame()
    {
        _loopTimer?.Pause();
        isGameOver = true;
        MaskButton.gameObject.SetActive(true);
        this.DelayAction(2, () =>
        {
            if (dataIndex >= game2Datas.Count - 1)
            {
                MaskButton.gameObject.SetActive(true);
                foreach (var item in selectItemControllers)
                {
                    if (!item.isSelected)
                    {
                        int temp = Random.Range(0, spriteIndexs.Length);
                        item.SetSprite(spriteIndexs[temp]);
                        item.PlayAnimationAndShow();
                    }
                }
                this.DelayAction(4f, () =>
                {
                    gameObject.SetActive(false);
                    Clear();
                });
            }
        });
    }

    private void Clear()
    {
        _loopTimer?.Cancel();
        if (targetScoreItemControllers != null && targetScoreItemControllers.Length > 0)
        {
            for (int i = 0; i < targetScoreItemControllers.Length; i++)
            {
                Destroy(targetScoreItemControllers[i].gameObject);
                targetScoreItemControllers[i] = null;
            }
        }
        if(selectItemControllers != null && selectItemControllers.Length > 0)
        {
            for (int i = 0; i < selectItemControllers.Length; i++)
            {
                Destroy(selectItemControllers[i].gameObject);
                selectItemControllers[i] = null;
            }
        }
        MiniGameDataManagers.Instance.ResetAutoSpint();
        MiniGameDataManagers.Instance.ShowGameReward(CurrentTotalScore);
    }

    public void IsFinishGame()
    {
        if (currentClickIndex >= 3)
        {
            _loopTimer?.Cancel(); 
            MaskButton.gameObject.SetActive(true);
            foreach (var item in selectItemControllers)
            {
                item.SetColor(Color.gray);
            }
            this.DelayAction(2.5f, () =>
            {
                foreach (var item in selectItemControllers)
                {
                    if (!item.isSelected)
                    {
                        int temp = Random.Range(0, spriteIndexs.Length);
                        item.SetSprite(spriteIndexs[temp]);
                        item.PlayAnimationAndShow();
                    }
                }
            });
            if (!isGameOver)
            {
                this.DelayAction(5f, () =>
                {
                    Reset();
                });
            }
        }
    }

    public void Reset()
    {
        currentClickIndex = 0;
        PlaySelectsAnimation();
        MaskButton.gameObject.SetActive(false);
        StartCountDown();
    }

    public void SetTotalScore(int totalScore)
    {
        CurrentTotalScore += (totalScore * currentBet);
        TotalWinTxt.text = "Total Win \n" + CurrentTotalScore.ToString("N0");
    }
}
