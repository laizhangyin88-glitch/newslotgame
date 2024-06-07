using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreItemController : MonoBehaviour
{
    [SerializeField]
    private Sprite[] ScoreSprites;

    public static int[] scoreArray = new int[8] { 100, 70, 50, 20, 10, 6, 4, 2 };

    private Text score;
    private Image icon;

    public int Index = 0;

    public int currentScore;

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
        currentScore = scoreArray[Index];
    }

    public void SetScore(int score)
    {
        this.score.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
