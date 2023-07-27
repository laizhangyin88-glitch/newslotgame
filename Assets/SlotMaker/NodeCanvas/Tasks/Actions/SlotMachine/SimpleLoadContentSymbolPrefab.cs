using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class SimpleLoadContentSymbolPrefab : ActionTask
{
    public BBParameter<int> symbolIndex;
    public BBParameter<Transform> parent;
    public BBParameter<GameObject> delegator;
    public BBParameter<GameObject> animator;

    protected override string info
    {
        get { return string.Format("{0} = LoadSymbolPrefab({1})", delegator, symbolIndex); }
    }

    private string GetSymbolName()
    {
        return ContentCustomData.Instance.symbolName[symbolIndex.value];
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

        animator.value.SetActive(false);

        EndAction();
    }

    protected string GetBundleName()
    {
        return BlackboardUtils.FindVariable<string>(null, "./game/gameTitle").value;
    }
}

}
