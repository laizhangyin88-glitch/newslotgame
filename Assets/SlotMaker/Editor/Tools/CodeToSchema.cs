using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEditor;

namespace SlotMaker.Json
{
    public static class CodeToSchema
    {
        enum ReadSequence
        {
            ReadObject,
            ReadProperties
        };

        static readonly char[] SPLIT_CHARACTERS = new char[]{ ' ', '\t', ';' };
        static readonly char[] SPLIT_COLLECTIONS = new char[]{ '<', '>', ',' };
        static readonly char[] SPLIT_JSON_PROPERTY = new char[]{ ' ', '\t', '[', ']', '(', ')', '=', '\"', ',' };
        static readonly char SLASH = '/';

        static HashSet<string> primitiveMap;
        static HashSet<string> enumMap;

        [MenuItem("Assets/Code To Schema", false, 703)]
        static void CreateSchema()
        {
            var objects = Selection.GetFiltered(typeof(TextAsset), SelectionMode.Assets);
            foreach (var obj in objects)
            {
                ToSchema(obj as TextAsset);
            }
        }

        static void ToSchema(TextAsset textAsset)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            Dictionary<string, object> newSchema = null;
            Dictionary<string, object> properties = null;
            ReadSequence sequence = ReadSequence.ReadObject;

            primitiveMap = new HashSet<string>{
                "bool", "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal", "string"
            };
            enumMap = new HashSet<string>();

            var reader = new StringReader(textAsset.text);
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                    break;

                var tokens = CreateTokensWithoutComment(line);
                if (tokens.Length == 0)
                    continue;

                switch (sequence)
                {
                case ReadSequence.ReadObject:
                    for (int i = 0; i < tokens.Length; ++i)
                    {
                        if (string.Equals(tokens[i], "class"))
                        {
                            newSchema = new Dictionary<string, object>();
                            newSchema["name"] = tokens[i + 1];
                            // newSchema["description"] = tokens[i + 1];
                            var schema = new Dictionary<string, object>();
                            schema["type"] = "object";
                            properties = new Dictionary<string, object>();
                            schema["properties"] = properties;
                            newSchema["schema"] = schema;
                            result.Add(newSchema);

                            line = reader.ReadLine();
                            if (line == null)
                                break;

                            tokens = line.Split(SPLIT_CHARACTERS);
                            foreach (var token in tokens)
                            {
                                if (string.Equals(token, "{"))
                                {
                                    sequence = ReadSequence.ReadProperties;
                                    break;
                                }
                            }
                            break;
                        }
                        else if (string.Equals(tokens[i], "enum"))
                        {
                            enumMap.Add(tokens[i + 1]);
                            break;
                        }
                    }
                    break;
                case ReadSequence.ReadProperties:
                    {
                        if (tokens.Length > 0 && tokens[0][0] == '[')
                        {
                            var attribute = line.Split(SPLIT_JSON_PROPERTY, StringSplitOptions.RemoveEmptyEntries);
                            string reference = string.Empty;
                            string discriminator = string.Empty;
                            string inherit = "false";
                            for (int i = 1; i < attribute.Length; ++i)
                            {
                                if (string.Equals(attribute[i], "Reference"))
                                    reference = attribute[i + 1];
                                else if (string.Equals(attribute[i], "Discriminator"))
                                    discriminator = attribute[i + 1];
                                else if (string.Equals(attribute[i], "Inherit"))
                                    inherit = attribute[i + 1];
                            }

                            line = reader.ReadLine();
                            var nextTokens = line.Split(SPLIT_CHARACTERS, StringSplitOptions.RemoveEmptyEntries);
                            tokens = new string[5];
                            tokens[0] = attribute[0];
                            tokens[1] = nextTokens[2];
                            tokens[2] = reference;
                            tokens[3] = discriminator;
                            tokens[4] = inherit;

                            properties[tokens[1]] = ParseProperty(tokens, 0);
                            break;
                        }

                        if (string.Equals(tokens[0], "}"))
                        {
                            sequence = ReadSequence.ReadObject;
                            break;
                        }
                        else 
                        {
                            properties[tokens[2]] = ParseProperty(tokens[1].Split(SPLIT_COLLECTIONS, StringSplitOptions.RemoveEmptyEntries), 0);
                            break;
                        }
                    }
                }
            }

            FileSystem.SaveJsonAsset(System.IO.Path.ChangeExtension(AssetDatabase.GetAssetPath(textAsset), ".json"), result);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static string[] CreateTokensWithoutComment(string line)
        {
            string lineWithoutComment = line;

            string[] tokens = line.Split(SLASH);
            if (tokens.Length > 1)
                lineWithoutComment = tokens[0];

            return lineWithoutComment.Split(SPLIT_CHARACTERS, StringSplitOptions.RemoveEmptyEntries);
        }

        static Dictionary<string, object> ParseProperty(string[] tokens, int i)
        {
            if (primitiveMap.Contains(tokens[i]))
            {
                return new Dictionary<string, object>{
                    { "type", tokens[i] }
                };
            }
            else if (enumMap.Contains(tokens[i]))
            {
                return new Dictionary<string, object>{
                    { "type", "int" }
                };
            }
            else if (string.Equals(tokens[i], "List"))
            {
                return new Dictionary<string, object>{
                    { "type", "array" },
                    { "items", ParseProperty(tokens, i + 1) }
                };
            }
            else if (string.Equals(tokens[i], "Dictionary"))
            {
                return new Dictionary<string, object>{
                    { "type", "map" },
                    { "code", ParseProperty(tokens, i + 1) },
                    { "text", ParseProperty(tokens, i + 2) }
                };
            }
            else if (string.Equals(tokens[i], "BlackboardJsonProperty"))
            {
                return new Dictionary<string, object>{
                    { "ref", tokens[i + 2] },
                    { "discriminator", tokens[i + 3] },
                    { "inherit", Convert.ChangeType(tokens[i + 4], typeof(bool), CultureInfo.InvariantCulture) }
                };
            }
            else
            {
                return new Dictionary<string, object>{
                    { "ref", tokens[i] }
                };
            }
        }
    }
}
















