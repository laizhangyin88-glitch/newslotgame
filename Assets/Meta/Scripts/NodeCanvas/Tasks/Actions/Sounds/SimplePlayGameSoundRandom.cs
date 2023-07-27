using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Task.Action
{

    [Category("★ BagelCode/Meta/Sounds")]
    public class SimplePlayGameSoundRandom : ActionTask
    {
        public BBParameter<List<string>> idList;

        protected override string info { get { return "Play " + idList + " in random"; } }

        protected override void OnExecute()
        {
            GSManager.Instance.GetHandler(idList.value[Random.Range(0, idList.value.Count)]).Play();

            EndAction();
        }
    }

}
