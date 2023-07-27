using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/String")]
public class UpdateStringTable : ActionTask 
{
    public BBParameter<List<string>> dlcList;
    public BBParameter<string> bundleName;
    public BBParameter<string> assetName;
    public StringTable.StringTableType type;

    protected override string info
    {
        get { return "Update " + GetAssetName(); }
    }

    protected override void OnExecute()
    {
        if(dlcList == null || dlcList.value.Count < 0)
        {
            EndAction();
            return;
        }

        List<StringStringPairVariable> stringTable = null;

        for(int i=0; i<dlcList.value.Count; ++i)
        {
            if(dlcList.value[i] == GetBundleName())
            {
                stringTable = AssetBundleManager.LoadAsset<StringTableObject>(GetBundleName(), GetAssetName()).stringTable;
                break;
            }
        }

        if(stringTable != null)
        {
            var bb = StringTable.Get(type);

            foreach (var pair in stringTable)
            {
                BlackboardUtils.SetOrCreateValue<string>(bb, pair.key, pair.value);
            }
        }
        
        EndAction();
    }

    protected string GetBundleName()
    {
        return ApplicationSettings.MakeApplicationBundleName(bundleName.value);
    }

    protected string GetAssetName()
    {
        return ApplicationSettings.GetSystemLanguage() + "_" + assetName.value;
    }
}

}