using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using TMPro;

namespace BagelCode
{

public class LuckySpinSymbol : MonoBehaviour
{
	public List<GameObject> customObjects;
	public Image slotThumbnail;
    public List<GameObject> spinCountObjList;
    public List<TextMeshProUGUI> spinCountTextList;

    

    private const string betTextKey = "x{0}";

    public List<TextMeshProUGUI> betTextList;
	public List<GameObject> betList;
    public List<int> betGradeRange;

    public int highlightCount = 0;

    private void Awake()
    {
        highlightCount = BlackboardQueryUtils.GetHighlightSpinCountThreshold();
    }

	public void Apply(BaseSymbol symbol)
	{
		if (symbol == null || symbol.symbolInfo == null) return;
		int customType = GetCustomType(symbol.symbolInfo.mask);
		ToggleCustomObject(customType);

		switch (customType)
		{
		case 0:
            if(LuckySpinSlotThumbnailLoader.Instance.symbols.ContainsKey(symbol.symbolInfo.symbol))
            {
                slotThumbnail.sprite = LuckySpinSlotThumbnailLoader.Instance.symbols[symbol.symbolInfo.symbol];
            }
            else
            {
                // Error Log
                Debug.Log(string.Format("with our symbol {0}", symbol.symbolInfo.symbol));
            }
			break;
		case 1:
            {
                if(highlightCount <= symbol.symbolInfo.symbol)
                {
                    spinCountObjList[0].SetActive(false);

                    spinCountObjList[1].SetActive(true);
                    spinCountTextList[1].text = symbol.symbolInfo.symbol.ToString();
                }
                else
                {
                    spinCountObjList[0].SetActive(true);
                    spinCountTextList[0].text = symbol.symbolInfo.symbol.ToString();

                    spinCountObjList[1].SetActive(false);
                }
            }
			break;
		case 2:
			SetBetScale(symbol.symbolInfo.symbol);
			break;
		}
	}

	private int GetCustomType(SymbolAttribute mask)
	{
		switch (mask)
		{
		case SymbolAttribute.Scatter1:
			return 0;
		case SymbolAttribute.Scatter2:
			return 1;
		case SymbolAttribute.Scatter3:
			return 2;
		case SymbolAttribute.Scatter4:
			return 3;
		}

		return 0;
	}

	private void ToggleCustomObject(int customType)
	{
		int count = customObjects.Count;
		for (int i = 0; i < count; ++i)
		{
			customObjects[i].SetActive(i == customType);
		}
	}

	private void SetBetScale(int betType)
	{
        for(int i=0; i < betTextList.Count; ++i)
        {
            betTextList[i].text = string.Format(betTextKey, betType);
        }

        int betIndex = 0;
        for(int i=0; i < betGradeRange.Count; ++i)
        {
            if(betType >= betGradeRange[i])
            {
                betIndex = i;
            }
        }
        
		for (int i = 0; i < betList.Count; ++i)
		{
			betList[i].SetActive(i == betIndex);
		}
	}
}

}
