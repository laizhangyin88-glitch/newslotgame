using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BSS.Utils {
    public static class ListUtility 
    {
        public static T Pop<T>(this IList<T> list) {
            T item = list[list.Count - 1];
            list.Remove(item);
            return item;
        }
        public static T Peek<T>(this IList<T> list) {
            return list[list.Count - 1];
        }
        public static T RandomPeek<T>(this IList<T> list) {
            return list[new Random().Next(0, list.Count-1)];
        }

        public static bool AddUnique<T>(this IList<T> collection, T item) {
            if(collection.Contains(item)) return false;
            collection.Add(item);
            return true;
        }
        public static void AddRangeUnique<T>(this IList<T> collection, IEnumerable<T> items) {
            foreach (var item in items) {
                collection.AddUnique(item);
            }
        }
        public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB) {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list;
        }
        public static IList<T> Fill<T>(this IList<T> list,T item,int count) {
            for (int i=0;i<count;i++) {
                list.Add(item);
            }
            return list;
        }
    }
}
