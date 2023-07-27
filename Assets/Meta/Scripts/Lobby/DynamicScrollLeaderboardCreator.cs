using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{

public class DynamicScrollLeaderboardCreator : DynamicScrollItemCreator 
{
    public  List<Blackboard> userList = null;

    public  string           userRankText = "";
    public  SortTypes        sortType = SortTypes.TYPE_MAX_WIN;
    public  PeriodTypes      periodType = PeriodTypes.TYPE_DAILY;

    public  ObjectPool       objectPool;

    private void Awake()
    {
        {
            var sortTypeIndex = BlackboardUtils.FindVariable<int>(null, "/leaderboardResponse/sortType");
            if(sortTypeIndex != null)
                sortType = (SortTypes)sortTypeIndex.value;
            var periodTypeIndex = BlackboardUtils.FindVariable<int>(null, "/leaderboardResponse/priodType");

            string currentBBName = string.Format("{0}_{1}", sortTypeIndex.value, periodTypeIndex.value);
            var currentBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "leaderboardResponse");
            currentBB = BlackboardUtils.GetOrCreateBlackboard(currentBB, currentBBName);

            var variable = BlackboardUtils.FindVariable<List<Blackboard>>(currentBB, "userList");
            if (variable != null)
            {
                userList = variable.value;

                // expireTimestamp
                
                if(userList.Count != 0)
                {
                    BlackboardUtils.DestroyBlackboard(currentBB, "customUserData");
                    var userBB = BlackboardUtils.GetOrCreateBlackboard(currentBB, "customUserData");

                    var name = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/me/name");
                    var profileUrl = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/me/profileUrl");
                    var userId = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/me/userId");
                    var tier = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "/me/tier");

                    BlackboardUtils.SetOrCreateValue( userBB, "name", name.value);
                    BlackboardUtils.SetOrCreateValue( userBB, "profileUrl", profileUrl.value);
                    BlackboardUtils.SetOrCreateValue( userBB, "userId", userId.value);
                    BlackboardUtils.SetOrCreateValue( userBB, "tier", tier.value);

                    var userValue = BlackboardUtils.FindVariable<long>(currentBB, "userValue");
                    BlackboardUtils.SetOrCreateValue( userBB, "maxWin", userValue.value);
                    BlackboardUtils.SetOrCreateValue( userBB, "totalWin", userValue.value);
                    BlackboardUtils.SetOrCreateValue( userBB, "coin", userValue.value);

                    userList.Insert(0, userBB as Blackboard);

                    var userRank = BlackboardUtils.FindVariable<int>(currentBB, "userRank");
                    // var userCount = BlackboardUtils.FindVariable<int>(currentBB, "leaderboardUserCount");
                    
                    userRankText = string.Format("{0}", userRank.value);
                }
            }
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
        bb.SetValue("info", userList[index]);
        bb.SetValue("rank", index);
        bb.SetValue("sortType", sortType);

        if(index == 0)
        {
            BlackboardUtils.SetOrCreateValue( bb, "userRank", userRankText);
        }
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
        if (backIndex == userList.Count)
            return false;

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
