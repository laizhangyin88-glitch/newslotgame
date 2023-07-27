using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class ProfileEventGroupVIP : ProfileEventGroup
    {
        protected override float DisplayTime => 2f;

        public ProfileEventGroupVIP(ContextElement _root) : base(_root)
        {

        }

        public override bool IsAvailable()
        {
            return BlackboardQueryUtils.IsVipLoungeActiveBenefit();
        }

        protected override void Initialize()
        {
            if (isInit) return;
            isInit = true;

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Event Tag VIP Lounge";
            Transform parent = eventTagAreaElement.transform;

            tagObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            anim = tagObj.GetComponent<Animator>();
            anim.SetBool("Appear", false);
        }
    }
}
