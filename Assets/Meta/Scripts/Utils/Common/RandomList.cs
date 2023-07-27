using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace BagelCode
{
    // Wrapper class of 'List<T>' that cat set probability of each elements
    // The probability affects when selecting element randomly
    public class RandomList<T> : IEnumerable, IEnumerator
    {
        private int position;

        public List<Item> Items { get; private set; }

        public class Item
        {
            private RandomList<T> owner;
            public T value;
            private float probability;

            public Item(RandomList<T>  _owner, T _value, float _probabilty)
            {
                owner = _owner;
                value = _value;
                probability = _probabilty;
                Validate();
            }

            public Item(T _value, float _probabilty)
                : this(null, _value, _probabilty) { }

            public RandomList<T> GetOwner() => owner;
            public bool SetOwner(RandomList<T> _owner)
            {
                owner = _owner;
                return Validate();
            }

            public float GetProbability() => probability;
            public bool SetProbabaility(float _probability)
            {
                probability = _probability;
                return Validate();
            }

            public bool Validate()
            {
                if(probability < 0f && probability != REMAINDER)
                {
                    Debug.LogWarning(string.Format(
                        "Probability is set to {0:0.##\\%}. It can't be less than zero.",
                        probability * 100f));
                    return false;
                }

                if (owner != null)
                {
                    float total = owner.TotalProbability();
                    if (total > 1f)
                    {
                        Debug.LogWarning(string.Format(
                            "Total probability is {0:0.##\\%}. It can't exceed 100%.",
                            total * 100f));
                        return false;
                    }
                }
                return true;
            }
        }

        public RandomList()
        {
            Items = new List<Item>();
        }

        // If item's probability set to REMAINDER, Their probability is remainder of total probability.
        private const float REMAINDER = float.MinValue;

        // Return random value(T) by probability of each items
        public T SelectRandom(bool ignoreProbability = false)
        {
            if(ignoreProbability)
                return Items.Count() > 0 ? Items[UnityEngine.Random.Range(0, Items.Count())].value : default;

            float total = TotalProbability();
            float remainder = 1f - total;
            int remainderCount = Items.Count(t => t.GetProbability() == REMAINDER);
            float remainderProbability = remainderCount > 0 ? Mathf.Max(remainder / remainderCount, 0f) : 0f;

            if(total < 1f && remainderCount == 0)
            {
                Debug.LogWarning(string.Format(
                    "Total probability is {0:0.##\\%}. If less than 100%, return can be null.",
                    total * 100f));
            }

            float r = UnityEngine.Random.Range(0f, 1f);
            float sum = 0f;
            return Items.FirstOrDefault(t =>
            {
                float p = t.GetProbability();
                if (p == REMAINDER) p = remainderProbability;
                return r < (sum += p);
            }).value;
        }

        public void Add(T value, float probability)
        {
            var item = new Item(value, probability);
            item.SetOwner(this);
            Items.Add(item);
        }

        public void Add(T value)
        {
            Add(value, REMAINDER);
        }

        public void AddRange(T[] values)
        {
            AddRange(values, REMAINDER);
        }

        public void AddRange(T[] values, float probability)
        {
            Items.AddRange(values.Select(v =>
            {
                var item = new Item(v, probability);
                item.SetOwner(this);
                return item;
            }));
        }

        public void AddRange(List<T> values)
        {
            AddRange(values, REMAINDER);
        }

        public void AddRange(List<T> values, float weight)
        {
            Items.AddRange(values.Select(v =>
            {
                var item = new Item(v, weight);
                item.SetOwner(this);
                return item;
            }));
        }

        public bool Remove(T target)
        {
            return Items.Remove(Items.FirstOrDefault(t => t.value.Equals(target)));
        }

        public void RemoveAt(int i)
        {
            if (!IsValidIndex(i))
            {
                Debug.LogWarning(string.Format("RemoveAt Failure: {0} is invalid index", i));
                return;
            }

            Items.RemoveAt(i);
        }

        public void Clear()
        {
            Items.Clear();
        }

        public float TotalProbability()
        {
            return Items.Sum(t => (t.GetProbability() != REMAINDER) ? t.GetProbability(): 0f);
        }

        public bool HasRemainder()
        {
            return Items.Any(t => t.GetProbability() == REMAINDER);
        }

        public T GetValueAt(int i)
        {
            if (!IsValidIndex(i))
            {
                Debug.LogWarning(string.Format("GetValueAt Failure: {0} is invalid index", i));
                return default;
            }

            return Items[i].value;
        }

        public bool SetValueAt(int i, T value)
        {
            if (!IsValidIndex(i)) return false;

            var item = Items[i];
            item.value = value;
            return true;
        }

        public float GetProbabilityAt(int i)
        {
            if (!IsValidIndex(i))
            {
                Debug.LogWarning(string.Format("GetProbabilityAt Failure: {0} is invalid index", i));
                return default;
            }

            return Items[i].GetProbability();
        }

        public bool SetProbabilityAt(int i, float probability)
        {
            if (!IsValidIndex(i)) return false;

            var item = Items[i];
            item.SetProbabaility(probability);
            return true;
        }

        public bool SetRemainderAt(int i)
        {
            if (!IsValidIndex(i)) return false;

            var item = Items[i];
            item.SetProbabaility(REMAINDER);
            return true;
        }

        public bool IsValidIndex(int i)
        {
            return 0 <= i && i < Items.Count;
        }

        public object Current
        {
            get
            {
                return Items[position];
            }
        }

        public bool MoveNext()
        {
            if (position == Items.Count - 1)
            {
                Reset();
                return false;
            }

            return ++position < Items.Count;
        }

        public void Reset()
        {
            position = -1;
        }

        public IEnumerator GetEnumerator()
        {
            for(int i = 0; i < Items.Count; ++i)
            {
                yield return Items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            Dispose();
        }
    }
}