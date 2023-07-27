using UnityEngine;

namespace BagelCode
{
    public class MetaObjectController : MonoBehaviour
    {
        private MetaObjectPooler pooler;
        public bool Actived
        {
            get => actived;
            set
            {
                actived = value;
                if (actived) Init();
                else OnDeactived();
            }
        }
        private bool actived;

        public bool IsPrefab { get; private set; }

        public void ReleaseInstance()
        {
            if (!Actived) return;

            pooler?.ReleaseInstance(gameObject);
        }

        public void InitPrefab(MetaObjectPooler _pooler)
        {
            pooler = _pooler;
            IsPrefab = true;
        }

        public void InitPooledObject(MetaObjectPooler _pooler, bool _actived)
        {
            pooler = _pooler;
            Actived = _actived;
            IsPrefab = false;
        }

        public void Deactivate()
        {
            Actived = false;
        }
        
        //

        public virtual void Init()
        {

        }

        public virtual void OnDeactived()
        {

        }

        protected virtual void OnDestroy()
        {
            pooler?.OnDestroyInstance(gameObject);
        }
    }
}