using NodeCanvas.Framework;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPagePaytable : MonoBehaviour
{

    public Transform layout;

    public GameObject linePrefab;

    public Color color = Color.green;

    void Start()
    {
        var payLinesBB = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./game/payLines").value;
        //int column = BlackboardUtils.FindVariable<int>(null, "./customData/slotDataList/0/column").value;
        //int row = BlackboardUtils.FindVariable<int>(null, "./customData/slotDataList/0/row").value;

        List<Blackboard> payLineBBList = payLinesBB[0].GetValue<List<Blackboard>>("value");
        for (int i =0;i< payLineBBList.Count; i++)
        {
            List<int> payLine = payLineBBList[i].GetValue<List<int>>("value");

            GameObject go = Instantiate(linePrefab);
            go.name = $"line{i+1}";

            go.transform.SetParent(layout, false);
            PayTableLineSegments comp = go.GetComponent<PayTableLineSegments>();
            comp.SetPayLine(i+1,payLine, color);
        }
    }
}
