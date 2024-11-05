using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Context")]
    public class RemoveChildrenAtContextElement : ActionTask<ContextElement>
    {
        protected override string info { get { return string.Format("{0}.RemoveChildren", agentInfo); } }

        protected override void OnExecute()
        {
            var enumerator = agent.GetEnumerator();
            if (enumerator != null)
            {
                var removeList = new List<ContextElement>();
                while (enumerator.MoveNext())
                    removeList.Add(enumerator.Current as ContextElement);

                int removeCount = removeList.Count;
                for (int i = 0; i < removeCount; ++i)
                {
                    agent.RemoveContextElement(removeList[i]);
                }
            }

        	EndAction();
        }
    }
}
