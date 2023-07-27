using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using NodeCanvas.Framework;
using UnityEngine;

namespace BagelCode
{
    public class PopupBundleLoadingController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private ContextElement progressBarElement;

        private GameObject nextSceneObj = null;

        private List<string> bundles = new List<string>();

        private string bundle = null;
        private bool isLoad = true;

        public void Init()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            bundle = bb.GetValue<string>("bundle");
            if (!string.IsNullOrEmpty(bundle)) bundles.Add(bundle);

            isLoad = bb.GetValue<bool>("isLoad");

            // Text
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text",
                isLoad ?
                "COMMON_LOADING_DESCRIPTION_TEXT" :
                "COMMON_UNLOADING_DESCRIPTION_TEXT",
                ContextSearchingType.ChildrenSearch);

            progressBarElement = ContextUtils.FindElement(root, "Progress Bar", ContextSearchingType.ChildrenSearch);

            // Back Button
            MetaSystem.SubscribeBackButton(this.GetHashCode(), () => SendEvent("OnClose"));

            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", () => SendEvent("OnClose"));
        }

        public IEnumerator BundleLoadingCoroutine()
        {
            if (isLoad)
            {
                yield return StartCoroutine(MetaAssetBundleUtils.LoadAssetBundleCoroutine(bundles, true,
                    (progress) => MetaContextElementUtils.SetFloatProperty(progressBarElement, progress),
                    () => SendEvent("OnSuccess"),
                    () => SendEvent("OnFail")));

                BlackboardQueryUtils.AddUsingMetaAssetBundles(bundles);
            }
            else
            {
                for (int i = 0; i < bundles.Count; ++i)
                {
                    AssetBundleManager.UnloadAssetBundle(bundles[i], false);

                    AsyncOperation operation = Resources.UnloadUnusedAssets();

                    yield return new WaitUntil(() => operation.isDone);

                    SendEvent("OnSuccess");
                }
            }
        }

        public void OnClose()
        {
            if(nextSceneObj != null)
            {
                EventSender.SendCalleeCallback(gameObject, nextSceneObj);
            }
            else
            {
                EventSender.SendCalleeCallback(gameObject);
            }
            MetaSystem.UnSubscribeBackButton(this.GetHashCode());
            PopupManager.Instance.Close(gameObject);
            anim.SetTrigger("Close");
        }

        public IEnumerator LoadNextSceneCoroutine()
        {
            var nextScene = bb.GetValue<string>("nextScene");
            if (string.IsNullOrEmpty(nextScene)) yield break;

            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            nextSceneObj = null;
            yield return MetaPopupUtils.OpenPopupCoroutine(bundle, nextScene, parent,
                (GameObject popupObj) => nextSceneObj = popupObj);

            MetaObjectUtils.SetCalleeCaller(nextSceneObj, gameObject);

            var nextSceneBB = nextSceneObj.GetComponent<Blackboard>();
            string fromType = bb.GetValue<string>("fromType");
            if (string.IsNullOrEmpty(fromType)) fromType = "";
            nextSceneBB.AddVariable("enterType", fromType);
            nextSceneBB.AddVariable("isEnter", true);

            bool isOpenInstantly = bb.GetValue<bool>("openNextSceneInstantly");
            if (isOpenInstantly) MetaPopupUtils.OpenPopup(nextSceneObj);
        }

        private void SendEvent(string eventName)
        {
            EventSender.SendEvent(gameObject, eventName);
        }
    }
}
