using BagelCode;
using GameUtil;
using UnityEngine;
using UnityEngine.UI;

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
    private Image redIcon;

    public int Index = 0;

    private float changeTime = 0.2f;

    public int ResultIndex = 0;

    private LoopTimer _loopTimer;

    // Start is called before the first frame update
    private void Start()
    {
        ResultIndex = 0;
        stopTime = FruitPartyMiniGameController1.AnimationTime;
        icon = transform.Find("icon").GetComponent<Image>();
        icon.sprite = FruitPartyMiniGameController1.Instance.spritesArray[Random.Range(0, 8)]; 
        redIcon = transform.Find("redicon").GetComponent<Image>();
        redIcon.gameObject.SetActive(false);
        RectTransform rectTransform = GetComponent<RectTransform>();
        this.DelayAction(1, () =>
        {
            startPosition = transform.localPosition;
        });
        //SetSprite(FruitPartyMiniGameController1.Instance.sprites[Random.Range(0, FruitPartyMiniGameController1.Instance.sprites.Length)]);
    }

    public void SetSprite(Sprite sprite)
    {
        icon.sprite = sprite;
    }

    private void Update()
    {
        if (isMove)
        {
            transform.localPosition += new Vector3(0, Speed * Time.deltaTime, 0);
            if (transform.localPosition.y <= bottomPositionY)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, topPositionY, 0);
            }
            if ((changeTime -= Time.deltaTime) < 0)
            {
                changeTime = 0.2f;
                SetSprite(FruitPartyMiniGameController1.Instance.spritesArray[Random.Range(0, FruitPartyMiniGameController1.Instance.spritesArray.Length - 1)]);
            }
        }
    }

    public void ResetPosition()
    {
        transform.localPosition = startPosition;
    }

    public void PlayResult()
    {
        if (ResultIndex > 0)
        {
            SetSprite(FruitPartyMiniGameController1.Instance.spritesArray[ResultIndex - 1]);
            redIcon.sprite = FruitPartyMiniGameController1.Instance.redSpritesArray[ResultIndex - 1];
        }
    }

    public void Pingpong()
    {
        Color color = Color.white;
        int count = 0;
        float timer = 1f;
        int id = 0;
        redIcon.gameObject.SetActive(true);
        _loopTimer?.Cancel();
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
                    redIcon.gameObject.SetActive(false);
                    _loopTimer.Cancel();
                }
            }
        });
    }
}
