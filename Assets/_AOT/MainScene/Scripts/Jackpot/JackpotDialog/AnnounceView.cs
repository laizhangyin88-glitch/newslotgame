using BagelCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 跑马灯文字设置
/// </summary>
public class AnnounceView : MonoBehaviour
{

    private Text text;     //跑马灯text.
    private Queue<string> queue;     //跑马灯队列.
    private bool isEnd = false;   //判断当前text中的跑马灯是否跑完.

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        text = transform.Find("Text").GetComponent<Text>();
        queue = new Queue<string>();
    }

    /// <summary>
    /// 添加跑马灯文字...
    /// </summary>
    /// <param name="msg">文字内容</param>
    public void AddMessage(string msg)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            Init();
        }

        queue.Enqueue(msg);
        if (isEnd) return;
        StartCoroutine(Marquee());
    }

    public IEnumerator Marquee()
    {
        float begin_X = 440;
        float end_X = -440;

        while (queue.Count > 0)
        {
            Vector3 pos = text.rectTransform.localPosition;

            float duration = 10f;  //默认时间
            float speed = 100f;    //滚动速度
            int loop = 1;          //循环次数

            string msg = queue.Dequeue();
            text.text = msg;
            float txetWidth = text.preferredWidth;
            float distance = begin_X - end_X + txetWidth;
            duration = distance / speed;

            isEnd = true;

            while (loop-- > 0)
            {
                text.rectTransform.localPosition = new Vector3(begin_X, pos.y, pos.z);  //归位...
                AsyncActionUtils.ApplyLocalMovement(this, text.rectTransform, new Vector3(begin_X, pos.y, pos.z), new Vector3(-distance, pos.y, pos.z), duration, TweenUtils.VectorTweenLinear);
                yield return new WaitForSeconds(duration);
            }
            yield return null;
        }
        isEnd = false;
        gameObject.SetActive(false);
        yield break;
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

