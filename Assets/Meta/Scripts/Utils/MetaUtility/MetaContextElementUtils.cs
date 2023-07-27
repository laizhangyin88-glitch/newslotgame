using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;
using SlotMaker;
using ParadoxNotion;

using Action = System.Action;

namespace BagelCode
{

    public class MetaContextElementUtils
    {
        public static void SetPropertySafty<T>(ContextElement element, T value)
        {
            var behaviour = MainController.Instance;

            if (behaviour == null || element == null) return;

            if (!behaviour.isActiveAndEnabled)
            {
                if (ApplicationSettings.LogTest())
                {
                    Debug.LogWarning("SetPropertySafty failure. behaviour is deactivated.");
                    return;
                }
            }

            behaviour.StartCoroutine(SetPropertyCoroutine(element, value));
        }

        public static IEnumerator SetPropertyCoroutine<T>(ContextElement element, T value)
        {
            yield return new WaitUntil(() => element == null || element.isActiveAndEnabled);

            if (element == null) yield break;

            if (value is int)
            {
                SetIntProperty(element, Convert.ToInt32(value));
            }
            else if(value is float)
            {
                SetFloatProperty(element, Convert.ToSingle(value));
            }
            else if(value is bool)
            {
                SetBooleanProperty(element, Convert.ToBoolean(value));
            }
        }

        public static void SetIntProperty(ContextElement element, int value)
        {
            if (element is IContextIntProperty propertyElement)
                propertyElement.SetIntProperty(value);
        }

        public static int GetIntProperty(ContextElement element, int defaultValue = 0)
        {
            if (element is IContextIntProperty propertyElement)
                return propertyElement.GetIntProperty();

            return defaultValue;
        }

        public static void SetFloatProperty(ContextElement element, float value)
        {
            if (element is IContextFloatProperty propertyElement)
                propertyElement.SetFloatProperty(value);
        }

        public static float GetFloatProperty(ContextElement element, float defaultValue = 0f)
        {
            if (element is IContextFloatProperty propertyElement)
                return propertyElement.GetFloatProperty();

            return defaultValue;
        }

        public static bool ToggleBooleanProperty(ContextElement element, bool defaultValue = false)
        {
            if (element is IContextBooleanProperty propertyElement)
            {
                bool state = !GetBooleanProperty(element, !defaultValue);
                propertyElement.SetBooleanProperty(state);
                return state;
            }
            return defaultValue;
        }

        public static void SetBooleanProperty(ContextElement element, bool value)
        {
            if (element is IContextBooleanProperty propertyElement)
                propertyElement.SetBooleanProperty(value);
        }

        public static bool GetBooleanProperty(ContextElement element, bool defaultValue = false)
        {
            if (element is IContextBooleanProperty propertyElement)
                return propertyElement.GetBooleanProperty();

            return defaultValue;
        }

        public static void SetText(ContextElement element, string value)
        {
            if (element is IContextText textElement)
                textElement.SetText(value);
        }

        public static void SetTextGlobal(ContextElement element, string key, params object[] args)
        {
            SetText(element, StringTableUtils.GetString(StringTable.StringTableType.Global, key, args));
        }

        public static string GetText(ContextElement element, string defaultValue = "")
        {
            if (element is IContextText textElement)
                return textElement.GetText();

            return defaultValue;
        }

        public static void SetBlackboardValue<T>(ContextElement element, string key, T value)
        {
            if (element == null) return;

            Blackboard bb = element.gameObject.GetComponent<Blackboard>();

            if (bb != null)
                BlackboardUtils.SetOrCreateValue<T>(bb, key, value);
        }

        public static void SetInputFieldAttrribute(ContextElement element, int characterLimit, InputField.ContentType contentType)
        {
            if (element is IContextInputField textElement)
            {
                if (characterLimit > 0)
                    textElement.characterLimit = characterLimit;
                textElement.contentType = contentType;
            }
        }

        public static void SetSprite(ContextElement element, Sprite sprite)
        {
            if (element is IContextImage imageElement)
                imageElement.SetSprite(sprite);
        }

        public static void SetColor(ContextElement element, Color color)
        {
            if (element is IContextImage imageElement)
                imageElement.SetColor(color);
            // TODO
            // Implemented by hard coding because there is no SetTextColor function in IContextText
            else if (element is IContextText)
                element.GetComponent<TMPro.TextMeshProUGUI>().color = color;
        }

