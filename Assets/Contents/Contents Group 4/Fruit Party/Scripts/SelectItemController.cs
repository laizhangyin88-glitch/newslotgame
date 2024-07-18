using BagelCode;
using GameUtil;
using SlotMaker;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectItemController : MonoBehaviour
{
    private Button button;
    private Image icon;

    private int spriteIndex = 0;

    public bool isSelected = false;

    [NonSerialized]
    public Vector3 endPosition;

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
    }

    private void AnimationFinishEvent()
    {
        ShowAnimation(); 
    }

    public void OnClickButton()
    {
        GSManager.Instance.GetHandler("game2_hit_D02").Play();
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
    }

    public void Reset()
    {
        isSelected = false;
        icon.gameObject.SetActive(false);
        button.gameObject.SetActive(true);
        SetColor(Color.white);
        frameImage.color = Color.white;
        //transform.localPosition = endPosition;
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
        this.gameObject.SetActive(true);
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
        result[2] = endPosition;
        transform.localPosition = result[0];
        loopTimer?.Cancel();
        AsyncActionUtils.ApplyLocalMovement(this, transform, transform.localPosition, result[1], 0.2f ,TweenUtils.VectorTweenLinear, 0, () =>
        {
            AsyncActionUtils.ApplyLocalMovement(this, transform, transform.localPosition, result[2], 0.2f, TweenUtils.VectorTweenLinear, 0, () =>
            {
                transform.localScale = Vector3.one * 1.2f;
                AsyncActionUtils.ApplyScaling(this, transform, Vector3.one * 1.2f, Vector3.one, 0.2f, TweenUtils.VectorTweenLinear);
            });
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
