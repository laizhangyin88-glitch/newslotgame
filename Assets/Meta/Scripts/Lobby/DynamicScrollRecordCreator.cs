using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{

public class DynamicScrollRecordCreator : DynamicScrollItemCreator 
{
    public  List<Blackboard> recordList {
        get {
            return BlackboardUtils.FindVariable<List<Blackboard>>(GetComponent<Blackboard>(), "recordList").value;
        }
    }
    public  ObjectPool       objectPool;

    private void Awake()
    {
        {
        }
    }

    public override void OnInitialize()
    {
        for (int i = 0; i < maxBufferingCount; ++i)
        {
            if (!PushBack())
                break;
        }

        RebuildContentBounds();
        dynamicScrollRect.verticalNormalizedPosition = 1f;
    }

    private void InitSlotItem(int index, Blackboard bb, GameObject obj)
    {
        if (index < recordList.Count)
            bb.SetValue("record", recordList[index]);
            bb.SetValue("colorIndex", index % 2);
            obj.SetActive(true);

    }

    private GameObject PushFront(ObjectPool pool)
    {
        var go = pool.GetObject().gameObject;
        go.transform.SetParent(content, false);
        go.transform.SetAsFirstSibling();
        go.SetActive(false);
        return go;
    }

    protected override bool PushFront()
    {
        if (frontIndex == 0)
            return false;

        var go = PushFront(objectPool);
        InitSlotItem(frontIndex - 1, go.GetComponent<Blackboard>(), go);
        frontIndex -= 1;
        return true;
    }

    private GameObject PushBack(ObjectPool pool)
    {
        var go = pool.GetObject().gameObject;
        go.transform.SetParent(content, false);
        go.transform.SetAsLastSibling();
        go.SetActive(false);
        return go;
    }

    protected override bool PushBack()
    {
        if (recordList == null) {
            return false;
        } else {
            if (backIndex == recordList.Count)
                return false;
        }

        var go = PushBack(objectPool);
        InitSlotItem(backIndex, go.GetComponent<Blackboard>(), go);
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
