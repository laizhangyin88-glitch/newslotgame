using GameUtil;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreItemController : MonoBehaviour
{
    [SerializeField]
    private Sprite[] ScoreSprites;

    //public static int[] scoreArray = new int[8] { 100, 70, 50, 20, 10, 6, 4, 2 };

    private Text score;
    private Image icon;

    public int Index = 0;

    public int currentScore;

    private LoopTimer _loopTimer;
    // Start is called before the first frame update
    void Start()
    {
        icon = transform.Find("icon").GetComponent<Image>();
        score = transform.Find("score").GetComponent<Text>();
        SetSprite();
    }
    
    public void SetSprite()
    {
        icon.sprite = ScoreSprites[Index];
        currentScore = FruitPartyMiniGameController1.Instance.scoreArray[Index];
    }

    public void SetScore(int score)
    {
        this.score.text = score.ToString();
    }

    public void Pingpong()
    {
        Color color = Color.white;
        int count = 0;
        float timer = 1f;
        _loopTimer?.Cancel();
        _loopTimer = this.LoopAction(Time.deltaTime, (time) =>
        {
            color.a = Mathf.PingPong(5 * Time.time, 1f);
            icon.color = color;
            if ((timer -= Time.deltaTime) < 0)
            {
                timer = 1f;
                count++;
                if (count >= 3)
                {
                    icon.color = Color.white;
                    _loopTimer.Cancel();
                }
            }
        });
    }
}
