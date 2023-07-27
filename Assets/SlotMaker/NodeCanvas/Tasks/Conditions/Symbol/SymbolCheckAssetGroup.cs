using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Conditions
{

[Category("★ SlotMaker/Symbol")]
public class SymbolCheckAssetGroup : ConditionTask
{
    public CompareMethod checkType = CompareMethod.EqualTo;
    public int compareValue;

    protected override string info { get { return "AssetGroup" + OperationUtils.GetCompareString(checkType) + compareValue; } }

    protected override bool OnCheck()
    {
        return OperationUtils.Compare(GlobalSymbolAssets.Instance.assetGroupId, compareValue, checkType);
    }
}

}
