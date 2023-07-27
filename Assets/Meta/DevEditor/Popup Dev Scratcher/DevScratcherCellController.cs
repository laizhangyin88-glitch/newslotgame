using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using TMPro;

namespace BagelCode
{
    public class DevScratcherCellController : MonoBehaviour
    {
        public ContextElement symbolAreaElement;
        public ContextElement symbolImageElement;
        public ContextElement symbolTextElement;
        public ContextElement symbolPrizeElement;

        public CanvasRendererProperty symbolRenderer;
        public TextMeshProUGUI symbolTextTmp;

        public Image symbolImage;
        public Color symbolImageDefaultColor;
        public Color symbolTextDefaultColor;

        public ContextElement coverAreaElement;
        public ContextElement coverBaseElement;
        public ContextElement coverSymbolElement;

        public ContextElement cellTextElement;

        private bool isInit = false;

        public void Init()
        {
            if(isInit) return;

            ContextElement agent = GetComponent<ContextElement>();
            agent.UpdateContext(false);

            symbolAreaElement = ContextUtils.FindElement(agent, "Symbol", ContextSearchingType.ChildrenSearch);
            symbolImageElement = ContextUtils.FindElement(symbolAreaElement, "Icon/Image", ContextSearchingType.FullNameSearch);
            symbolTextElement = ContextUtils.FindElement(symbolAreaElement, "Icon/Text", ContextSearchingType.FullNameSearch);
            symbolPrizeElement = ContextUtils.FindElement(symbolAreaElement, "Prize/Text", ContextSearchingType.FullNameSearch);

            coverAreaElement = ContextUtils.FindElement(agent, "Cover", ContextSearchingType.ChildrenSearch);
            coverBaseElement = ContextUtils.FindElement(coverAreaElement, "Cover Mask/Cover Base", ContextSearchingType.FullNameSearch);
            coverSymbolElement = ContextUtils.FindElement(coverAreaElement, "Cover Mask/Cover Symbol", ContextSearchingType.FullNameSearch);

            cellTextElement = ContextUtils.FindElement(agent, "Name", ContextSearchingType.ChildrenSearch);

            symbolImage = symbolImageElement.GetComponent<Image>();
            symbolRenderer = symbolImageElement.GetComponent<CanvasRendererProperty>();
            symbolTextTmp = symbolTextElement.GetComponent<TextMeshProUGUI>();

            symbolImageDefaultColor = symbolRenderer.color;
            symbolTextDefaultColor = symbolTextTmp.color;

            isInit = true;
        }

        public void Refresh(ScratcherDevCellInfo cellInfo)
        {
            Init();
            
            if(cellInfo == null) return;

            if(cellInfo.symbol != null)
            {
                symbolAreaElement.gameObject.SetActive(true);
                coverAreaElement.gameObject.SetActive(false);

                MetaContextElementUtils.SetText(symbolTextElement, cellInfo.symbol.text);
                MetaContextElementUtils.SetText(cellTextElement, string.Format("SYMBOL_{0}", cellInfo.index));

                if(cellInfo.symbol.sprite != null)
                {
                    symbolImageElement.gameObject.SetActive(true);
                    MetaContextElementUtils.SetSprite(symbolImageElement, cellInfo.symbol.sprite);
                    symbolImageElement.GetComponent<Image>().SetNativeSize();
                }
                else
                {
                    symbolImageElement.gameObject.SetActive(false);
                }

                if(cellInfo.symbol.color != Color.clear)
                {
                    symbolRenderer.color = cellInfo.symbol.color;
                    symbolTextTmp.color = cellInfo.symbol.color;
                }
                else
                {
                    symbolRenderer.color = symbolImageDefaultColor;
                    symbolTextTmp.color = symbolTextDefaultColor;
                }
            }
            else if(cellInfo.cover != null)
            {
                symbolAreaElement.gameObject.SetActive(false);
                coverAreaElement.gameObject.SetActive(true);

                MetaContextElementUtils.SetSprite(coverSymbolElement, cellInfo.cover.sprite);
                MetaContextElementUtils.SetColor(coverBaseElement, cellInfo.cover.color);
                MetaContextElementUtils.SetText(cellTextElement, string.Format("({0}) COVER_{1}", cellInfo.cover.stopFrame == 0 ? "SINGLE" : "DOUBLE", cellInfo.index));
            }
        }
    }
}
