using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace BSS.Utils {
    public static class LinqUtility {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> act) {
            foreach(var it in source) {
                act?.Invoke(it);
            }
        }
        public static void ForEach<T>(this IEnumerable<T> source, Action<T,int> act) {
            int index = 0;
            foreach(var it in source) {
                act?.Invoke(it, index++);
            }
        }
        public static IEnumerable<(int index, T item)> Enumerate<T>(this IEnumerable<T> source) {
            int index = 0;
            foreach(var it in source) {
                yield return (index, it);
                index++;
            }
        }
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, IEnumerable<T> append) {
            foreach(var item in source) {
                yield return item;
            }
            foreach(var item in append) {
                yield return item;
            }
        }
    }
}
