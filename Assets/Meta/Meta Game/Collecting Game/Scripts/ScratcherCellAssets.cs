using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
using BagelCode.ClientModels;
#endif

namespace BagelCode.Scratcher
{
    [CreateAssetMenu(fileName="New ScratcherCellAssets", menuName="Meta/ScriptableObject/ScratcherCellAssets")]
    public class ScratcherCellAssets : ScriptableObject
    {
        public string scratcherCellImagePath = "Assets/Meta/App0/Textures/Scratcher";
        [ListDrawerSettings(NumberOfItemsPerPage = 7)]
        public List<ScratcherSymbolAsset> symbolAssets;
        [ListDrawerSettings(NumberOfItemsPerPage = 7)]
        public List<ScratcherCoverAsset> customCoverAssets;
        [ShowInInspector]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine, KeyLabel = "Symbol ID", ValueLabel = "Sprite")]
        public List<CustomHighlightAsset> customHighlightAssets = new List<CustomHighlightAsset>();

#if UNITY_EDITOR
        [ContextMenu("Setup")]
        void Setup()
        {
            symbolAssets = new List<ScratcherSymbolAsset>();

            for (int i = 0; i <= 99; i++)
            {
                ScratcherSymbolAsset symbolAsset = new ScratcherSymbolAsset();
                symbolAsset.text = i.ToString();
                symbolAsset.value = i.ToString();
                symbolAsset.color = Color.clear;
                symbolAssets.Add(symbolAsset);
            }
            
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
            for (int i = 0; i < alpha.Length; i++)
            {                
                ScratcherSymbolAsset symbolAsset = new ScratcherSymbolAsset();
                symbolAsset.text = alpha[i].ToString();
                symbolAsset.value = alpha[i].ToString();
                symbolAsset.color = Color.clear;
                symbolAssets.Add(symbolAsset);
            }

            string[] vegasNightSymbols =
            {
                "Symbol Seven.png",
                "Symbol Hotel.png",
                "Symbol Chip.png",
                "Symbol Multiplier 20.png",
                "Symbol Dice Instant.png",
                "Symbol Dice 1.png",
                "Symbol Dice 2.png",
                "Symbol Dice 3.png",
                "Symbol Dice 4.png",
                "Symbol Dice 5.png",
                "Symbol Dice 6.png"
            };
            for (int i = 0; i < vegasNightSymbols.Length; i++)
            {
                ScratcherSymbolAsset symbolAsset = new ScratcherSymbolAsset();
                symbolAsset.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(scratcherCellImagePath + "/" + vegasNightSymbols[i]);
                if (i >= 5)
                    symbolAsset.value = (i-4).ToString();
                symbolAsset.color = Color.clear;
                symbolAssets.Add(symbolAsset);
            }
            
            for (int i = 100; i <= 200; i++)
            {
                ScratcherSymbolAsset symbolAsset = new ScratcherSymbolAsset();
                symbolAsset.text = i.ToString();
                symbolAsset.value = i.ToString();
                symbolAsset.color = Color.clear;
                symbolAssets.Add(symbolAsset);
            }
            
            string[] pokerSymbols =
            {
                "Symbol Spade.png",
                "Symbol Heart.png",
                "Symbol Diamond.png",
                "Symbol Clover.png"
            };

            string[] pokerTexts =
            {
                "A",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "10",
                "J",
                "Q",
                "K"
            };

            Color red = new Color(0.70f, 0f, 0f);
            Color black = Color.black;

            for (int i = 0; i < pokerTexts.Length; i++)
            {
                for (int j = 0; j < pokerSymbols.Length; j++)
                {
                    ScratcherSymbolAsset symbolAsset = new ScratcherSymbolAsset();
                    symbolAsset.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(scratcherCellImagePath + "/" + pokerSymbols[j]);
                    symbolAsset.text = pokerTexts[i];
                    if (j == 0 || j == 3)
                        symbolAsset.color = black;
                    else
                        symbolAsset.color = red;
                    symbolAssets.Add(symbolAsset);
                }
            }
            
            ScratcherSymbolAsset jokerSymbolAsset = new ScratcherSymbolAsset();
            Color jokerColor = new Color(0.42f, 0f, 1f);
            jokerSymbolAsset.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(scratcherCellImagePath + "/Symbol Joker.png");
            jokerSymbolAsset.text = "J K R";
            jokerSymbolAsset.color = jokerColor;
            symbolAssets.Add(jokerSymbolAsset);

            for (int i = 1; i < 100; i++)
            {
                ScratcherSymbolAsset symbolAsset = new ScratcherSymbolAsset();
                symbolAsset.text = i + "x";
                symbolAsset.color = Color.clear;
                symbolAssets.Add(symbolAsset);
            }
            
            string[] richPalaceSymbols =
            {
                "Symbol Rich Palace 1.png",
                "Symbol Rich Palace 2.png",
                "Symbol Rich Palace 3.png",
                "Symbol Rich Palace 4.png"
            };
            
            for (int i = 0; i < richPalaceSymbols.Length; i++)
            {
                ScratcherSymbolAsset symbolAsset = new ScratcherSymbolAsset();
                symbolAsset.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(scratcherCellImagePath + "/" + richPalaceSymbols[i]);
                symbolAsset.color = Color.clear;
                symbolAssets.Add(symbolAsset);
            }

            ScratcherSymbolAsset multiplier10XSymbolAsset = new ScratcherSymbolAsset();
            multiplier10XSymbolAsset.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(scratcherCellImagePath + "/Symbol Multiplier 10.png");
            multiplier10XSymbolAsset.color = Color.clear;
            symbolAssets.Add(multiplier10XSymbolAsset);

        }

        private void OnValidate()
        {
            for (int i = 0; i < symbolAssets.Count; i++)
            {
                ScratcherSymbolAsset symbolAsset = symbolAssets[i];

                if (!string.IsNullOrEmpty(symbolAsset.text) && symbolAsset.sprite != null)
                {
                    symbolAsset.name = string.Format("[{0}, {1}, {2}] {3}, {4}", i, "SPRITE", "TEXT", symbolAsset.sprite.name, symbolAsset.text);
                }
                else if (!string.IsNullOrEmpty(symbolAsset.text))
                {
                    symbolAsset.name = string.Format("[{0}, {1}] {2}", i, "TEXT", symbolAsset.text);
                }
                else if (symbolAsset.sprite != null)
                {
                    symbolAsset.name = string.Format("[{0}, {1}] {2}", i, "SPRITE", symbolAsset.sprite.name);
                }
                else
                {
                    symbolAsset.name = string.Format("[{0}]", i);
                }
                
            }

            for(int i = 0; i < customCoverAssets.Count; ++i)
            {
                customCoverAssets[i].id = i;
            }
        }

        [ContextMenu("GeneratePreset")]
        void GeneratePreset()
        {
            string symbolMapper = "const symbolMapper = {\n";
            for (int i = 0; i < symbolAssets.Count; i++)
            {
                string symbol = "";
                if (!string.IsNullOrEmpty(symbolAssets[i].text) && symbolAssets[i].sprite != null)
                {
                    symbol = symbol + "    SYMBOL_" + symbolAssets[i].sprite.name.Replace("Symbol ", "").Replace(" ", "_").ToUpper() + "_" + symbolAssets[i].text.Replace(" ", "_") + ": " + i + ",\n";
                }
                else if (!string.IsNullOrEmpty(symbolAssets[i].text))
                {
                    symbol = symbol + "    SYMBOL_" + symbolAssets[i].text.Replace(" ", "_") + ": " + i + ",\n";
                }
                else if (symbolAssets[i].sprite != null)
                {
                    symbol = symbol + "    SYMBOL_" + symbolAssets[i].sprite.name.Replace("Symbol ", "").Replace(" ", "_").ToUpper() + ": " + i + ",\n";
                }

                symbolMapper = symbolMapper + symbol;
            }

            symbolMapper = symbolMapper + "};\n";
            
            var path = EditorUtility.SaveFilePanel(
                "Save Assets as Mapper",
                "Assets/Meta/Meta Game/Collecting Game/Datas",
                "SymbolMapper" + ".txt",
                "txt");

            if (path.Length != 0)
            {
                System.IO.File.WriteAllText(path, symbolMapper);
            }
        }

        //[ContextMenu("GenerateCoverPreset")]
        //void GenerateCoverPreset()
        //{
        //    customCoverAssets.Clear();

        //    string scratcherSymbolsMapper = "const scratcherList = {\n";

        //    for(int i=0; i < coverAssets.Count; ++i)
        //    {
        //        int totalSymbolCount = 0;

        //        BagelCode.ClientModels.ScratcherName scratcherName = (BagelCode.ClientModels.ScratcherName)(i+1);
        //        string scratcherData = string.Format("    {0} = ", scratcherName.ToString()) + "{\n";
        //        for(int j=0; j < coverAssets[i].assets.Count; ++j)
        //        {
        //            int symbolCount = GetSymbolCount((BagelCode.ClientModels.ScratcherName)(i+1), j);

        //            if(symbolCount > 0)
        //            {
        //                for(int k=0; k < symbolCount; ++k)
        //                {
        //                    int coverIndex = AddCoverSymbol(coverAssets[i].assets[j], scratcherName.ToString());
        //                    scratcherData = scratcherData + string.Format("        Symbol: {0}, Cover: {1}\n", totalSymbolCount, coverIndex);
        //                    ++totalSymbolCount;
        //                }
        //            }
        //            else
        //            {
        //                AddCoverSymbol(coverAssets[i].assets[j], "");
        //            }
        //        }

        //        scratcherData = scratcherData + "    };\n";

        //        if(totalSymbolCount > 0)
        //            scratcherSymbolsMapper += scratcherData;
        //    }

        //    scratcherSymbolsMapper = scratcherSymbolsMapper + "};\n";

        //    string coverMapper = "const coverList = {\n";

        //    for(int i=0; i < customCoverAssets.Count; ++i)
        //    {
        //        coverMapper += string.Format("    Cover_{0} : ", i);

        //        if(customCoverAssets[i].useScratcherNameList.Count > 0)
        //        {
        //            for(int k=0; k < customCoverAssets[i].useScratcherNameList.Count; ++k)
        //            {
        //                if(k != 0)
        //                    coverMapper += ", ";
        //                coverMapper += customCoverAssets[i].useScratcherNameList[k];
        //            }
        //        }
        //        else
        //        {
        //            coverMapper += "New Cover";
        //        }

        //        coverMapper += "\n";
        //    }

        //    coverMapper = coverMapper + "};\n\n";

        //    var path = EditorUtility.SaveFilePanel(
        //        "Save Assets as Mapper",
        //        "Assets/Meta/Meta Game/Collecting Game/Datas",
        //        "CoverMapper" + ".txt",
        //        "txt");

        //    if (path.Length != 0)
        //    {
        //        System.IO.File.WriteAllText(path, coverMapper + scratcherSymbolsMapper);
        //    }
        //}

        //int AddCoverSymbol(ScratcherCoverAsset newCoverAsset, string scratcherName)
        //{
        //    for (int i = 0; i < customCoverAssets.Count; ++i)
        //    {
        //        if (customCoverAssets[i].sprite != newCoverAsset.sprite) continue;
        //        if (customCoverAssets[i].color != newCoverAsset.color) continue;
        //        if (customCoverAssets[i].textColor != newCoverAsset.textColor) continue;
        //        if (customCoverAssets[i].stopFrame != newCoverAsset.stopFrame) continue;

        //        if (!string.IsNullOrEmpty(scratcherName) && !customCoverAssets[i].useScratcherNameList.Contains(scratcherName))
        //        {
        //            customCoverAssets[i].useScratcherNameList.Add(scratcherName);
        //        }

        //        return i;
        //    }

        //    newCoverAsset.useScratcherNameList = new List<string>();
        //    if (!string.IsNullOrEmpty(scratcherName))
        //        newCoverAsset.useScratcherNameList.Add(scratcherName);

        //    customCoverAssets.Add(newCoverAsset);

        //    return customCoverAssets.Count - 1;
        //}

        //int GetSymbolCount(BagelCode.ClientModels.ScratcherName scratcherName, int symbolType)
        //{
        //    switch (scratcherName)
        //    {
        //        case BagelCode.ClientModels.ScratcherName.VEGAS_SEVENS:
        //        case BagelCode.ClientModels.ScratcherName.GRAND_PRIZE_HOTEL:
        //        case BagelCode.ClientModels.ScratcherName.HOCKEY_MATCH:
        //        case BagelCode.ClientModels.ScratcherName.BASEBALL_CHAMPIONS:
        //            if(symbolType == 0) return 7;
        //            break;
        //        case BagelCode.ClientModels.ScratcherName.CASINO_NIGHT:
        //        case BagelCode.ClientModels.ScratcherName.MASSIVE_20X:
        //        case BagelCode.ClientModels.ScratcherName.EXTREME_10X:
        //        case BagelCode.ClientModels.ScratcherName.SUPER_DOGGY:
        //        case BagelCode.ClientModels.ScratcherName.FOOTBALL_FEVER:
        //        case BagelCode.ClientModels.ScratcherName.MASSIVE_20X_BALL:
        //            if(symbolType == 0) return 6;
        //            if(symbolType == 1) return 12;
        //            break;
        //        case BagelCode.ClientModels.ScratcherName.VEGAS_NIGHT:
        //        case BagelCode.ClientModels.ScratcherName.FINAL_TOUCHDOWN:
        //            if(symbolType == 0) return 3;
        //            if(symbolType == 1) return 8;
        //            if(symbolType == 2) return 4;
        //            break;
        //        case BagelCode.ClientModels.ScratcherName.JOKERS_WILD_POKER:
        //        case BagelCode.ClientModels.ScratcherName.JOKERS_POKER_RED:
        //        case BagelCode.ClientModels.ScratcherName.JOKERS_POKER_GREEN:
        //            if(symbolType == 0) return 45;
        //            if(symbolType == 1) return 8;
        //            break;
        //        case BagelCode.ClientModels.ScratcherName.JOKERS_WILD_POKER_ADVANCE:
        //        case BagelCode.ClientModels.ScratcherName.JOKERS_10X_PURPLE:
        //        case BagelCode.ClientModels.ScratcherName.JOKERS_10X_GOLD:
        //            if(symbolType == 0) return 45;
        //            if(symbolType == 1) return 8;
        //            if(symbolType == 2) return 1;
        //            break;
        //        case BagelCode.ClientModels.ScratcherName.RICH_PALACE:
        //        case BagelCode.ClientModels.ScratcherName.LUCKY_PETS:
        //            if(symbolType == 0) return 12;
        //            if(symbolType == 1) return 4;
        //            break;
        //        case BagelCode.ClientModels.ScratcherName.MATCH_3_TRIPLER:
        //        case BagelCode.ClientModels.ScratcherName.CUTE_3_PAWS:
        //            if(symbolType == 0) return 6;
        //            if(symbolType == 1) return 1;
        //            break;
        //        case BagelCode.ClientModels.ScratcherName.CASH_DESERT:
        //        case BagelCode.ClientModels.ScratcherName.CATS_TOWER:
        //            if(symbolType == 0) return 3;
        //            if(symbolType == 1) return 14;
        //            if(symbolType == 2) return 4;
        //            break;
        //        case BagelCode.ClientModels.ScratcherName.WORLD_TOUR:
        //        case BagelCode.ClientModels.ScratcherName.TOP_DOG:
        //            if(symbolType == 0) return 6;
        //            if(symbolType == 1) return 12;
        //            if(symbolType == 2) return 1;
        //            break;
        //        default:
        //            break;
        //    }

        //    return 0;
        //}
