using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/String")]
public class LoadStringTable : ActionTask 
{
	public BBParameter<string> bundleName;
	public BBParameter<string> assetName;
    public BBParameter<bool> combineApplicationType;
    public BBParameter<bool> combineApplicationLanguage;
	public StringTable.StringTableType type;

	protected override string info
	{
		get { return "Load " + GetAssetName(); }
	}

	protected override void OnExecute()
	{
		var bb = StringTable.Get(type);
		bb.variables.Clear();

		var stringTable = AssetBundleManager.LoadAsset<StringTableObject>(GetBundleName(), GetAssetName()).stringTable;
		foreach (var pair in stringTable)
		{
			var variable = bb.AddVariable(pair.key, typeof(string));
			variable.value = pair.value;
		}
		EndAction();
	}

    protected string GetBundleName()
    {
        return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
    }

	protected string GetAssetName()
	{
		return combineApplicationLanguage.value ? ApplicationSettings.GetSystemLanguage() + "_" + assetName.value : assetName.value;
	}
}

}
