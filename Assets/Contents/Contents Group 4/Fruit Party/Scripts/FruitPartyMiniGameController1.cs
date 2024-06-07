using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Runtime.CompilerServices;


public class FruitPartyMiniGameController1 : MonoBehaviour
{
    public static FruitPartyMiniGameController1 Instance;

    public Sprite ExitSprite;

    public static float AnimationTime = 3.0f;

    public int laps = 5;

    public Sprite[] sprites;

    private int slotCount = 26;

    private GameObject RingItem;
    private GameObject ScoreItem;
    private GameObject BigRingItem;

    private Transform Ring;
    private Transform Score;
    private Transform Selected;

    private RingItemController[] ringItemControllers;
    private ScoreItemController[] scoreItemControllers;
    private BigRingItemController[] bigRingItemControllers;

    public int TotalWinScore = 0;

    private Text totalWin;
    private Text totalBet;
    private Text getScore;
    private FruitPartyMiniGameTigerMachine fruitPartyMiniGameTigerMachine;

    private int endNumber = 0;


    private int _selectIndex = 0;
    /// <summary>
    /// 当前的倍率
    /// </summary>
    public int CurrentBet = 60;
    /// <summary>
    /// 转圈的结果数字
    /// </summary>
    public int SlotResultNumber = 0;

    /// <summary>
    /// 转圈的图标的数组
    /// </summary>
    public int[] SlotSpriteIndexArray;

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
                _results[i] = UnityEngine.Random.Range(0, FruitPartyMiniGameController1.Instance.SlotSpriteIndexArray.Length);
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

        set {
            ringItemControllers[_selectIndex].SelectOn();
            _selectIndex = value % (slotCount);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        TotalWinScore = 0;
        _selectIndex = 0;
        SlotSpriteIndexArray = new int[slotCount];
        RingItem = transform.Find("RingItem").gameObject;
        ScoreItem = transform.Find("ScoreItem").gameObject;
        BigRingItem = transform.Find("BigRingItem").gameObject;
        Ring = transform.Find("Ring");
        Score = transform.Find("Score");
        Selected = transform.Find("Selected");

        totalWin = transform.Find("Image1/Total Win").GetComponent<Text>();
        totalBet = transform.Find("Image2/Total Bet").GetComponent<Text>();
        getScore = transform.Find("Image3/Get Score").GetComponent<Text>();

        totalWin.text = "0";
        totalBet.text = CurrentBet.ToString();

        InitView();
        fruitPartyMiniGameTigerMachine = transform.Find("Selected").GetComponent<FruitPartyMiniGameTigerMachine>();
    }

    private void InitView()
    {
        InitRing();
        InitScore();
        //InitSelected();
    }

    private void InitRing()
    {
        int count = Ring.childCount;
        if(ringItemControllers == null || ringItemControllers.Length == 0)
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
                GameObject temp = GameObject.Instantiate(RingItem);
                temp.transform.parent = parent;
                temp.transform.localPosition = Vector3.zero;
                temp.transform.localRotation = Quaternion.identity;
                temp.transform.localScale = Vector3.one;
                RingItemController controller = temp.GetComponent<RingItemController>();
                controller.itemIndex = index;
                int tempIndex = UnityEngine.Random.Range(0, sprites.Length);
                SlotSpriteIndexArray[index] = tempIndex;
                controller.SetSprite(sprites[tempIndex]);
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
            GameObject temp = GameObject.Instantiate(ScoreItem);
            temp.transform.parent = Score;
            temp.transform.localPosition = Vector3.zero;
            temp.transform.localRotation = Quaternion.identity;
            temp.transform.localScale = Vector3.one;
            ScoreItemController controller = temp.GetComponent<ScoreItemController>();
            controller.Index = i;
            scoreItemControllers[i] = controller;
        }
    }

    private void InitSelected()
    {
        bigRingItemControllers = new BigRingItemController[3];
        for (int i = 0; i < 3; i++)
        {
            GameObject temp = GameObject.Instantiate(BigRingItem);
            temp.transform.parent = Selected;
            temp.transform.localPosition = Vector3.zero;
            temp.transform.localRotation = Quaternion.identity;
            temp.transform.localScale = Vector3.one;
            BigRingItemController controller = temp.GetComponent<BigRingItemController>();
            bigRingItemControllers[i] = controller;
        }
    }

    public void PlayRingAnimation()
    {
        
        //DOTween.To(() => SelIndex, x => SelIndex = x, endNum, animTime).SetEase(Ease.InOutQuad).OnComplete(() => FinishAnim());
        SlotResultNumber = UnityEngine.Random.Range(0, slotCount);
       
        int endNum = laps * slotCount + SlotResultNumber;
        //endNum = ;
        endNumber = endNum;
        DOTween.To(() => SelectIndex, x => SelectIndex = x, endNum, AnimationTime).SetEase(Ease.InOutQuad).OnComplete(() => FinishAnimation());
    }
    private void FinishAnimation()
    {
        _selectIndex = endNumber % (slotCount);
        MyTimerManagers.Instance.AddTimer(0.2f, 1, () =>
        {
            ringItemControllers[_selectIndex].SetSelectIsActive(true);
            ShowItemPingPong(ringItemControllers[_selectIndex]);
        });
        CalculateScore();
    }

    private void ShowItemPingPong(RingItemController ringItemController)
    {
        ringItemController.PingPong();
    }

    private void CalculateScore()
    {
        int[] hits = new int[3];
        int[] results = _results;
        for (int i = 0; i < results.Length; i++)
        {
            if (sprites[SlotSpriteIndexArray[SlotResultNumber]] == sprites[SlotSpriteIndexArray[results[i]]])
            {
                hits[i] = SlotSpriteIndexArray[SlotResultNumber];
            }
        }
        int score = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] > 0)
            {
                int temp = ScoreItemController.scoreArray[hits[i]];
                score += (ScoreItemController.scoreArray[hits[i]] * CurrentBet);
            }
        }
        //Debug.Log("增加的分数...." + score);
        TotalWinScore += score;
        //Debug.Log("总分...." + TotalWinScore);
        totalWin.text = TotalWinScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            PlayRingAnimation();
            fruitPartyMiniGameTigerMachine.PlaySlotAnimation();
        }
    }


}
