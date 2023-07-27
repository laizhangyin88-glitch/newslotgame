using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/GameObject")]
public class SetLayer : ActionTask<Component>
{
    public BBParameter<string> layerName;
    public BBParameter<bool> recursive;

    protected override string info
    {
        get { return string.Format("SetLayer({0}{1})", layerName, (recursive.value ? ", Recursive" : "")); }
    }

    protected override void OnExecute()
    {
        int layerNumber = LayerMask.NameToLayer(layerName.value);

        if (!recursive.value)
        {
            agent.gameObject.layer = layerNumber;
        }
        else
        {
            Transform[] children = agent.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                child.gameObject.layer = layerNumber;
            }
        }

        EndAction();
    }
}

}
