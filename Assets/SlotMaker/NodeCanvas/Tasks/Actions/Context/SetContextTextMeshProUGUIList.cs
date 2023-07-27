using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace SlotMaker
{

public class SetContextTextMeshProUGUIList : ContextCompositor, IContextText
{
    public List<TextMeshProUGUI> textMeshProUGUIList = new List<TextMeshProUGUI>();

    public void SetText(string text)
    {
        for(int i=0; i<textMeshProUGUIList.Count; ++i)
        {
            textMeshProUGUIList[i].text = text;
        }
    }

    public string GetText()
    {
        return textMeshProUGUIList[0].text;
    }
}

}
