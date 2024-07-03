using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardScoreItemController : MonoBehaviour
{
    private Text scoreTxt;
    private Image icon;

    public int spriteIndex;
    public int score;

    private int index = 0;
    // Start is called before the first frame update
    void Awake()  
    {
        icon = transform.Find("icon").GetComponent<Image>();
        scoreTxt = transform.Find("bg/score").GetComponent<Text>();
    }

    public void SetIndex(int index)
    {
        this.index = index; 
    }

    public void SetRewardScoreData(int spriteIndex, int score)
    {
        this.spriteIndex = spriteIndex;
        this.score = score;
        icon.sprite = FruitPartyMiniGameController3.Instance.sprites[this.spriteIndex];
        scoreTxt.text = this.score.ToString();
        if (spriteIndex == 9)
        {
            icon.SetNativeSize();
        }
    }
}

