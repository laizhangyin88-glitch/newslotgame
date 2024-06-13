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

    private int CurrentTotalScore = 0;

    private bool isGameOver = false;

    private Button MaskButton;

    private TargetScoreItemController[] targetScoreItemControllers = new TargetScoreItemController[4];
    private SelectItemController[] selectItemControllers = new SelectItemController[10];

    private void Awake()
    {
        Instance = this;
        isGameOver = false;
    }

    private void Start()
    {
        TargetList = transform.Find("TargetList");
        SelectList = transform.Find("SelectList");
        BetTxt = transform.Find("Image/Bet").GetComponent<Text>();
        TotalWinTxt = transform.Find("Image/TotalWin").GetComponent<Text>();
        CountDownTxt = transform.Find("Image/CountDown").GetComponent<Text>();
        MaskButton = transform.Find("MaskButton").GetComponent<Button>();

        MaskButton.gameObject.SetActive(false);
        InitTargetScore();
        InitSelectList();

        UpdateTargetScore();
        SetTotalScore(0);
    }

    public void UpdateTargetScore()
    {
        int[] totalScore = new int[4] { 1200, 800, 400, 100 };
        int[] spriteIndex = new int[4] { 3, 6, 9, 10 };
        int index = 0;
        foreach (var item in targetScoreItemControllers)
        {
            item.SetItemData(totalScore[index], spriteIndex[index]);
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
            //temp.gameObject.SetActive(true);
            temp.transform.SetParent(SelectList);
            temp.transform.localScale = Vector3.one;
            temp.transform.localRotation = Quaternion.identity;
            SelectItemController controller = temp.GetComponent<SelectItemController>();
            controller.SelectIndex = i;
            selectItemControllers[i] = controller;
        }
        MyTimerManagers.Instance.AddTimer(0.5f, 1, () =>
        {
            SelectList.GetComponent<GridLayoutGroup>().enabled = false;
        });
    }

    public void ClickSelectItem(int spriteIndex)
    {
        currentClickIndex++;
        foreach (var item in targetScoreItemControllers)
        {
            item.CompareSpriteIndex(spriteIndex);
        }
    }

    public void FinishGame()
    {
        Debug.LogError("游戏结束.........");
        isGameOver = true;
        MaskButton.gameObject.SetActive(true);
    }

    public void IsFinishGame()
    {
        if (currentClickIndex >= 3 && !isGameOver)
        {
            MaskButton.gameObject.SetActive(true);
            foreach (var item in selectItemControllers)
            {
                item.SetColor(Color.gray);
            }
            MyTimerManagers.Instance.AddTimer(2.5f, 1, () =>
            {
                foreach (var item in selectItemControllers)
                {
                    if (!item.isSelected)
                    {
                        item.PlayAnimationAndShow();
                    }
                }
            });
            MyTimerManagers.Instance.AddTimer(5f, 1, () =>
            {
                Reset();
            });
        }
    }

    public void Reset()
    {
        currentClickIndex = 0;
        for (int i = 0; i < selectItemControllers.Length; i++)
        {
            selectItemControllers[i].Reset();
        }
        MaskButton.gameObject.SetActive(false);
    }

    public void SetTotalScore(int totalScore)
    {
        CurrentTotalScore += totalScore;
        TotalWinTxt.text = "Total Win \n" + CurrentTotalScore.ToString();
    }
}
