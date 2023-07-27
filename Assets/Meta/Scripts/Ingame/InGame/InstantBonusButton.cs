using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using System.Collections.Generic;

namespace BagelCode
{
    public class InstantBonusButton : MonoBehaviour
    {
        private Variable<bool> isSale;
        private Variable<int> salePercentage;
        private Variable<long> productEventMultiplier;

        private ContextElement agent;
        private ContextElement eventTagArea;
        private ContextElement eventText;

        private void Awake()
        {
            isSale  = BlackboardUtils.FindVariable<bool>("./bonusIamInfo/isSale");
            salePercentage = BlackboardUtils.FindVariable<int>(ContentBlackboard.Get(), "./bonusIamInfo/salePercentage");

            Blackboard iamInfo = BlackboardUtils.FindValue<Blackboard>("./bonusIamInfo");
            if (iamInfo != null)
            {
                List<Blackboard> componentList = BlackboardUtils.FindVariable<List<Blackboard>>(iamInfo, "componentList").value;

                foreach (var component in componentList)
                {
                    var eventMultiplier = BlackboardUtils.FindVariable<long>(component, "action/product/eventMultiplierNumerator");
                    if (eventMultiplier != null)
                    {
                        productEventMultiplier = eventMultiplier;
                        break;
                    }
                }
            }
        }

        private void Start()
        {
            if (agent != null) return;

            agent = GetComponent<ContextElement>();
            agent.UpdateContext();

            eventTagArea = ContextUtils.FindElement(agent, "Event Tag", ContextSearchingType.ChildrenSearch);
            if(eventTagArea != null)
                eventText = ContextUtils.FindElement(eventTagArea, "Text", ContextSearchingType.ChildrenSearch);
        }

        public void OnInitGame()
        {
            if (eventTagArea != null)
            {
                if (isSale != null)
                {
                    MetaContextElementUtils.SetActive(eventTagArea, isSale.value);

                    if (salePercentage != null)
                        ContextUtils.SetGlobalText(eventText, "TEXT_COMMON_PASSIVE_EVENT_PERCENT_SALE", salePercentage.value);
                }

                //세일 중이 아닐때 1.0x배 초과만 표시
                if ((isSale == null || isSale.value == false) && productEventMultiplier != null && productEventMultiplier.value > 100)
                {
                    MetaContextElementUtils.SetActive(eventTagArea, true);
                    ContextUtils.SetGlobalText(eventText, "SLOT_EVENT_TAG_MULTIPLY", NumberUtils.GetMultiplierFromNumerator(productEventMultiplier.value));
                }

                //둘 중에 하나라도 보인다면 보이도록 설정해준다.  
                if (eventTagArea.gameObject.activeSelf)
                    agent.transform.Find("Anchor/Event Tag Area").gameObject.SetActive(true);
            }
        }
    }
}
