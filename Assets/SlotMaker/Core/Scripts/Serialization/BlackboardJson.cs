using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using UnityEngine;
using NodeCanvas.Framework;

namespace SlotMaker.Json
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class BlackboardJsonProperty : Attribute
    {
        public string Reference;
        public string Discriminator;
        public bool Inherit;
    }

    public static class BlackboardJson
    {
        static Dictionary<string, SchemaProperty> commonSchema = new Dictionary<string, SchemaProperty>();
        static Dictionary<string, SchemaProperty> volatilitySchema = new Dictionary<string, SchemaProperty>();

        static Dictionary<string, Type> schemaTypes;

        static BlackboardJson()
        {
            schemaTypes = new Dictionary<string, Type>
            {
                { "bool",    typeof(SchemaBool)    },
                { "byte",    typeof(SchemaByte)    },
                { "sbyte",   typeof(SchemaSByte)   },
                { "short",   typeof(SchemaShort)   },
                { "ushort",  typeof(SchemaUShort)  },
                { "int",     typeof(SchemaInt)     },
                { "uint",    typeof(SchemaUInt)    },
                { "long",    typeof(SchemaLong)    },
                { "ulong",   typeof(SchemaULong)   },
                { "float",   typeof(SchemaFloat)   },
                { "double",  typeof(SchemaDouble)  },
                { "decimal", typeof(SchemaDecimal) },
                { "string",  typeof(SchemaString)  },
                { "array",   typeof(SchemaArray)   },
                { "map",     typeof(SchemaMap)     },
                { "object",  typeof(SchemaObject)  }
            };
        }

        static INamingStrategy currentNamingStrategy;
        public static INamingStrategy CurrentNamingStrategy
        {
            get
            {
                return currentNamingStrategy ?? (currentNamingStrategy = new DefaultNamingStrategy());
            }
            set
            {
                currentNamingStrategy = value;
            }
        }

        static IJsonSerializerStrategy currentJsonSerializerStrategy;
        public static IJsonSerializerStrategy CurrentJsonSerializerStrategy
        {
            get
            {
                return currentJsonSerializerStrategy ?? (currentJsonSerializerStrategy = new PocoJsonSerializerStrategy());
            }
            set
            {
                currentJsonSerializerStrategy = value;
            }
        }

        public static void LoadSchema(string json, bool volatility = false)
        {
            var obj = SlotSimpleJson.DeserializeObject(json);
            IDictionary<string, object> objAsMap = obj as IDictionary<string, object>;
            if (objAsMap != null)
            {
                var schema = GetSchema(volatility);
                string schemaName = objAsMap["name"] as string;
                if (!schema.ContainsKey(schemaName))
                    schema.Add(schemaName, CreateSchemaProperty(objAsMap["schema"] as IDictionary<string, object>));
            }
            else
            {
                IList<object> objAsList = obj as IList<object>;
                if (objAsList != null)
                {
                    foreach (object o in objAsList)
                    {
                        var objects = o as IDictionary<string, object>;

                        var schema = GetSchema(volatility);
                        string schemaName = objects["name"] as string;
                        if (!schema.ContainsKey(schemaName))
                            schema.Add(schemaName, CreateSchemaProperty(objects["schema"] as IDictionary<string, object>));
                    }
                }
            }
        }

        static Dictionary<string, SchemaProperty> GetSchema(bool volatility)
        {
            return volatility ? volatilitySchema : commonSchema;
        }

        static SchemaProperty CreateSchemaProperty(IDictionary<string, object> objects)
        {
            SchemaProperty property = null;
            object jsonValue;
            if (objects.TryGetValue("type", out jsonValue))
            {
                property = (SchemaProperty)ConstructorStrategy.GetConstructor(schemaTypes[(string)jsonValue])();
                if (property.IsArray())
                {
                    var array = property as SchemaArray;
                    array.items = CreateSchemaProperty(objects["items"] as IDictionary<string, object>);
                }
                else if (property.IsMap())
                {
                    var map = property as SchemaMap;
                    map.code = CreateSchemaProperty(objects["code"] as IDictionary<string, object>);
                    map.text = CreateSchemaProperty(objects["text"] as IDictionary<string, object>);
                }
                else if (property.IsObject())
                {
                    var obj = property as SchemaObject;
                    var properties = objects["properties"] as IDictionary<string, object>;
                    foreach (KeyValuePair<string, object> kvp in properties)
                        obj.properties[kvp.Key] = CreateSchemaProperty(kvp.Value as IDictionary<string, object>);
                }
            }
            else
            {
                var reference = new SchemaReference();
                reference.reference = objects["ref"] as string;
                if (objects.TryGetValue("discriminator", out jsonValue))
                    reference.discriminator = jsonValue as string;
                if (objects.TryGetValue("inherit", out jsonValue))
                    reference.inherit = (bool)jsonValue;
                property = reference;
            }
            return property;
        }

        public static void UnloadVolatilitySchema()
        {
            volatilitySchema.Clear();
        }

        public static SchemaProperty Get(string schemaName)
        {
            SchemaProperty property;
            if (volatilitySchema.TryGetValue(schemaName, out property))
                return property;
            else if (commonSchema.TryGetValue(schemaName, out property))
                return property;
            return null;
        }

        public static void DeserializeObject(IBlackboard bb, string json, string schemaName, INamingStrategy namingStrategy = null)
        {
            DeserializeObject(bb, SlotSimpleJson.DeserializeObject(json), schemaName, namingStrategy);
        }

        public static void DeserializeObject(IBlackboard bb, object json, string schemaName, INamingStrategy namingStrategy = null)
        {
            Get(schemaName).DeserializeObject(bb, json, namingStrategy ?? CurrentNamingStrategy);
        }

        public static string SerializeObject(object json, IJsonSerializerStrategy jsonStrategy = null)
        {
            return SlotSimpleJson.SerializeObject(json, jsonStrategy ?? CurrentJsonSerializerStrategy);
        }
    }

    public abstract class SchemaProperty
    {
        public abstract Type GetSchemaType();
        public abstract void DeserializeObject(IBlackboard bb, string key, object value, INamingStrategy namingStrategy);

        public virtual SchemaProperty Resolve(IDictionary<string, object> map = null, INamingStrategy namingStrategy = null) { return this; }
        public virtual string GetTypeName() { return GetSchemaType().Name; }
        public virtual bool IsPrimitive() { return false; }
        public virtual bool IsArray() { return false; }
        public virtual bool IsMap() { return false; }
        public virtual bool IsObject() { return false; }
        public virtual void DeserializeObject(IBlackboard bb, object value, INamingStrategy namingStrategy) {}
        public virtual object DeserializeObject(object value) { return value; }

        protected const string WRAPPER_KEY = "value";
    }

    public class SchemaPrimitive<T> : SchemaProperty
    {
        public override Type GetSchemaType() { return typeof(T); }
        public override bool IsPrimitive() { return true; }

        public override void DeserializeObject(IBlackboard bb, string key, object value, INamingStrategy namingStrategy)
        {
#if UNITY_EDITOR
            try
            {
                BlackboardUtils.SetOrCreateValue(bb, namingStrategy.ToBlackboardPropertyName(key), DeserializeObject(value), GetSchemaType());
            }
            catch (Exception e)
            {
                Debug.LogError("Invalid value with type : " + key + " " + value.ToString() + " " + GetSchemaType());
            }
#else
            BlackboardUtils.SetOrCreateValue(bb, namingStrategy.ToBlackboardPropertyName(key), DeserializeObject(value), GetSchemaType());
#endif
        }

        public override object DeserializeObject(object value)
        {
            if (value == null)
                return default(T);

#if UNITY_EDITOR
            try
            {
                return Convert.ChangeType(value, GetSchemaType(), CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Debug.LogError("Invalid value with type : " + value.ToString() + " " + GetSchemaType());
                return default(T);
            }
#else
            return Convert.ChangeType(value, GetSchemaType(), CultureInfo.InvariantCulture);
#endif
        }
    }

    public class SchemaBool : SchemaPrimitive<bool> {}
    public class SchemaByte : SchemaPrimitive<byte> {}
    public class SchemaSByte : SchemaPrimitive<sbyte> {}
    public class SchemaShort : SchemaPrimitive<short> {}
    public class SchemaUShort : SchemaPrimitive<ushort> {}
    public class SchemaInt : SchemaPrimitive<int> {}
    public class SchemaUInt : SchemaPrimitive<uint> {}
    public class SchemaLong : SchemaPrimitive<long> {}
    public class SchemaULong : SchemaPrimitive<ulong> {}
    public class SchemaFloat : SchemaPrimitive<float> {}
    public class SchemaDouble : SchemaPrimitive<double> {}
    public class SchemaDecimal : SchemaPrimitive<decimal> {}
    public class SchemaString : SchemaPrimitive<string>
    {
        public override object DeserializeObject(object value)
        {
            return value;
        }
    }

    public class SchemaArray : SchemaProperty
    {
        public SchemaProperty items;

        public override Type GetSchemaType() { return typeof(List<>).MakeGenericType(items.GetSchemaType()); }
        public override bool IsArray() { return true; }

        public override void DeserializeObject(IBlackboard bb, string key, object value, INamingStrategy namingStrategy)
        {
            IList<object> valueAsList = value as IList<object>;
            if (valueAsList != null)
            {
                if (items.IsPrimitive())
                    BlackboardUtils.SetOrCreateValue(bb, namingStrategy.ToBlackboardPropertyName(key), DeserializeObject(value), GetSchemaType());
                else
                {
                    int count = valueAsList.Count;
                    var bbList = ConstructorStrategy.SetOrCreateBlackboardList(bb, namingStrategy.ToBlackboardPropertyName(key), items.GetTypeName(), count);
                    for (int i = 0; i < count; ++i)
                        items.Resolve().DeserializeObject(bbList[i], valueAsList[i], namingStrategy);
                }
            }
        }

        public override void DeserializeObject(IBlackboard bb, object value, INamingStrategy namingStrategy)
        {
            DeserializeObject(bb, WRAPPER_KEY, value, namingStrategy);
        }

        public override object DeserializeObject(object value)
        {
            IList<object> valueAsList = value as IList<object>;
            if (valueAsList != null && items.IsPrimitive())
            {
                IList list = null;
                Type valueType = value.GetType();
                if (valueType.IsArray)
                {
                    int count = valueAsList.Count;
                    list = (IList)ConstructorStrategy.GetConstructor(valueType)(count);
                    for (int i = 0; i < count; ++i)
                        list[i] = items.DeserializeObject(valueAsList[i]);
                }
                else
                {
                    list = (IList)ConstructorStrategy.GetConstructor(GetSchemaType())();
                    foreach (object o in valueAsList)
                        list.Add(items.DeserializeObject(o));
                }
                return list;
            }
            return value;
        }
    }

    public class SchemaMap : SchemaProperty
    {
        public SchemaProperty code;
        public SchemaProperty text;

        public override Type GetSchemaType() { return typeof(Dictionary<,>).MakeGenericType(code.GetSchemaType(), text.GetSchemaType()); }
        public override bool IsMap() { return true; }

        public override void DeserializeObject(IBlackboard bb, string key, object value, INamingStrategy namingStrategy)
        {
            IDictionary<string, object> valueAsMap = value as IDictionary<string, object>;
            if (valueAsMap != null && code.IsPrimitive())
            {
                if (!text.IsObject())
                    BlackboardUtils.SetOrCreateValue(bb, namingStrategy.ToBlackboardPropertyName(key), DeserializeObject(value), GetSchemaType());
                else
                {
                    var dictKeys = valueAsMap.Keys;
                    var bbDict = ConstructorStrategy.SetOrCreateBlackboardDict(bb, namingStrategy.ToBlackboardPropertyName(key), dictKeys, code);

                    foreach (string dictKey in dictKeys)
                    {
                        Blackboard dictValueBB = (Blackboard)bbDict[code.DeserializeObject(dictKey)];
                        text.Resolve().DeserializeObject(dictValueBB, valueAsMap[dictKey], namingStrategy);
                    }
                }
            }
        }

        public override void DeserializeObject(IBlackboard bb, object value, INamingStrategy namingStrategy)
        {
            DeserializeObject(bb, WRAPPER_KEY, value, namingStrategy);
        }

        public override object DeserializeObject(object value)
        {
            IDictionary<string, object> valueAsMap = value as IDictionary<string, object>;
            if (valueAsMap != null)
            {
                if (code.IsPrimitive())
                {
                    IDictionary map = (IDictionary)ConstructorStrategy.GetConstructor(GetSchemaType())();
                    foreach (KeyValuePair<string, object> kvp in valueAsMap)
                        map.Add(code.DeserializeObject(kvp.Key), text.DeserializeObject(kvp.Value));
                    return map;
                }
            }
            return value;
        }
    }

    public class SchemaObject : SchemaProperty
    {
        public Dictionary<string, SchemaProperty> properties = new Dictionary<string, SchemaProperty>();

        public override Type GetSchemaType() { return typeof(object); }
        public override bool IsObject() { return true; }

        public override void DeserializeObject(IBlackboard bb, string key, object value, INamingStrategy namingStrategy)
        {
            IDictionary<string, object> valueAsMap = value as IDictionary<string, object>;
            if (valueAsMap != null)
            {
                IBlackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, namingStrategy.ToBlackboardPropertyName(key));
                DeserializeObject(newBB, value, namingStrategy);
            }
        }

        public override void DeserializeObject(IBlackboard bb, object value, INamingStrategy namingStrategy)
        {
            IDictionary<string, object> valueAsMap = value as IDictionary<string, object>;
            if (valueAsMap != null)
            {
                foreach (KeyValuePair<string, SchemaProperty> kvp in properties)
                {
                    var property = kvp.Value.Resolve(valueAsMap, namingStrategy);

                    object jsonValue;
                    if (valueAsMap.TryGetValue(namingStrategy.ToJsonPropertyName(kvp.Key), out jsonValue))
                        property.DeserializeObject(bb, namingStrategy.ToBlackboardPropertyName(kvp.Key), jsonValue, namingStrategy);
                    else if (!property.IsObject())
                        property.DeserializeObject(bb, namingStrategy.ToBlackboardPropertyName(kvp.Key), ConstructorStrategy.GetConstructor(property.GetSchemaType()), namingStrategy);
                }
            }
        }
    }

    public class SchemaInheritObject : SchemaProperty
    {
        public SchemaProperty proxy;

        public SchemaInheritObject(SchemaProperty property)
        {
            proxy = property;
        }

        public override Type GetSchemaType() { return proxy.GetSchemaType(); }
        public override bool IsObject() { return true; }

        public override void DeserializeObject(IBlackboard bb, string key, object value, INamingStrategy namingStrategy)
        {
            DeserializeObject(bb, value, namingStrategy);
        }

        public override void DeserializeObject(IBlackboard bb, object value, INamingStrategy namingStrategy)
        {
            proxy.DeserializeObject(bb, value, namingStrategy);
        }
    }

    public class SchemaReference : SchemaProperty
    {
        public string reference;
        public string discriminator;
        public bool inherit;

        public override Type GetSchemaType() { return typeof(object); }
        public override bool IsObject() { return true; }

        public override SchemaProperty Resolve(IDictionary<string, object> map = null, INamingStrategy namingStrategy = null)
        {
            string key = null;
            if (string.IsNullOrEmpty(discriminator))
                key = reference;
            else
            {
                object jsonValue;
                if (map.TryGetValue(namingStrategy.ToJsonPropertyName(discriminator), out jsonValue))
                    key = string.Format(reference, jsonValue);
            }

            SchemaProperty ret = BlackboardJson.Get(key) ?? this;
            if (inherit)
                ret = new SchemaInheritObject(ret);
            return ret;
        }

        public override void DeserializeObject(IBlackboard bb, string key, object value, INamingStrategy namingStrategy) {}
        public override void DeserializeObject(IBlackboard bb, object value, INamingStrategy namingStrategy) {}
    }

    internal static class ConstructorStrategy
    {
        static IDictionary<Type, ReflectionUtils.ConstructorDelegate> ConstructorCache = new ReflectionUtils.ThreadSafeDictionary<Type, ReflectionUtils.ConstructorDelegate>(ContructorDelegateFactory);

        static readonly Type[] EmptyTypes = new Type[0];
        static readonly Type[] ArrayConstructorParameterTypes = new Type[] { typeof(int) };

        static ReflectionUtils.ConstructorDelegate ContructorDelegateFactory(Type key)
        {
            return ReflectionUtils.GetContructor(key, key.IsArray ? ArrayConstructorParameterTypes : EmptyTypes);
        }

        public static ReflectionUtils.ConstructorDelegate GetConstructor(Type type)
        {
            return ConstructorCache[type];
        }

        public static List<Blackboard> SetOrCreateBlackboardList(IBlackboard bb, string key, string typeName, int count)
        {
            BlackboardUtils.DestroyBlackboardList(bb, key);

            var listGo = new GameObject();
            listGo.name = key;
            listGo.transform.parent = bb.propertiesBindTarget.transform;

            List<Blackboard> bbList = new List<Blackboard>();
            for (int i = 0; i < count; ++i)
            {
                var go = new GameObject();
                go.name = typeName;
                go.transform.parent = listGo.transform;

                var newBB = go.AddComponent<Blackboard>();
                bbList.Add(newBB);
            }
            bb.AddVariable(key, bbList);

            return bbList;
        }

        public static IDictionary SetOrCreateBlackboardDict(IBlackboard bb, string key, IEnumerable<string> dictKeys, SchemaProperty keySchema)
        {
            BlackboardUtils.DestroyBlackboardDict(bb, key);

            var dictGo = new GameObject();
            dictGo.name = key;
            dictGo.transform.parent = bb.propertiesBindTarget.transform;

            var dictType = typeof(Dictionary<,>).MakeGenericType(keySchema.GetSchemaType(), typeof(Blackboard));

            IDictionary bbDict = ConstructorStrategy.GetConstructor(dictType)() as IDictionary;
            foreach (string dictKey in dictKeys)
            {
                var go = new GameObject();
                go.name = dictKey;
                go.transform.parent = dictGo.transform;

                var newBB = go.AddComponent<Blackboard>();
                bbDict.Add(keySchema.DeserializeObject(dictKey), newBB);
            }
            bb.AddVariable(key, bbDict);

            return bbDict;
        }
    }

    public interface INamingStrategy
    {
        string ToJsonPropertyName(string propertyName);
        string ToBlackboardPropertyName(string propertyName);
    }

    public class DefaultNamingStrategy : INamingStrategy
    {
        public string ToJsonPropertyName(string propertyName) { return propertyName; }
        public string ToBlackboardPropertyName(string propertyName) { return propertyName; }
    }

    public class SnakeToCamelCaseNamingStrategy : INamingStrategy
    {
        StringBuilder cachedStringBuilder = new StringBuilder(256);
        const char UNDER_SCORE = '_';

        public string ToJsonPropertyName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || char.IsUpper(propertyName[0]))
                return propertyName;

            cachedStringBuilder.Length = 0;
            cachedStringBuilder.Append(propertyName[0]);
            for (int i = 1; i < propertyName.Length; ++i)
            {
                if (char.IsUpper(propertyName[i]))
                {
                    if (!char.IsUpper(propertyName[i - 1]))
                    {
                        cachedStringBuilder.Append(UNDER_SCORE);
                        cachedStringBuilder.Append(char.ToLowerInvariant(propertyName[i]));
                    }
                }
                else
                {
                    cachedStringBuilder.Append(propertyName[i]);
                }
            }
            return cachedStringBuilder.ToString();
        }

        public string ToBlackboardPropertyName(string propertyName)
        {
            return propertyName;
        }
    }

    public class BlackboardJsonSerializerStrategy : PocoJsonSerializerStrategy
    {
        INamingStrategy namingStrategy;
        public INamingStrategy NamingStrategy
        {
            get
            {
                return namingStrategy ?? (namingStrategy = new DefaultNamingStrategy());
            }
            set
            {
                namingStrategy = value;
            }
        }

        protected override string MapClrMemberNameToJsonFieldName(MemberInfo memberInfo)
        {
            // TODO: Optimize and/or cache
            foreach (JsonProperty eachAttr in memberInfo.GetCustomAttributes(typeof(JsonProperty), true))
                if (!string.IsNullOrEmpty(eachAttr.PropertyName))
                    return eachAttr.PropertyName;

            return namingStrategy.ToJsonPropertyName(memberInfo.Name);
        }

        protected override void MapClrMemberNameToJsonFieldName(MemberInfo memberInfo, out string jsonName, out JsonProperty jsonProp)
        {
            jsonName = namingStrategy.ToJsonPropertyName(memberInfo.Name);
            jsonProp = null;

            // TODO: Optimize and/or cache
            foreach (JsonProperty eachAttr in memberInfo.GetCustomAttributes(typeof(JsonProperty), true))
            {
                jsonProp = eachAttr;
                if (!string.IsNullOrEmpty(eachAttr.PropertyName))
                    jsonName = eachAttr.PropertyName;
            }
        }
    }
}
