using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class ReplaceCustomDatasOnReelStrip : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<GameObject> reelStripsObject = null;
    public BBParameter<int> reelIndex;
    public BBParameter<int> beginIndex;
    public BBParameter<string> customDataKey;
    public BBObjectParameter customDataValues;
    public BBParameter<bool> useSingleCustomData;
    public BBParameter<bool> clearCustomData;

    protected override string info
    {
        get { return string.Format("Replace CustomDatas On ReelStrip({0}, {1}, {2})", reelIndex, beginIndex, customDataValues); }
    }

    protected override void OnExecute()
    {
        var customDataValueList = customDataValues.value as IList;
        var strips = (reelStripsObject.isNull || reelStripsObject.isNone) ? GlobalReelStrips.Instance.GetReelStrips() : reelStripsObject.value.GetComponent<ReelStrips>();
        var reelStrip = strips.GetReelStrip(reelIndex.value);
        ReplaceOnReepStrip(reelStrip, customDataValueList);

        EndAction();
    }

    private int GetCustomDataIndex(int index)
    {
        return useSingleCustomData.value ? 0 : index;
    }

    private void ReplaceOnReepStrip(BaseReelStrip reelStrip, IList customDataValueList)
    {
        var newList = new List<SymbolInfo>();
        for (var i = 0; i < customDataValueList.Count; i++)
        {
            var newSymbolInfo = (SymbolInfo)reelStrip.GetSymbol(reelStrip.CalcIndex(beginIndex.value + i)).Clone();
            if (newSymbolInfo.customData == null)
                newSymbolInfo.customData = new Dictionary<string, object>();
            else if (clearCustomData.value)
                newSymbolInfo.customData.Clear();

            int customDataIndex = GetCustomDataIndex(i);
            newSymbolInfo.customData[customDataKey.value] = customDataValueList[customDataIndex];
            newList.Add(newSymbolInfo);
        }

        reelStrip.ReplaceRange(beginIndex.value % reelStrip.stripCount, newList);
    }

    ////////////////////////////////////////
    ///////////GUI AND EDITOR STUFF/////////
    ////////////////////////////////////////
#if UNITY_EDITOR

    protected override void OnTaskInspectorGUI() {
        DrawDefaultInspector();
        if ( GUILayout.Button("Select Target List Variable Type") ) {
            EditorUtils.ShowPreferedTypesSelectionMenu(typeof(object), (t) =>
            {
                customDataValues.SetType(typeof(List<>).MakeGenericType(t));
            });
        }
    }

#endif
}

}
