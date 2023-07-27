using UnityEngine;
using System;
using System.Collections;


namespace BSS.Utils {
    public static class CoroutineUtility  {
        private class CoroutineHandler : MonoBehaviour { }

        private static GameObject _persistentObj;
        private static CoroutineHandler persistentMono {
            get {
                if (_persistentObj == null) {
                    _persistentObj = new GameObject("CoroutineHandler");
                    _persistentObj.AddComponent<CoroutineHandler>();
                    UnityEngine.Object.DontDestroyOnLoad(_persistentObj);
                }
                return persistentMono.GetComponent<CoroutineHandler>();
            }
        }

        public static Coroutine ExcuteAfterSeconds(this MonoBehaviour mono, float sec, Action action) {
            return mono.StartCoroutine(CoExcuteAfterSeconds(sec, action));
        }

        public static Coroutine ExcuteAfterCondition(this MonoBehaviour mono, Func<bool> condition, Action action) {
            return mono.StartCoroutine(CoExcuteAfterCondition(condition, action));
        }

        public static Coroutine ExcuteAfterFrame(this MonoBehaviour mono, int frameCount, Action action) {
            return mono.StartCoroutine(CoExcuteAfterFrame(frameCount, action));
        }

        public static Coroutine ExcuteWhileCondition(this MonoBehaviour mono, Func<bool> condition, Action action) {
            return mono.StartCoroutine(CoExcuteWhileCondition(condition, action));
        }

        public static Coroutine ExcuteAfterSecondsPersistently(float sec, Action action) {
            return persistentMono.StartCoroutine(CoExcuteAfterSeconds(sec, action));
        }
        public static Coroutine ExcuteAfterConditionPersistently( Func<bool> condition, Action action) {
            return persistentMono.StartCoroutine(CoExcuteAfterCondition(condition, action));
        }

        public static Coroutine ExcuteAfterFramePersistently(int frameCount, Action action) {
            return persistentMono.StartCoroutine(CoExcuteAfterFrame(frameCount, action));
        }

        public static Coroutine ExcuteWhileConditionPersistently(Func<bool> condition, Action action) {
            return persistentMono.StartCoroutine(CoExcuteWhileCondition(condition, action));
        }



        private static IEnumerator CoExcuteAfterCondition(Func<bool> condition, Action act) {
            yield return new WaitUntil(condition);
            act?.Invoke();
        }
        private static IEnumerator CoExcuteWhileCondition(Func<bool> condition, Action act) {
            while (true) {
                if (condition == null || !condition.Invoke()) yield break;
                act?.Invoke();
                yield return null;
            }
        }
        private static IEnumerator CoExcuteAfterFrame(int frameCount, Action act) {
            for (int i = 0; i < frameCount; i++) {
                yield return null;
            }
            act?.Invoke();
        }
        private static IEnumerator CoExcuteAfterSeconds(float seconds, Action act) {
            yield return new WaitForSeconds(seconds);
            act?.Invoke();
        }

    }
}