        public static void SetDropdownStringOptions(ContextElement element, List<string> options)
        {
            if (element is ContextTextMeshProDropdown dropdownContext)
            {
                var dropdown = dropdownContext.dropdown;
                dropdown.ClearOptions();
                dropdown.AddOptions(options);
            }
        }

        public static void SimpleSetSliderValue(ContextElement rootElement, string elementName, float value, ContextSearchingType searchingType)
        {
            ContextElement element = ContextUtils.FindElement(rootElement, elementName, searchingType);

            if (element is ContextSlider sliderElement)
                sliderElement.SetFloatProperty(value);
        }

        public static void SimpleSetIntProperty(ContextElement rootElement, string elementName, int value, ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch)
        {
            ContextElement element = ContextUtils.FindElement(rootElement, elementName, searchingType);

            if (element is IContextIntProperty property)
                property.SetIntProperty(value);
        }

        public static void SimpleSetFloatProperty(ContextElement rootElement, string elementName, float value, ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch)
        {
            ContextElement element = ContextUtils.FindElement(rootElement, elementName, searchingType);

            if (element is IContextFloatProperty property)
                property.SetFloatProperty(value);
        }

        public static void SimpleSetBooleanProperty(ContextElement rootElement, string elementName, bool value, ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch)
        {
            ContextElement element = ContextUtils.FindElement(rootElement, elementName, searchingType);

            if (element is IContextBooleanProperty property)
                property.SetBooleanProperty(value);
        }

        public static void SimpleSetText(ContextElement rootElement, string elementName, string value, ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch)
        {
            ContextElement element = ContextUtils.FindElement(rootElement, elementName, searchingType);

            if (element is IContextText textElement)
                textElement.SetText(value);
        }

        public static void SimpleSetTextGlobal(ContextElement rootElement, string elementName, string key, ContextSearchingType searchingType, params object[] args)
        {
            ContextElement element = ContextUtils.FindElement(rootElement, elementName, searchingType);

            if (element is IContextText)
                SetTextGlobal(element, key, args);
        }

        public static void SimpleSetDropdownStringOptions(ContextElement rootElement, string elementName, List<string> options, ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch)
        {
            ContextElement element = ContextUtils.FindElement(rootElement, elementName, searchingType);

            if (element != null)
                SetDropdownStringOptions(element, options);
        }

        public static void SimpleSetClickable(ContextElement rootElement, string elementName, GameObject receiver, string eventType, string eventName, bool sendGlobal = false, bool resetListener = true, ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch)
        {
            ContextElement element = ContextUtils.FindElement(rootElement, elementName, searchingType);

            if (element != null)
                SetClickable(element, receiver, eventType, new EventData(eventName), sendGlobal, resetListener);
        }

        public static void SimpleSetClickable(ContextElement rootElement, string elementName, System.Action action, bool resetListener = true, ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch)
        {
            ContextElement element = ContextUtils.FindElement(rootElement, elementName, searchingType);

            if (element != null)
                SetClickable(element, action, resetListener);
        }

        public static void SimpleSetWebImage(ContextElement rootElement, string elementName, string imageUrl, Action onLoadAction = null, Action onLoadFail = null, ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch)
        {
            ContextElement element = ContextUtils.FindElement(rootElement, elementName, searchingType);

            if (element != null)
                SetWebImage(element, imageUrl, CacheType.FileCache, false, onLoadAction, onLoadFail);
        }

        public static void SimpleSetSprite(ContextElement rootElement, string elementName, string bundle, string asset, ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch)
        {
            ContextElement element = ContextUtils.FindElement(rootElement, elementName, searchingType);

            if (element != null)
                SetSprite(element, bundle, asset);
        }

        public static void SimpleSetSprite(ContextElement rootElement, string elementName, Sprite sprite, ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch)
        {
            ContextElement element = ContextUtils.FindElement(rootElement, elementName, searchingType);

            if (element != null)
                SetSprite(element, sprite);
        }

        public static void DisableRemainingTimer(ContextElement element, string text)
        {
            var timerBB = element.gameObject.GetComponent<Blackboard>();
            if (timerBB != null)
            {
                BlackboardUtils.SetOrCreateValue<long>(timerBB, "targetTimestamp", 0L);
                BlackboardUtils.SetOrCreateValue<string>(timerBB, "expireText", text);
                BlackboardUtils.SetOrCreateValue<GameObject>(timerBB, "caller", null);
            }

            SimpleSetTextGlobal(element, "Text", "POPUP_CHALLENGE_EVENT_WAIT_NEXT", ContextSearchingType.ChildrenSearch);
        }

