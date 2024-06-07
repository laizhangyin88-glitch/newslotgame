using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Globalization;

public class FruitPartyMiniGameController3 : MonoBehaviour
{
    public static FruitPartyMiniGameController3 Instance;

    public Sprite[] sprites;

    public GameObject RewardScoreItem;
    public GameObject SelectStarItem;

    public Sprite ExitSprite;

    private Transform TargetList;
    private Transform SelectList;
    private Text BetTxt;

    private Text TotalWinTxt;

    private Text CountDownTxt;

    public int totalScore = 0;

    private Button MaskButton;

    private RewardScoreItemController[] rewardScoreItemControllers = new RewardScoreItemController[5];
    private SelectStarItemController[] selectStarItemControllers = new SelectStarItemController[6];

    private Transform point;

    private void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        TargetList = transform.Find("TargetList");
        SelectList = transform.Find("SelectList");
        point = SelectList.transform.Find("point");
        BetTxt = transform.Find("Image/Bet").GetComponent<Text>();
        TotalWinTxt = transform.Find("Image/TotalWin").GetComponent<Text>();
        CountDownTxt = transform.Find("Image/CountDown").GetComponent<Text>();
        MaskButton = transform.Find("MaskButton").GetComponent<Button>();
        MaskButton.gameObject.SetActive(false);

        TotalWinTxt.text = "0";

        InitRewardScoreList();
        InitSelectList();

        UpdateRewarScoreList();
    }

    public void UpdateRewarScoreList()
    {
        int[] scores = new int[5] { 900, 450, 270, 180, 180 };
        int[] spriteIndexs = new int[5] { 3, 5, 6, 9, 0 };
        int index = 0;
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
        //selectStarItemControllers[Random.Range(0, selectStarItemControllers.Length)].SetExit(ExitSprite);
    }

    public void ClickStarItem(int spriteIndex, bool isExit)
    {
        MaskButton.gameObject.SetActive(true);
        foreach (var item in rewardScoreItemControllers)
        {
            if(spriteIndex == item.spriteIndex) ///选中了，加分
            {
                totalScore += item.score;
                TotalWinTxt.text = "Total Win \n" + totalScore;
            }
        }
        foreach (var item in selectStarItemControllers)
        {
            item.SetItemColor(Color.gray);
        }
        MyTimerManagers.Instance.AddTimer(3, 1, () =>
        {
            foreach(var item in selectStarItemControllers)
            {
                if (!item.isClick)
                {
                    item.PlayAnimation();
                }
            }
        });

        //if(!isExit)
        {
            MyTimerManagers.Instance.AddTimer(5, 1, () =>
            {
                foreach (var item in selectStarItemControllers)
                {
                    item.Reset();
                }
                selectStarItemControllers[Random.Range(0, selectStarItemControllers.Length)].SetExit(ExitSprite);
                MaskButton.gameObject.SetActive(false);
            });            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
