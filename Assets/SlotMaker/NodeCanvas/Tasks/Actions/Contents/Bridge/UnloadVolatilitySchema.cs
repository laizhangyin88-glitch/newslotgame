using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker.Json;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Json")]
    public class UnloadVolatilitySchema : ActionTask
    {
        protected override void OnExecute()
        {
            BlackboardJson.UnloadVolatilitySchema();
            
            EndAction();
        }
    }
}