        public static void SetCommonRemainingTimer(ContextElement element, long targetTime, long warningTime, string timeFormatKey, string outputFormatKey, string warningFormatKey, string expireText, bool useCommonTimer, GameObject caller, string onEndTimerEvent = "")
        {
            var timerBB = element.gameObject.GetComponent<Blackboard>();
            if (timerBB != null)
            {
                BlackboardUtils.SetOrCreateValue<long>(timerBB, "targetTimestamp", targetTime);
                BlackboardUtils.SetOrCreateValue<long>(timerBB, "warningTimestamp", warningTime);

                BlackboardUtils.SetOrCreateValue<string>(timerBB, "textFormatKey", timeFormatKey);
                BlackboardUtils.SetOrCreateValue<string>(timerBB, "outputFormatKey", outputFormatKey);
                BlackboardUtils.SetOrCreateValue<string>(timerBB, "outputWarningKey", warningFormatKey);

                BlackboardUtils.SetOrCreateValue<string>(timerBB, "expireText", expireText);
                BlackboardUtils.SetOrCreateValue<bool>(timerBB, "useCommonTimer", useCommonTimer);
                BlackboardUtils.SetOrCreateValue<bool>(timerBB, "_initTimer", true);

                // OnEndTimer
                if (caller != null)
                {
                    if (!string.IsNullOrEmpty(onEndTimerEvent))
                        BlackboardUtils.SetOrCreateValue<string>(timerBB, "OnEndTimerEvent", onEndTimerEvent);

                    var variable = BlackboardUtils.GetOrCreateVariable<GameObject>(timerBB, "caller");
                    variable.value = caller;
                }
            }
        }

        public static void SetActive(ContextElement element, bool setTo)
        {
            if (element != null)
                element.gameObject.SetActive(setTo);
        }

        public static void SimpleSetActive(ContextElement rootElement, string elementName, bool value, ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch)
        {
            ContextElement element = ContextUtils.FindElement(rootElement, elementName, searchingType);

            if (element != null)
                element.gameObject.SetActive(value);
        }

        public static void SetWebImage(ContextElement element, string imageUrl, CacheType cachingType, bool priority, Action actionOnLoad)
        {
            SetWebImage(element, imageUrl, cachingType, priority, actionOnLoad, null);
        }

        public static void SetWebImage(ContextElement element, string imageUrl, CacheType cachingType, bool priority, Action actionOnLoad, Action actionOnFailed)
        {
            SetWebImage(element, imageUrl, cachingType, priority, actionOnLoad, actionOnFailed, null);
        }

        public static void SetWebImage(ContextElement element, string imageUrl, CacheType cachingType, bool priority, Action actionOnLoad, Action actionOnFailed, Action<float> onProgress)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                Debug.Log("SetWebImage Failure: The 'imageUrl' is null or empty.");
                return;
            }

