using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{

public class DynamicScrollClubRequestListCreator : DynamicScrollItemCreator 
{
    public  List<Blackboard> clubJoinRequestsList = null;
    public  ObjectPool       objectPool;
    public  GameObject       noRequestsObject;
    public  GameObject       caller;
    public  Blackboard       agent;

    private long clubID;
    private PIDButton acceptAllButton;

    private const string ON_META_UI_EVENT = "OnMetaUIEvent";

    private const string ON_REMOVE_CLUB_JOIN_REQUEST = "OnRemoveClubJoinRequest";
    private const string ON_REMOVE_ALL_REQUEST = "OnRemoveAllRequest";

    private void Start()
    {
        MessageDispatcher.Register(ON_META_UI_EVENT, OnMetaUIEvent);
    }

    private void OnDestroy()
    {
        MessageDispatcher.UnRegister(ON_META_UI_EVENT, OnMetaUIEvent);
    }

    public void RefreshList()
    {
        if(agent == null) agent = gameObject.GetComponent<Blackboard>();

        RemoveAll();

        clubJoinRequestsList = new List<Blackboard>();

        clubID = BlackboardUtils.FindVariable<long>(agent, "clubJoinRequestsResponse/clubId").value;
        
        acceptAllButton = BlackboardUtils.FindVariable<GameObject>(agent, "_acceptAllButton").value.GetComponent<PIDButton>();
        
        var variable = BlackboardUtils.FindVariable<List<Blackboard>>(agent, "clubJoinRequestsResponse/clubRequests");

        if (variable != null && variable.value.Count > 0)
        {
            noRequestsObject.SetActive(false);
            clubJoinRequestsList.AddRange( variable.value );

            for (int i = 0; i < maxBufferingCount; ++i)
            {
                if (!PushBack())
                    break;
            }
            
            acceptAllButton.interactable = true;
        }
        else
        {
            noRequestsObject.SetActive(true);
            acceptAllButton.interactable = false;
        }

        RebuildContentBounds();
        dynamicScrollRect.verticalNormalizedPosition = 1f;
    }

    public override void OnInitialize()
    {
    }

    private void InitSlotItem(int index, Blackboard bb)
    {
        bb.SetValue("requestInfo", clubJoinRequestsList[index]);
        bb.SetValue("caller", caller);
        bb.SetValue("cellIndex", index);
        bb.SetValue("clubID", clubID);
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
        InitSlotItem(frontIndex, go.GetComponent<Blackboard>());
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
        if (backIndex >= clubJoinRequestsList.Count)
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

    protected void RemoveAll()
    {
        while(content.childCount > 0)
        {
            content.GetChild(0).GetComponent<PooledObject>().ReturnToPool();
        }

        frontIndex = -1;
        backIndex = 0;
    }

    private void OnMetaUIEvent(EventData eventData)
    {
        if (eventData.name == ON_REMOVE_CLUB_JOIN_REQUEST)
        {
            if(eventData.value != null && eventData.value is string)
            {
                RemoveClubJoinRequest((string)eventData.value);
            }
        }
        else if (eventData.name == ON_REMOVE_ALL_REQUEST)
        {
            RemoveAllRequest();
        }
    }

    private void RemoveAllRequest()
    {
        if (clubJoinRequestsList != null)
            clubJoinRequestsList.Clear();
            
        RemoveAll();
        noRequestsObject.SetActive(true);
        acceptAllButton.interactable = false;
            
        var newStuffExists = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "clubBadgeInfo/newStuffExists");
        newStuffExists.value = false;
    }
    
    private void RemoveClubJoinRequest(string removeUserID)
    {
        if(clubJoinRequestsList == null) return;

        int removeIndex = -1;

        for(int i=0; i<clubJoinRequestsList.Count; ++i)
        {
            var userID = BlackboardUtils.FindVariable<string>(clubJoinRequestsList[i], "userId");

            if(userID.value == removeUserID)
            {
                clubJoinRequestsList.RemoveAt(i);
                removeIndex = i;
                break;
            }
        }

        if(removeIndex != -1)
        {
            RemoveAt(removeIndex);
        }

        noRequestsObject.SetActive(clubJoinRequestsList.Count == 0);
        acceptAllButton.interactable = clubJoinRequestsList.Count != 0;

        if(clubJoinRequestsList.Count == 0)
        {
            var newStuffExists = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "clubBadgeInfo/newStuffExists");
            newStuffExists.value = false;
        }
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

        Transform child = content.GetChild(index - (frontIndex + 1));
        child.GetComponent<PooledObject>().ReturnToPool();

        backIndex -= 1;

        PushBack();
    }

}

}

