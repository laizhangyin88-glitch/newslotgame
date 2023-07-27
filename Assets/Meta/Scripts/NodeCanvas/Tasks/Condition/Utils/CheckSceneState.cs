using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using System.Collections.Generic;
using BagelCode;
using SlotMaker;

namespace BagelCode.Tasks.Condition
{

    [Category("★ BagelCode/Utils")]
    public class CheckSceneState : ConditionTask<Blackboard> 
    {
        public BBParameter<SceneState> sceneState;

        public bool isPrev;

        protected override string info
        {
            get
            {
                if(isPrev)
                {
                    return string.Format("Prev Scene == {0}", sceneState);
                }

                return string.Format("Current Scene == {0}", sceneState);
            }
        }

        protected override bool OnCheck() 
        {
            if(isPrev)
            {
                var prevSceneState = BlackboardUtils.GetOrCreateVariable<SceneState>(MainBlackboard.Get(), "prevSceneState");
                return prevSceneState.value == sceneState.value;
            }

            var currentSceneState = BlackboardUtils.GetOrCreateVariable<SceneState>(MainBlackboard.Get(), "currentSceneState");
            return currentSceneState.value == sceneState.value;
        }
    }

}
