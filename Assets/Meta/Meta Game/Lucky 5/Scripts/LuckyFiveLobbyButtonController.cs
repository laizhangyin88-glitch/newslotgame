using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class LuckyFiveLobbyButtonController : MonoBehaviour
    {
        private ContextElement root;

        private GameObject badgeObj = null;
        private Animator badgeAnim;

        private bool isInit = false;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();

            root.UpdateContext();

            UpdateBadge();

            isInit = true;
        }

        private void OnEnable()
        {
            UpdateBadge();
        }

        private void UpdateBadge()
        {
            if (!isInit) return;

            if (BlackboardQueryUtils.IsLuckyFiveClaimable())
            {
                if(badgeObj == null)
                {
                    string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
                    string asset = "Badge";
                    Transform parent = ContextUtils.FindElement(root, "Badge Area", ContextSearchingType.ChildrenSearch).transform;
                    badgeObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);

                    badgeAnim = badgeObj.GetComponent<Animator>();
                    badgeAnim.SetInteger("value", 1);
                }
                else
                {
                    badgeAnim.SetInteger("value", 1);
                }
            }
            else
            {
                if (badgeObj != null)
                {
                    badgeAnim.SetInteger("value", 0);
                }
            }
        }
    }
}
