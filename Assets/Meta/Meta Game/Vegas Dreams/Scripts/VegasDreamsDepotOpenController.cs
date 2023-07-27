using UnityEngine;
using SlotMaker;
using System.Collections.Generic;
using System;
using BagelCode.ClientModels;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsDepotOpenController : EventMonoBehaviour
    {
        private ContextElement root;
        private Animator anim;
        private Blackboard bb;

        private bool isInit = false;
        private bool isLevelUp = false;

        private float timer = 0;

        private ContextElement buildingAreaElement;
        private ContextElement gaugeAreaElement;

        private GameObject buildingObject;
        private GameObject gaugeObject;

        private long totalEarnCredit = 0;
        private long prevTotalEarnCredit = 0;
        
        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private List<Animator> depots = new List<Animator>();

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            InitContents();

            if (bb.GetValue<bool>("fromWildPuzzle"))
            {
                anim.SetTrigger("skipDepot");
            }
            else
            {
                GSManager.Instance.GetHandler(VegasDreams.Defines.OPEN_DEPOT).Play();
                MakeDepotIcon();
            }
            StartCoroutine(ObjectAppear());

            isInit = true;
        }

        private void InitContents()
        {
            // Building Anchor
            buildingAreaElement = ContextUtils.FindElement(root, "Building Area", CHILDREN);
            gaugeAreaElement = ContextUtils.FindElement(root, "Gauge Area", CHILDREN);

            // Button Skip
            var skipButtonElement = ContextUtils.FindElement(root, "Button Skip", CHILDREN);
            MetaContextElementUtils.SetClickable(skipButtonElement, () =>
            {
                EventSender.SendEvent(gameObject, VegasDreams.Events.ON_SKIP);
            });

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Total Reward", "COMMA_STYLE_COIN", CHILDREN, 0);
        }

        private void MakeDepotIcon()
        {
            var depotAreaElement = ContextUtils.FindElement(root, "Depot Anchor", CHILDREN);
            var depotObjs = new List<GameObject>();

            foreach (DepotType type in Enum.GetValues(typeof(DepotType)))
            {
                if (type == DepotType.UNKNOWN) continue;

                var count = VegasDreams.Utils.UsedDepot.GetValue<int>(type.ToString().ToLower());

                if (count > 0)
                {
                    var obj = MetaObjectUtils.MakePrefab(VegasDreams.Defines.CONTENTS_BUNDLE, $"Popup Depot Open Depot {type.ToString()}", depotAreaElement.transform, null, type.ToString());
                    MakeDepotIconBadge(obj, count);
                    depotObjs.Add(obj);
                }
            }

            if (depotObjs.Count > 0)
            {
                foreach (var depotObj in depotObjs)
                {
                    depotObj.transform.localScale = Vector3.one * 0.75f;
                }
            }
        }

        public void UpdateDepotOpenResult()
        {
            foreach (var depotOpenResult in VegasDreams.Utils.DepotOpenResultList)
            {
                var updatedBuilding = depotOpenResult.GetValue<Blackboard>("updatedBuilding");
                BlackboardQueryUtils.UpdateBuildingList(updatedBuilding);
                BlackboardQueryUtils.UpdateGurusBuilding(updatedBuilding);
            }

            BlackboardQueryUtils.UpdateGurusBuildingPreset(VegasDreams.Utils.NewGurusBuildingPreset);
            BlackboardQueryUtils.UpdateExhibitionSeasonList();
        }

        private void MakeDepotIconBadge(GameObject obj, int count)
        {
            var objRoot = obj.GetComponent<ContextElement>();
            objRoot.UpdateContext();

            var badgeAreaElement = ContextUtils.FindElement(objRoot, "Badge Area", CHILDREN);

            // Make Badge            
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Badge Big";
            Transform parent = badgeAreaElement.transform;

            var badgeObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            var badgeElement = badgeObj.GetComponent<ContextElement>();

            // Count Sync
            badgeAreaElement.UpdateContext();

            MetaContextElementUtils.SimpleSetIntProperty(badgeAreaElement, "Badge Big", count);
            MetaContextElementUtils.SimpleSetText(badgeAreaElement, "Badge Big/Text", count.ToString(), FULL);
        }

        private void ResetBaseAnimation()
        {
            anim.SetBool("isChange", false);
        }

        public IEnumerator ObjectAppear()
        {
            var resultList = VegasDreams.Utils.DepotOpenResultList;

            if (resultList.Count == 1 && BlackboardUtils.FindVariable<int>(resultList[0], "updatedBuilding/buildingIndex").value == VegasDreams.Utils.GurusBuildingIndex) // gurus open
            {
                MakeGurusObject(resultList[0]);
                yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
                StartCoroutine(ObjectExpTrigger());
                yield return StartCoroutine(GurusSequenceAppear(resultList[0]));
                yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(2).normalizedTime > 0.99f && anim.GetCurrentAnimatorStateInfo(2).IsName("Disappear"));
            }
            else
            {
                foreach (var result in resultList)
                {
                    ResetBaseAnimation();
                    MakeObject(result);
                    yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
                    StartCoroutine(ObjectExpTrigger());
                    yield return StartCoroutine(ObjectSequenceAppear(result));
                    yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(2).normalizedTime > 0.99f && anim.GetCurrentAnimatorStateInfo(2).IsName("Disappear"));
                    Destroy(buildingObject);
                    Destroy(gaugeObject);
                }
            }
            

            EventSender.SendEvent(gameObject, VegasDreams.Events.ON_SKIP);
        }

        private IEnumerator ObjectSequenceAppear(Blackboard result)
        {
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(2).IsName("Idle"));

            anim.SetTrigger("isGetExp");

            var building = result.GetValue<Blackboard>("updatedBuilding");
            var index = building.GetValue<int>("buildingIndex");
            var targetLevel = building.GetValue<int>("level");
            var targetExp = building.GetValue<long>("exp");
            var earnedCredit = result.GetValue<long>("earnedCredit");

            var level = VegasDreams.Utils.GetBuildingLevel(index - 1);
            var exp = VegasDreams.Utils.GetBuildingExp(index - 1);
            var requireExp = VegasDreams.Utils.GetBuildingPresetData(index - 1, targetLevel + 1, "exp");
            
            var gaugeElement = gaugeObject.GetComponent<ContextElement>();
            var gaugeAnim = gaugeObject.GetComponent<Animator>();

            var buildingElment = buildingObject.GetComponent<ContextElement>();

            totalEarnCredit += earnedCredit;

            float timer = 0f;

            isLevelUp = targetLevel != level;

            if (isLevelUp) // level up
            {
                for (int target = level + 1; target <= targetLevel; target++)
                {
                    var eachLevelRequreExp = VegasDreams.Utils.GetBuildingPresetData(index - 1, target, "exp");
                    gaugeAnim.SetTrigger("isActive");
                    MetaContextElementUtils.SimpleSetActive(gaugeElement, "Progress Bar/Handle", true, FULL);
                    GSManager.Instance.GetHandler(VegasDreams.Defines.EXP_UP).Play();
                    while (timer < 1f)
                    {
                        timer += Time.deltaTime;
                        MetaContextElementUtils.SimpleSetFloatProperty(gaugeElement, "Progress Bar", Mathf.Lerp(0, 1, timer), CHILDREN);
                        MetaContextElementUtils.SimpleSetTextGlobal(gaugeElement, "Progress Bar/Text Progress Bar", "A_PER_B", FULL, (long)Mathf.Lerp(0, eachLevelRequreExp, timer), eachLevelRequreExp);
                        yield return null;
                    }

                    // gauge star animation
                    var star = ContextUtils.FindElement(gaugeElement, $"Star {target}", CHILDREN);
                    star.GetComponent<Animator>().SetTrigger("isGet");

                    GSManager.Instance.GetHandler(VegasDreams.Defines.HAMMER_EFFECT).Play();
                    var obj = ContextUtils.FindElement(buildingElment, $"Building {index} Object {VegasDreams.Utils.GetBuildingObjectIndex(index - 1)[target - 1]}", CHILDREN);
                    obj.GetComponent<Animator>().SetTrigger("isChange");

                    GSManager.Instance.GetHandler(VegasDreams.Defines.FULL_EXP).Play();
                    MetaContextElementUtils.SimpleSetActive(gaugeElement, "Progress Bar/Handle", false, FULL);
                    timer = 0f;
                    gaugeAnim.SetTrigger("isComplete");

                    yield return new WaitForSeconds(1.5f);
                }
            }

            exp = isLevelUp ? 0 : exp;

            gaugeAnim.SetTrigger("isActive");
            MetaContextElementUtils.SimpleSetActive(gaugeElement, "Progress Bar/Handle", targetExp != 0, FULL);

            if (requireExp == -1L)
            {
                MetaContextElementUtils.SimpleSetFloatProperty(gaugeElement, "Progress Bar", 1f, CHILDREN);
                MetaContextElementUtils.SimpleSetText(gaugeElement, "Progress Bar/Text Progress Bar", "MAX");
            }
            else
            {
                GSManager.Instance.GetHandler(VegasDreams.Defines.EXP_UP).Play();
                while (timer < 1f)
                {
                    timer += Time.deltaTime;
                    MetaContextElementUtils.SimpleSetFloatProperty(gaugeElement, "Progress Bar", Mathf.Lerp(0, (float)targetExp / requireExp, timer), CHILDREN);
                    MetaContextElementUtils.SimpleSetTextGlobal(gaugeElement, "Progress Bar/Text Progress Bar", "A_PER_B", FULL, (long)Mathf.Lerp(exp, targetExp, timer), requireExp);
                    yield return null;
                }
            }
            yield return new WaitForSeconds(0.2f); // a little bit delay

            if (isLevelUp)
            {
                // gauge reward
                MetaContextElementUtils.SimpleSetTextGlobal(gaugeElement, "Reward Base/Text Level Up Reward", "COMMA_STYLE_COIN", FULL, earnedCredit);
                gaugeAnim.SetBool("isReward", earnedCredit > 0);
                gaugeAnim.SetTrigger("isRewardCollect");
                anim.SetBool("isTotalReward", true);
                anim.SetTrigger("isLevelUp");

                GSManager.Instance.GetHandler(VegasDreams.Defines.LEVEL_UP).Play();
                yield return new WaitForSeconds(0.1f);
                GSManager.Instance.GetHandler(VegasDreams.Defines.GET_STAR).Play();

                var callbackTrigger = new EventTrigger(gameObject, VegasDreams.Events.ON_COLLISION_TOTAL_REWARD);
                yield return new WaitUntilTrigger(callbackTrigger);
                GSManager.Instance.GetHandler(VegasDreams.Defines.GET_TOTAL_REWARD).Play();

                anim.SetTrigger("TotalRewardCollect");
                
                timer = 0f;

                while (timer < 1f)
                {
                    timer += Time.deltaTime;
                    MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Total Reward", "COMMA_STYLE_COIN", CHILDREN, (long)Mathf.Lerp(prevTotalEarnCredit, totalEarnCredit, timer));
                    yield return null;
                }
                prevTotalEarnCredit = totalEarnCredit;

                yield return new WaitForSeconds(1f);
            }

            anim.SetBool("isChange", true);
            anim.SetBool("isTotalReward", false);
        }

        private IEnumerator GurusSequenceAppear(Blackboard result)
        {
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(2).IsName("Idle"));

            anim.SetTrigger("isGetExp");

            var building = result.GetValue<Blackboard>("updatedBuilding");
            var targetLevel = building.GetValue<int>("level");
            var targetExp = building.GetValue<long>("exp");
            var earnedCredit = result.GetValue<long>("earnedCredit");

            var gurusBuilding = VegasDreams.Utils.GurusBuilding;

            var level = gurusBuilding.GetValue<int>("level");
            var exp = gurusBuilding.GetValue<long>("exp");
            var requireExp = VegasDreams.Utils.GurusBuildingPreset.GetValue<long>("exp");
            
            var gaugeElement = gaugeObject.GetComponent<ContextElement>();
            var gaugeAnim = gaugeObject.GetComponent<Animator>();

            var buildingElment = buildingObject.GetComponent<ContextElement>();

            totalEarnCredit += earnedCredit;

            float timer = 0f;

            isLevelUp = targetLevel != level;

            if (isLevelUp)
            {
                gaugeAnim.SetTrigger("isActive");
                MetaContextElementUtils.SimpleSetActive(gaugeElement, "Progress Bar/Handle", true, FULL);

                GSManager.Instance.GetHandler(VegasDreams.Defines.EXP_UP).Play();
                while (timer < 1f)
                {
                    timer += Time.deltaTime;
                    MetaContextElementUtils.SimpleSetFloatProperty(gaugeElement, "Progress Bar", Mathf.Lerp(0, 1, timer), CHILDREN);
                    MetaContextElementUtils.SimpleSetTextGlobal(gaugeElement, "Progress Bar/Text Progress Bar", "A_PER_B", FULL, (long)Mathf.Lerp(0, requireExp, timer), requireExp);
                    yield return null;
                }

                GSManager.Instance.GetHandler(VegasDreams.Defines.HAMMER_EFFECT).Play();
                buildingElment.GetComponent<Animator>().SetTrigger("IsChange");

                GSManager.Instance.GetHandler(VegasDreams.Defines.FULL_EXP).Play();
                MetaContextElementUtils.SimpleSetActive(gaugeElement, "Progress Bar/Handle", false, FULL);
                timer = 0f;
                gaugeAnim.SetTrigger("isComplete");

                //Text Point

                while (timer < 1.5f)
                {
                    timer += Time.deltaTime;
                    MetaContextElementUtils.SimpleSetText(gaugeElement, "Text Point", ((int)Mathf.Lerp(level, targetLevel, timer / 1.5f)).ToString(), FULL);
                    yield return null;
                }
            }

            timer = 0f;

            exp = isLevelUp ? 0 : exp;
            requireExp = VegasDreams.Utils.NewGurusBuildingPreset.GetValue<long>("exp");

            gaugeAnim.SetTrigger("isActive");
            MetaContextElementUtils.SimpleSetActive(gaugeElement, "Progress Bar/Handle", targetExp != 0, FULL);
            MetaContextElementUtils.SimpleSetText(gaugeElement, "Text Point", targetLevel.ToString(), FULL);

            GSManager.Instance.GetHandler(VegasDreams.Defines.EXP_UP).Play();
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                MetaContextElementUtils.SimpleSetFloatProperty(gaugeElement, "Progress Bar", Mathf.Lerp(0, (float)targetExp / requireExp, timer), CHILDREN);
                MetaContextElementUtils.SimpleSetTextGlobal(gaugeElement, "Progress Bar/Text Progress Bar", "A_PER_B", FULL, (long)Mathf.Lerp(exp, targetExp, timer), requireExp);
                yield return null;
            }

            yield return new WaitForSeconds(0.2f); // a little bit delay

            if (isLevelUp)
            {
                // gauge reward
                MetaContextElementUtils.SimpleSetTextGlobal(gaugeElement, "Reward Base/Text Level Up Reward", "COMMA_STYLE_COIN", FULL, earnedCredit);
                gaugeAnim.SetBool("isReward", earnedCredit > 0);
                gaugeAnim.SetTrigger("isRewardCollect");
                anim.SetTrigger("isLevelUp");
                GSManager.Instance.GetHandler(VegasDreams.Defines.FULL_EXP).Play();
                GSManager.Instance.GetHandler(VegasDreams.Defines.LEVEL_UP).Play();
            }

            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
            anim.SetBool("isChange", true);
        }

        private IEnumerator ObjectExpTrigger()
        {
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
        }

        private void MakeObject(Blackboard result)
        {
            var building = result.GetValue<Blackboard>("updatedBuilding");
            var earnedExp = result.GetValue<long>("earnedExp");
            var index = building.GetValue<int>("buildingIndex");

            //Text Building Name
            //Text Building Exp
            MetaContextElementUtils.SimpleSetText(root, "Text Building Name", VegasDreams.Utils.GetBuildingName(index - 1));
            MetaContextElementUtils.SimpleSetText(root, "Text Building Exp", $"+{earnedExp}");

            buildingObject = MetaObjectUtils.MakePrefab(
                VegasDreams.Utils.GetObjectSeasonBundle(),
                $"Theme {VegasDreams.Utils.SeasonThemeId} Building {index}",
                buildingAreaElement.transform
            );
            buildingObject.GetComponent<Blackboard>().SetValue("index", index - 1);

            gaugeObject = MetaObjectUtils.MakePrefab(
                VegasDreams.Defines.CONTENTS_BUNDLE,
                $"Gauge Rarity {VegasDreams.Utils.GetBuildingRank(index - 1)}",
                gaugeAreaElement.transform
            );
            var gaugeBB = gaugeObject.GetComponent<Blackboard>();
            gaugeBB.SetValue("index", index - 1);
            gaugeBB.SetValue("initZero", true);
        }

        private void MakeGurusObject(Blackboard result)
        {
            var building = result.GetValue<Blackboard>("updatedBuilding");
            var earnedExp = result.GetValue<long>("earnedExp");

            //Text Building Name
            //Text Building Exp
            MetaContextElementUtils.SimpleSetText(root, "Text Building Name", "");
            MetaContextElementUtils.SimpleSetText(root, "Text Building Exp", $"+{earnedExp}");

            buildingObject = MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(), 
                $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Place",
                buildingAreaElement.transform
            );

            gaugeObject = MetaObjectUtils.MakePrefab(
                VegasDreams.Defines.CONTENTS_BUNDLE,
                "Gauge Rarity Gurus",
                gaugeAreaElement.transform
            );
            var gaugeBB = gaugeObject.GetComponent<Blackboard>();
            gaugeBB.SetValue("initZero", true);
        }

        public void Close(bool instantly)
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
            if (instantly)
            {
                Destroy(gameObject);
            }
            else
            {
                anim.SetTrigger("Close");
            }
        }

        public IEnumerator Make()
        {
            yield return null;
        }
    }
}
