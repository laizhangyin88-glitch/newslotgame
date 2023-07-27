using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BagelCode
{
    public static class GenericExtension
    {
        #region General

        public static bool TryCast<T>(object o, out T result) where T : class
        {
            result = o is T res ? res : null;
            return result != null;
        }

        public static T Default<T>() where T : new()
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
                return new T();

            return default;
        }

        public static bool Is<T>(this object instance)
        {
            return instance.GetType() == typeof(T);
        }

        public static bool Is(this object instance, System.Type type)
        {
            return instance.GetType() == type;
        }

        #endregion

        #region Array

        public static void ForEach<T>(this T[] arr, System.Action<T> action)
        {
            foreach (var o in arr) action(o);
        }

        public static bool IsValidIndex<T>(this T[] arr, int idx)
        {
            return arr != null && 0 <= idx && idx < arr.Length;
        }

        public static T Random<T>(this T[] arr)
        {
            if (arr.Length > 0)
                return arr[UnityEngine.Random.Range(0, arr.Length)];
            else return default;
        }

        public static T CircularIndexing<T>(this T[] arr, ref int idx)
        {
            if (arr.Length == 0) return default;

            while (idx >= arr.Length)
                idx -= arr.Length;

            while (idx < 0)
                idx += arr.Length;

            return arr[idx];
        }

        public static T CircularIndexing<T>(this T[] arr, int idx)
        {
            if (arr.Length == 0) return default;

            while (idx >= arr.Length)
                idx -= arr.Length;

            while (idx < 0)
                idx += arr.Length;

            return arr[idx];
        }

        #endregion

        #region List

        public static void ForEachIndex<T>(this List<T> list, System.Action<int> action)
        {
            if (list == null) return;
            for (int i = 0; i < list.Count; ++i) action?.Invoke(i);
        }

        public static void AddDistinctly<T>(this List<T> list, T item)
        {
            if (list != null && !list.Contains(item)) list.Add(item);
        }

        public static T CircularIndexing<T>(this List<T> list, ref int idx)
        {
            if (list == null || list.Count == 0) return default;

            while (idx >= list.Count)
                idx -= list.Count;

            while (idx < 0)
                idx += list.Count;

            return list[idx];
        }

        public static T CircularIndexing<T>(this List<T> list, int idx)
        {
            if (list == null || list.Count == 0) return default;

            while(idx >= list.Count)
                idx -= list.Count;

            while (idx < 0)
                idx += list.Count;

            return list[idx];
        }

        public static void Swap<T>(this List<T> list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public static int IndexOfRange(this List<int> range, int value)
        {
            if (range == null || range.Count == 0) return -1;

            for (int i = 0; i < range.Count; ++i)
                if (value <= range[i]) return i;

            return range.Count;
        }

        public static int IndexOfRange(this List<float> range, float value)
        {
            if (range == null || range.Count == 0) return -1;

            for (int i = 0; i < range.Count; ++i)
                if (value <= range[i]) return i;

            return range.Count;
        }

        public static int IndexOfRange(this List<double> range, double value)
        {
            if (range == null || range.Count == 0) return -1;

            for (int i = 0; i < range.Count; ++i)
                if (value <= range[i]) return i;

            return range.Count;
        }

        public static bool IsValidIndex<T>(this List<T> list, int idx)
        {
            return list != null && 0 <= idx && idx < list.Count;
        }

        public static T IndexOfSafty<T>(this List<T> list, int idx)
        {
            if (list == null || !list.IsValidIndex(idx)) return default;

            return list[idx];
        }

        public static T Pop<T>(this List<T> list)
        {
            if (list.Count() == 0)
                return default;

            T last = list.Last();
            list.Remove(last);
            return last;
        }

        public static T PopFirst<T>(this List<T> list)
        {
            if (list.Count() == 0)
                return default;

            T first = list.First();
            list.Remove(first);
            return first;
        }

        public static T PopRandom<T>(this List<T> list)
        {
            if (list.Count() == 0)
                return default;

            T r = list.ElementAt(UnityEngine.Random.Range(0, list.Count()));
            list.Remove(r);
            return r;
        }

        public static T PopTarget<T>(this List<T> list, System.Func<T, bool> match)
        {
            if (list.Count() == 0)
                return default;

            var result = list.FirstOrDefault(match);
            list.Remove(result);
            return result;
        }

        public static List<T> PopAll<T>(this List<T> list, System.Func<T, bool> match)
        {
            if (list.Count() == 0)
                return default;

            List<T> result = new List<T>();
            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (match(list[i]))
                {
                    result.Add(list[i]);
                    list.RemoveAt(i);
                }
            }

            return result;
        }

        public static bool IsEmpty<T>(this List<T> list)
        {
            return list.Count == 0;
        }

        // return true if sequence has the target
        public static bool Pop<T>(this List<T> list, T target)
        {
            if (list.Contains(target))
            {
                list.Remove(target);
                return true;
            }
            return false;
        }

        public static bool Contains<T>(this List<T> list, List<T> list2)
        {
            return list.Any(e => list2.Contains(e));
        }

        #endregion

        #region Dictionary

        public static bool ChangeKey<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey from, TKey to)
        {
            if (!dict.ContainsKey(from) ||
                dict.ContainsKey(to))
                return false;

            TValue value = dict[from];
            dict.Remove(from);
            dict.Add(to, value);

            return true;
        }

        public static bool SetOrAddValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict == null) return false;
            else if (dict.ContainsKey(key)) dict[key] = value;
            else dict.Add(key, value);

            return true;
        }

        #endregion

        #region Linq

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, System.Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
                if (seenKeys.Add(keySelector(element)))
                    yield return element;
        }

        #endregion
    }
}
