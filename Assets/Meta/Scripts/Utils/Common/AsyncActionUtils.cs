using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;

using VectorTweener = System.Func<UnityEngine.Vector3, UnityEngine.Vector3, float, UnityEngine.Vector3>;
using ColorTweener = System.Func<UnityEngine.Color, UnityEngine.Color, float, UnityEngine.Color>;
using UnityEngine.UI;

namespace BagelCode
{
    public static class AsyncActionUtils
    {
        public static Coroutine ProgressiveAction(MonoBehaviour agent, float time, System.Action start, System.Action<float> update, System.Action end)
        {
            return agent.StartCoroutine(ProgressiveActionCoroutine(time, start, update, end));
        }

        public static Coroutine ApplyMovement(MonoBehaviour agent, Transform transform, Vector3 from, Vector3 to, float time, VectorTweener tween, float delayBeforeStart = 0f, System.Action onComplete = null)
        {
            transform.position = from;
            return agent.StartCoroutine(MoveCoroutine(agent, transform, from, to, time, tween, delayBeforeStart, onComplete));
        }

        public static Coroutine ApplyAnchoredMovement(MonoBehaviour agent, Transform transform, Vector2 from, Vector2 to, float time, VectorTweener tween, float delayBeforeStart = 0f, System.Action onComplete = null)
        {
            transform.GetComponent<RectTransform>().anchoredPosition = from;
            return agent.StartCoroutine(AnchoredMoveCoroutine(agent, transform, from, to, time, tween, delayBeforeStart, onComplete));
        }

        public static Coroutine ApplyLocalMovement(MonoBehaviour agent, Transform transform, Vector3 from, Vector3 to, float time, VectorTweener tween, float delayBeforeStart = 0f, System.Action onComplete = null)
        {
            transform.position = from;
            return agent.StartCoroutine(LocalMoveCoroutine(agent, transform, from, to, time, tween, delayBeforeStart, onComplete));
        }

        public static Coroutine ApplyMovement(MonoBehaviour agent, Transform transform, Transform from, Transform to, float time, VectorTweener tween, float delayBeforeStart = 0f, System.Action onComplete = null)
        {
            transform.position = from.position;
            return agent.StartCoroutine(MoveCoroutine(agent, transform, from, to, time, tween, delayBeforeStart, onComplete));
        }

        public static Coroutine ApplyRotation(MonoBehaviour agent, Transform transform, Vector3 from, Vector3 to, float time, VectorTweener tween, float delayBeforeStart = 0f, System.Action onComplete = null)
        {
            transform.rotation = Quaternion.Euler(from);
            return agent.StartCoroutine(RotatingCoroutine(agent, transform, from, to, time, tween, delayBeforeStart, onComplete));
        }

        public static Coroutine ApplyScaling(MonoBehaviour agent, Transform transform, Vector3 from, Vector3 to, float time, VectorTweener tween, float delayBeforeStart = 0f, System.Action onComplete = null)
        {
            transform.localScale = from;
            return agent.StartCoroutine(ScalingCoroutine(agent, transform, from, to, time, tween, delayBeforeStart, onComplete));
        }

        public static Coroutine ApplyImageColor(MonoBehaviour agent, RawImage image, Color from, Color to, float time, ColorTweener tween, float delayBeforeStart = 0f, System.Action onComplete = null)
        {
            image.color = from;
            return agent.StartCoroutine(ImageColorCoroutine(agent, image, from, to, time, tween, delayBeforeStart, onComplete));
        }

        public static Coroutine ApplyTextColor(MonoBehaviour agent, Text text, Color from, Color to, float time, ColorTweener tween, float delayBeforeStart = 0f, System.Action onComplete = null)
        {
            text.color = from;
            return agent.StartCoroutine(TextColorCoroutine(agent, text, from, to, time, tween, delayBeforeStart, onComplete));
        }

        public static Coroutine DelayedAction(MonoBehaviour agent, float delay, System.Action action)
        {
            return agent.StartCoroutine(DelayedActionCoroutine(delay, action));
        }

        public static Coroutine DelayedDestroy(MonoBehaviour agent, float delay, GameObject go)
        {
            return agent.StartCoroutine(DelayedActionCoroutine(delay, () => { if (go != null) GameObject.Destroy(go); }));
        }

        public static Coroutine DelayedActiveAnimator(MonoBehaviour agent, float delay, Animator anim, string key, bool isActive)
        {
            return agent.StartCoroutine(DelayedActionCoroutine(delay, () => anim.SetBool(key, isActive)));
        }

        public static IEnumerator MoveCoroutine(MonoBehaviour agent, Transform transform, Transform from, Transform to, float time, VectorTweener tween, float delayBeforeStart, System.Action onComplete)
        {
            yield return new WaitForSeconds(delayBeforeStart);

            if (agent == null || transform == null || from == null || to == null)
            {
                Debug.LogWarning("MoveCoroutine failure. agent: " + agent + ", transform: " + transform);
                yield break;
            }

            yield return agent.StartCoroutine(ProgressiveActionCoroutine(time, null, (float progress) =>
            {
                if (!(agent == null || transform == null || from == null || to == null))
                    transform.position = tween(from.position, to.position, progress);
            }, onComplete));
        }

