using UnityEngine;
using SlotMaker;
using System.Collections.Generic;
using System;
using BagelCode.ClientModels;
using System.Collections;
using NodeCanvas.Framework;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsFreeDepotController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private bool isInit = false;

        private float timer = 0;
        private float delay = 2f;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private List<Animator> depots = new List<Animator>();

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            GSManager.Instance.GetHandler(VegasDreams.Defines.GET_FREE_DEPOT).Play();

            // Init Elements
            InitContents();
            StartCoroutine(AutoClose());

            isInit = true;
        }

        private void InitContents()
        {
            MetaContextElementUtils.SetClickable(root, 
                () => 
                {
                    timer = VegasDreams.Defines.FREE_DEPOT_AUTO_CLOSE_SECONDS;
                });

            var depotAreaElement = ContextUtils.FindElement(root, "Depot Area", CHILDREN);

            string text = "";

            foreach (DepotType depot in Enum.GetValues(typeof(DepotType)))
            {
                if (depot == DepotType.UNKNOWN) continue;

                var count = VegasDreams.Utils.EarnedDepot.GetValue<int>(depot.ToString().ToLower());

                if (count > 0)
                {
                    depots.Add(MetaObjectUtils.MakePrefab(VegasDreams.Defines.CONTENTS_BUNDLE, $"Vegas Dream Free Depot {depot.ToString()}", depotAreaElement.transform, null, depot.ToString()).GetComponent<Animator>());

                    if (!string.IsNullOrEmpty(text)) text += "<br>";
                    text += StringTableUtils.GetString(StringTable.StringTableType.Global, "VEGAS_DREAMS_FREE_DEPOT_TEXT", count, depot.ToString());
                }
            }

            //Text Depot Info
            MetaContextElementUtils.SimpleSetText(root, "Text Depot Info", text, CHILDREN);
        }

        private IEnumerator AutoClose()
        {
            while (timer < VegasDreams.Defines.FREE_DEPOT_AUTO_CLOSE_SECONDS)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            anim.SetTrigger("Depot Fly");

            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"));

            foreach (var depot in depots)
            {
                depot.SetTrigger("isFly");

                var from = depot.transform;
                var to = bb.GetValue<Transform>(depot.gameObject.name);

                AsyncActionUtils.ApplyMovement(
                    this,
                    depot.transform,
                    from.position,
                    to.position,
                    0.67f,
                    TweenUtils.VectorTweenCollectMove,
                    0f
                );


            }

            yield return new WaitForSeconds(delay);

            EventSender.SendEvent(gameObject, "OnClose");
        }
    }
}
