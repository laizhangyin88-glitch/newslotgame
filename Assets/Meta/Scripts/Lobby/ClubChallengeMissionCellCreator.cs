using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{

public class ClubChallengeMissionCellCreator : MonoBehaviour
{
    public  Transform        parentArea;
    public  GameObject       caller;
    public  Blackboard       agent;

    public  SceneInfoObject  sceneInfoObject;
    private GameObject       prefab;

    public  List<GameObject> missionObjects = new List<GameObject>();

    private ContextElement titleTexTelement;
    private ClubChallengeStageInfoController clubStageController;

    public  int CLUB_MISSION_COUNT = 4;
    private const string PLUS = " + ";

    private bool isInit = false;

    private void InitList()
    {
        if(isInit) return;

        if(agent == null)
            agent = gameObject.GetComponent<Blackboard>();

        if(prefab == null)
        {
            prefab = SceneManager.LoadScene(transform, sceneInfoObject.GetSceneInfo());
            prefab.SetActive(false);
        }

        for(int i=0; i<missionObjects.Count; ++i)
        {
            Destroy(missionObjects[i]);
        }

        missionObjects.Clear();

        for(int i=0; i<CLUB_MISSION_COUNT; ++i)
        {
            var go = Instantiate(prefab) as GameObject;
            go.SetActive(false);
            go.name = prefab.name;
            go.transform.SetParent(parentArea, false);
            missionObjects.Add(go);
        }

        var rootElement = GetComponent<ContextElement>();
        rootElement.UpdateContext();

        var clubStageInfoElement = ContextUtils.FindElement(rootElement, "Stage Info", ContextSearchingType.ChildrenSearch);
        clubStageController = clubStageInfoElement.gameObject.GetComponent<ClubChallengeStageInfoController>();


        titleTexTelement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);

        isInit = true;
    }

    public void RequestInfo()
    {
        InitList();
        // Set loading.
        titleTexTelement.gameObject.SetActive(false);
        clubStageController.OnUpdateVariables(-1, -1, 0, null);
    }

    public void RefreshInfo()
    {
        InitList();

        var challengeInfo = BlackboardUtils.FindVariable<Blackboard>(agent, "challengeResponse/clubChallengeInfo");

        if (challengeInfo != null)
        {
            titleTexTelement.gameObject.SetActive(true);

            // Set Missions.
            var topContributionList = BlackboardUtils.FindVariable<List<Blackboard>>(agent, "challengeResponse/topContributionList");
            var missions = BlackboardUtils.FindVariable<List<Blackboard>>(challengeInfo.value, "missionList");
            int stage = challengeInfo.value.GetValue<int>("stage");
            int maxStage = BlackboardQueryUtils.GetClubChallengeMaxStage(challengeInfo.value);
            int progressCount = 0;
            bool isSingle = maxStage <= 1;

            if(missions != null)
            {
                for(int i=0; i<missions.value.Count; ++i)
                {
                    if(i >= missionObjects.Count)
                        break;

                    var go = missionObjects[i];
                    go.SetActive(false);

                    var contributionList = BlackboardUtils.FindVariable<List<Blackboard>>(topContributionList.value[i], "contributionList");

                    var bb = go.GetComponent<Blackboard>();
                    BlackboardUtils.SetOrCreateValue<Blackboard>(bb, "clubChallengeInfo", challengeInfo.value);
                    BlackboardUtils.SetOrCreateValue<Blackboard>(bb, "missionInfo", missions.value[i]);
                    BlackboardUtils.SetOrCreateValue<List<Blackboard>>(bb, "contributionList", contributionList.value);

                    go.SetActive(true);

                    var missionComplete = missions.value[i].GetValue<bool>("done");
                    int completedStage = missions.value[i].GetValue<int>("completedStage");
                    if(stage >= 0 && stage == completedStage)
                        missionComplete = true;

                    if(missionComplete)
                        progressCount++;
                }
            }

            bool isComplete = challengeInfo.value.GetValue<bool>("done");
            string titleText = null;

            // Set Title.
            if(isComplete)
            {
                titleText = StringTableUtils.GetString(StringTable.StringTableType.Global, isSingle ? "CLUB_CHALLENGE_COMPLETE_TITLE_ONE" : "CLUB_CHALLENGE_COMPLETE_TITLE");
            }
            else
            {
                Blackboard rewardBB = challengeInfo.value.GetValue<Blackboard>("totalReward");

                var clubMultiplier  = ClubUtils.GetClubRewardMultiplierNumerator();
                long rewardCoin = rewardBB != null ? NumberUtils.GetMultiplierNumeratorValue(rewardBB.GetValue<long>("credit"), clubMultiplier) : 0L;

                titleText = StringTableUtils.GetString(StringTable.StringTableType.Global, isSingle ? "CLUB_CHALLENGE_TITLE_ONE" : "CLUB_CHALLENGE_TITLE", rewardCoin);

                var leaguePoint = BlackboardUtils.FindVariable<long>(challengeInfo.value, "leaguePoint");
                if(leaguePoint != null && leaguePoint.value > 0)
                {
                    titleText += PLUS + StringTableUtils.GetString(StringTable.StringTableType.Global, "COMMA_LP", leaguePoint.value);
                }

                List<long> rewardRatioNumeratorList = null;

                var stagePreset = BlackboardUtils.FindVariable<Blackboard>(challengeInfo.value, "stagePreset");
                if(stagePreset != null && stagePreset.value != null)
                    rewardRatioNumeratorList = BlackboardUtils.FindVariable<List<long>>(stagePreset.value, "coinRewardRatioNumeratorList").value;

                clubStageController.OnUpdateVariables(stage, maxStage, progressCount, rewardRatioNumeratorList);
            }

            MetaContextElementUtils.SetText(titleTexTelement, titleText);
        }
    }
}

}