            if (element is IContextImage imageElement)
            {
                imageElement.SetHash(imageUrl.GetHashCode().ToString());

                WebImageDownloader.Instance.LoadWebImage(
                    imageUrl,
                    cachingType,
                    priority,
                    null,
                    (Sprite img) =>
                    {
                        if (imageElement != null && img != null)
                        {
                            if (imageElement.CheckHash(imageUrl.GetHashCode().ToString()))
                            {
                                imageElement.SetSprite(img);
                                actionOnLoad?.Invoke();
                            }
                        }
                    },
                    onProgress,
                    (WebImageDownloader.WebImageDownloadError error) => actionOnFailed?.Invoke()
                );
            }
        }

        // only load chached image
        public static bool SetWebImage(ContextElement element, string imageUrl, Action actionOnLoad = null)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                Debug.Log("SetWebImage Failure: The 'imageUrl' is null or empty.");
                return false;
            }

            if (WebImageDownloader.Instance.CheckCachedImage(imageUrl, CacheType.FileCache))
            {
                SetWebImage(
                    element,
                    imageUrl,
                    CacheType.FileCache,
                    false,
                    () => actionOnLoad?.Invoke()
                );

                return true;
            }

            return false;
        }

        public static void UpdateContextImageOrientationScaler(ContextElement element, float scaleFactorWidth, float scaleFactorHeight)
        {
            if (element != null)
            {
                Image image = element.GetComponent<Image>();

                if (image != null && image.sprite != null && image.sprite.texture != null)
                {
                    Vector2 origSize = new Vector2(image.sprite.texture.width, image.sprite.texture.height);
                    Vector2 factorSize = new Vector2(scaleFactorWidth, scaleFactorHeight);

                    image.rectTransform.sizeDelta = ScreenUtils.GetScaleSize(origSize, factorSize);
                }
            }
        }

        public static void SetSliderValue(ContextElement element, float value)
        {
            if (element is ContextSlider sliderElement)
                sliderElement.SetFloatProperty(value);
        }

        public static void SetListenableGlobal<T>(ContextElement element, string eventName)
            => SetListenableGlobal<T>(element, EventSender.ON_CUSTOM_EVENT, eventName);

        public static void SetListenableGlobal<T>(ContextElement element, string eventType, string eventName)
        {
            if (element is IContextListenable<T> listenableElement)
            {
                listenableElement.AddListener((T value) =>
                {
                    var eventData = new EventData<T>(eventName, value);
                    EventSender.SendGlobalEvent(eventType, eventData);
                });
            }
        }

        public static void SetListenable<T>(ContextElement element, System.Action<T> action)
        {
            if (element is IContextListenable<T> listenableElement)
            {
                listenableElement.AddListener((T value) =>
                {
                    action?.Invoke(value);
                });
            }
        }

        public static void SetListenable<T>(ContextElement element, string eventName, GameObject listener)
            => SetListenable<T>(element, EventSender.ON_CUSTOM_EVENT, eventName, listener);

        public static void SetListenable<T>(ContextElement element, string eventType, string eventName, GameObject listener)
        {
            if (element is IContextListenable<T> listenableElement)
            {
                listenableElement.AddListener((T value) =>
                {
                    var eventData = new EventData<T>(eventName, value);
                    EventSender.SendEvent(listener, eventType, eventData);
                });
            }
        }

        public static void SetClickableGlobal(ContextElement element, string eventType, string eventName, bool resetListener)
        {
            if (element is IContextClickable clickableElement)
            {
                if (resetListener)
                    clickableElement.RemoveAllListener();

                clickableElement.AddListenerOnClick((ContextElement sender)
                    =>
                {
                    EventSender.SendGlobalEvent(eventType, eventName);
                });
            }
        }

        public static void SetClickable<T>(ContextElement element, GameObject receiver, string eventType, EventData<T> eventData, bool sendGlobal, bool resetListener)
        {
            if (element is IContextClickable clickableElement)
            {
                if (resetListener)
                    clickableElement.RemoveAllListener();

                if (sendGlobal)
                {
                    clickableElement.AddListenerOnClick((ContextElement sender)
                        =>
                    {
                        EventSender.SendGlobalEvent(eventType, eventData);
                    });
                }
                else
                {
                    clickableElement.AddListenerOnClick((ContextElement sender)
                        =>
                    {
                        EventSender.SendEvent(receiver, eventType, eventData);
                    });
                }
            }
        }

        public static void SetClickable(ContextElement element, GameObject receiver, string eventType, EventData eventData, bool sendGlobal, bool resetListener = true)
        {
            if (element is IContextClickable clickableElement)
            {
                if (resetListener)
                    clickableElement.RemoveAllListener();

                if (sendGlobal)
                {
                    clickableElement.AddListenerOnClick((ContextElement sender)
                        =>
                    {
                        EventSender.SendGlobalEvent(eventType, eventData);
                    });
                }
                else
                {
                    clickableElement.AddListenerOnClick((ContextElement sender)
                        =>
                    {
                        EventSender.SendEvent(receiver, eventType, eventData);
                    });
                }
            }
        }

        public static void SetClickable(ContextElement element, GameObject receiver, string eventType, string eventName, bool sendGlobal, bool resetListener = true)
        {
            SetClickable(element, receiver, eventType, new EventData(eventName), sendGlobal, resetListener);
        }

        public static void SetClickable(ContextElement element, GameObject receiver, string eventName, bool sendGlobal, bool resetLithener = true)
        {
            SetClickable(element, receiver, EventSender.ON_CUSTOM_EVENT, eventName, sendGlobal, resetLithener);
        }

        public static void SetClickable<T>(ContextElement element, string eventName, T eventValue, bool ignoreReset, bool sendGlobal, Action<string, T> SendEvent, ITaskSystem ownerSystem = null)
        {
            if (element is IContextClickable clickableElement)
            {
                var e = new EventData<T>(eventName, eventValue);

                if (ignoreReset == false)
                {
                    clickableElement.RemoveAllListener();
                }
                
                if (sendGlobal)
                    clickableElement.AddListenerOnClick( (ContextElement sender) => { Graph.SendGlobalEvent(e, sender); } );
                else 
                {
                    GraphOwner owner = null;

                    if(ownerSystem != null)
                        owner = ownerSystem.agent.GetComponent<GraphOwner>();

                    if(owner != null)
                        clickableElement.AddListenerOnClick( (ContextElement sender) => { owner.SendEvent(e, sender); } );
                    else
                        clickableElement.AddListenerOnClick( (ContextElement sender) => { SendEvent(eventName, eventValue); } );
                }
            }
        }
        
        public static void SetClickable(ContextElement element, string eventName, bool ignoreReset, bool sendGlobal, Action<string, ContextElement> SendEvent, ITaskSystem ownerSystem = null)
        {
            if (element is IContextClickable clickableElement)
            {
                if (ignoreReset == false)
                {
                    clickableElement.RemoveAllListener();
                }

                if (sendGlobal)
                    clickableElement.AddListenerOnClick((ContextElement sender) => { GraphOwner.SendGlobalEvent(eventName, sender); });
                else
                {
                    GraphOwner owner = null;

                    if (ownerSystem != null)
                        owner = ownerSystem.agent.GetComponent<GraphOwner>();

                    if (owner != null)
                        clickableElement.AddListenerOnClick((ContextElement sender) => { owner.SendEvent(eventName, sender); });
                    else
                        clickableElement.AddListenerOnClick((ContextElement sender) => { SendEvent(eventName, sender); });
                }
            }
        }

        public static void SetClickable(ContextElement buttonelement, string eventName, ContextElement ownerElement, object sender, bool resetListener = true)
        {
            if (buttonelement is IContextClickable clickableElement)
            {
                if (resetListener)
                    clickableElement.RemoveAllListener();

                clickableElement.AddListenerOnClick((ContextElement s) =>
               {
                   MetaContextElementUtils.SendEvent(buttonelement, eventName, ownerElement, sender);
               });
            }
        }
        
        public static void SetClickable<T>(ContextElement buttonelement, string eventName, T eventValue, ContextElement ownerElement, object sender, bool resetListener = true)
        {
            if (buttonelement is IContextClickable clickableElement)
            {
                if (resetListener)
                    clickableElement.RemoveAllListener();

                clickableElement.AddListenerOnClick((ContextElement s) =>
               {
                   MetaContextElementUtils.SendEvent(buttonelement, eventName, eventValue, ownerElement, sender);
               });
            }
        }

        public static void SetClickable(ContextElement buttonelement, System.Action action, bool resetListener = true)
        {
            if (buttonelement is IContextClickable clickableElement)
            {
                if (resetListener)
                    clickableElement.RemoveAllListener();

                clickableElement.AddListenerOnClick((ContextElement s) =>
                {
                    action();
                });
            }
        }

        // use EventSender instead this
        public static void SendEvent(ContextElement element, string eventName, ContextElement ownerElement, object sender)
        {
            if(element == null) return;

            GraphOwner graphOwner;
            if (ownerElement != null)
                graphOwner = ownerElement.gameObject.GetComponent<GraphOwner>();
            else
                graphOwner = element.gameObject.GetComponent<GraphOwner>();

            if(graphOwner != null)
                graphOwner.SendEvent(new EventData(eventName), sender);
        }

        public static void SendEvent<T>(ContextElement element, string eventName, T eventValue, ContextElement ownerElement, object sender)
        {
            if(element == null) return;

            GraphOwner graphOwner;
            if (ownerElement != null)
                graphOwner = ownerElement.gameObject.GetComponent<GraphOwner>();
            else
                graphOwner = element.gameObject.GetComponent<GraphOwner>();

            if(graphOwner != null)
                graphOwner.SendEvent(new EventData<T>(eventName, eventValue), sender);
        }

        public static void SetFlippingText(ContextElement element, string defaultText, List<string> flippingTextList, bool isFlip, float intervalTime)
        {
            Blackboard bb = element.GetComponent<Blackboard>();

            if(!string.IsNullOrEmpty(defaultText))
                BlackboardUtils.SetOrCreateValue<string>(bb, "defaultText", defaultText);

            BlackboardUtils.SetOrCreateValue<List<string>>(bb, "flippingTextList", flippingTextList);
            BlackboardUtils.SetOrCreateValue<float>(bb, "intervalTime", intervalTime);
            BlackboardUtils.SetOrCreateValue<bool>(bb, "isQuickStart", isFlip);
            BlackboardUtils.SetOrCreateValue<bool>(bb, "isRefresh", true);

            element.gameObject.SetActive(true);
        }

        public static void SetContextCountryImage(ContextElement element, string countryCode)
        {
            string countrySpriteName = StringTableUtils.GetString(StringTable.StringTableType.Global, "COUNTRY_ICON_FORMAT", countryCode);
            MetaContextElementUtils.SetSprite(element, MetaObjectUtils.MakeSprite(MetaStringDefine.LOBBY_BUNDLE_NAME, countrySpriteName));
        }

        public static void SetSprite(ContextElement element, string bundleName, string assetName)
        {
            if (element is IContextImage ImageElement)
            {
                var sprite = MetaObjectUtils.MakeSprite(bundleName, assetName);
                if (sprite != null)
                {
                    SetSprite(element, sprite);
                }
                else
                {
                    Debug.LogWarning(string.Format(
                        "MetaContextElementUtils.SetSprite failure. bundle:{0}, asset:{1}", bundleName, assetName));
                }
            }
        }

        public static void SetClickableMetaUIEvent(ContextElement element, EventData eventData, bool ignoreReset, GameObject ownerObj)
        {
            if (element is IContextClickable clickableElement)
            {
                if (ignoreReset == false)
                    clickableElement.RemoveAllListener();

                if (ownerObj == null)
                {
                    clickableElement.AddListenerOnClick((ContextElement s) =>
                    {
                        EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);
                    });
                }
                else
                {
                    MetaUIEventDispatcher dispatcher = ownerObj.GetComponent<MetaUIEventDispatcher>();
                    if (dispatcher != null)
                        clickableElement.AddListenerOnClick((ContextElement s) => { dispatcher.Dispatch(eventData); });
                }
            }

        }

        public static IEnumerator IncreaseProgressEffect(ContextElement element, float fromDelta, float toDelta, int overflow, float increaseEffectTime, float resetDelay, UnityAction<int> resetCallback, UnityAction endCallback)
        {
            SetFloatProperty(element, fromDelta);

            float totalProgressDelta = toDelta - fromDelta + overflow;

            float totalTimedelta = 0f;
            int resetCount = 0;

            if(totalProgressDelta > 0f)
            {
                while(totalTimedelta < increaseEffectTime)
                {
                    totalTimedelta += Time.deltaTime;
                    float currentDelta = Mathf.Lerp(0f, totalProgressDelta, totalTimedelta/increaseEffectTime);
                    if(resetCount + 1 == (int)(fromDelta + currentDelta) / 1)
                    {
                        ++resetCount;
                        if(resetCallback != null)
                        {
                            SetFloatProperty(element, 1f);
                            resetCallback.Invoke(resetCount);

                            yield return new WaitForSeconds(resetDelay);
                        }
                    }

                    float delta = (fromDelta + currentDelta) % 1f;
                    SetFloatProperty(element, delta);

                    yield return null;
                }
            }

            SetFloatProperty(element, toDelta);

            endCallback?.Invoke();
        }
        
        public static IEnumerator IncreaseProgressEffect(float fromDelta, float toDelta, int overflow, float increaseEffectTime, float resetDelay, UnityAction<float> updateCallback, UnityAction<int> resetCallback, UnityAction endCallback)
        {
            updateCallback?.Invoke(fromDelta);

            float totalProgressDelta = toDelta - fromDelta + overflow;

            float totalTimedelta = 0f;
            int resetCount = 0;

            if(totalProgressDelta > 0f)
            {
                while(totalTimedelta < increaseEffectTime)
                {
                    totalTimedelta += Time.deltaTime;
                    float currentDelta = Mathf.Lerp(0f, totalProgressDelta, totalTimedelta/increaseEffectTime);
                    if(resetCount + 1 == (int)(fromDelta + currentDelta) / 1)
                    {
                        ++resetCount;
                        if(resetCallback != null)
                        {
                            resetCallback.Invoke(resetCount);

                            yield return new WaitForSeconds(resetDelay);
                        }
                    }

                    updateCallback?.Invoke(currentDelta);

                    yield return null;
                }
            }

            updateCallback?.Invoke(totalProgressDelta);

            endCallback?.Invoke();
        }

        public static string GetMaximumLengthExceededChangeText(string baseText, int maximumLength, string changeText = "...")
        {
            if (string.IsNullOrEmpty(baseText))
                return baseText;

            int baseLength = baseText.Length;
            if (baseLength <= maximumLength || maximumLength <= 0)
                return baseText;

            return baseText.Substring(0, maximumLength) + changeText;
        }
    }

}
