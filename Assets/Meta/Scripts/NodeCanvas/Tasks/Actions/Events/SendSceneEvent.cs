using UnityEngine;
using System.Collections;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Events")]
public class SendSceneEvent : ActionTask
{
    public BBParameter<string> bundleName;
    public bool combineApplicationType;
    public BBParameter<string> assetName;
    public BBParameter<Transform> parent;
    public BBParameter<string> parentName;
    public bool isPopup; 
    
    protected override void OnUpdate()
    {
        MonoManager.current.StartCoroutine(SceneUtils.LoadSceneAsync(
            GetBundleName(), assetName.value, GetParent(), false, 
            (result) =>
            {
                if (isPopup)
                    PopupManager.Instance.Open(result);
            }
        ));
        
        EndAction();
    }
    
    protected string GetBundleName()
	{
		return combineApplicationType ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
	}
    
    protected Transform GetParent()
    {
        var _parent = parent.value;
        if (!string.IsNullOrEmpty(parentName.value))
        {
            if (_parent == null)
                _parent = GameObject.Find(parentName.value).transform;
            else 
                _parent = _parent.Find(parentName.value);
        }
        return _parent;
    }
}
    
}
