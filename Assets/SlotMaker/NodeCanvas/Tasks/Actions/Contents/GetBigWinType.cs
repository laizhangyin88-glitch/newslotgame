using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/Contents")]
    public class GetBigWinType : ActionTask<Blackboard>
    {
        public BBParameter<long> betCredit;
        public BBParameter<string> earnCredit = "./spin/earnCredit";
        public BBParameter<string> referenceTable = "./game/winTypeMultiplierInfo";
        public BBParameter<List<string>> bigWinTypeList = new List<string>{ "BIG", "SUPER_BIG", "MEGA", "SUPER_MEGA", "EPIC" };

        [BlackboardOnly]
        public BBParameter<int> saveAs;

        protected override void OnExecute()
        {
            var variable = BlackboardUtils.FindVariable<Dictionary<string, int>>(agent, referenceTable.value);
            if (variable == null)
            {
                Debug.Log("[Blackboard](" + agent.name + ") Null variable founded in " + referenceTable.value);
                EndAction(false);
            }
            else 
            {
                saveAs.value = -1;
                long earnCredit_ = BlackboardUtils.FindVariable<long>(agent, earnCredit.value).value;

                var list = bigWinTypeList.value;
                for (int i = list.Count - 1; i >= 0; --i)
                {
                    int multiplier;
                    if (variable.value.TryGetValue(list[i], out multiplier))
                    {
                        if (earnCredit_ >= betCredit.value * multiplier)
                        {
                            saveAs.value = i;
                            break;
                        }
                    }
                }
            }

            EndAction();
        }
    }

}