#endif
    }

    [Serializable]
    public class ScratcherSymbolAsset
    {
#if UNITY_EDITOR
        [LabelWidth(80)]
        public string name;
#endif

        [LabelWidth(80)]
        public string text;
        [LabelWidth(80)]
        public string value;
        [HorizontalGroup("Sprite", 0.8f), PreviewField(40, ObjectFieldAlignment.Left), LabelWidth(80)]
        public Sprite sprite;
        [HorizontalGroup("Sprite", 0.2f), HideLabel]
        public Color color;
    }

    //[Serializable]
    //public class ScratcherCoverAssets
    //{
    //    public List<ScratcherCoverAsset> assets;
    //}

    [Serializable]
    public class ScratcherCoverAsset
    {
        //#if UNITY_EDITOR
        //[NonSerialized]
        //public List<string> useScratcherNameList;
        //#endif

        [LabelWidth(80)]
        public int id;
        [LabelWidth(80)]
        public int stopFrame;
        [HorizontalGroup("Text", 0.8f), LabelWidth(80)]
        public bool displayText;
        [ShowIf("displayText", false), HorizontalGroup("Text", 0.2f), HideLabel]
        public Color textColor;
        [HorizontalGroup("Sprite", 0.8f), PreviewField(40, ObjectFieldAlignment.Left), LabelWidth(80)]
        public Sprite sprite;
        [HorizontalGroup("Sprite", 0.2f), HideLabel]
        public Color color;
    }

    [Serializable]
    public class CustomHighlightAsset
    {
        [LabelWidth(80)]
        public int coverId;
        [PreviewField(40, ObjectFieldAlignment.Left), LabelWidth(80)]
        public Sprite sprite;
    }
}
