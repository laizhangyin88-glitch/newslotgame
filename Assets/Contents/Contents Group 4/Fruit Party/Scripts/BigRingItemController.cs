using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Reflection;

public class BigRingItemController : MonoBehaviour
{
    public float Speed = 10f;

    public float topPositionY;

    public float bottomPositionY;

    private Vector3 startPosition;

    private Vector3 endPosition;

    public bool isMove = false;

    public bool isStart = false;

    private float stopTime = 0;

    private Image icon;

    public int Index = 0;

    private float changeTime = 0.2f;

    public int ResultIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        stopTime = FruitPartyMiniGameController1.AnimationTime - 0.5f;
        icon = transform.Find("icon").GetComponent<Image>();
        RectTransform rectTransform = GetComponent<RectTransform>();
        MyTimerManagers.Instance.AddTimer(1, 1, () =>
        {
            startPosition = transform.localPosition;
        });
        SetSprite(FruitPartyMiniGameController1.Instance.sprites[Random.Range(0, FruitPartyMiniGameController1.Instance.sprites.Length)]);
    }

    public void SetSprite(Sprite sprite)
    {
        icon.sprite = sprite;
    }

    private void Update()
    {
        if (isMove)
        {
            if (!isStart)
            {
                isStart = true;
                MyTimerManagers.Instance.AddTimer(stopTime + Index * 0.2f, 1, () =>
                {
                    isStart= false;
                    isMove= false;
                    ResetPosition();
                    if (ResultIndex == 0)
                    {
                        PlayResult(Random.Range(0, FruitPartyMiniGameController1.Instance.sprites.Length));
                    }
                    else
                    {
                        PlayResult(ResultIndex);
                    }
                });
            }
            transform.localPosition += new Vector3(0, Speed * Time.deltaTime, 0);
            if(transform.localPosition.y < bottomPositionY)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, topPositionY, 0);
            }
            if((changeTime -= Time.deltaTime) < 0)
            {
                changeTime = 0.2f;
                SetSprite(FruitPartyMiniGameController1.Instance.sprites[Random.Range(0, FruitPartyMiniGameController1.Instance.sprites.Length)]);
            }
        }
    }

    public void ResetPosition()
    {
        transform.localPosition = startPosition;
    }

    public void PlayResult(int index)
    {
        SetSprite(FruitPartyMiniGameController1.Instance.sprites[FruitPartyMiniGameController1.Instance.SlotSpriteIndexArray[index]]);
        RectTransform rectTransform = GetComponent<RectTransform>();
        //transform.DOLocalMoveY(startPosition.y - rectTransform.rect.height, 0.2f);
    }
}
