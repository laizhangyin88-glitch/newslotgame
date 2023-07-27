using System;
using System.Collections.Generic;
using BagelCode.ClientModels;
using UnityEngine;
using SlotMaker;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

using static BagelCode.Scratcher.ScratcherCellPlayGroup.PlayOption;

namespace BagelCode.Scratcher
{
    public class ScratcherCellController : MonoBehaviour
    {
        public ScratcherCellInfo cellInfo;
        
        public ScratcherSymbolType SymbolType { get { return cellInfo.symbol.type; } }
        public int SymbolId { get { return cellInfo.symbol.id; } }
        public long SymbolPrize { get { return cellInfo.symbol.prize; } }
        public Sprite SymbolSprite { get { return cellInfo.symbol.sprite; } }
        public string SymbolText { get { return cellInfo.symbol.text; } }
        public Color SymbolColor { get { return cellInfo.symbol.color; } }
        public string SymbolValue { get { return cellInfo.symbol.value; } }

        public int CoverId { get { return cellInfo.cover.coverID; } }
        public Color CoverColor { get { return cellInfo.cover.color; } }
        public Color CoverTextColor { get { return cellInfo.cover.textColor; } }
        public Sprite CoverSprite { get { return cellInfo.cover.sprite; } }
        public int CoverStopFrame { get { return cellInfo.cover.stopFrame; } }
        
        public List<ScratcherCellController> HighlightCells { get { return cellInfo.highlightInfo.highlightCells; } }
        public bool IsHighWin { get { return cellInfo.symbol.isHighWin; } }
        
        public bool IsHit { get { return cellInfo.highlightInfo.isHit; } }
        public List<GameObject> GameObjectsToActivate { get { return cellInfo.highlightInfo.gameObjectsToActivate; } }

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        private const string HIGH_WIN_COLOR = "#000B96";
        public const int END_FRAME = 13;

        private Animator animator;
        private ContextElement rootElement;
        private ContextElement particleElement;
        private ParticleSystem.EmissionModule particleEmission;

        private const float PLAY_PARTICLE_EFFECT_TIME = 0.25f;
        private const float WAIT_TIME = 0.1f;

        public void Init()
        {
            animator = GetComponent<Animator>();
            rootElement = GetComponent<ContextElement>();

            particleElement = ContextUtils.FindElement(rootElement, "Particle", ContextSearchingType.ChildrenSearch);
            if(particleElement != null)
            {
                var particleScratchElement = ContextUtils.FindElement(particleElement, "Particle Scratch", ContextSearchingType.ChildrenSearch);
                if(particleScratchElement != null)
                {
                    particleEmission = particleScratchElement.GetComponent<ParticleSystem>().emission;
                }
            }

            SetSymbolElement();
            SetCoverElement();
        }

        public IEnumerator Open(bool playSound, params ScratcherCellPlayGroup.PlayOption[] options)
        {
            for(int i = 0; i < options.Length; ++i)
            {
                switch(options[i])
                {
                    case ACTIVATE_PARTICLE:
                        StartCoroutine(PlayParticleEffect(PLAY_PARTICLE_EFFECT_TIME));
                        break;
                    case PLAY_HIGHLIGHT:
                        PlayHighlight(playSound);
                        break;
                    case DISABLE_HIGHLIGHT:
                        DisableHighlight();
                        break;
                    case OPEN_CELL:
                        OpenCell(playSound);
                        break;
                    case WAIT_BETWEEN:
                        yield return new WaitForSeconds(WAIT_TIME);
                        break;
                }
            }
        }

        private IEnumerator PlayParticleEffect(float duration)
        {
            if(particleElement != null)
            {
                MetaContextElementUtils.SetActive(particleElement, true);
                particleEmission.rateOverTime = 50.0f;

                yield return new WaitForSeconds(duration);

                MetaContextElementUtils.SetActive(particleElement, false);
            }
        }

        public void AddHighlightCells(List<ScratcherCellController> cells)
        {
            HighlightCells.AddRange(cells.Where(c => !HighlightCells.Contains(c)));
        }

        public void AddHighlightCells(ScratcherCellController cell)
        {
            if (!HighlightCells.Contains(cell))
                HighlightCells.Add(cell);
        }

        public void AddHighlightCellsSelf()
        {
            if (!HighlightCells.Contains(this))
                HighlightCells.Add(this);
        }
        
        public void AddGameObjectsToActivate(GameObject go)
        {
            GameObjectsToActivate.Add(go);
        }

        public bool HasHighlightCells()
        {
            return HighlightCells.Count > 0;
        }

        public void ClearHighlightCells()
        {
            HighlightCells.Clear();
        }

        private void PlayCellOpenSound()
        {
            GSManager.Instance.GetHandler("Collecting_Game_Scratcher_Scratch").Play();
        }

        private void PlayHighlightSound()
        {
            GSManager.Instance.GetHandler("Collecting_Game_Scratcher_Highlight").Play();
        }

        private void PlayHighlight(bool playSound)
        {
            if (playSound & HasHighlightCells()) PlayHighlightSound();

            for (int i = 0; i < HighlightCells.Count; i++)
            {
                var anim = HighlightCells[i].GetComponent<Animator>();
                anim.SetBool("Highlight", true);
            }

            for (int i = 0; i < GameObjectsToActivate.Count; i++)
                GameObjectsToActivate[i].SetActive(true);
        }

        private void DisableHighlight()
        {
            for (int i = 0; i < HighlightCells.Count; i++)
            {
                var anim = HighlightCells[i].GetComponent<Animator>();
                anim.SetBool("Highlight", false);
            }
        }

