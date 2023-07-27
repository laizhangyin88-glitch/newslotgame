using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/Sounds")]
    public class SimplePlayGameSoundRandom : ActionTask
    {
        public BBParameter<List<string>> idList;

        protected override string info { get { return "Play " + idList + " in random (SlotMaker Action)"; } }

        protected override void OnExecute()
        {
            GSManager.Instance.GetHandler(idList.value[Random.Range(0, idList.value.Count)]).Play();

            EndAction();
        }
    }

}
