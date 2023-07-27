using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{

public class DynamicScrollLootBoxInfoCreator : DynamicScrollItemCreator 
{
    public  List<Blackboard> probList = null;
    public  ObjectPool       objectPool;
    public  Blackboard       owner;

    public override void OnInitialize()
    {
        probList = new List<Blackboard>();

        var probsInfo = BlackboardUtils.FindVariable<Blackboard>(owner, "probsInfo");
        if (probsInfo != null)
        {
            var probInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(probsInfo.value, "probList");
            
            for(int i = 0; i < probInfoList.value.Count; i++)
            {
                probList.Add(probInfoList.value[i]);
            }

            frontIndex = -1;
            endIndex = probList.Count;

            for (int i = 0; i < maxBufferingCount; ++i)
            {
                if (!PushBack())
                    break;
            }

            RebuildContentBounds();
            dynamicScrollRect.verticalNormalizedPosition = 1f;
        }
    }

    private void InitSlotItem(int index, Blackboard bb)
    {
        bb.SetValue("probInfo", probList[index]);
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
        InitSlotItem(frontIndex - 1, go.GetComponent<Blackboard>());
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
        if (backIndex == probList.Count)
            return false;

        var go = PushBack(objectPool);
        InitSlotItem(backIndex, go.GetComponent<Blackboard>());
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
