using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Purchase")]
public class GetProductListFromGroup : ActionTask<Blackboard>
{
    public BBParameter<Blackboard> fromProductGroup;

    [BlackboardOnly]
    public BBParameter<List<Blackboard>> saveAsProudctList;

    protected override string info
    {
        get { return string.Format("{0} = Get {1} Product List", saveAsProudctList, fromProductGroup); }
    }

    protected override void OnExecute()
    {
        saveAsProudctList.value = BlackboardQueryUtils.GetProductList(fromProductGroup.value);

        EndAction();
    }
}

}
