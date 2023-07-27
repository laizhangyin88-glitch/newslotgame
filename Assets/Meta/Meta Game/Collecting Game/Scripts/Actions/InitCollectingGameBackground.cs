using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class InitCollectingGameBackground : ActionTask<ContextElement>
    {
        public BBParameter<int> chestId;
        public BBParameter<string> sharedBundle;
        public BBParameter<GameObject> effect;
        
        protected override string info
        {
            get { return "Init Collecting Game Background Scene"; }
        }

        protected override void OnExecute()
        {
            ContextElement effectAreaElement = ContextUtils.FindElement(agent, "Effect Area", ContextSearchingType.ChildrenSearch);
            int chestIndex = BlackboardQueryUtils.GetChestIndex(chestId.value);
            GameObject effectExplosion = null;
            switch (chestIndex)
            {
                case 0:
                    effectExplosion = MetaObjectUtils.MakePrefab(sharedBundle.value, "Effect Explosion Bronze", effectAreaElement.transform);
                    break;
                case 1:
                    effectExplosion = MetaObjectUtils.MakePrefab(sharedBundle.value, "Effect Explosion Silver", effectAreaElement.transform);
                    break;
                case 2:
                    effectExplosion = MetaObjectUtils.MakePrefab(sharedBundle.value, "Effect Explosion Gold", effectAreaElement.transform);
                    break;
                case 3:
                    effectExplosion = MetaObjectUtils.MakePrefab(sharedBundle.value, "Effect Explosion Diamond", effectAreaElement.transform);
                    break;
            }
            
            if (effectExplosion != null)
            {
                effectExplosion.SetActive(false);
                
                OverrideRelativeParticleSortingLayer[] overriders = effectExplosion.GetComponentsInChildren<OverrideRelativeParticleSortingLayer>();
                for (int i = 0; i < overriders.Length; i++)
                    overriders[i].UpdateSortingLayer();

                effect.value = effectExplosion;
            }
            
            EndAction(true);
        }
    }
}
