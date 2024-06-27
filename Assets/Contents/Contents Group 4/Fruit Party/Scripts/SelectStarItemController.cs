using BagelCode;
using GameUtil;
using UnityEngine;
using UnityEngine.UI;

public class SelectStarItemController : MonoBehaviour
{
    private Button button;
    private Image icon;

    private int spriteIndex = 0;

    public bool isExit = false;

    private FrameAnimator frameAnimator;

    private Image frameImage;

    public bool isClick = false;

    public Vector3 endPosition;

    private Vector3 startPosition;

    private LoopTimer _loopTimer;

    private void Awake()
    {
        isExit = false;
        button = transform.Find("Button").GetComponent<Button>();

        frameAnimator = transform.Find("animation").GetComponent<FrameAnimator>();
        frameImage = frameAnimator.gameObject.GetComponent<Image>();
        frameAnimator.Stop();
        frameAnimator.gameObject.SetActive(false);
        frameAnimator.FinishEvent += AnimationFinish;
        frameAnimator.Loop = false;

        button.onClick.AddListener(OnClickButton);
        icon = transform.Find("icon").GetComponent<Image>();
        icon.gameObject.SetActive(false);

        //this.DelayAction(1, () =>
        //{
        //    PlayStartAnimation();
        //});
    }

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    public void SetItemColor(Color color)
    {
        if (!isClick)
        {
            icon.color = color;
            button.gameObject.GetComponent<Image>().color = color;
        }
    }

    private void AnimationFinish()
    {
        frameAnimator.gameObject.SetActive(false);
        ShowIconAnimation();
    }

    private void ShowIconAnimation()
    {
        icon.gameObject.SetActive(true);
        //transform.DOScale(Vector3.one * 1.2f, 0.2f).OnComplete(() =>
        //{
        //    transform.localScale = Vector3.one;
        //});
    }

    public void OnClickButton()
    {
        isClick = true;
        spriteIndex = FruitPartyMiniGameController3.Instance.getSpriteIndex();
        UpdateStarItem(spriteIndex);
        button.gameObject.SetActive(false);
        PlayAnimation();
        frameImage.color = Color.white;
        FruitPartyMiniGameController3.Instance.ClickStarItem(spriteIndex);
        FruitPartyMiniGameController3.Instance.SetBonusValue();
    }

    public void PlayAnimation()
    {
        frameAnimator.gameObject.SetActive(true);
        frameImage.color = Color.gray;
        button.gameObject.SetActive(false);
        frameAnimator.Reset();
        frameAnimator.Play();
    }

    public void UpdateStarItem(int spriteIndex)
    {
        if (FruitPartyMiniGameController3.Instance != null)
        {
            if (spriteIndex == 100)///bonus图标额外设置
            {
                icon.sprite = FruitPartyMiniGameController3.Instance.BonusSprite;
            }
            else
            {
                icon.sprite = FruitPartyMiniGameController3.Instance.sprites[spriteIndex];
            }
        }
    }

    public void UpdateStarItem(Sprite sprite)
    {
        icon.sprite = sprite;
    }

    public void Reset()
    {
        icon.gameObject.SetActive(false);
        button.gameObject.SetActive(true);
        isExit = false;
        frameImage.color = Color.white;
        frameAnimator.gameObject.SetActive(false);
        spriteIndex = -10;
        frameImage.color = Color.white;
        SetItemColor(Color.white);
        isClick = false;
        PlayStartAnimation();
    }

    public void Show()
    {
        button.gameObject.SetActive(false);
        icon.gameObject.SetActive(true);
    }

    public void SetExit(Sprite sprite)
    {
        isExit = true;
        icon.sprite = sprite;
    }



    private void OnDestroy()
    {
        frameAnimator.FinishEvent -= AnimationFinish;
    }

    public void PlayStartAnimation()
    {
        gameObject.SetActive(true);
        transform.position = endPosition;
        transform.localScale = Vector3.one * 0.2f;
        //transform.DORotate(new Vector3(0, 0, 360), 1.5f, RotateMode.FastBeyond360).SetEase(Ease.InCubic);
        //transform.DOScale(Vector3.one, 1.5f);
        //transform.DOLocalMove(Vector3.zero, 1.5f);
        AsyncActionUtils.ApplyRotation(this, transform, Vector3.zero, new Vector3(0, 0, 720), 0.5f, TweenUtils.VectorTweenLinear);
        AsyncActionUtils.ApplyScaling(this, transform, Vector3.one, Vector3.one, 0.5f, TweenUtils.VectorTweenLinear);
        AsyncActionUtils.ApplyLocalMovement(this, transform, transform.localPosition, Vector3.zero, 0.5f, TweenUtils.VectorTweenLinear);
    }

    public void PingPong()
    {
        Color color = Color.white;
        int count = 0;
        float timer = 1f;
        int id = 0;
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
