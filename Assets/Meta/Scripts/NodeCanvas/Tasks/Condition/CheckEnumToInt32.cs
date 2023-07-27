using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using NodeCanvas.Framework.Internal;

namespace NodeCanvas.Tasks.Conditions
{

    [Category("★ BagelCode")]
    [Description("Checks if (int)enumType compare int ")]
    public class CheckEnumToInt32 : ConditionTask<Blackboard>
    {

        public BBObjectParameter valueA = new BBObjectParameter(typeof(System.Enum));
        public BBParameter<int> valueB = new BBParameter<int>();

        protected override string info
        {
            get { return valueA + " == " + valueB; }
        }

        protected override bool OnCheck()
        {
            var variableA = (int)valueA.value;
            return variableA == valueB.value;
        }
    }
}
