using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher
{
    public class CollectingGameChestOpenController : MonoBehaviour
    {
        private ContextElement agent;
        private ContextElement shadowPoolElement;
        private ContextElement shadowAnchorElement;
        private ObjectPool shadowPool;
        private Animator animator;

        public void OnInit()
        {
            agent = GetComponent<ContextElement>();
            
            shadowPoolElement = ContextUtils.FindElement(agent, "Anchor/Base Add Shadow Pool", ContextSearchingType.FullNameSearch);
            shadowPool = shadowPoolElement.GetComponent<ObjectPool>();
            
            shadowAnchorElement = ContextUtils.FindElement(agent, "Anchor/Base Add Shadow Anchor", ContextSearchingType.FullNameSearch);

            animator = GetComponent<Animator>();
        }
        
        public void OnAddShadow(int chestId)
        {
            PooledObject pooledObject = shadowPool.GetObject();

            Sprite chestImage = CollectingGameChestData.Instance.chestAssets.assets[BlackboardQueryUtils.GetChestIndex(chestId)];
            ContextElement imageElement = pooledObject.gameObject.GetComponentInChildren<ContextElement>();
            MetaContextElementUtils.SetSprite(imageElement, chestImage);
            
            pooledObject.transform.SetParent(shadowAnchorElement.transform);

            StartCoroutine("TriggerClose");
        }

        IEnumerator TriggerClose()
        {
            yield return new WaitForSeconds(0.13f);
            animator.SetTrigger("Add");
        }
    }
}