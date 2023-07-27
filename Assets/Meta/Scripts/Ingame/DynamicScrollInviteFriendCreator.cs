using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

using BagelCode.ClientModels;
using BagelCode;
using SlotMaker.Json;
using SlotMaker;

namespace BagelCode
{

public class InviteInfo
{
    public Blackboard  userInfo;
    public bool        isOnline = true;
    public bool        isInvitable;
    public string      stringFormat;
    public Toggle      toggle;
    public bool        toggleValue;

    public void OnToggle(bool isOn)
    {
        this.toggleValue = isOn;
    }
}

public class DynamicScrollInviteFriendCreator : DynamicScrollItemCreator
{
    public  Blackboard          agentBB;
    public  ObjectPool          objectPool;
    public  GameObject          empty;
    public  string              type;

    private  List<string> 		          onlineUserList;
   	private  List<Blackboard>	          inroomPlayerList;
    private  Dictionary<string, long>     alreadyInviteDict;

    private  List<Blackboard> userList = new List<Blackboard>();
    private  List<InviteInfo> inviteInfoList = new List<InviteInfo>();

    private int tierRestriction;
	private int levelRestriction;

    private bool isInvitable;
    private bool hasFriend;

    private static string invitedKey = "INVITED_USER_ID_DICT";

    public override void OnInitialize()
    {
        SetUpData();

        for (int i = 0; i < maxBufferingCount; ++i)
        {
            if (!PushBack())
                break;
        }

        RebuildContentBounds();
        dynamicScrollRect.verticalNormalizedPosition = 1f;
    }

    private bool SetInfo()
    {
        if (string.Equals(type, "friend"))
        {
            userList = BlackboardUtils.FindVariable<List<Blackboard>>(agentBB, "_response/friendList").value;
            onlineUserList = BlackboardUtils.FindVariable<List<string>>(agentBB, "_response/friendOnlineList").value;
        }
        else if (string.Equals(type, "club"))
        {
            userList = BlackboardUtils.FindVariable<List<Blackboard>>(agentBB, "_response/clubMemberList").value;
            onlineUserList = BlackboardUtils.FindVariable<List<string>>(agentBB, "_response/clubOnlineList").value;
        }

        hasFriend = (userList?.Count ?? 0) != 0;
        return hasFriend;
    }

    private void SetAlreadyInviteDict()
    {
        string json = PlayerPrefs.GetString(invitedKey, "");
        if (string.IsNullOrEmpty(json)) return;

        alreadyInviteDict = (Dictionary<string, long>)SlotSimpleJson.DeserializeObject(json, typeof(Dictionary<string, long>));
        List<string> removeList = new List<string>();
        foreach (var item in alreadyInviteDict)
        {
            long timeStamp = BagelCode.TimeUtils.GetTimeStamp();
            if(timeStamp >= item.Value)
            {
                removeList.Add(item.Key);
            }
        }

        for (int i = 0; i < removeList.Count; ++i)
        {
            alreadyInviteDict.Remove(removeList[i]);
        }

        json = SlotSimpleJson.SerializeObject(alreadyInviteDict);
        PlayerPrefs.SetString(invitedKey, json);
    }

    private void SetUpData()
    {
        if (!SetInfo())
        {
            agentBB.SetValue("hasFriend", hasFriend);
            empty.SetActive(true);
            return;
        }
        SetAlreadyInviteDict();
        inroomPlayerList = BlackboardUtils.FindVariable<List<Blackboard>>(ContentBlackboard.Get(), "./room/players").value;
        var gameId = BlackboardUtils.FindVariable<int>(ContentBlackboard.Get(), "./game/gameId");
        levelRestriction = BlackboardQueryUtils.GetGameMinLevelRestriction(gameId.value);
        tierRestriction = 0; // TBD..

        List<InviteInfo> onlinePossibleList = new List<InviteInfo>();
        List<InviteInfo> offlinePossibleList = new List<InviteInfo>();
        List<InviteInfo> onlineImpossibleList = new List<InviteInfo>();
        List<InviteInfo> offlineImpossibleList = new List<InviteInfo>();

        for(int i = 0; i < userList.Count; i++)
        {
            InviteInfo inviteInfo = new InviteInfo();

            inviteInfo.userInfo = userList[i];

            if(CheckUserOnline(inviteInfo))
            {
                if(CheckUserInvitable(inviteInfo))
                    onlinePossibleList.Add(inviteInfo);
                else
                    onlineImpossibleList.Add(inviteInfo);
            }
            else
            {
                if(CheckUserInvitable(inviteInfo))
                    offlinePossibleList.Add(inviteInfo);
                else
                    offlineImpossibleList.Add(inviteInfo);
            }
        }

        inviteInfoList.AddRange(onlinePossibleList);
        inviteInfoList.AddRange(offlinePossibleList);
        inviteInfoList.AddRange(onlineImpossibleList);
        inviteInfoList.AddRange(offlineImpossibleList);

        agentBB.SetValue("hasFriend", hasFriend);
        agentBB.SetValue("isInvitable", isInvitable);
    }

