using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{

public class DynamicScrollWallOfEpicCreator : DynamicScrollItemCreator 
{
    public List<List<Blackboard>> dataList = new List<List<Blackboard>>();

    public  ObjectPool objectPool;
    public  ObjectPool endTextPool;
    public  GameObject caller;

    private void InitList()
    {
        var variable = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/wallOfEpic/recordList");
        if (variable != null)
        {
            List<Blackboard> newLine = null;

            for(int i=0; i<variable.value.Count; ++i)
            {
                if(i%3 == 0)
                {
                    newLine = new List<Blackboard>();
                    dataList.Add(newLine);
                }

                newLine.Add(variable.value[i]);
            }
        }
    }

    public override void OnInitialize()
    {
        InitList();

        for (int i = 0; i < maxBufferingCount; ++i)
        {
            if (!PushBack())
                break;
        }

        RebuildContentBounds();
        dynamicScrollRect.verticalNormalizedPosition = 1f;
    }

    private void SetData(int index, Blackboard bb)
    {
        bb.SetValue("infoList", dataList[index]);
        // bb.SetValue("caller", caller);
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
        if (frontIndex == 0)
            return false;

        var go = PushFront(objectPool);
        SetData(frontIndex - 1, go.GetComponent<Blackboard>());
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
        if (backIndex >= dataList.Count)
        {
            if(backIndex == dataList.Count)
            {
                var endText = endTextPool.GetObject();
                endText.transform.SetParent(content, false);
                endText.transform.SetAsLastSibling();
                ++backIndex;
            }
            return false;
        }

        var go = PushBack(objectPool);
        SetData(backIndex, go.GetComponent<Blackboard>());
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

    protected void RemoveAt(int index)
    {
        if(index > backIndex) return;
        if(index < frontIndex)
        {
            frontIndex -= 1;
            backIndex -=1;
            return;
        }

        content.GetChild(index - frontIndex).GetComponent<PooledObject>().ReturnToPool();
        backIndex -= 1;

        PushBack();
    }

}

}
