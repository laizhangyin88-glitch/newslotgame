using ParadoxNotion;
using Sirenix.OdinInspector;
using SlotMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShowInfo : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject signalText;
    public GameObject peopleText;

    public GameObject signalNode;
    public GameObject peopleNode;


    public Image signalImg;

    public List<Sprite> singleImgs = new List<Sprite>();

    public List<Sprite> wifiImgs = new List<Sprite>();


    static List<long> datasLength = new List<long>();

    void Start()
    {
        if (!isShow)
        {
            signalNode.SetActive(false);
            peopleNode.SetActive(false);
            return;
        }

        if(datasLength.Count>0)
            SetData();

        MessageDispatcher.Register("OnContentEvent01", OnShowInfo);
    }
    private void OnDestroy()
    {
        MessageDispatcher.UnRegister("OnContentEvent01", OnShowInfo);
    }

    int lastGameID = 0;
    void Update()
    {
        if (!isShow)
        {
            return;
        }
        if (globalStore.nowGameID != lastGameID)
        {
            lastGameID = globalStore.nowGameID;
            if (globalStore.nowGameID == -1)
            {
                peopleNode.SetActive(false);
            }
            else
            {
                peopleNode.SetActive(true);
            }
        }
    }

    bool isShow
    {
        get{
            if (ApplicationSettings.Instance.isMachine)
            {
                return true;                 
            }

            //ApplicationSettings.Instance.newLoginUrlApp
            if (!string.IsNullOrEmpty(ApplicationSettings.Instance.newLoginUrlApp)
                && ApplicationSettings.Instance.newLoginUrlApp.Contains("8.138.117.128:7501"))
            {
                return false;
            }
#if UNITY_EDITOR
            return true;
#endif
            return false;
        }
    }

    [Button]
    void test_isShow()
    {
        Debug.Log($" is 26 game model : {isShow}");
    }

    [Button]

    void test_setSignal(int speedMS)
    {
        setSignal(speedMS);
        signalText.GetComponent<Text>().text = $"{speedMS}ms";
    }


    void setSignal(int speedMS)
    {

        /*
1、1~30ms：极快，几乎察觉不出有延迟，玩任何游戏速度都特别顺畅。
2、31~50ms：良好，可以正常游戏，没有明显的延迟情况。
3、51~100ms：普通，对抗类游戏能感觉出明显延迟，稍有停顿。
4、100ms：差，无法正常游戏，有卡顿，丢包并掉线现象。
        */

        List<Sprite> temp = singleImgs;
        if (speedMS <= 30)
        {
            signalImg.sprite = temp[0];
            signalImg.color = Color.green;
        }
        else if (speedMS <= 50)
        {
            signalImg.sprite = temp[1];
            signalImg.color = Color.green;
        }
        else if (speedMS <= 100)
        {
            signalImg.sprite = temp[2];
            signalImg.color = Color.yellow;
        }
        else if (speedMS <= 500)
        {
            signalImg.sprite = temp[3];
            signalImg.color = Color.red;
        }
        else 
        {
            signalImg.sprite = temp[4];
            signalImg.color = Color.red;
        }

    }

    public void OnShowInfo(EventData eventData)
    {
        if (eventData.name == "ShowInfo")
        {
            SimpleJSON.JSONNode data = SimpleJSON.JSONNode.Parse(eventData.value as string);

            SetData(data["cur_time"], data["count"]);

            /*long t = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - data["cur_time"];

            datasLength.Insert(0, t);
            if (datasLength.Count > 10)
            {
                datasLength.RemoveRange(10, datasLength.Count - 10);
            }
            List<long> temp = new List<long>(datasLength);
            temp.Sort();//从小到大
            if (temp.Count > 6)
            {
                // 计算要删除的最后一个元素的索引  
                int lastIndexToRemove = temp.Count - 1;
                // 删除从倒数第二个元素开始到列表末尾的所有元素  
                temp.RemoveRange(lastIndexToRemove - 1, 2);
            }

            long sum = temp.Sum();
            int average = (int)((double)sum / temp.Count);

            setSignal(average);
            signalText.GetComponent<Text>().text = $"{average}ms";
            peopleText.GetComponent<Text>().text = data["count"] == 0 ? "" : $"{data["count"]}";
            */
        }
    }


    private void SetData(long t0 = -1, long num = 1)
    {
        if (t0 != -1 )
        {
            long t = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - t0;
            datasLength.Insert(0, t);
        }

        if (datasLength.Count > 10)
        {
            datasLength.RemoveRange(10, datasLength.Count - 10);
        }
        List<long> temp = new List<long>(datasLength);
        temp.Sort();//从小到大
        if (temp.Count > 6)
        {
            // 计算要删除的最后一个元素的索引  
            int lastIndexToRemove = temp.Count - 1;
            // 删除从倒数第二个元素开始到列表末尾的所有元素  
            temp.RemoveRange(lastIndexToRemove - 1, 2);
        }

        long sum = temp.Sum();
        int average = (int)((double)sum / temp.Count);

        setSignal(average);
        signalText.GetComponent<Text>().text = $"{average}ms";
        peopleText.GetComponent<Text>().text = num == 0 ? "" : $"{num}";
    }
}
