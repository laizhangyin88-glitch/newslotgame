using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{

// TODO : THIS CLASS HAS BEEN REPLACED WITH INBOXITEMS WHICH IMPLEMENT OSA
public class DynamicScrollInboxCreator : DynamicScrollItemCreator 
{
	public  List<Blackboard> inboxList = null;
	public  ObjectPool       objectPool;
    public  GameObject       caller;

	private void SetUpList()
	{
        caller = gameObject.GetComponent<Blackboard>().GetValue<GameObject>("caller");
	    
        var variable = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/inboxBannerList");
        if (variable != null)
        {
            for (int i = 0; i < variable.value.Count; i++)
            {
                var inboxBannerType = variable.value[i].GetVariable<InboxBannerTypes>("inboxBannerType");

                if (inboxBannerType.value == InboxBannerTypes.NEWS)
                {
                    var id = variable.value[i].GetVariable<int>("id").value;
                    if (BlackboardQueryUtils.IsWatchedInboxBannerItem(id))
                    {
                        continue;
                    }
                }

                if (inboxBannerType.value == InboxBannerTypes.UNKNOWN)
                    continue;
                
                inboxList.Add(variable.value[i]);
            }
        }
        
        variable = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/inboxList");
        if (variable != null)
        {
            for (int i = variable.value.Count - 1; i >= 0; i--)
            {
                if (variable.value[i].GetVariable<InboxTypes>("type").value == InboxTypes.UNKNOWN)
                {
                    continue;
                }

                inboxList.Add(variable.value[i]);
            }
        }
	}

	public override void OnInitialize()
	{
        frontIndex      = 0;
        backIndex       = 0;
        
        SetUpList();

		for (int i = 0; i < maxBufferingCount; ++i)
		{
			if (!PushBack())
				break;
		}

		RebuildContentBounds();
		dynamicScrollRect.verticalNormalizedPosition = 1f;
	}

	private void InitSlotItem(int index, Blackboard bb)
	{
        bb.SetValue("inboxInfo", inboxList[index]);
		bb.SetValue("caller", caller);
	}

	private GameObject PushFront(ObjectPool pool)
	{
		var go = pool.GetObject().gameObject;
		go.transform.SetParent(content, false);
		go.transform.SetAsFirstSibling();
		return go;
	}

	protected override bool PushFront()
	{
		if (frontIndex <= 0)
			return false;

		var go = PushFront(objectPool);
		InitSlotItem(frontIndex - 1, go.GetComponent<Blackboard>());
		frontIndex -= 1;
		return true;
	}

	private GameObject PushBack(ObjectPool pool)
	{
		var go = pool.GetObject().gameObject;
		go.transform.SetParent(content, false);
		go.transform.SetAsLastSibling();
		return go;
	}

	protected override bool PushBack()
	{
		if (backIndex == inboxList.Count)
			return false;

		var go = PushBack(objectPool);
		InitSlotItem(backIndex, go.GetComponent<Blackboard>());
		backIndex += 1;
		return true;
	}

    private void RemoveChild(int index) // Refresh inboxInfo
    {
        Transform child = content.GetChild(index);

        child.GetComponent<Blackboard>().SetValue("inboxInfo", null);
        child.GetComponent<PooledObject>().ReturnToPool();
    }

	protected override bool PopFront()
	{
		frontIndex += 1;

        RemoveChild(0);

		return true;
	}

	protected override bool PopBack()
	{
		backIndex -= 1;

        RemoveChild(content.childCount - 1);

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

        RemoveChild(index - frontIndex);

        backIndex -= 1;

        PushBack();
    }

    // Execute Function. FSM. 
    public void RemoveItemFromID(int removeInboxID)
    {
        if(inboxList == null) return;

        int removeIndex = -1;

        for(int i=0; i<inboxList.Count; ++i)
        {
            Variable<int> id = BlackboardUtils.FindVariable<int>(inboxList[i], "id");

            if(id.value == removeInboxID)
            {
                inboxList.RemoveAt(i);
                removeIndex = i;
                break;
            }
        }

        if(removeIndex != -1)
        {
            RemoveAt(removeIndex);
        }
    }

    public void Clear()
    {
        inboxList.Clear();
        
        while (content.childCount > 0)
        {
            RemoveChild(0);
        }
    }
    
    public void Refresh()
    {
        dynamicScrollRect.enabled = false;
        Clear();
        OnInitialize();
        dynamicScrollRect.enabled = true;
    }
}

}