        private void OpenCell(bool playSound)
        {
            if (playSound) PlayCellOpenSound();

            int setFrame = END_FRAME;

            // if there is only prize or symbol, open all no matter what the double touch is true or not
            if (SymbolType == ScratcherSymbolType.BOTH)
            {
                int currentFrame = animator.GetInteger("StopFrame");
                if (CoverStopFrame > currentFrame)
                    setFrame = CoverStopFrame;
            }

            // in case of opening a cell, make a StopFrame value at animator parameter to int value of stop frame
            animator.SetInteger("StopFrame", setFrame);
        }

        //

        private void SetSymbolElement()
        {
            ContextElement symbolIconElement = ContextUtils.FindElement(rootElement, "Symbol/Icon", ContextSearchingType.FullNameSearch);
            if(symbolIconElement != null)
            {
                ContextElement symbolIconImageElement = ContextUtils.FindElement(symbolIconElement, "Image", ContextSearchingType.ChildrenSearch);
                if(symbolIconImageElement != null)
                {
                    if(SymbolSprite != null)
                    {
                        MetaContextElementUtils.SetSprite(symbolIconImageElement, SymbolSprite);
                        symbolIconImageElement.GetComponent<Image>().SetNativeSize();
                    }

                    if (SymbolColor != Color.clear)
                        symbolIconImageElement.GetComponent<CanvasRendererProperty>().color = SymbolColor;

                    symbolIconImageElement.gameObject.SetActive(SymbolSprite != null);
                }

                ContextElement symbolIconTextElement = ContextUtils.FindElement(symbolIconElement, "Text", ContextSearchingType.ChildrenSearch);
                if(symbolIconTextElement != null)
                {
                    if (!String.IsNullOrEmpty(SymbolText))
                        MetaContextElementUtils.SetText(symbolIconTextElement, SymbolText);

                    if(SymbolColor != Color.clear)
                        symbolIconTextElement.GetComponent<TextMeshProUGUI>().color = SymbolColor;


                    symbolIconTextElement.gameObject.SetActive(!String.IsNullOrEmpty(SymbolText));
                }

                symbolIconElement.gameObject.SetActive(SymbolType == ScratcherSymbolType.BOTH || SymbolType == ScratcherSymbolType.SYMBOL);
            }

            ContextElement symbolPrizeElement = ContextUtils.FindElement(rootElement, "Symbol/Prize", ContextSearchingType.FullNameSearch);
            if(symbolPrizeElement != null)
            {
                ContextElement prizeTextElement = ContextUtils.FindElement(symbolPrizeElement, "Text", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetText(prizeTextElement, StringTableUtils.GetString(tableType, "COLLECTING_GAME_SIMPLE_STYLE_COIN", SymbolPrize));

                if (IsHighWin)
                {
                    Color highWinColor;
                    ColorUtility.TryParseHtmlString(HIGH_WIN_COLOR, out highWinColor);
                    prizeTextElement.GetComponent<TextMeshProUGUI>().color = highWinColor;
                }

                symbolPrizeElement.gameObject.SetActive(SymbolType == ScratcherSymbolType.BOTH || SymbolType == ScratcherSymbolType.PRIZE);
            }

            ContextElement symbolBaseElement = ContextUtils.FindElement(rootElement, "Symbol/Base", ContextSearchingType.FullNameSearch);
            if (symbolBaseElement != null)
            {
                if (SymbolColor != Color.clear)
                    symbolBaseElement.GetComponent<CanvasRendererProperty>().color = SymbolColor;
            }

            ContextElement symbolHighlightElement = ContextUtils.FindElement(rootElement, "Symbol/Highlight", ContextSearchingType.FullNameSearch);
            if (symbolHighlightElement != null)
            {
                var customHighlightAssets = ScratcherCustomData.Instance.scratcherCellAssets.customHighlightAssets;
                var asset = customHighlightAssets.FirstOrDefault(a => a.coverId == CoverId);
                if (asset != null)
                {
                    MetaContextElementUtils.SetSprite(symbolHighlightElement, asset.sprite);
                }
            }
        }

        private void SetCoverElement()
        {
            ContextElement coverElement = ContextUtils.FindElement(rootElement, "Cover", ContextSearchingType.ChildrenSearch);
            if(coverElement != null)
            {
                ContextElement coverBaseElement = ContextUtils.FindElement(coverElement, "Cover Mask/Cover Base", ContextSearchingType.FullNameSearch);
                ContextElement coverSymbolElement = ContextUtils.FindElement(coverElement, "Cover Mask/Cover Symbol", ContextSearchingType.FullNameSearch);
                ContextElement coverTextElement = ContextUtils.FindElement(coverElement, "Cover Mask/Cover Text", ContextSearchingType.FullNameSearch);

                if (coverSymbolElement != null && CoverSprite != null)
                {
                    MetaContextElementUtils.SetSprite(coverSymbolElement, CoverSprite);
                }

                if (coverBaseElement != null && CoverColor != Color.clear)
                {
                    MetaContextElementUtils.SetColor(coverBaseElement, CoverColor);
                }

                if (coverTextElement != null && CoverTextColor != Color.clear && !String.IsNullOrEmpty(SymbolText))
                {
                    MetaContextElementUtils.SetText(coverTextElement, SymbolText);
                    MetaContextElementUtils.SetColor(coverTextElement, CoverTextColor);
                }
            }
        }
    }
}