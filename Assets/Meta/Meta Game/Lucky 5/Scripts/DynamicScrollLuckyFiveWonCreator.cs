using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{

public class DynamicScrollLuckyFiveWonCreator : DynamicScrollItemCreator 
{
    public  List<Blackboard> wonList = null;
    public  ObjectPool       objectPool;
    public  Blackboard       agent;

    public override void OnInitialize()
    {
        if(agent == null) agent = gameObject.GetComponent<Blackboard>();

        frontIndex = -1;
        backIndex = 0;

        wonList = new List<Blackboard>();
        
        var variable = BlackboardUtils.FindVariable<List<Blackboard>>(agent, "winListResponse/recordList");

        if (variable != null && variable.value.Count > 0)
        {
            wonList.AddRange( variable.value );

            for (int i = 0; i < maxBufferingCount; ++i)
            {
                if (!PushBack())
                    break;
            }
        }
        else
        {
            wonList.Add( null );
            PushBack();
        }

        RebuildContentBounds();
        dynamicScrollRect.verticalNormalizedPosition = 1f;
    }

    private void InitWonInfo(int index, Blackboard bb)
    {
        bb.SetValue("info", wonList[index]);
        bb.SetValue("cellIndex", index);
    }

    private GameObject PushFront(ObjectPool pool)
    {
        var go = pool.GetObject(false).gameObject;
        go.transform.SetParent(content, false);
        go.transform.SetAsFirstSibling();
        return go;
    }

    protected override bool PushFront()
    {
        if (frontIndex < 0)
            return false;

        var go = PushFront(objectPool);
        InitWonInfo(frontIndex, go.GetComponent<Blackboard>());
        go.SetActive(true);
        frontIndex -= 1;
        return true;
    }

    private GameObject PushBack(ObjectPool pool)
    {
        var go = pool.GetObject(false).gameObject;
        go.transform.SetParent(content, false);
        go.transform.SetAsLastSibling();
        return go;
    }

    protected override bool PushBack()
    {
        if (backIndex == wonList.Count)
            return false;

        var go = PushBack(objectPool);
        InitWonInfo(backIndex, go.GetComponent<Blackboard>());
        go.SetActive(true);
        backIndex += 1;
        return true;
    }

    protected override bool PopFront()
    {
        frontIndex += 1;

        content.GetChild(0).GetComponent<PooledObject>().ReturnToPool();

        return true;
    }

    protected override bool PopBack()
    {
        backIndex -= 1;

        content.GetChild(content.childCount - 1).GetComponent<PooledObject>().ReturnToPool();

        return true;
    }
}

}
