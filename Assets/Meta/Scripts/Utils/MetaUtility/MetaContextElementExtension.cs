using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using StringTableType = SlotMaker.StringTable.StringTableType;

namespace BagelCode {
    public static class MetaContextElementExtension  {
        public static void ApplyWebImage(this ContextImage contextImage,string imageUrl,System.Action OnFailAct=null) {
            WebImageDownloader.Instance.LoadWebImage(imageUrl, CacheType.MemCache, false, null,
                    //On Load
                    (spr) => {
                        contextImage.SetSprite(spr);
                    },
                    null,
                    //On Fail
                    (error) => {
                        OnFailAct?.Invoke();
                    });
        }
        public static void SetGlobalText(this IContextText contextText, string key, params object[] args) {
            string text = StringTableUtils.GetString(StringTableType.Global, key, args);
            contextText.SetText(text);
        }

        public static T FindElement<T>(this ContextElement context, string name) where T : Component {
            var targetContext = FindElement(context, name);
            var targetComp = targetContext.GetComponent<T>();
            if (targetComp == null) {
                Debug.LogError($"Context Component Not Find. ({name} : {typeof(T).Name})");
            }
            return targetComp;
        }
        public static ContextElement FindElement(this ContextElement context, string name) {
            ContextElement targetContext = null;
            if (name.Contains("/")) {
                targetContext = ContextUtils.FindElement(context, name, ContextSearchingType.FullNameSearch);
            } else {
                targetContext = context.Find(name);
            }
            if (targetContext == null) {
                Debug.LogError($"Context Not Find. ({name})");
            }
            return targetContext;
        }
    }
}
