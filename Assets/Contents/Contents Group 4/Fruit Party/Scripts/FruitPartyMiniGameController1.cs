using Boo.Lang;
using SlotMaker;
using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class FruitPartyMiniGameController1 : MonoBehaviour
{
    public static FruitPartyMiniGameController1 Instance;

    public Sprite ExitSprite;

    public static float AnimationTime = 3.0f;

    public float intervalTime = 0;
    [NonSerialized]
    public int laps = 3; 
     
    private List<int> fruitList = new List<int>() { 10, 5, 6, 7, 1, 8, 6, 4, 10, 2, 7, 8, 3, 10, 5, 6, 7, 9, 8, 6, 4, 10, 2, 7, 8, 3 };

    public Sprite[] spritesArray;
    public Sprite[] redSpritesArray;

    private int slotCount = 26;

    private Transform RingItem;
    private Transform ScoreItem;

    private Transform Ring;
    private Transform Score;

    private RingItemController[] ringItemControllers;
    private ScoreItemController[] scoreItemControllers;

    public long TotalWinScore = 0;

    private Text totalWin;
    private Text totalBet;
    private Text getScore;
    private FruitPartyMiniGameTigerMachine fruitPartyMiniGameTigerMachine;

    private int endNumber = 0;

    private int _selectIndex = 0;

    private bool isPlayAnimation = false;

    private float interval = 0.2f;

    private int currentNumber = 0;

    private int currentResultIndex = 0;

    private Game1Data currentGame1Data;
    [NonSerialized]
    public int[] scoreArray;

    /// <summary>
    /// 当前的倍率
    /// </summary>
    [NonSerialized]
    public long CurrentBet = 0;

    /// <summary>
    /// 转圈的结果数字
    /// </summary>
    public int SlotResultNumber = 0;


    private int[] _results;

    public int[] Results
    {
        get
        {
            if (_results == null)
            {
                _results = new int[fruitPartyMiniGameTigerMachine.row];
            }
            for (int i = 0; i < 3; i++)
            {
                //_results[i] = UnityEngine.Random.Range(0, FruitPartyMiniGameController1.Instance.SlotSpriteIndexArray.Length);
            }
            return _results;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public int SelectIndex
    {
        get { return _selectIndex; }

        set
        {
            ringItemControllers[_selectIndex].SelectOn();
            _selectIndex = value % (slotCount);
        }
    }

    private void InitScoreArray()
    {
        var node = BlackboardUtils.GetOrCreateVariable<JSONNode>(ContentBlackboard.Get(), "MiniGameData");
        var mutiple_dict = node.value["light"]["mutiple_dict"];
        scoreArray = new int[mutiple_dict.Count];
        int index = 0;
        foreach (var item in mutiple_dict)
        {
            scoreArray[index] = item.Value;
            index++;
        }
    }
     
    public void OnStart()
    {
        InitScoreArray();
        CurrentBet = BlackboardUtils.FindVariable<long>("./betCredit").value;
        isPlayAnimation = false;
        currentNumber = 0;
        currentResultIndex = 0;
        currentGame1Data = null;

        TotalWinScore = 0;
        _selectIndex = 0;
        RingItem = transform.Find("RingItem");
        ScoreItem = transform.Find("ScoreItem");
        Ring = transform.Find("Ring");
        Score = transform.Find("Score"); 

        totalWin = transform.Find("Image1/Total Win").GetComponent<Text>();
        totalBet = transform.Find("Image2/Total Bet").GetComponent<Text>();
        getScore = transform.Find("Image3/Get Score").GetComponent<Text>();

        totalWin.text = "0";
        totalBet.text = CurrentBet.ToString();

        InitView();
        fruitPartyMiniGameTigerMachine = transform.Find("Selected").GetComponent<FruitPartyMiniGameTigerMachine>();
        StartMachine(2);
    }

    private void InitView()
    {
        InitRing();
        InitScore();
    }

    private void InitRing()
    {
        int count = Ring.childCount;
        if (ringItemControllers == null || ringItemControllers.Length == 0)
        {
            ringItemControllers = new RingItemController[slotCount];
        }
        int index = 0;
        for (int i = 0; i < count; i++)
        {
            Transform child = Ring.GetChild(i);
            for (int j = 0; j < child.childCount; j++)
            {
                Transform parent = child.GetChild(j);
                GameObject temp = GameObject.Instantiate(RingItem.gameObject);
                temp.transform.parent = parent;
                temp.transform.localPosition = Vector3.zero;
                temp.transform.localRotation = Quaternion.identity;
                temp.transform.localScale = Vector3.one;
                RingItemController controller = temp.GetComponent<RingItemController>();
                controller.itemIndex = index;
                //SlotSpriteIndexArray[index] = tempIndex;
                int tempIndex = fruitList[index] - 1;
                controller.SetSprite(spritesArray[tempIndex]);
                controller.SetRedSprite(redSpritesArray[tempIndex]);
                ringItemControllers[index] = controller;
                index++;
            }
        }
    }

    private void InitScore()
    {
        scoreItemControllers = new ScoreItemController[8];
        for (int i = 0; i < 8; i++)
        {
            GameObject temp = GameObject.Instantiate(ScoreItem.gameObject);
            temp.transform.parent = Score;
            temp.transform.localPosition = Vector3.zero;
            temp.transform.localRotation = Quaternion.identity;
            temp.transform.localScale = Vector3.one;
            ScoreItemController controller = temp.GetComponent<ScoreItemController>();
            controller.Index = i;
            scoreItemControllers[i] = controller;
        }
    }

    public void PlayRingAnimation()
    {
        isPlayAnimation = true;
         
        currentGame1Data = MiniGameDataManagers.Instance.game1Datas[this.currentResultIndex];
        SlotResultNumber = getCardIndexPos(currentGame1Data.card_index);

        endNumber = laps * slotCount + SlotResultNumber;
        intervalTime = (AnimationTime / (endNumber + _selectIndex));
        interval = intervalTime;
        this.currentResultIndex++;    
    }

    private int getCardIndexPos(int index)
    {
        int start = _selectIndex;
        while (fruitList[start] != index)
        {
            start++;
            if(start >= fruitList.Count)
            {
                start = 0;
            }
        }
        return start;
    }

    private void FinishAnimation()
    {
        isPlayAnimation = false;
        _selectIndex = endNumber % (slotCount);
        currentNumber = _selectIndex;
        this.LoopCountAction(0.2f, 1, (count) =>
        {
            ringItemControllers[_selectIndex].SetSelectIsActive(true);
            ShowItemPingPong(ringItemControllers[_selectIndex]);
        }); 
        fruitPartyMiniGameTigerMachine.PlayResult(currentGame1Data.middle_list);
        CalculateScore();
        float time = 4;
        ///击中的分数
        if(currentGame1Data.round_mutiple > 0)
        {
            time = 5;
        }

        if (this.currentResultIndex < MiniGameDataManagers.Instance.game1Datas.Count)
        {
            StartMachine(time);
        }
        else
        {
            Debug.LogError("小游戏结束了................");
            this.DelayAction(time, () =>
            {
                Clear();
                gameObject.SetActive(false);
            });
        }
    }

    private void Clear()
    {
        if(ringItemControllers.Length > 0)
        {
            for (global::System.Int32 i = 0; i < ringItemControllers.Length; i++)
            {
                Destroy(ringItemControllers[i].gameObject, i * 0.2f);
                ringItemControllers[i] = null;
            }
        }
        if(scoreItemControllers.Length > 0)
        {
            for (global::System.Int32 i = 0; i < scoreItemControllers.Length; i++)
            {
                Destroy(scoreItemControllers[i].gameObject, i * 0.2f);
                scoreItemControllers[i] = null;
            }
        }
    }

    private void ShowItemPingPong(RingItemController ringItemController)
    {
        ringItemController.PingPong();
    }

    private void CalculateScore()
    {
        int[] hits = currentGame1Data.middle_list;
        long score = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] > 0)
            {
                if (hits[i] == currentGame1Data.card_index || currentGame1Data.card_index == 9 || hits[i] == 9) //万能图标都可以中
                {
                    int temp = scoreArray[hits[i] - 1];
                    score += (temp * CurrentBet);
                    fruitPartyMiniGameTigerMachine.Pingpong(i);
                    scoreItemControllers[hits[i] - 1].Pingpong();
                }
            }
        }
        TotalWinScore += score;
        totalWin.text = TotalWinScore.ToString();
    }

    private void Update()
    {
        if(isPlayAnimation)
        {
            interval -= Time.deltaTime;
            if(interval < 0)
            {
                interval = intervalTime;
                currentNumber++;
                SelectIndex = currentNumber;
                if(currentNumber >= endNumber)
                {
                    FinishAnimation();
                }
            }
        }
    }
    public void StartMachine(float time)
    {
        this.DelayAction(time, () =>
        {
            PlayRingAnimation();
            fruitPartyMiniGameTigerMachine.PlaySlotAnimation();
        });
    }
}
