using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using NodeCanvas.Tasks.Actions;
using SlotMaker.Tasks.Actions;
using UnityEngine;

namespace GameStudio.Slot.SVD
{
    public class SVDGoldEggSymbolBehaviour : SVDCommonSymbolBehaviour
    {
        private const int BaseLayerHash = -1422705371;
        private const string ContextAnimatorName = "_WinAnimatorContext";
        private const string GlobalAnimatorBBName = "Animator_BB";

        private readonly int TitleAnimationStateHash = Animator.StringToHash("Title");
        private readonly int JPAnimationStateHash = Animator.StringToHash("JP");
        private readonly int EggAnimationStateHash = Animator.StringToHash("Egg");
        private readonly int IsMultiplierForJpAnimationStateHash = Animator.StringToHash("isMultiplierForJP");
        private readonly int StartFlowTypeAnimationStateHash = Animator.StringToHash("startFlowType");
        private readonly int WinAnimationStateHash = Animator.StringToHash("Win");
        private readonly int FlyAnimationStateHash = Animator.StringToHash("Fly");

        private Blackboard symbolBB;

        public override void OnEntry()
        {
            base.OnEntry();
            symbolBB = symbol.GetComponent<SVDSymbolEventHandler>().GetSymbolBlackboard();

            SetVariable("currentWeightsIndex", 0, symbolBB, true);
            SetVariable("coinValuesIndex", 0, symbolBB, true);

            SymbolSetSprite(GetBaseImage(symbolBB), 0, symbolBB);
            SymbolSetSortingOrder(GetBaseImage(symbolBB), BaseLayerHash, 0, symbolBB);

            SymbolSetActiveCachingObjectExecute(0, SymbolSetActiveCachingObject.SetActiveMode.Deactivate, symbolBB);

            animator.SetInteger(TitleAnimationStateHash, 1);
            SetVariable("_finishedFly",false, symbolBB, true);
            SetVariable("_row", GetVariable<int>("row", symbolBB), symbolBB, true);
            SetVariable("_column", GetVariable<int>("column", symbolBB), symbolBB, true);
            SetVariable("applyCoinValue", 0d, symbolBB, true);
            SetVariable("shouldFly", false, symbolBB, true);
            SetVariable("_respinExpandType", 0, symbolBB, true);

            if (GetAnimatorBB().GetVariable<bool>("isRespinBonus").value)
            {
                SetVariable("_respinExpandType",
                    GetVariable<int>("./customData/respinExpandType", symbolBB), symbolBB);
            }

            var goldProbabilitiesList = GetVariable<List<List<float>>>("goldEggProbabilityTable", symbolBB);
            var probabilitiesIndex = GetVariable<int>("_respinExpandType", symbolBB);
            var targetProbabilitiesList = goldProbabilitiesList[probabilitiesIndex];

            symbolBB.GetComponent<WeightRandomGenerator>().probabilities = targetProbabilitiesList;

            var symbolJackpotCoinDouble = GetParentObject(symbolBB).GetComponent<SymbolJackpotCoinDouble>();
            var symbolObj = GetVariable<BaseSymbol>("baseSymbol", symbolBB);

            symbolJackpotCoinDouble.Change(symbolObj);
            symbolJackpotCoinDouble.Apply(symbolObj);

            SetVariable("_tempCoinIndex", GetVariable<int>("coinIndex", symbolBB), symbolBB, true);

            symbolJackpotCoinDouble.Apply(symbolObj);

            if (GetVariable<int>("_tempCoinIndex", symbolBB) > 11)
            {
                SetVariable("jpIndex", GetVariable<int>("_tempCoinIndex", symbolBB) - 12, symbolBB, true);

                SymbolSetSortingOrder(GetVariable<SpriteRenderer>("eggImage", symbolBB), BaseLayerHash, 0, symbolBB);
                var jpIndex = GetVariable<int>("jpIndex", symbolBB);

                animator.SetInteger(TitleAnimationStateHash, 2);
                animator.SetInteger(JPAnimationStateHash, jpIndex);
                animator.SetInteger(EggAnimationStateHash, jpIndex);

                GetBlackboardValueAtList<int>("jpIndexList",
                    jpIndex,
                    GetVariable<int>("_jpMult", symbolBB), symbolBB);

                GetBlackboardValue<long>("./betCredit", GetVariable<long>("_Bet", symbolBB), symbolBB);

                SetVariable("_cashPayValue", GetVariable<long>("_Bet", symbolBB) *
                                             GetVariable<int>("_jpMult", symbolBB), symbolBB, true);

                SetContextText1(GetVariable<ContextElement>("jpCoinText", symbolBB),
                    "VALUE_CREDIT_TEXT", StringTable.StringTableType.Content, "_cashPayValue", symbolBB);
            }
            else
            {
                animator.SetInteger(EggAnimationStateHash, -1);
                SymbolSetSprite(GetBaseImage(symbolBB), 0, symbolBB);

                var coinTextElement = symbolBB.GetComponent<SymbolJackpotCoinDouble>().coinText
                    .GetComponent<ContextElement>();

                SetVariable("cashPayText", coinTextElement, symbolBB, true);
                GetVariable<RectTransform>("CounterTransform", symbolBB).gameObject.SetActive(true);

                var cashPayContextText = GetVariable<ContextElement>("cashPayText", symbolBB);

                var cashPayText = ((IContextText) cashPayContextText).GetText();
                BlackboardUtils.GetOrCreateVariable<string>(symbolBB, "_cashPayValueString").value = cashPayText;

                GetVariable<GameObject>("base", symbolBB).SetActive(true);
            }
        }

