using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{

public class DynamicScrollClubSymbolListCreator : DynamicScrollItemCreator 
{
    public  string          symbolFormat;
    public  int             symbolCount;

    public  ObjectPool      objectPool;
    public  Blackboard      blackboard;

    public  ContextToggleGroup contextToggleGroupElement;

    public override void OnInitialize()
    {
        var selectSymbolName = blackboard.GetValue<string>("selectSymbolName");

        GameObject selectSymbol = null;
        int selectSymbolIndex = -1;

        for (int i = 0; i < symbolCount; ++i)
        {
            var go = objectPool.GetObject(false).gameObject;
            go.transform.SetParent(content, false);
            go.transform.SetAsLastSibling();
            InitSymbol(i, go.GetComponent<Blackboard>());
            go.SetActive(true);

            string symbolName = string.Format(symbolFormat, i + 1);
            if(symbolName == selectSymbolName)
            {
                selectSymbolIndex = i;
                selectSymbol = go;
            }
        }

        RebuildContentBounds();

        ContextElement agent = GetComponent<ContextElement>();
        if(agent != null) agent.UpdateContext(true);

        if(selectSymbol != null)
        {
            var viewportRectTransform = (RectTransform)dynamicScrollRect.viewport.transform;
            var contentRectTransform = (RectTransform)dynamicScrollRect.content.transform;
            var symbolRectTransform = (RectTransform)selectSymbol.transform;

            var limitY = contentRectTransform.rect.height - viewportRectTransform.rect.height;

            var tempPosition = contentRectTransform.anchoredPosition;
            float deltaY = -symbolRectTransform.localPosition.y - 50f;
            tempPosition.y = Mathf.Min(deltaY, limitY);

            contentRectTransform.anchoredPosition = tempPosition;

            contextToggleGroupElement.SetIntProperty(selectSymbolIndex);
        }
        else
        {
            dynamicScrollRect.verticalNormalizedPosition = 1f;
        }
    }

    private void InitSymbol(int index, Blackboard bb)
    {
        bb.SetValue("symbolName", string.Format(symbolFormat, index + 1));
        bb.SetValue("cellIndex", index + 1);
    }

    private void SelectPosition()
    {
        // var limitY = contentRectTransform.rect.height - viewportRectTransform.rect.height;
        // tempPosition.y = Mathf.Min(delta.value, limitY);
    }

    protected override bool PushFront()
    {
        return false;
    }

    protected override bool PushBack()
    {
        return false;
    }

    protected override bool PopFront()
    {
        return false;
    }

    protected override bool PopBack()
    {
        return true;
    }

}

}

