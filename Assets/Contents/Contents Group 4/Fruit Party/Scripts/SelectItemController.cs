using BagelCode;
using GameUtil;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectItemController : MonoBehaviour
{
    private Button button;
    private Image icon;

    private int spriteIndex = 0;

    public bool isSelected = false;

    private Vector3 startPosition;

    [NonSerialized]
    public int SelectIndex = 0;

    private FrameAnimator frameAnimator;

    private Image frameImage;

    private LoopTimer loopTimer;

    // Start is called before the first frame update
    private void Awake()
    {
        button = transform.Find("Button").GetComponent<Button>();
        button.onClick.AddListener(OnClickButton);
        icon = transform.Find("icon").GetComponent<Image>();
        icon.gameObject.SetActive(false);

        frameAnimator = transform.Find("animation").GetComponent<FrameAnimator>();
        frameImage = frameAnimator.gameObject.GetComponent<Image>();

        frameAnimator.Loop = false;
        frameAnimator.Stop();
        frameAnimator.FinishEvent += AnimationFinishEvent;
        frameAnimator.gameObject.SetActive(false);
        this.DelayAction(1, () =>
        {
            startPosition = transform.localPosition;
            gameObject.SetActive(true);
            //SetSelectItemData(spriteIndex);
        });
    }

    private void AnimationFinishEvent()
    {
        ShowAnimation(); 
    }

    public void OnClickButton()
    {
        isSelected = true;
        button.gameObject.SetActive(false);
        frameAnimator.gameObject.SetActive(true);
        frameAnimator.Reset();
        frameAnimator.Play();

        this.spriteIndex = FruitPartyMiniGameController2.Instance.getClickSpriteIndex();
        icon.sprite = FruitPartyMiniGameController2.Instance.sprites[spriteIndex];
        
        FruitPartyMiniGameController2.Instance.ClickSelectItem(spriteIndex);
        FruitPartyMiniGameController2.Instance.IsFinishGame();
    }

    public void SetSprite(int index)
    {
        icon.sprite = FruitPartyMiniGameController2.Instance.sprites[index];
    }

    public void PlayAnimationAndShow()
    {
        frameAnimator.Play();
        frameImage.color = Color.gray;
        frameAnimator.gameObject.SetActive(true);
        button.gameObject.SetActive(false);
    }

    public void ShowAnimation()
    {
        frameAnimator.Stop();
        frameAnimator.gameObject.SetActive(false);
        icon.gameObject.SetActive(true);
        button.gameObject.SetActive(false);
        //transform.DOScale(Vector3.one * 1.5f, 0.5f).OnComplete(() =>
        //{
        //    transform.DOScale(Vector3.one, 0.5f);
        //});
    }

    public void Reset()
    {
        isSelected = false;
        icon.gameObject.SetActive(false);
        button.gameObject.SetActive(true);
        SetColor(Color.white);
        frameImage.color = Color.white;
        transform.localPosition = startPosition;
        spriteIndex = 0;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
        frameAnimator.FinishEvent -= AnimationFinishEvent;
    }

    public void SetColor(Color color)
    {
        if (!isSelected)
        {
            icon.color = color;
            button.GetComponent<Image>().color = color;
        }
    }

    public void PlayMoveAnamtion()
    {
        if (SelectIndex < 5)
        {
            DoPath(0, 2);
        }
        else
        {
            DoPath(2, 4);
        }
    }

    private void DoPath(int start, int end)
    {
        Vector3[] result = new Vector3[3];
        int index = 0;
        for (int i = start; i < end; i++)
        {
            Vector3 pos = FruitPartyMiniGameController2.Instance.point.GetChild(i).localPosition;
            result[index] = pos;
            index++;
        }
        result[2] = startPosition;
        transform.localScale = Vector3.one * 0.5f;
        transform.localPosition = result[0];
        loopTimer?.Cancel();
        //transform.rotation = Vector3.
        this.loopTimer = this.LoopAction(Time.deltaTime, null, (deltaTime) =>
        {

        });
        //transform.DOLocalPath(result, 2f).OnComplete(() =>
        //{
        //});
        //transform.DOScale(Vector3.one * 1.2f, 2f).OnComplete(() =>
        //{
        //    transform.localScale = Vector3.one;
        //});
    }
}
