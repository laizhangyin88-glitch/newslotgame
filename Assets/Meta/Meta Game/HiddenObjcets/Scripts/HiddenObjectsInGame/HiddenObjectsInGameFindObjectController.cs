using UnityEngine;
using ParadoxNotion;
using SlotMaker;
using NodeCanvas.Framework;
using System.Collections;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsInGameFindObjectController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private new BoxCollider collider;

#if DEV
        private GameObject cheatEffectObj;
#endif

        private bool isSearched = false;

        private ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private void Start()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            collider = GetComponent<BoxCollider>();

            root.UpdateContext(false);

            // Init Events
            InitRouter();
            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_SEARCH, OnSearch);

#if DEV
            if (PlayerPrefs.GetInt(HiddenObjects.Defines.PLAYER_PREFS_SHOW_HINT) == 1)
            {
                cheatEffectObj = MetaObjectUtils.MakePrefab(HiddenObjects.Defines.CONTENTS_BUNDLE, "Particle Hint", transform);
            }
#endif
        }

        private void OnSearch(EventData eventData)
        {
            if (isSearched) return;
            isSearched = true;

#if DEV
            if (cheatEffectObj != null)
                Destroy(cheatEffectObj);

            if (PlayerPrefs.GetInt(HiddenObjects.Defines.PLAYER_PREFS_SHOW_RECT) == 1)
            {
                var boundaryElement = ContextUtils.FindElement(root, "Boundary Visualizer", CHILDREN);
                if (boundaryElement != null)
                {
                    Destroy(boundaryElement.gameObject);
                }
            }

            if (PlayerPrefs.GetInt(HiddenObjects.Defines.PLAYER_PREFS_SHOW_ALL) == 1)
            {
                var numberElement = ContextUtils.FindElement(root, "Test Object Number", CHILDREN);
                if (numberElement != null)
                {
                    Destroy(numberElement.gameObject);
                }
            }
#endif

            // Collider Disable
            collider.enabled = false;
            GetComponent<NonDrawingGraphic>().raycastTarget = false;

            var originalAreaElement = ContextUtils.FindElement(root, "Original Area", CHILDREN);

            // Make Original Object
            string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
            string asset = "Common Original Object";
            Transform parent = originalAreaElement.transform;

            var originalObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            var originalObjectTransform = originalObj.transform;
            originalObjectTransform.eulerAngles = new Vector3(); // reset rotation

            var originalObjElement = originalObj.GetComponent<ContextElement>();
            originalObjElement.UpdateContext(true);

            // Layer
            int findOrder = bb.GetValue<int>("findOrder");
            var originalObjCanvas = originalObj.GetComponent<Canvas>();
            originalObjCanvas.sortingOrder += findOrder;

            // Set Sprite
            int chapter = bb.GetValue<int>("chapter");
            int stage = bb.GetValue<int>("stage");

            bundle = HiddenObjects.Utils.GetStageBundleName(chapter, stage);
            asset = name + " Original";
            var tex2D = AssetBundleManager.LoadAsset<Texture2D>(bundle, asset);
            if (tex2D != null)
            {
                Rect rec = new Rect(0, 0, tex2D.width, tex2D.height);
                var sprite = Sprite.Create(tex2D, rec, new Vector2(0, 0), 1);
                MetaContextElementUtils.SimpleSetSprite(originalObjElement, "Anchor/Image", sprite, FULL);
                MetaContextElementUtils.SimpleSetSprite(originalObjElement, "Anchor/Image Glow", sprite, FULL);
                //MetaContextElementUtils.SimpleSetSprite(originalObjElement, "Anchor/Image", bundle, asset, FULL);
                //MetaContextElementUtils.SimpleSetSprite(originalObjElement, "Anchor/Image Glow", bundle, asset, FULL);
            }

            // Set Score
            long score = (eventData as EventData<long>).value;
            MetaContextElementUtils.SimpleSetTextGlobal(originalObjElement, "Text Score", "TEXT_COMMA_NUMBER", CHILDREN, score);

            // Hide Image
            var imageElement = ContextUtils.FindElement(root, "Anchor/Image", FULL);
            MetaContextElementUtils.SetActive(imageElement, false);

            // Play Effect
            StartCoroutine(PlayFlyEffectCoroutine(originalObj));
        }

        private IEnumerator PlayFlyEffectCoroutine(GameObject originalObj)
        {
            var originalObjElement = originalObj.GetComponent<ContextElement>();
            Transform targetTransform = bb.GetValue<Transform>("targetTextTransform");
            var anchorElement = ContextUtils.FindElement(originalObjElement, "Anchor", CHILDREN);
            var controller = anchorElement.GetComponent<DirectionalWeightPositionController>();

            controller.enabled = false;

            const float DELAY = 0.5f;
            yield return new WaitForSeconds(DELAY);

            var anim = originalObj.GetComponent<Animator>();
            anim.SetTrigger("isFly");

            controller.enabled = true;

            controller.from = originalObj.transform;
            controller.to = targetTransform;
            bool isLeft = originalObj.transform.position.x < targetTransform.position.x;
            controller.width = isLeft ? 0.9f : -0.9f;

            const float FLYING_TIME = 0.62f;
            yield return new WaitForSeconds(FLYING_TIME);

            GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_OBJECT_DISAPPEAR).Play();
            
            controller.enabled = false;

            anim.SetTrigger("isDisappear");

            var caller = bb.GetValue<GameObject>("caller");
            int objectId = bb.GetValue<int>("objectId");
            EventSender.SendEvent(caller, new EventData<int>(HiddenObjects.Events.ON_FIND_OBJECT_DESTROYED, objectId));
        }
    }
}
