using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using System;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/Contents")]
    public class UpdateReelStripsIndex : ActionTask<Blackboard>
    {
        public BBParameter<string> key;

        protected override void OnExecute()
        {
            try
            {

                Blackboard src = BlackboardUtils.FindVariable<Blackboard>(agent, key.value).value;
                //Debug.LogError($"key.value = {key.value}  agent.gameObject.name ={agent.gameObject.name}  {src.transform.parent.parent.parent.name}/{src.transform.parent.parent.name}/{src.transform.parent.name}/{src.gameObject.name}");
                var dst = BlackboardUtils.FindVariable<Blackboard>(null, "./game/reelSetIndex").value;
                dst.SetValue("currentIndex", src.GetValue<int>("currentIndex"));
                dst.SetValue("nextIndex", src.GetValue<int>("nextIndex"));
                EndAction();
            }
            catch (Exception ex)
            {
                Debug.LogError($"key.value = {key.value}  agent.gameObject.name ={agent.gameObject.name}");
                Debug.LogError($"error = {ex}");
            }

        }
    }

}
