using NodeCanvas.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public static class BlackboardUtils
    {
        public const char SPLIT_CHARACTER = '/';
        public const string LIST_WRAPPER_KEY = "value";
        public const string GAME_DIR = ".";

        public static IBlackboard CreateBlackboard(string name)
        {
            var go = new GameObject();
            go.name = name;
            return go.AddComponent<Blackboard>();
        }

        public static IBlackboard GetOrCreateBlackboard(IBlackboard bb, string key)
        {
            Variable variable = bb.GetVariable(key, typeof(Blackboard));
            if (variable == null)
            {
                var go = new GameObject();
                go.name = key;
                go.transform.parent = bb.propertiesBindTarget.GetComponent<Transform>();

                variable = bb.AddVariable(key, typeof(Blackboard));
                variable.value = go.AddComponent<Blackboard>();
            }

            return variable.value as IBlackboard;
        }

        public static List<Blackboard> GetOrCreateBlackboardList(IBlackboard bb, string key)
        {
            Variable variable = bb.GetVariable(key, typeof(List<Blackboard>));
            if (variable == null)
            {
                var go = new GameObject();
                go.name = key;
                go.transform.parent = bb.propertiesBindTarget.transform;

                variable = bb.AddVariable(key, typeof(List<Blackboard>));
                variable.value = new List<Blackboard>();
            }

            return variable.value as List<Blackboard>;
        }

        public static List<Blackboard> AddToBlackboardList(IBlackboard bb, string key, IBlackboard child)
        {
            var blackboard = (Blackboard)child;

            var bbList = GetOrCreateBlackboardList(bb, key);
            bbList.Add(blackboard);

            blackboard.transform.parent = ((Blackboard)bb).GetComponent<Transform>().Find(key);
            blackboard.transform.SetAsLastSibling();

            return bbList;
        }

        public static void RemoveAtBlackboardList(IBlackboard bb, string key, int index)
        {
            var bbList = bb.GetValue<List<Blackboard>>(key);
            GameObject.Destroy(bbList[index].gameObject);
            bbList.RemoveAt(index);
        }

        public static void SetOrCreateValue<T>(IBlackboard bb, string key, T value)
        {
            Variable variable = bb.GetVariable<T>(key);
            if (variable == null)
                variable = bb.AddVariable(key, typeof(T));
            variable.value = value;
        }

        public static void SetOrCreateValue(IBlackboard bb, string key, object value, Type ofType)
        {
            Variable variable = bb.GetVariable(key, ofType);
            if (variable == null)
                variable = bb.AddVariable(key, ofType);
            variable.value = value;
        }

        public delegate void SerializeToBB<T>(IBlackboard b, T value);

        public delegate void SerializeToBBEntry<T>(IBlackboard b, string key, T value);

        public static SerializeToBB<List<T>> WrapAnonymousList<T>(SerializeToBB<T> serialize)
        {
            return delegate (IBlackboard bb, List<T> list)
            {
                SetOrCreateList(bb, LIST_WRAPPER_KEY, list, serialize);
            };
        }

        public static SerializeToBB<T> WrapAnonymousList2<T>(SerializeToBBEntry<T> serialize)
        {
            return delegate (IBlackboard bb, T value)
            {
                serialize(bb, LIST_WRAPPER_KEY, value);
            };
        }

        public static SerializeToBB<Dictionary<TK, TV>> WrapDict<TK, TV>(SerializeToBB<TV> valueSerialize)
        {
            return delegate (IBlackboard bb, Dictionary<TK, TV> value)
            {
                SetOrCreateDict(bb, value, valueSerialize);
            };
        }

        public static SerializeToBB<Dictionary<TK, TV>> WrapDict2<TK, TV>(SerializeToBBEntry<TV> valueSerialize)
        {
            return delegate (IBlackboard bb, Dictionary<TK, TV> value)
            {
                if (value == null) return;
                foreach (var kvp in value)
                {
                    valueSerialize(bb, kvp.Key.ToString(), kvp.Value);
                }
            };
        }

        public static void SetOrCreateList<T>(IBlackboard bb, string key, List<T> list, SerializeToBB<T> serialize)
        {
            if (list == null) return;

            DestroyBlackboardList(bb, key);

            var listGo = new GameObject();
            listGo.name = key;
            listGo.transform.parent = bb.propertiesBindTarget.transform;

            List<Blackboard> blackboardList = new List<Blackboard>();
            int count = list.Count;
            for (int i = 0; i < count; ++i)
            {
                var go = new GameObject();
                go.name = typeof(T).Name;
                go.transform.parent = listGo.transform;

                var newBB = go.AddComponent<Blackboard>();
                serialize(newBB, list[i]);
                blackboardList.Add(newBB);
            }

            var variable = bb.AddVariable(key, typeof(List<Blackboard>));
            variable.value = blackboardList;
        }

        public static void SetOrCreateDict<K, V>(IBlackboard bb, string key, Dictionary<K, V> dict, SerializeToBB<V> serialize)
        {
            if (dict == null) return;

            DestroyBlackboard(bb, key);

            var dictBb = GetOrCreateBlackboard(bb, key);
            SetOrCreateDict(dictBb, dict, serialize);
            var variable = bb.AddVariable(key, typeof(Blackboard));
            variable.value = dictBb;
        }

        private static void SetOrCreateDict<K, V>(IBlackboard bb, Dictionary<K, V> dict, SerializeToBB<V> serialize)
        {
            foreach (KeyValuePair<K, V> kvp in dict)
            {
                var go = new GameObject();
                go.name = kvp.Key.ToString();
                go.transform.parent = bb.propertiesBindTarget.transform;

                var newBB = go.AddComponent<Blackboard>();
                serialize(newBB, kvp.Value);
                var variable = bb.AddVariable(kvp.Key.ToString(), typeof(Blackboard));
                variable.value = newBB;
            }
        }

        public static void DestroyBlackboard(IBlackboard bb, string key)
        {
            if (bb == null) return;

            var variable = bb.RemoveVariable(key);
            if (variable != null)
            {
                Blackboard removeBB = variable.value as Blackboard;
                if (removeBB != null)
                    GameObject.Destroy(removeBB.gameObject);
            }
        }

        public static void DestroyBlackboardList(IBlackboard bb, string key)
        {
            if (bb == null) return;

            var variable = bb.RemoveVariable(key);
            if (variable != null)
            {
                var listTransform = ((Blackboard)bb).transform.Find(key);
                if (listTransform != null)
                    GameObject.Destroy(listTransform.gameObject);
            }
        }

        public static void DestroyBlackboardDict(IBlackboard bb, string key)
        {
            // For now, it should do completely the same thing with BlackboardList while deleting BlackboardDict.
            DestroyBlackboardList(bb, key);
        }

        public static void ClearBlackboard(IBlackboard bb)
        {
            if (bb == null) return;

            bb.variables.Clear();
            ((Blackboard)bb).gameObject.DestroyChildren();
        }

        /// LEGACY - DUE 2019.1
        public static void CopyBlackboardVariables(IBlackboard from, IBlackboard to)
        {
            foreach (var pair in from.variables)
            {
                if (pair.Value.varType == typeof(Blackboard))
                {
                    var bb = (Blackboard)GetOrCreateBlackboard(to, pair.Key);
                    CopyBlackboardVariables((Blackboard)pair.Value.value, bb);
                }
                else
                {
                    var variable = to.AddVariable(pair.Key, pair.Value.varType);
                    variable.value = pair.Value.value;
                }
            }
        }

        public static void CopyBlackboard(IBlackboard src, IBlackboard dst)
        {
            foreach (var kvp in src.variables)
            {
                Type varType = kvp.Value.varType;
                if (varType == typeof(Blackboard))
                {
                    CopyBlackboard(kvp.Value.value as Blackboard, GetOrCreateBlackboard(dst, kvp.Key));
                }
                else if (varType == typeof(List<Blackboard>))
                {
                    SetOrCreateList<Blackboard>(dst, kvp.Key, kvp.Value.value as List<Blackboard>,
                        (_dst, _src) => { CopyBlackboard(_src, _dst); });
                }
                else
                {
                    // Warning: Reference type values are just linked
                    SetOrCreateValue(dst, kvp.Key, kvp.Value.value, varType);
                }
            }
        }

        public static IBlackboard FindBlackboard(IBlackboard bb, string name, ref string variableName)
        {
            variableName = null;

            if (string.IsNullOrEmpty(name))
                return null;

            var tokens = name.Split(SPLIT_CHARACTER);
            if (tokens == null || tokens.Length == 0)
                return null;

            int variableIdx = tokens.Length - 1;
            variableName = tokens[variableIdx];

            for (int i = 0; i < variableIdx; ++i)
            {
                if (i == 0)
                {
                    if (string.IsNullOrEmpty(tokens[i]))
                    {
                        bb = MainBlackboard.Get();
                        continue;
                    }
                    else if (tokens[i].Equals(GAME_DIR, StringComparison.Ordinal))
                    {
                        bb = ContentBlackboard.Get();
                        continue;
                    }
                }

                var newVariable = bb.GetVariable(tokens[i]);
                if (newVariable == null || newVariable.value == null)
                {
                    if (ApplicationSettings.LogSystem())
                        Debug.LogError("[Blackboard] Null(" + tokens[i] + ") blackboard variable founded in " + name);
                    return null;
                }

                if (newVariable.CanConvertTo(typeof(Blackboard)))
                {
                    bb = (IBlackboard)newVariable.value;
                }
                else if (newVariable.CanConvertTo(typeof(List<Blackboard>)))
                {
                    var bbList = (List<Blackboard>)newVariable.value;
                    if (++i < variableIdx)
                    {
                        int listIndex;
                        if (Int32.TryParse(tokens[i], out listIndex))
                        {
                            if (listIndex < bbList.Count)
                            {
                                bb = bbList[listIndex];
                                continue;
                            }
                            else
                            {
                                if (ApplicationSettings.LogSystem())
                                    Debug.LogWarning("[Blackboard] IndexOutOfRange(" + listIndex + ") founded in " + name);
                                return null;
                            }
                        }
                        else
                        {
                            if (ApplicationSettings.LogSystem())
                                Debug.LogWarning("[Blackboard] IndexParseError(" + tokens[i] + ") founded in " + name);
                            return null;
                        }
                    }
                    else
                    {
                        if (ApplicationSettings.LogSystem())
                            Debug.LogWarning("[Blackboard] ListIndexEmpty founded in " + name);
                        return null;
                    }
                }
                else
                {
                    if (ApplicationSettings.LogSystem())
                        Debug.LogWarning("[Blackboard] Unknown(" + tokens[i] + ") blackboard founded in " + name);
                    return null;
                }
            }

            return bb;
        }

        public static Variable FindVariable(IBlackboard bb, string name, Type type)
        {
            string variableName = null;
            bb = FindBlackboard(bb, name, ref variableName);
            if (bb == null) return null;

            return bb.GetVariable(variableName, type);
        }

        public static Variable<T> FindVariable<T>(IBlackboard bb, string name)
        {
            string variableName = null;
            bb = FindBlackboard(bb, name, ref variableName);
            if (bb == null) return null;

            return bb.GetVariable<T>(variableName);
        }

        public static Variable<T> FindVariable<T>(string name)
        {
            return FindVariable<T>(null, name);
        }

        public static Variable<T> GetOrCreateVariable<T>(IBlackboard bb, string name, bool doNotSearch = false)
        {
            string variableName = null;
            if (!doNotSearch)
            {
                bb = FindBlackboard(bb, name, ref variableName);
                if (bb == null) return null;
            }
            else
            {
                variableName = name;
            }

            var variable = bb.GetVariable<T>(variableName);
            if (variable == null)
                variable = bb.AddVariable(variableName, typeof(T)) as Variable<T>;

            return variable;
        }

        public static Variable<T> GetOrCreateVariable<T>(string name)
        {
            return GetOrCreateVariable<T>(null, name);
        }

        public static Variable FindOrCreateVariable(IBlackboard bb, string name, Type ofType)
        {
            string variableName = null;
            bb = FindBlackboard(bb, name, ref variableName);
            if (bb == null) return null;

            var variable = bb.GetVariable(variableName);
            if (variable == null)
                variable = bb.AddVariable(variableName, ofType);

            return variable;
        }

        public static Variable FindVariable(IBlackboard bb, string name)
        {
            string variableName = null;
            bb = FindBlackboard(bb, name, ref variableName);
            if (bb == null) return null;

            return bb.GetVariable(variableName);
        }

        public static object FindValue(IBlackboard bb, string name)
        {
            var variable = FindVariable(bb, name);
            if (variable == null) 
            {
                //Debug.LogWarning("Blackboard库(库名：" + bb.name + ") 的字段： " + name +" 为 null");
                Debug.LogError("[Blackboard](" + bb.name + ") Null variable founded in " + name);
                return null;
            }
            return variable.value;
        }

        public static T FindValue<T>(IBlackboard bb, string name)
        {
            return (T)FindValue(bb, name);
        }

        public static T FindValue<T>(string name)
        {
            return FindValue<T>(null, name);
        }

        public static Blackboard GetContentFSMBlackboard()
        {
            GameObject go = GameObject.Find("Meta System/Content FSM");
            if(go != null)
            {
                Blackboard bb = go.GetComponent<Blackboard>();
                return bb;
            }
            return null;
        }

        public static Blackboard GetGameContentsBlackboard()
        {
            GameObject Base = GameObject.Find("Game Canvas/Game Contents/Animator");
            if (Base != null)
            {
                return Base.GetComponent<Blackboard>();
            }
            return null;
        }
    }
}
