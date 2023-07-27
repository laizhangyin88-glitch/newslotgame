using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace SlotMaker
{
    public static class BlackboardSerializer
    {
        private static IBlackboardDeserializeStrategy currentBlackboardDeserializeStrategy;
        public static IBlackboardDeserializeStrategy CurrentBlackboardDeserializeStrategy
        {
            get
            {
                return currentBlackboardDeserializeStrategy ?? (currentBlackboardDeserializeStrategy = SimpleBlackboardDeserializeStrategy);
            }
            set
            {
                currentBlackboardDeserializeStrategy = value;
            }
        }

        private static IBlackboardDeserializeStrategy simpleBlackboardDeserializeStrategy;
        public static IBlackboardDeserializeStrategy SimpleBlackboardDeserializeStrategy
        {
            get
            {
                return simpleBlackboardDeserializeStrategy ?? (simpleBlackboardDeserializeStrategy = new SimpleBlackboardDeserializeStrategy());
            }
        }

        public static IDictionary<string, object> Deserialize(Blackboard bb, IBlackboardDeserializeStrategy blackboardDeserializeStrategy = null)
    	{
            if (blackboardDeserializeStrategy == null)
                blackboardDeserializeStrategy = CurrentBlackboardDeserializeStrategy;

    		IDictionary<string, object> BBdic = new Dictionary<string, object>();

    		foreach (var key in bb.GetVariableNames())
    		{
                var value = bb.GetVariable(key).value;
                if (value != null)
                {
                    BBdic[key] = DeserializeValue(bb, value, blackboardDeserializeStrategy);
                }
                else
                {
                    BBdic[key] = null;
                }
    		}

    		return BBdic;
    	}

        static object DeserializeValue(Blackboard bb, object value, IBlackboardDeserializeStrategy blackboardDeserializeStrategy)
        {
            object nestedBlackboard;
            bool success = blackboardDeserializeStrategy.TryDeserializeBlackboard(bb.transform, value, out nestedBlackboard);
            if (success)
            {
                return nestedBlackboard;
            }
            else
            {
                IDictionary dict = value as IDictionary;
                if (dict != null && dict.Count > 0)
                {
                    return DeserializeDictionary(bb, dict.Keys, dict.Values, blackboardDeserializeStrategy);
                }
                else
                {
                    IList list = value as IList;
                    if (list != null && list.Count > 0)
                    {
                        return DeserializeList(bb, list, blackboardDeserializeStrategy);
                    }
                    else if (IsBuiltInOrEnumType(value))
                    {
                        return value;
                    }
                }
            }
            return null;
        }

        static IDictionary<string, object> DeserializeDictionary(Blackboard bb, ICollection iKeys, ICollection iValues, IBlackboardDeserializeStrategy blackboardDeserializeStrategy)
        {
            foreach (var key in iKeys)
            {
                if (!IsBuiltInOrEnumType(key))
                    return null;
                else
                    break;
            }

            IDictionary<string, object> retDict = new Dictionary<string, object>();
            IEnumerator ke = iKeys.GetEnumerator();
            IEnumerator ve = iValues.GetEnumerator();

            while (ke.MoveNext() && ve.MoveNext())
            {
                string key = ke.Current.ToString();
                object value = ve.Current;

                retDict[key] = DeserializeValue(bb, value, blackboardDeserializeStrategy);
            }
            return retDict;
        }

        static IList DeserializeList(Blackboard bb, IList list, IBlackboardDeserializeStrategy blackboardDeserializeStrategy)
        {
            if (IsBuiltInOrEnumType(list[0]))
                return list;

            IList retList = new List<object>();
            foreach (var value in list)
            {
                retList.Add(DeserializeValue(bb, value, blackboardDeserializeStrategy));
            }
            return retList;
        }

        static bool IsBuiltInOrEnumType(object value)
        {
            // Numeric
            if (value is sbyte) return true;
            if (value is byte) return true;
            if (value is short) return true;
            if (value is ushort) return true;
            if (value is int) return true;
            if (value is uint) return true;
            if (value is long) return true;
            if (value is ulong) return true;
            if (value is float) return true;
            if (value is double) return true;
            if (value is decimal) return true;
            // Bool
            if (value is bool) return true;
            // String
            if (value is string) return true;
            // Enum (not built in)
            if (value.GetType().IsEnum) return true;

            return false;
        }
    }

    public interface IBlackboardDeserializeStrategy
    {
        bool TryDeserializeBlackboard(Transform parent, object input, out object output);
    }

    class SimpleBlackboardDeserializeStrategy : IBlackboardDeserializeStrategy
    {
        // This only permits to deserialize nested blackboard when hierarchy of game objects are in order
        public virtual bool TryDeserializeBlackboard(Transform parent, object input, out object output)
        {
            if (input is List<Blackboard>)
            {
                var bbList = new List<object>();
                foreach (var val in (List<Blackboard>)input)
                {
                    if (val.transform.parent.parent == parent || val.transform.parent == parent)
                        bbList.Add(BlackboardSerializer.Deserialize(val));
                    else
                        bbList.Add("Incorrect Path");
                }
                output = bbList;
                return true;
            }
            else if (input is Blackboard)
            {
                var inputBB = input as Blackboard;
                if (inputBB.transform.parent == parent)
                    output = BlackboardSerializer.Deserialize(inputBB);
                else
                    output = "Incorrect Path";
                return true;
            }
            output = null;
            return false;
        }
    }
}
