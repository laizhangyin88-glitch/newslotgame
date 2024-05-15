using Dreamteck.Splines.Primitives;
using ParadoxNotion;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShowInfo : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject time;
    public GameObject people;


    void Start()
    {
        MessageDispatcher.Register("OnContentEvent01", OnShowInfo);
    }
    private void OnDestroy()
    {
        MessageDispatcher.UnRegister("OnContentEvent01", OnShowInfo);
    }

    int lastGameID = 0;
    void Update()
    {
        if (globalStore.nowGameID != lastGameID)
        {
            lastGameID = globalStore.nowGameID;
            if (globalStore.nowGameID == -1)
            {
                people.SetActive(false);
            }
            else
            {
                people.SetActive(true);
            }
        }
    }

    List<long> datasLength = new List<long>();
    public void OnShowInfo(EventData eventData)
    {
        if (eventData.name == "ShowInfo")
        {
            SimpleJSON.JSONNode data = SimpleJSON.JSONNode.Parse(eventData.value as string);

            long t = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - data["cur_time"];


            this.datasLength.Insert(0, t);
            if (this.datasLength.Count > 10)
            {
                this.datasLength.RemoveRange(10, this.datasLength.Count - 10);
            }
            List<long> temp = new List<long>(this.datasLength);
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


            time.GetComponent<Text>().text = $"delay :{average}ms";
            people.GetComponent<Text>().text = data["count"] == 0 ? "" : $"people:{data["count"]}";
        }
    }
}