        public override void OnSkip()
        {
            SymbolSetSprite(GetBaseImage(symbolBB), 0, symbolBB);
            SymbolSetActiveCachingObjectExecute(0, SymbolSetActiveCachingObject.SetActiveMode.Deactivate, symbolBB);
        }

        public override void OnStopEffect()
        {
            SymbolSetSprite(GetBaseImage(symbolBB), 1, symbolBB);
            var jpIndex = GetVariable<int>("jpIndex", symbolBB);

            if (GetVariable<int>("_tempCoinIndex", symbolBB) <= 11)
            {
                SymbolGetGameObject(0, 0, symbolBB);
                UpdateContext(true, symbolBB);

                BlackboardUtils.GetOrCreateVariable<ContextElement>(symbolBB, "_lendingTextElement").value =
                    ContextUtils.FindElement(symbolBB.GetComponent<ContextElement>(), "Amount", ContextSearchingType.ChildrenSearch);

                SetContextText1(GetVariable<ContextElement>("_lendingTextElement", symbolBB),
                    "RESPIN_VALUE", StringTable.StringTableType.Content, "_cashPayValueString", symbolBB);
            }
            else
            {
                SymbolSetSortingOrder(GetVariable<SpriteRenderer>("eggImage", symbolBB), BaseLayerHash, 0, symbolBB);

                animator.SetInteger(TitleAnimationStateHash, 2);
                animator.SetInteger(JPAnimationStateHash, jpIndex);
                animator.SetInteger(EggAnimationStateHash, jpIndex);

                GetBlackboardValueAtList<int>("jpIndexList", jpIndex,
                    GetVariable<int>("_jpMult", symbolBB), symbolBB);

                GetBlackboardValue<long>("./betCredit", GetVariable<long>("_Bet", symbolBB), symbolBB);

                SetVariable("_cashPayValue", GetVariable<long>("_Bet", symbolBB) * GetVariable<int>("_jpMult", symbolBB),
                    symbolBB, true);

                SetContextText1(GetVariable<ContextElement>("jpCoinText", symbolBB),
                    "VALUE_CREDIT_TEXT", StringTable.StringTableType.Content, "_cashPayValue", symbolBB);

                SetVariable("_jpOffsetIndex", jpIndex + 6, symbolBB, true);

                SymbolGetGameObject(GetVariable<int>("_jpOffsetIndex", symbolBB), 0, symbolBB);
            }

            if (GetAnimatorBB().GetVariable<bool>("isRespinBonus").value)
            {
                ClearSymbolGraphics();
            }
        }

        public void Drop()
        {
            SymbolSetActiveCachingObjectExecute(0, SymbolSetActiveCachingObject.SetActiveMode.Deactivate, symbolBB);
            ClearSymbolGraphics();
        }

        public void OnDrop()
        {
            SymbolSetActiveCachingObjectExecute(0, SymbolSetActiveCachingObject.SetActiveMode.Deactivate, symbolBB);
            ClearSymbolGraphics();
        }

