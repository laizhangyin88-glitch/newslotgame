using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class SimpleLoadContentExpandSymbolPrefab : ActionTask<BaseSymbol>
{
    public BBParameter<Transform> parent;
    public BBParameter<GameObject> delegator;

    public BBParameter<string> format = "{0} {1}x{2}";

    protected override string info
    {
        get { return string.Format("{0} = LoadSymbolExpandPrefab({1})", delegator, agent); }
    }

    private string GetSymbolName()
    {
        var info = agent.symbolInfo;
        return string.Format(format.value,
            ContentCustomData.Instance.symbolName[info.symbol],
            info.link.columnCount, info.link.rowCount);
    }

    protected override void OnExecute()
    {
        if (delegator.value == null)
        {
            var symbolName = GetSymbolName();
            var prefab = AssetBundleManager.LoadAsset<GameObject>(GetBundleName(), symbolName);
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.name = symbolName;

            var _parent = parent.value;
            go.transform.SetParent(_parent, false);

            delegator.value = go;
        }
        else
        {
            delegator.value.SetActive(true);
        }

        EndAction();
    }

    protected string GetBundleName()
    {
        return BlackboardUtils.FindVariable<string>(null, "./game/gameTitle").value;
    }
}

}