    private bool CheckUserOnline(InviteInfo inviteInfo)
    {
        for(int i = 0; i < onlineUserList.Count; i++)
        {
            var userId = inviteInfo.userInfo.GetValue<string>("userId");
            if(userId == onlineUserList[i])
                return true;
        }
        inviteInfo.isOnline = false;
        return false;
    }

    private bool CheckUserInvitable(InviteInfo inviteInfo)
    {
        var userId = inviteInfo.userInfo.GetValue<string>("userId");
        var level = BlackboardUtils.FindVariable<int>(inviteInfo.userInfo, "level");
        var tier = BlackboardUtils.FindVariable<int>(inviteInfo.userInfo, "tier");

        for (int i = 0; i < inroomPlayerList.Count; i++)
        {
            var inroomUserId = BlackboardUtils.FindVariable<string>(inroomPlayerList[i], "userId");
            if(inroomUserId.value == userId)
            {
                inviteInfo.stringFormat = "POPUP_INVITE_FRIENDS_INROOM_RESTRICTION";
                return false;
            }
        }
         
        if (level.value < levelRestriction)
        {
            inviteInfo.stringFormat = "POPUP_INVITE_FRIENDS_LEVEL_RESTRICTION";
            return false;
        }

        if (tier.value < tierRestriction)
        {
            inviteInfo.stringFormat = "POPUP_INVITE_FRIENDS_TIER_RESTRICTION";
            return false;
        }   

        if (alreadyInviteDict != null)
        {
            if (alreadyInviteDict.ContainsKey(userId))
            {
                inviteInfo.stringFormat = "POPUP_INVITE_FRIENDS_ALREADY_INVITE_RESTRICTION";
                return false;                
            }
        }

        inviteInfo.isInvitable = true;
        isInvitable = true;
        return true;
    }

    private void InitCellItem(int index, GameObject obj)
    {
        Blackboard bb = obj.GetComponent<Blackboard>();

        InviteInfo inviteInfo = inviteInfoList[index];

        bb.SetValue("userInfo", inviteInfo.userInfo);
        bb.SetValue("stringFormat", inviteInfo.stringFormat);
        bb.SetValue("isOnline", inviteInfo.isOnline);
        bb.SetValue("isInvitable", inviteInfo.isInvitable);

        Toggle toggle = obj.transform.Find("Toggle").GetComponent<Toggle>();

        toggle.onValueChanged.AddListener( inviteInfo.OnToggle );
        toggle.isOn = inviteInfo.toggleValue;

        inviteInfo.toggle = toggle;        
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

        InitCellItem(frontIndex - 1, go);
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
        if (backIndex == userList.Count)
            return false;

        var go = PushBack(objectPool);
        InitCellItem(backIndex, go);
        go.SetActive(true);
        
        backIndex += 1;
        
        return true;
    }

    protected override bool PopFront()
    {
        frontIndex += 1;

        PopToggle(frontIndex - 1);
        content.GetChild(0).GetComponent<PooledObject>().ReturnToPool();

        return true;
    }

    protected override bool PopBack()
    {
        backIndex -= 1;

        PopToggle(backIndex);
        content.GetChild(content.childCount - 1).GetComponent<PooledObject>().ReturnToPool();

        return true;
    }

    private void PopToggle(int index)
    {
        Toggle toggle = inviteInfoList[index].toggle;
        toggle.onValueChanged.RemoveListener( inviteInfoList[index].OnToggle );
        toggle = null;
    }

    public void SelectAll()
    {
        for (int i = 0; i < inviteInfoList.Count; ++i)
        {
            if (!inviteInfoList[i].isInvitable) continue;

            inviteInfoList[i].toggleValue = true;
            if (inviteInfoList[i].toggle != null)
            {
                inviteInfoList[i].toggle.isOn = inviteInfoList[i].toggleValue;
            }
        }
    }

    public List<string> GetSelectedUserIdList()
    {
        List<string> userIdList = new List<string>();
        for (int i = 0; i < inviteInfoList.Count; ++i)
        {
            if (inviteInfoList[i].toggleValue)
            {
                userIdList.Add(inviteInfoList[i].userInfo.GetValue<string>("userId"));
            }
        }

        return userIdList;
    }
}

}