        public void DropSymbolWin()
        {
            SetVariable("shouldFly", false, symbolBB, true);
            if (GetVariable<bool>("isJackpot", symbolBB))
            {
                if (OnCheck("./customData/originalJPIndex"))
                {
                    new SymbolGetCustomData<int>()
                    {
                        key = "originalJPIndex",
                        saveAs = GetVariable<int>("jpIndex", symbolBB)
                    }.ExecuteAction(symbolBB, symbolBB);
                }

                SymbolSetSortingOrder(GetVariable<SpriteRenderer>("eggImage", symbolBB), BaseLayerHash, 0, symbolBB);
                var jpIndex = GetVariable<int>("jpIndex", symbolBB);

                animator.SetInteger(TitleAnimationStateHash, 2);
                animator.SetInteger(JPAnimationStateHash, jpIndex);
                animator.SetInteger(EggAnimationStateHash, jpIndex);

                GetBlackboardValueAtList<int>("jpIndexList",
                    jpIndex,
                    GetVariable<int>("_jpMult", symbolBB), symbolBB);

                BlackboardUtils.GetOrCreateVariable<string>(symbolBB, "_jpPrefabName").value =
                    BlackboardUtils.GetOrCreateVariable<List<string>>(symbolBB, "jpWinPrefabNamesList")
                        .value[jpIndex];

                SetVariable("_jpOffsetIndex", jpIndex + 2, symbolBB, true);

                SymbolGetGameObject(GetVariable<int>("_jpOffsetIndex", symbolBB), 0, symbolBB);
                UpdateContext(true, symbolBB);

                BlackboardUtils .GetOrCreateVariable<ContextElement>(symbolBB, ContextAnimatorName).value =
                    ContextUtils.FindElement(symbolBB.GetComponent<ContextElement>(), BlackboardUtils.GetOrCreateVariable<string>(symbolBB, "_jpPrefabName").value, ContextSearchingType.FullNameSearch);
                var winAnimatorContext = BlackboardUtils .GetOrCreateVariable<ContextElement>(symbolBB, ContextAnimatorName).value;
                BlackboardUtils.GetOrCreateVariable<ContextElement>(symbolBB, "_winAmountContextText").value =
                    ContextUtils.FindElement(winAnimatorContext, "Amount Base", ContextSearchingType.FullNameSearch);

                SetContextText1(GetVariable<ContextElement>("_winAmountContextText", symbolBB),
                    "VALUE_CREDIT_TEXT", StringTable.StringTableType.Content, "applyCoinValue", symbolBB);

                BlackboardUtils.GetOrCreateVariable<ContextElement>(symbolBB, "_winMultipliedAmountContextText").value =
                    ContextUtils.FindElement(winAnimatorContext, "Amount Multiplied", ContextSearchingType.FullNameSearch);

                BlackboardUtils.GetOrCreateVariable<ContextElement>(symbolBB, "_winMultiplierContextText").value =
                    ContextUtils.FindElement(winAnimatorContext, "Text - Multiplier", ContextSearchingType.FullNameSearch);

                GetContextAnimator(symbolBB, ContextAnimatorName).SetInteger(StartFlowTypeAnimationStateHash, 2);
            }
            else
            {
                ClearCachedObjects();
                SymbolGetGameObject(1, 0, symbolBB);
                UpdateContext(true, symbolBB);

                var parentContext = symbolBB.GetComponent<ContextElement>();
                BlackboardUtils.GetOrCreateVariable<ContextElement>(symbolBB, ContextAnimatorName).value =
                    ContextUtils.FindElement(parentContext, "Re-Spin Golden Win", ContextSearchingType.FullNameSearch);

                BlackboardUtils.GetOrCreateVariable<ContextElement>(symbolBB, "_winAmountContextText").value =
                    ContextUtils.FindElement(parentContext, "Re-Spin Golden Win/Amount Base", ContextSearchingType.FullNameSearch);

                SetContextText1(GetVariable<ContextElement>("_winAmountContextText", symbolBB),
                    "VALUE_CREDIT_TEXT", StringTable.StringTableType.Content, "applyCoinValue", symbolBB);

                BlackboardUtils.GetOrCreateVariable<ContextElement>(symbolBB, "_winMultipliedAmountContextText").value =
                    ContextUtils.FindElement(parentContext, "Re-Spin Golden Win/Amount Multiplied", ContextSearchingType.FullNameSearch);

                BlackboardUtils.GetOrCreateVariable<ContextElement>(symbolBB, "_winMultiplierContextText").value =
                    ContextUtils.FindElement(parentContext, "Re-Spin Golden Win/Text - Multiplier", ContextSearchingType.FullNameSearch);

                if (GetVariable<double>("coinMultiplier", symbolBB) > 1)
                {
                    GetContextAnimator(symbolBB, ContextAnimatorName).SetInteger("startFlowType", 0);
                }
            }

            if (GetVariable<double>("coinMultiplier", symbolBB) > 1)
            {
                GetContextAnimator(symbolBB, ContextAnimatorName).SetBool(IsMultiplierForJpAnimationStateHash, true);

                SetContextText1(GetVariable<ContextElement>("_winMultiplierContextText", symbolBB),
                    "MULTIPLIER_CARD_TEXT", StringTable.StringTableType.Content, "coinMultiplier", symbolBB);

                SetVariable("applyCoinValue",
                    GetVariable<double>("applyCoinValue", symbolBB) * GetVariable<double>("coinMultiplier", symbolBB),
                    symbolBB);

                SetContextText1(GetVariable<ContextElement>("_winMultipliedAmountContextText", symbolBB),
                    "VALUE_CREDIT_TEXT", StringTable.StringTableType.Content, "applyCoinValue", symbolBB);
            }
            else
            {
                GetContextAnimator(symbolBB, ContextAnimatorName).SetBool(IsMultiplierForJpAnimationStateHash, false);

                if (GetVariable<bool>("isJackpot", symbolBB))
                {
                    SetVariable("applyCoinValue",
                        GetVariable<double>("applyCoinValue", symbolBB) * GetVariable<double>("coinMultiplier", symbolBB),
                        symbolBB, true);

                    SetContextText1(GetVariable<ContextElement>("_winMultipliedAmountContextText", symbolBB),
                        "VALUE_CREDIT_TEXT", StringTable.StringTableType.Content, "applyCoinValue", symbolBB);
                }
                else
                {
                    GetContextAnimator(symbolBB, ContextAnimatorName).SetInteger(StartFlowTypeAnimationStateHash, 1);
                }
            }

            GetContextAnimator(symbolBB, ContextAnimatorName).SetTrigger(WinAnimationStateHash);

            SetVariable("shouldFly", true, symbolBB, true);
            BlackboardUtils.GetOrCreateVariable<ContextElement>(symbolBB, "_flyFXContextCompositor").value =
                ContextUtils.FindElement(GetVariable<ContextElement>(ContextAnimatorName, symbolBB), "Fly FX anchor", ContextSearchingType.ChildrenSearch);

            var flyController = GetVariable<ContextElement>("_flyFXContextCompositor", symbolBB)
                .GetComponent<DirectionalWeightPositionController>();

            flyController.from = symbolBB.GetVariable<Transform>("symbolTransform").value;

            var flySymbolBB = flyController.GetComponent<Blackboard>();

            SetVariable("_flySymbolBB", flySymbolBB, symbolBB);

            flySymbolBB.SetValue("applyCoinValue", GetVariable<double>("applyCoinValue", symbolBB));

            new SetOtherBlackboardVariable
            {
                targetVariableName = "_tooltipCoinRectTransform",
                newValue = new BBObjectParameter { name = "Animator_BB/tooltipCoinRectTransform" },
            }.ExecuteAction(symbolBB, symbolBB);

            flyController.to = symbolBB.GetVariable<RectTransform>("_tooltipCoinRectTransform").value;

            SetVariable("WinAnimator", ((ContextAnimator) GetVariable<ContextElement>(ContextAnimatorName, symbolBB)).animator, symbolBB);

            var winAnimatorBB = GetVariable<ContextElement>(ContextAnimatorName, symbolBB).GetComponent<Blackboard>();

            ClearSymbolGraphics();
            SetVariable("applyCoinValue", 0d, symbolBB, true);

            if (GetVariable<bool>("./customData/canFlySymbol", symbolBB))
            {
                if (GetVariable<bool>("shouldFly", symbolBB))
                {
                    OnFly();
                }
            }
        }

