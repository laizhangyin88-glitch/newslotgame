using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Club")]

public class GetClubLeaderCellInfoBB : ActionTask<Blackboard>
{
    public BBParameter<string>  valueA;
    public BBParameter<int>     cellIndex;
    public BBParameter<List<GameObject>> bgList; // 0, 1 : member, 2 : Me

    public BBParameter<string>  rankText;
    public BBParameter<string>  userNameText;
    public BBParameter<string>  scoreText;

    public BBParameter<string>  profileURL;
    public BBParameter<int>     tier;
    public BBParameter<bool>    isMe;

    private const string RANK_KEY = "POPUP_CHALLENGE_LEADER_USER_RANK";
    private const string USER_NAME_KEY = "POPUP_CHALLENGE_LEADER_USER_NAME";
    private const string SCORE_KEY = "POPUP_CHALLENGE_LEADER_USER_SCORE";

    protected override string info
    {
        get { return "Get Club Leader Cell Info BB"; }
    }

    protected override void OnExecute()
    {
        var memberInfo = BlackboardUtils.FindVariable<Blackboard>(agent, valueA.value);

        if(memberInfo != null)
        {
            var userName = memberInfo.value.GetValue<string>("name");
            var score = memberInfo.value.GetValue<long>("count");
            profileURL.value = memberInfo.value.GetValue<string>("profileUrl");
            tier.value = memberInfo.value.GetValue<int>("tier");

            UpdateBG();

            bool isError = false;

            rankText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, RANK_KEY, cellIndex.value + 1, out isError);
            userNameText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, USER_NAME_KEY, userName, out isError);
            scoreText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, SCORE_KEY, score, out isError);
        }

        EndAction();
    }

    private void UpdateBG()
    {
        int cellStyle = cellIndex.value%2;
        if(isMe.value) cellStyle = 2;

        for(int i=0; i<bgList.value.Count; ++i)
        {
            bgList.value[i].SetActive(i==cellStyle);
        }
    }
}

}
