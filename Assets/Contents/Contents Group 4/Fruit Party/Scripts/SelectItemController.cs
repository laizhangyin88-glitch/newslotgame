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
            transform.localPosition = Vector2.one * (SelectIndex < 5 ? -3000 : 3000);
            gameObject.SetActive(true);
            SetSelectItemData(spriteIndex);
        });
    }

    private void AnimationFinishEvent()
    {
        ShowAnimation();
    }

    public void SetSelectItemData(int spriteIndex)
    {
        this.spriteIndex = spriteIndex;
        if (FruitPartyMiniGameController2.Instance != null)
        {
            this.spriteIndex = UnityEngine.Random.Range(0, FruitPartyMiniGameController2.Instance.sprites.Length);
            icon.sprite = FruitPartyMiniGameController2.Instance.sprites[this.spriteIndex];
        }
        this.DelayAction(0.2f, () =>
        {
            button.gameObject.SetActive(true);
            PlayMoveAnamtion();
        });
    }

    private void OnClickButton()
    {
        isSelected = true;
        button.gameObject.SetActive(false);
        frameAnimator.gameObject.SetActive(true);
        frameAnimator.Reset();
        frameAnimator.Play();
        FruitPartyMiniGameController2.Instance.ClickSelectItem(spriteIndex);
        FruitPartyMiniGameController2.Instance.IsFinishGame();
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
        transform.localPosition = Vector2.one * (SelectIndex < 5 ? -3000 : 3000);
        icon.gameObject.SetActive(false);
        button.gameObject.SetActive(false);
        SetColor(Color.white);
        frameImage.color = Color.white;
        this.spriteIndex = UnityEngine.Random.Range(0, FruitPartyMiniGameController2.Instance.sprites.Length);
        SetSelectItemData(this.spriteIndex);
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
        //transform.DOLocalPath(result, 2f).OnComplete(() =>
        //{
        //});
        //transform.DOScale(Vector3.one * 1.2f, 2f).OnComplete(() =>
        //{
        //    transform.localScale = Vector3.one;
        //});
    }
}
