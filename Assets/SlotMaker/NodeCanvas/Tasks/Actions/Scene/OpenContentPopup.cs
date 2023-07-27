using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace SlotMaker.Tasks.Actions
{

[Category("★ BagelCode/Popup")]
public class OpenContentPopup : ActionTask<Transform>
{
    public BBParameter<string> assetName;
    public BBParameter<string> uniqueName;

    public bool isGlobalPopup = false;

    [BlackboardOnly]
    public BBParameter<GameObject> saveAs;

    protected override string info
    {
        get { return string.Format("{0}OpenContentPopup({1})", (!saveAs.isNone ? (saveAs.ToString() + " = ") : ""), assetName); }
    }

    protected override void OnExecute()
    {
        var prefab = AssetBundleManager.LoadAsset<GameObject>(GetBundleName(), assetName.value);
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        go.name = uniqueName.value;

        if (isGlobalPopup)
            go.transform.SetParent(PopupManager.Instance.transform, false);
        else
            go.transform.SetParent(PopupManager.Instance.contents, false);
        PopupManager.Instance.Open(go);

        var bb = go.GetComponent<Blackboard>();
        if (bb != null)
        {
            var variable = BlackboardUtils.GetOrCreateVariable<GameObject>(bb, "caller");
            variable.value = agent.gameObject;
        }

        if (!saveAs.isNone)
            saveAs.value = go;
            
        EndAction();
    }

    protected string GetBundleName()
    {
        return BlackboardUtils.FindVariable<string>(null, "./game/gameTitle").value;
    }
}

}
