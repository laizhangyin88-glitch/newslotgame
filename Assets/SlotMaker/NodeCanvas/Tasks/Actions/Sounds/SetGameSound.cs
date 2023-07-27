using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/Sounds")]
    public class SetGameSound : ActionTask
    {
        public BBParameter<GameSound> valueA;
        public BBParameter<GameSound> valueB;

        protected override string info { get { return string.Format("{0} = {1}", valueA, valueB); } }

        protected override void OnExecute()
        {
            valueA.value = valueB.value;
            EndAction();
        }
    }

}
