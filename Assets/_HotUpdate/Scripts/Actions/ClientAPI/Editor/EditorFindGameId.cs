using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI/Editor")]
public class EditorFindGameId : ActionTask
{
    public BBParameter<int> gameId;
    public BBParameter<string> betZoneId;

    protected override void OnExecute()
    {
#if UNITY_EDITOR
        string gameTitle = EditorPrefs.GetString("gameTitle");
        var gameInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/gameInfoList").value;
        
        for (int i = 0; i < gameInfoList.Count; ++i)
        {
            if (string.Equals(gameInfoList[i].GetValue<string>("gameTitle"), gameTitle))
            {
                gameId.value = gameInfoList[i].GetValue<int>("gameId");
                betZoneId.value = "free";
                break;
            }
        }
#endif
        EndAction();
    }
}

}
