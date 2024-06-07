using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RingItemController : MonoBehaviour
{
    private Image icon;
    private GameObject select;
    private Sprite sprite1;

    public int itemIndex = 0;

    private bool isExit = false;

    // Start is called before the first frame update
    void Start()
    {
        icon = transform.Find("icon").GetComponent<Image>();
        select = transform.Find("select").gameObject;
        select.SetActive(false);
        if (sprite1 != null)
        {
            if (itemIndex == 8 || itemIndex == 21 || itemIndex == 13 || itemIndex == 0)
            {
                icon.sprite = FruitPartyMiniGameController1.Instance.ExitSprite;
                icon.SetNativeSize();
                isExit = true;
            }
            else
            {
                icon.sprite = sprite1;
            }
        }
    }

    public void SetSprite(Sprite sprite)
    {
        this.sprite1 = sprite;
    }

    public void SetSelectIsActive(bool isActive)
    {
        select.SetActive(isActive);
    }

    public void SelectOn()
    {
        select.gameObject.SetActive(true);
        transform.DOScale(1.5f * Vector3.one, 0.1f).onComplete = () => {
            transform.localScale = Vector3.one;
            select.gameObject.SetActive(false);
        };
    }

    public void PingPong()
    {
        Color color = Color.white;
        int count = 0;
        float timer = 1f;
        int id = 0;
        id = MyTimerManagers.Instance.AddTimer(0, -1, () =>
        {
            color.a = Mathf.PingPong(5 * Time.time, 1f);
            icon.color = color;
            if((timer -= Time.deltaTime) < 0)
            {
                timer = 1f;
                count++;
                if(count >= 3)
                {
                    MyTimerManagers.Instance.RemoveTimerById(id);
                    icon.color = Color.white;
                }
            }
        });
    }
}
