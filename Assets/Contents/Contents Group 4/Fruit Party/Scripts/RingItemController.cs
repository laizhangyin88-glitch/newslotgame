using BagelCode;
using GameUtil;
using UnityEngine;
using UnityEngine.UI;

public class RingItemController : MonoBehaviour
{
    private Image icon;
    private GameObject select;
    private Sprite sprite1;
    private Sprite sprite2;
    private Image redIcon;

    public int itemIndex = 0;

    private bool isExit = false;

    private LoopTimer _loopTimer;

    // Start is called before the first frame update
    private void Start()
    {
        icon = transform.Find("icon").GetComponent<Image>();
        redIcon = transform.Find("redicon").GetComponent<Image>();
        redIcon.gameObject.SetActive(false);
        select = transform.Find("select").gameObject;
        select.SetActive(false);
        if (sprite1 != null)
        {
            icon.sprite = sprite1;
            if (itemIndex == 8 || itemIndex == 21 || itemIndex == 13 || itemIndex == 0)
            {
                icon.SetNativeSize();
                isExit = true;
            }
        }
        if(sprite2 != null)
        {
            redIcon.sprite = sprite2;
        }
    }

    public void SetSprite(Sprite sprite)
    {
        this.sprite1 = sprite;
    }
    public void SetRedSprite(Sprite sprite)
    {
        this.sprite2 = sprite;
    }

    public void SetSelectIsActive(bool isActive)
    {
        select.SetActive(isActive);
    }

    public void SelectOn()
    {
        select.gameObject.SetActive(true);
        redIcon.gameObject.SetActive(true);
        AsyncActionUtils.ApplyScaling(this, transform, Vector3.one, Vector3.one * 1.2f, FruitPartyMiniGameController1.Instance.intervalTime, TweenUtils.VectorTweenLinear, 0, () =>
        {
            transform.localScale = Vector3.one;
            select.gameObject.SetActive(false); 
            redIcon.gameObject .SetActive(false);
        });
    }

    public void PingPong()
    {
        Color color = Color.white;
        int count = 0;
        float timer = 1f;
        _loopTimer?.Cancel();
        redIcon.gameObject.SetActive(true); 
        _loopTimer = this.LoopAction(Time.deltaTime, (time) =>
        {
            color.a = Mathf.PingPong(5 * Time.time, 1f); 
            icon.color = color;
            redIcon.color = color;
            if ((timer -= Time.deltaTime) < 0)
            {
                timer = 1f;
                count++;
                if (count >= 3)
                {
                    icon.color = Color.white;
                    redIcon.color = Color.white;
                    _loopTimer.Cancel();
                }
            }
        });
    }
}
