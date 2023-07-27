using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using TMPro;

namespace GameStudio.Slot.FSF
{
    public class FSFSymbolEventHandler : DefaultSymbolEventHandler
    {
        public const string MULTIPLIER_STRING_FORMAT = "X{0}";
        public TextMeshProUGUI creditText;
        public SpriteRenderer jackpotRenderer;
        public RectTransform multiplierHighAnchor;
        public RectTransform multiplierNormalAnchor;

        public override void Apply(BaseSymbol symbol)
        {
            base.Apply(symbol);
            var symbolIndex = symbol.symbolIndex;
            animator.SetInteger("Symbol Index", symbolIndex);
            // EnterState("Entry");
            CallEntryFunc(symbol);
        }

        private void CallEntryFunc(BaseSymbol symbol)
        {
            var symbolIndex = symbol.symbolIndex;
            // jackpot symbol
            if (symbolIndex == 0)
            {
                symbol.GetComponent<FSFSymbolEventHandler>().creditText.gameObject.SetActive(false);
            }
            // fish symbol
            else if (symbolIndex > 0 && symbolIndex < 5)
            {
                var creditText = symbol.GetComponent<FSFSymbolEventHandler>().creditText;
                creditText.gameObject.SetActive(true);

                long pay = BlackboardUtils.FindValue<List<Blackboard>>(null, "./game/paytables")[0].GetValue<List<Blackboard>>("value")[symbol.symbolIndex].GetValue<List<long>>("value")[0];
                var baseWager = BlackboardUtils.FindValue<long>(null, "./game/baseWager");
                var baseBet = BlackboardUtils.FindValue<long>(null, "./betCredit");
                var actualPayCredit = pay * baseBet / baseWager;
                creditText.text = FormatUtility.SimpleNumberFormat(actualPayCredit);
            }
            // multiplier symbol
            else if (symbolIndex > 10 && symbolIndex < 16)
            {
                if (symbol.slotMachine.slotIndex != 0)
                {
                    var eventHandler = symbol.GetComponent<FSFSymbolEventHandler>();
                    var animator =eventHandler.animator;
                    animator.SetInteger("Symbol Index", FSFCommunityMainSlotFrameWinHandler.BLANK_SYMBOL_INDEX);

                    var renderer =symbol.GetComponentInChildren<SpriteRenderer>(true);
                    renderer.sortingOrder = 1;
                }
                else
                {
                    var symbolList = BlackboardUtils.FindValue<List<int>>("./game/symbolMultiplierInfo/symbolList");
                    var multiplierList = BlackboardUtils.FindValue<List<long>>("./game/symbolMultiplierInfo/multiplierList");
                    var multiplier = multiplierList[symbolList.IndexOf(symbol.symbolIndex)];

                    var creditText = symbol.GetComponent<FSFSymbolEventHandler>().creditText;
                    creditText.text = string.Format(FSFSymbolEventHandler.MULTIPLIER_STRING_FORMAT, multiplier);
                    creditText.gameObject.SetActive(true);
                }
            }
            else
            {
                symbol.GetComponent<FSFSymbolEventHandler>().creditText.gameObject.SetActive(false);
            }
        }
    }
}
