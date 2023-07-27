using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{

public class ClubAllTierIconCellCreator : MonoBehaviour 
{
    public  Transform        parentArea;
    public  Blackboard       agent;

    public  GameObject       prefab;

    public  List<GameObject> tierObjects = new List<GameObject>();

    private void InitList()
    {
        if(agent == null)
            agent = gameObject.GetComponent<Blackboard>();

        for(int i=0; i<tierObjects.Count; ++i)
        {
            Destroy(tierObjects[i]);
        }

        tierObjects.Clear();
    }

    public void RefreshInfo()
    {
        InitList();

        var tierList = BlackboardUtils.FindVariable<List<Blackboard>>(agent, "clubAllTierResponse/tierList");
        var clubTierInfo = BlackboardUtils.FindVariable<Blackboard>(agent, "clubTierInfo");

        // find first index
        int firstIndex = 0;
        for(int i=0; i < tierList.value.Count; ++i)
        {
            var opened = tierList.value[i].GetValue<bool>("opened");

            if(opened == true)
            {
                firstIndex = i;
                break;
            }
        }

        for(int i=firstIndex; i < tierList.value.Count; ++i)
        {
            var go = Instantiate(prefab) as GameObject;
            go.SetActive(false);
            go.name = prefab.name;
            go.transform.SetParent(parentArea, false);
            tierObjects.Add(go);

            var bb = go.GetComponent<Blackboard>();
            bb.SetValue("tierInfo", tierList.value[i]);
            bb.SetValue("clubTierInfo", clubTierInfo.value);
            bb.SetValue("cellIndex", i);

            go.SetActive(true);
        }
    }
}

}