        private IEnumerator Fly(Blackboard symbolBB, Blackboard winAnimatorBB)
        {
            yield return new WaitUntil(() =>
                GetVariable<bool>("./customData/canFlySymbol", symbolBB));

            SetVariable("./customData/goldSymbolWinCollectCount",
                GetVariable<int>("./customData/goldSymbolWinCollectCount", symbolBB) + 1, symbolBB);

            SetVariable("_goldSymbolCollectCount", BlackboardUtils.GetOrCreateVariable<int>(symbolBB, "./customData/goldSymbolWinCollectCount").value, symbolBB);

            var goldSymbolCollectCount = GetVariable<int>("_goldSymbolCollectCount", symbolBB);
            winAnimatorBB.SetValue("_goldSymbolCollectCount", goldSymbolCollectCount);

            GetContextAnimator(symbolBB, ContextAnimatorName).SetTrigger(FlyAnimationStateHash);
            SetVariable("shouldFly", false, symbolBB, true);
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnWin()
        {
        }

        public void OnResetSymbol()
        {
            ClearSymbolGraphics();
            SetVariable<double>("applyCoinValue", 0, symbolBB, true);

            if (GetVariable<bool>("./customData/canFlySymbol", symbolBB))
            {
                SetVariable("./customData/goldSymbolWinCollectCount",
                    GetVariable<int>("./customData/goldSymbolWinCollectCount", symbolBB) + 1, symbolBB);

                GetBlackboardValue<int>("./customData/goldSymbolWinCollectCount",
                    GetVariable<int>("_goldSymbolCollectCount", symbolBB), symbolBB);

                SetVariable("_goldSymbolCollectCount", GetVariable<int>("_goldSymbolCollectCount", symbolBB),
                    GetVariable<ContextElement>("_WinAnimatorContext", symbolBB).GetComponent<Blackboard>());

                GetVariable<Animator>("_WinAnimatorContext", symbolBB).SetTrigger(FlyAnimationStateHash);
                SetVariable("shouldFly", false, symbolBB, true);
            }
        }
        public void ResetSymbol()
        {
            ClearSymbolGraphics();
            SetVariable<double>("applyCoinValue", 0, symbolBB, true);

            if (GetVariable<bool>("./customData/canFlySymbol", symbolBB))
            {
                SetVariable("./customData/goldSymbolWinCollectCount",
                    GetVariable<int>("./customData/goldSymbolWinCollectCount", symbolBB) + 1, symbolBB);

                GetBlackboardValue<int>("./customData/goldSymbolWinCollectCount",
                    GetVariable<int>("_goldSymbolCollectCount", symbolBB), symbolBB);

                SetVariable("_goldSymbolCollectCount", GetVariable<int>("_goldSymbolCollectCount", symbolBB),
                    GetVariable<ContextElement>("_WinAnimatorContext", symbolBB).GetComponent<Blackboard>());

                GetVariable<Animator>("_WinAnimatorContext", symbolBB).SetTrigger(FlyAnimationStateHash);
                SetVariable("shouldFly", false, symbolBB, true);
            }
        }

        public void DropResult()
        {
            SymbolSetSortingOrder(GetBaseImage(symbolBB), BaseLayerHash, 0, symbolBB);
            animator.SetInteger(TitleAnimationStateHash, 0);
            SymbolSetActiveCachingObjectExecute(0, SymbolSetActiveCachingObject.SetActiveMode.Deactivate, symbolBB);
        }

        private void ClearSymbolGraphics()
        {
            animator.SetInteger(EggAnimationStateHash, -1);
            animator.SetInteger(TitleAnimationStateHash, 0);
            SymbolSetSprite(GetBaseImage(symbolBB), 1, symbolBB);
        }

        public void OnCheckEligibleJPIndex()
        {
            if (GetVariable<int>("jpIndex", symbolBB) > GetVariable<int>("Animator_BB/globalMaxJPIndex", symbolBB))
            {
                SymbolSetSortingOrder(GetVariable<SpriteRenderer>("eggImage", symbolBB), BaseLayerHash, 0, symbolBB);
                animator.SetInteger(TitleAnimationStateHash, 2);
                animator.SetInteger(JPAnimationStateHash, GetVariable<int>("Animator_BB/globalMaxJPIndex", symbolBB));
                animator.SetInteger(EggAnimationStateHash, GetVariable<int>("Animator_BB/globalMaxJPIndex", symbolBB));

                GetBlackboardValueAtList<int>(ContextAnimatorName,
                    GetVariable<int>("Animator_BB/globalMaxJPIndex", symbolBB),
                    GetVariable<int>("_jpMult", symbolBB), symbolBB);
            }
        }

        public void RetryFly()
        {
            if (GetVariable<bool>("shouldFly", symbolBB))
            {
                GetContextAnimator(symbolBB, ContextAnimatorName).Play("Fly");
                OnFly();
            }
        }

        private void OnFly()
        {
            SetVariable("_finishedFly",true, symbolBB);
            SetVariable("./customData/goldSymbolWinCollectCount",
                GetVariable<int>("./customData/goldSymbolWinCollectCount", symbolBB) + 1, symbolBB);
            SetVariable("_goldSymbolCollectCount", BlackboardUtils.GetOrCreateVariable<int>(symbolBB,
                "./customData/goldSymbolWinCollectCount").value, symbolBB);
            var goldSymbolCollectCount = GetVariable<int>("_goldSymbolCollectCount", symbolBB);
            GetVariable<ContextElement>(ContextAnimatorName, symbolBB).GetComponent<Blackboard>().SetValue("_goldSymbolCollectCount", goldSymbolCollectCount);

            GetContextAnimator(symbolBB, ContextAnimatorName).SetTrigger(FlyAnimationStateHash);
            SetVariable("shouldFly", false, symbolBB, true);
        }

        private bool OnCheck(BBParameter<string> key)
        {
            var symbolBB = symbol.GetComponent<SVDSymbolEventHandler>().symbol;
            return (symbolBB.symbolInfo.customData == null) ? false : symbolBB.symbolInfo.customData.ContainsKey(key.value);
        }

        private void SymbolSetSprite(SpriteRenderer spriteRenderer, int spriteId, Blackboard agent)
        {
            new SymbolSetSprite
            {
                spriteRenderer = spriteRenderer,
                spriteId = spriteId
            }.ExecuteAction(agent, symbolBB);
        }

        private void SymbolSetSortingOrder(SpriteRenderer spriteRenderer, int sortingLayerId, int sortingOrder,
            Blackboard agent)
        {
            new SymbolSetSortingOrder
            {
                spriteRenderer = spriteRenderer,
                sortingLayerId = sortingLayerId,
                sortingOrder = sortingOrder
            }.ExecuteAction(agent, symbolBB);
        }

        private void SymbolSetActiveCachingObjectExecute(int cachingId, SymbolSetActiveCachingObject.SetActiveMode mode,
            Blackboard agent)
        {
            new SymbolSetActiveCachingObject
            {
                cachingId = cachingId,
                setTo = mode
            }.ExecuteAction(agent, symbolBB);
        }

        private void GetBlackboardValueAtList<T>(string valueA, int index, BBParameter<T> saveAs, Blackboard agent,
            bool fromLast = false)
        {
            new GetBlackboardValueAtList<T>
            {
                valueA = valueA,
                index = index,
                fromLast = fromLast,
                saveAs = saveAs
            }.ExecuteAction(agent, symbolBB);
        }

        private void GetBlackboardValue<T>(string valueA, BBParameter<T> saveAs, Blackboard agent)
        {
            new GetBlackboardValue<T>
            {
                valueA = valueA,
                saveAs = saveAs
            }.ExecuteAction(agent, symbolBB);
        }

        private void SetContextText1(BBParameter<ContextElement> element, string key,
            StringTable.StringTableType tableType, string arg1, Blackboard agent)
        {
            new SetContextText1
            {
                element = element,
                key = key,
                tableType = tableType,
                arg1 = arg1
            }.ExecuteAction(agent, symbolBB);
        }

        private void SymbolGetGameObject(int prefabId, int cachingId, Blackboard agent)
        {
            new SymbolGetGameObject
            {
                prefabId = prefabId,
                cachingId = cachingId
            }.ExecuteAction(agent, symbolBB);
        }

        private void UpdateContext(bool isForceUpdate, Blackboard agent)
        {
            new UpdateContext
            {
                isForceUpdate = isForceUpdate
            }.ExecuteAction(agent, symbolBB);
        }

        private void GetContextElement(string elementName, ContextSearchingType searchingType, ContextElement saveAs,
            Component agent)
        {
            new GetContextElement
            {
                elementName = elementName,
                searchingType = searchingType,
                saveAs = saveAs
            }.ExecuteAction(agent, symbolBB);
        }

        private T GetVariable<T>(string name, IBlackboard agent)
        {
            return BlackboardUtils.GetOrCreateVariable<T>(agent, name).value;
        }

        private void SetVariable<T>(string name, T value, Blackboard agent, bool doNotSearch = false)
        {
            BlackboardUtils.GetOrCreateVariable<T>(agent, name, doNotSearch).value = value;
        }

        private Blackboard GetAnimatorBB()
        {
            return GlobalBlackboard.Find(GlobalAnimatorBBName);
        }

        private Animator GetContextAnimator(Blackboard symbolBB, string animatorName)
        {
            return ((ContextAnimator) GetVariable<ContextElement>(animatorName, symbolBB)).animator;
        }

        private Transform GetParentObject(Blackboard symbolBB)
        {
            return symbolBB.transform;
        }
    }
}
