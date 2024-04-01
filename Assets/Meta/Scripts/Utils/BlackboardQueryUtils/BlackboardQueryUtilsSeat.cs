using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;
using SlotMaker.Json;

namespace BagelCode
{

public static partial class BlackboardQueryUtils
{
    static public int seatCount = 4;

    static private string roomId = "";
    static private List<string> seatIdList = new List<string>();
    static private Dictionary<string, Blackboard> seatBBList = new Dictionary<string, Blackboard>();

    static public void UpdateSeat(Room room)
    {
        if(room == null)
        {
            roomId = "";
            ClearSeat(ref seatIdList);
            return;
        }

        if(room.roomId != roomId)
        {
            ClearSeat(ref seatIdList);
        }

        roomId = room.roomId;
        List<string> newseatIdList = new List<string>();

        ClearSeat(ref newseatIdList);

        string meID = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/userId").value;


        //string oldJson = JsonUtility.ToJson(room);
        //string buffer = SlotSimpleJson.SerializeObject(newseatIdList);
        //string buffer1 = SlotSimpleJson.SerializeObject(seatIdList);
        //Debug.Log($"@A SlotSpinResponseV3 = {oldJson}");


        if (room.players != null)
        {
            for(int i=0; i<room.players.Count; ++i)
            {
                if(meID == room.players[i].userId) continue;

                int index = GetSeatIndex(room.players[i].userId);
                if(index != -1)
                {
                    // orig user. 
                    newseatIdList[index] = room.players[i].userId;
                }
            }

            seatIdList.Clear();
            seatIdList.AddRange( newseatIdList );

            // Add New user..
            for(int i=0; i<room.players.Count; ++i)
            {
                if(meID == room.players[i].userId) continue;

                int index = GetSeatIndex(room.players[i].userId);
                if(index == -1)
                {
                    SetSeatUser(room.players[i].userId);
                }
            }
        }

        seatBBList.Clear();
        for(int i=0; i<seatIdList.Count; ++i)
        {
            if(!string.IsNullOrEmpty(seatIdList[i]))
            {
                Blackboard seatBB = GetSeatBlackboard(seatIdList[i]);
                seatBBList.Add(seatIdList[i], seatBB);
            }
        }
    }

    static public void SetSeatUser(string userID)
    {
        if(seatIdList.Contains(userID)) return;

        for(int i=0; i<seatIdList.Count; ++i)
        {
            if( string.IsNullOrEmpty(seatIdList[i]) )
            {
                seatIdList[i] = userID;
                break;
            }
        }
    }

    static public int GetSeatIndex(string userID)
    {
        if( string.IsNullOrEmpty(userID) ) return -1;
        
        for(int i=0; i<seatIdList.Count; ++i)
        {
            if(seatIdList[i] == userID)
            {
                return i;
            }
        }

        return -1;
    }

    static public Blackboard GetSeatBlackboard(int index)
    {
        if(index < 0) return null;
        if(index >= seatCount) return null;

        string userID = seatIdList[index];

        if(!string.IsNullOrEmpty(userID) && seatBBList.ContainsKey(userID))
        {
            return seatBBList[userID];
        }

        return null;
    }

    static public Blackboard GetSeatBlackboard(string findUserID)
    {
        var roomBB = BlackboardUtils.FindVariable<Blackboard>(null, "./room");


        if(roomBB != null && roomBB.value != null)
        {
            var userProfileList = BlackboardUtils.FindVariable<List<Blackboard>>(roomBB.value, "players").value;

            for(int i=0; i<userProfileList.Count; ++i)
            {
                string userID = BlackboardUtils.FindVariable<string>(userProfileList[i], "userId").value;
                if(userID == findUserID)
                {
                    return userProfileList[i];
                }
            }
        }
        
        return null;
    }

    static public void ClearSeat(ref List<string> seatList)
    {
        seatList.Clear();

        for(int i=0; i<seatCount; ++i)
        {
            seatList.Add("");
        }
    }

    public static bool IsRoomInClubMember()
    {
        long clubId = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId")?.value ?? 0;
        if (clubId > 0 && seatBBList != null && seatBBList.Count > 0)
        {
                foreach(var playerBB in seatBBList)
                {
                    if (playerBB.Value.GetValue<long>("clubId") == clubId)
                        return true;
                }
        }

        return false;
    }
}

}