        public static IEnumerator MoveCoroutine(MonoBehaviour agent, Transform transform, Vector3 from, Vector3 to, float time, VectorTweener tween, float delayBeforeStart, System.Action onComplete)
        {
            yield return new WaitForSeconds(delayBeforeStart);

            if (agent == null || transform == null)
            {
                Debug.LogWarning("MoveCoroutine failure. agent: " + agent + ", transform: " + transform);
                yield break;
            }

            yield return agent.StartCoroutine(ProgressiveActionCoroutine(time, null, (float progress) =>
            {
                if (!(agent == null || transform == null))
                    transform.position = tween(from, to, progress);
            }, onComplete));
        }
        

        public static IEnumerator AnchoredMoveCoroutine(MonoBehaviour agent, Transform transform, Vector2 from, Vector2 to, float time, VectorTweener tween, float delayBeforeStart = 0, System.Action onComplete = null)
        {
            yield return new WaitForSeconds(delayBeforeStart);

            if (agent == null || transform == null)
            {
                Debug.LogWarning("AnchoredMoveCoroutine failure. agent: " + agent + ", transform: " + transform);
                yield break;
            }

            yield return agent.StartCoroutine(ProgressiveActionCoroutine(time, null, (float progress) =>
            {
                if (!(agent == null || transform == null))
                    transform.GetComponent<RectTransform>().anchoredPosition = tween(from, to, progress);
            }, onComplete));
        }

        public static IEnumerator LocalMoveCoroutine(MonoBehaviour agent, Transform transform, Vector3 from, Vector3 to, float time, VectorTweener tween, float delayBeforeStart, System.Action onComplete)
        {
            yield return new WaitForSeconds(delayBeforeStart);

            if (agent == null || transform == null)
            {
                Debug.LogWarning("LocalMoveCoroutine failure. agent: " + agent + ", transform: " + transform);
                yield break;
            }

            yield return agent.StartCoroutine(ProgressiveActionCoroutine(time, null, (float progress) =>
            {
                if (!(agent == null || transform == null))
                    transform.localPosition = tween(from, to, progress);
            }, onComplete));
        }

        public static IEnumerator RotatingCoroutine(MonoBehaviour agent, Transform transform, Vector3 from, Vector3 to, float time, VectorTweener tween, float delayBeforeStart, System.Action onComplete)
        {
            yield return new WaitForSeconds(delayBeforeStart);

            if (agent == null || transform == null)
            {
                Debug.LogWarning("RotatingCoroutine failure. agent: " + agent + ", transform: " + transform);
                yield break;
            }

            yield return agent.StartCoroutine(ProgressiveActionCoroutine(time, null, (float progress) =>
            {
                if (!(agent == null || transform == null))
                    transform.rotation = Quaternion.Euler(tween(from, to, progress));
            }, onComplete));
        }

        public static IEnumerator ScalingCoroutine(MonoBehaviour agent, Transform transform, Vector3 from, Vector3 to, float time, VectorTweener tween, float delayBeforeStart, System.Action onComplete)
        {
            yield return new WaitForSeconds(delayBeforeStart);

            if (agent == null || transform == null)
            {
                Debug.LogWarning("ScalingCoroutine failure. agent: " + agent + ", transform: " + transform);
                yield break;
            }

            yield return agent.StartCoroutine(ProgressiveActionCoroutine(time, null, (float progress) =>
            {
                if (!(agent == null || transform == null))
                    transform.localScale = tween(from, to, progress);
            }, onComplete));
        }

        public static IEnumerator ImageColorCoroutine(MonoBehaviour agent, RawImage image, Color from, Color to, float time, ColorTweener tween, float delayBeforeStart, System.Action onComplete)
        {
            yield return new WaitForSeconds(delayBeforeStart);

            if (agent == null || image == null)
            {
                Debug.LogWarning("ImageColorCoroutine failure. agent: " + agent + ", image: " + image);
                yield break;
            }

            yield return agent.StartCoroutine(ProgressiveActionCoroutine(time, null, (float progress) =>
            {
                if (!(agent == null || image == null))
                    image.color = tween(from, to, progress);
            }, onComplete));
        }

        public static IEnumerator TextColorCoroutine(MonoBehaviour agent, Text text, Color from, Color to, float time, ColorTweener tween, float delayBeforeStart, System.Action onComplete)
        {
            yield return new WaitForSeconds(delayBeforeStart);

            if (agent == null || text == null)
            {
                Debug.LogWarning("TextColorCoroutine failure. agent: " + agent + ", text: " + text);
                yield break;
            }

            yield return agent.StartCoroutine(ProgressiveActionCoroutine(time, null, (float progress) =>
            {
                if (!(agent == null || text == null))
                    text.color = tween(from, to, progress);
            }, onComplete));
        }

        public static IEnumerator ProgressiveActionCoroutine(float time, System.Action start, System.Action<float> update, System.Action end)
        {
            start?.Invoke();
            update?.Invoke(0f);

            float startTime = Time.time;
            float elapsed = 0f;
            float progress = 0f;
            while (elapsed < time)
            {
                yield return new WaitForEndOfFrame();
                elapsed = Time.time - startTime;
                progress = elapsed / time;

                update?.Invoke(progress);
            }

            update?.Invoke(1f);
            end?.Invoke();
        }

        public static IEnumerator DelayedActionCoroutine(float delay, System.Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}
