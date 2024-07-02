using GameUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetScoreItemController : MonoBehaviour
{
    private Text TotalScore_Txt;

    private Text CurrentScore_Txt;

    private Transform Icons;

    private Image TargetIcon;

    public int totalScore;

    public int spriteIndex;

    public int activeIndex = 0;

    public int currentScore = 0;

    private Image[] images = new Image[3];

    private LoopTimer _loopTimer;

    // Start is called before the first frame update
    void Awake()
    {
        activeIndex = 0;
        currentScore = 0;
        TotalScore_Txt = transform.Find("TotalScore").GetComponent<Text>();
        Icons = transform.Find("Icons");
        TargetIcon = transform.Find("Image_bg/targetIcon").GetComponent<Image>();
        CurrentScore_Txt = transform.Find("Image_bg/CurrentScore").GetComponent<Text>();
    }


    public void SetItemData(int totalScore, int spriteIndex, int score)
    {
        this.totalScore = totalScore;
        this.spriteIndex = spriteIndex;
        this.currentScore = score;
        TotalScore_Txt.text = totalScore.ToString();
        CurrentScore_Txt.text = currentScore.ToString();
        SetSpriteList();
        TargetIcon.sprite = FruitPartyMiniGameController2.Instance.sprites[spriteIndex];
        
    }

    private void SetSpriteList()
    {
        for (int i = 0; i < Icons.childCount; i++)
        {
            Image image = Icons.GetChild(i).GetComponent<Image>();
            if (image != null)
            {
                image.sprite = FruitPartyMiniGameController2.Instance.sprites[spriteIndex];
                image.color = Color.gray;
                images[i] = image;
            }
        }
    }

    public void CompareSpriteIndex(int spriteIndex)
    {
        if(this.spriteIndex == spriteIndex)
        {
            if (activeIndex < images.Length)
            {
                images[activeIndex].color = Color.white;
            }
            int score = currentScore;
            FruitPartyMiniGameController2.Instance.SetTotalScore(score);
            activeIndex++;
            PingPong();
            if(activeIndex >= 3)
            {
                FruitPartyMiniGameController2.Instance.FinishGame();
                FruitPartyMiniGameController2.Instance.SetTotalScore(totalScore);
            }
        }
    }

    private void InitTotalScore()
    {

    }

    private void PingPong()
    {
        List<Image> images = new List<Image>();
        Image img1 = transform.Find("Image_bg").GetComponent<Image>();
        images.Add(img1);
        images.Add(TargetIcon);
        Color color = Color.white;
        int count = 0;
        float timer = 1f;
        int id = 0;
        _loopTimer?.Cancel();
        _loopTimer = this.LoopAction(Time.deltaTime, (time) =>
        {
            color.a = Mathf.PingPong(5 * Time.time, 1f);
            foreach(Image img in images)
            {
                img.color = color;
            }
            if ((timer -= Time.deltaTime) < 0)
            {
                timer = 1f;
                count++;
                if (count >= 3)
                {
                    foreach (Image img in images)
                    {
                        img.color = Color.white;
                    }
                    _loopTimer.Cancel();
                }
            }
        });
    }

}
