using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_GetSpotNumber: ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpotInstance> spot;
        public BBParameter<int> saveAs;

        protected override string info { get { return $"{saveAs} = Keno Spot Number"; } }

        protected override void OnExecute()
        {
            saveAs.value = spot.value.number;
            EndAction();
        }
    }
}