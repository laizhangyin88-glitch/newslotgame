using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/String")]
public class UpdateStringTableFromDeviceLanguage : ActionTask 
{
    public BBParameter<string> bundleName;
    public StringTable.StringTableType type;

    protected override string info
    {
        get { return string.Format("Update String Table({0})", GetAssetName()); }
    }

    protected override void OnExecute()
    {
        var bb = StringTable.Get(type);

        // "Afrikaans", "Arabic", "Basque", "Belarusian", "Bulgarian", "Catalan",
        // "Chinese", "Czech", "Danish", "Dutch", "English", "Estonian", "Faroese",
        // Finnish", "French", "German", "Greek", "Hebrew", "Icelandic", "Indonesian",
        // "Italian", "Japanese", "Korean", "Latvian", "Lithuanian", "Norwegian", "Polish",
        // "Portuguese", "Romanian", "Russian", "SerboCroatian", "Slovak", "Slovenian",
        // "Spanish", "Swedish", "Thai", "Turkish", "Ukrainian", "Vietnamese", "ChineseSimplified",
        //  "ChineseTraditional", "Unknown", "Hungarian"

        var stringTableObj = AssetBundleManager.LoadAsset<StringTableObject>(GetBundleName(), GetAssetName());
        if(stringTableObj != null)
        {
            var stringTable = stringTableObj.stringTable;
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
        return ApplicationSettings.GetDeviceLanguage().ToLower();
    }
}

}