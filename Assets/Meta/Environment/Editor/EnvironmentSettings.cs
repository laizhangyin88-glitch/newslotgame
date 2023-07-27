using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using SlotMaker;

namespace BagelCode
{
    [CreateAssetMenu(fileName="Environment Settings", menuName="Meta/Environment/Settings")]
    public class EnvironmentSettings : ScriptableObject
    {
        private static string PROJECT_PATH => Application.dataPath.Replace("Assets", "");
        private static string environmentPath = "Assets/Meta/Environment/Original";

        const string projectSettingsPath = "ProjectSettings/ProjectSettings.asset";

        public string clientVersion;
        public string apiUrl;
        public string chattingApiUrl;

        public string productName;
        public string productVersion;
        public string deeplinkUriScheme;

        public string thunbmailType;

        public string cloudProjectID;
        public string uniqueName;

        public List<EnvironmentAssets> copyAssets;

        public Dictionary_string_string eventTokenDict = new Dictionary_string_string();

        public List<UnityEngine.Object> removeObjects;

        public List<string> builtInSlotImagePattern;

        public List<EnvironmentAssets> whiteListCopyAssets;

        public string googleSpreadSheetId;
        public string tableName;

        [Button]
        public bool CheckVaildation()
        {
            bool isVaild = true;
            // vaildation copyAssets
            for(int i=0; i < copyAssets.Count; ++i)
            {
                if(copyAssets[i].sourceObj == null || copyAssets[i].destObj == null)
                {
                    string objName = "UNKNOWN";
                    if( copyAssets[i].sourceObj != null)
                        objName = copyAssets[i].sourceObj.name;
                    else if( copyAssets[i].destObj != null)
                        objName = copyAssets[i].destObj.name;

                    UnityEngine.Debug.Log( string.Format("CheckVaildation(ERROR). Copy Asset({0}) is null. index ({1})", objName, i) );
                    isVaild = false;
                }
            }

            if(isVaild)
                UnityEngine.Debug.Log( "CheckVaildation(OK)");

            return isVaild;
        }

        public void Apply(bool isBuild)
        {
            UnityEngine.Debug.Log("Start Environment Setting.");
            // AssetDatabase.StartAssetEditing();

            UpdateEnvironment(isBuild);
            PostProcess(isBuild);

            // AssetDatabase.StopAssetEditing();

            UnityEngine.Debug.Log("Change Environment is done.");
        }

        private void UpdateEnvironment(bool isBuild)
        {
            UnityEngine.Debug.Log("UpdateEnvironment.");
            UnityEditor.PlayerSettings.productName = productName;

            SetApplicationSettings(isBuild);
            SetProductSettings(isBuild);
            CopyAssets();

            AssetDatabase.SaveAssets();
            // AssetDatabase.ImportAsset(environmentPath, ImportAssetOptions.ImportRecursive);
            // AssetDatabase.Refresh();
        }

        private void PostProcess(bool isBuild)
        {
            UnityEngine.Debug.Log("PostProcess.");

            List<UnityEngine.U2D.SpriteAtlas> spriteAtlases = new List<UnityEngine.U2D.SpriteAtlas>();

            for(int i=0; i < copyAssets.Count; ++i)
            {
                var spriteAtlas = copyAssets[i].destObj as UnityEngine.U2D.SpriteAtlas;
                if(spriteAtlas != null)
                {
                    spriteAtlases.Add(spriteAtlas);
                }
                // UpdateStringTableAppName(copyAssets[i].destObj);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(environmentPath, ImportAssetOptions.ImportRecursive);
            AssetDatabase.Refresh();

            ChangeUnityCloudProject(isBuild);

            PostProcessSpriteAtlases(spriteAtlases.ToArray());
        }

        private void PostProcessSpriteAtlases(UnityEngine.U2D.SpriteAtlas[] spriteAtlases)
        {
            UnityEditor.U2D.SpriteAtlasUtility.PackAtlases(spriteAtlases, EditorUserBuildSettings.activeBuildTarget);
        }

        // private void UpdateStringTableAppName(UnityEngine.Object obj)
        // {
        //     UnityEngine.Debug.Log( string.Format("UpdateStringTableAppName : {0}", obj));

        //     if(obj == null)
        //         return;

        //     StringTableObject stringTableObj = obj as StringTableObject;
        //     if(stringTableObj == null)
        //         return;

        //     UnityEngine.Debug.Log(stringTableObj);

        //     string objPath = AssetDatabase.GetAssetPath(obj);
        //     var stringTable = File.ReadAllText(objPath);
        //     stringTable = Regex.Replace(stringTable, "{app_name}", productName, RegexOptions.None);
        //     stringTable = Regex.Replace(stringTable, "{app_name_upper}", productName.ToUpper(), RegexOptions.None);
        //     File.WriteAllText(objPath, stringTable);
        // }

        private void SetApplicationSettings(bool isBuild)
        {
            // Set Application Settings
            ApplicationSettings.Instance.clientVersion  = clientVersion;

            if(!isBuild)
            {
                ApplicationSettings.Instance.apiUrl         = apiUrl;
                ApplicationSettings.Instance.chattingApiUrl = chattingApiUrl;
            }

            EditorUtility.SetDirty(ApplicationSettings.Instance);
        }

        private void SetProductSettings(bool isBuild)
        {
            ProductSettings.Instance.productVersion = productVersion;
            ProductSettings.Instance.eventTokenDict = eventTokenDict;
            ProductSettings.Instance.deeplinkUriScheme = deeplinkUriScheme;
            ImportExtraThumbnailList(isBuild);

            EditorUtility.SetDirty(ProductSettings.Instance);
        }

        private void ImportExtraThumbnailList(bool isBuild)
        {
            string thumbnailDefaultPath = "Assets/Contents/Contents Group 1/_Slot Images/Slot Image";

            if(isBuild)
            {
                // Remove other type thunbnails.
                switch(thunbmailType)
                {
                    case "B":
                        {
                            string removePath = string.Format("{0}/Prefabs Type {1}", thumbnailDefaultPath, "A");
                            if(System.IO.Directory.Exists(removePath))
                                System.IO.Directory.Delete(removePath, true);
                        }
                        break;
                    case "A":
                    default:
                        {
                            string removePath = string.Format("{0}/Prefabs Type {1}", thumbnailDefaultPath, "B");
                            if(System.IO.Directory.Exists(removePath))
                                System.IO.Directory.Delete(removePath, true);
                        }
                        break;
                }
            }

            Dictionary_string_string thumbnailNames = new Dictionary_string_string();

            bool useRemove = isBuild && builtInSlotImagePattern.Count > 0;

#if UNITY_WEBGL || DEV_GS
            bool builtInAllFiles = true;
#else
            bool builtInAllFiles = false;
#endif
            // default slot images
            string slotImageDefaultFolderPath = string.Format("{0}/Prefabs Default", thumbnailDefaultPath);
            ProcessThunbmailImages(useRemove, false, builtInAllFiles, slotImageDefaultFolderPath, ref thumbnailNames);

            string thumbnailGroup3Path = "Assets/Contents/Contents Group 3/_Slot Images/Slot Image";
            string slotImageGroup3FolderPath = string.Format("{0}/Prefabs", thumbnailGroup3Path);
            ProcessThunbmailImages(useRemove, false, builtInAllFiles, slotImageGroup3FolderPath, ref thumbnailNames);

            // app type slot images
            string slotImageTypeFolderPath = string.Format("{0}/Prefabs Type {1}", thumbnailDefaultPath, thunbmailType);
            ProcessThunbmailImages(useRemove, true, builtInAllFiles, slotImageTypeFolderPath, ref thumbnailNames);

            ProductSettings.Instance.thumbnailNames = thumbnailNames;
        }

        private void ProcessThunbmailImages(bool useRemove, bool useTypePattern, bool builtInAllFiles, string slotImagePath, ref Dictionary_string_string thumbnailNames)
        {
            UnityEngine.Debug.LogError(slotImagePath);

            if(System.IO.Directory.Exists(slotImagePath))
            {
                var files = System.IO.Directory.GetFiles(slotImagePath, "*.prefab");

                foreach(var path in files)
                {
                    bool isContained = false;
                    string typePattern = string.Format(" {0}$", thunbmailType);

                    if(builtInAllFiles)
                    {
                        string nameWithExtra = System.IO.Path.GetFileNameWithoutExtension(path);
                        string nameWihtoutExtra = useTypePattern ? Regex.Replace(nameWithExtra, typePattern, "", RegexOptions.None) : nameWithExtra;

                        if(!thumbnailNames.ContainsKey(nameWihtoutExtra))
                            thumbnailNames[nameWihtoutExtra] = nameWithExtra;
                    }
                    else
                    {
                        foreach(var assetPattern in builtInSlotImagePattern)
                        {
                            if(Regex.IsMatch(path, assetPattern))
                            {
                                isContained = true;

                                string nameWithExtra = System.IO.Path.GetFileNameWithoutExtension(path);
                                string nameWihtoutExtra = useTypePattern ? Regex.Replace(nameWithExtra, typePattern, "", RegexOptions.None) : nameWithExtra;

                                if(!thumbnailNames.ContainsKey(nameWihtoutExtra))
                                    thumbnailNames[nameWihtoutExtra] = nameWithExtra;
                                else
                                    UnityEngine.Debug.LogError(nameWihtoutExtra + " is already name");

                                break;
                            }
                        }

                        if(useRemove && !isContained)
                        {
                            System.IO.File.Delete(path);
                            UnityEngine.Debug.LogError("Remove " + path);
                            // Remove .prefab & .meta files
                            string metaFilePath = path + ".meta";
                            if(System.IO.File.Exists(metaFilePath))
                            {
                                System.IO.File.Delete(metaFilePath);
                                UnityEngine.Debug.LogError("Remove " + metaFilePath);
                            }
                        }
                    }
                }

                foreach(var data in thumbnailNames)
                {
                    UnityEngine.Debug.Log( string.Format("{0} : {1}", data.Key, data.Value));
                }
            }
            else
            {
                UnityEngine.Debug.LogError( string.Format("Not exists folder : {0}", slotImagePath));
            }
        }

        private void ImportExtraAnimationThumbnailList(bool isBuild)
        {
            string targetPath = "Assets/Contents/Contents Group 1/_Slot Images/Slot Image/Prefabs Anim";

            Dictionary_string_string animationThumbnailNames = new Dictionary_string_string();

            if(System.IO.Directory.Exists(targetPath))
            {
                var files = System.IO.Directory.GetFiles(targetPath, "*.prefab");

                foreach(var path in files)
                {
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                    if(string.IsNullOrEmpty(fileName)) continue;

                    string assetBundleName = AssetDatabase.GetImplicitAssetBundleName(path);

                    if(!string.IsNullOrEmpty(assetBundleName))
                    {
                        if(!animationThumbnailNames.ContainsKey(fileName))
                            animationThumbnailNames[fileName] = assetBundleName;
                        else
                            UnityEngine.Debug.LogError(fileName + " is already file name");

                        UnityEngine.Debug.LogError(fileName + " : " + assetBundleName);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError(fileName + ".prefab file's assetbundle name is None");
                    }
                }
            }
            else
            {
                UnityEngine.Debug.LogError( string.Format("Not exists folder : {0}", targetPath));
            }

            ProductSettings.Instance.animationThumbnailNames = animationThumbnailNames;
        }

        private void CopyAssets()
        {
            for(int i=0; i < copyAssets.Count; ++i)
            {
                if(copyAssets[i].sourceObj == null || copyAssets[i].destObj == null)
                {
                    UnityEngine.Debug.LogError( string.Format("Copy Asset is null. index ({0})", i) );
                    continue;
                }
                string sourcePath = AssetDatabase.GetAssetPath(copyAssets[i].sourceObj);
                string destPath = AssetDatabase.GetAssetPath(copyAssets[i].destObj);

                // UnityEngine.Debug.Log( string.Format("source({0}) -> dest({1})", copyAssets[i].sourceObj.name, copyAssets[i].destObj.name) );

                File.Copy(PROJECT_PATH + sourcePath, PROJECT_PATH + destPath, true);
            }
        }

        private void ChangeUnityCloudProject(bool isBuild)
        {
            if(!isBuild) return;

            var projectSettingsString = File.ReadAllText(projectSettingsPath);

            var cloundProjectIdMatch = new Regex(@"cloudProjectId:.*").Match(projectSettingsString);
            projectSettingsString = projectSettingsString.Replace(cloundProjectIdMatch.Groups[0].Value, string.Format("cloudProjectId: {0}",cloudProjectID));

            var projectNameMatch = new Regex(@"projectName:.*").Match(projectSettingsString);
            projectSettingsString = projectSettingsString.Replace(projectNameMatch.Groups[0].Value, string.Format("projectName: {0}",productName));
            File.WriteAllText(projectSettingsPath, projectSettingsString);
            UnityEngine.Debug.Log("ChangeUnityCloudProject.");
        }

        [Button]
        public void SetAdjustTokens_CVS()
        {
            eventTokenDict = new Dictionary_string_string()
            {
                {"fb_connect", "i3eeq8"},
                {"email_connect", "klq0y8"},
                {"apple_connect", "etqmfi"},
                {"item_click", "h41neu"},
                {"level_up:02", "7becvs"},
                {"level_up:03", "m3uyd8"},
                {"level_up:05", "icbpet"},
                {"level_up:10", "qdk3fu"},
                {"level_up:20", "nr2qku"},
                {"level_up:30", "5o5obh"},
                {"level_up:40", "bm27h9"},
                {"level_up:50", "jpeh28"},
                {"level_up:60", "ett5wk"},
                {"level_up:70", "7pd34w"},
                {"level_up:80", "jcyy3g"},
                {"level_up:90", "5k3tpm"},
                {"level_up:100", "apyckg"},
                {"level_up:110", "lu0san"},
                {"level_up:120", "bu6j5h"},
                {"level_up:130", "spcrsa"},
                {"level_up:140", "82hoth"},
                {"level_up:150", "a73iql"},
                {"level_up:160", "n1btpn"},
                {"level_up:170", "7wv1tq"},
                {"level_up:180", "t9d6r2"},
                {"level_up:190", "2p2g6g"},
                {"level_up:200", "1u1c3x"},
                {"lobby", "85a4qa"},
                {"login", "tr71xi"},
                {"purchase", "4wyt6l"},
                {"purchase_1", "cut2cp"},
                {"purchase_2", "pwkvhb"},
                {"purchase_3", "p0ij2i"},
                {"purchase_4", "wh2aso"},
                {"purchase_5", "mbl15m"},
                {"purchase_6", "eb5xua"},
                {"first_purchase", "bdmr3e"},
                {"slot_enter", "43b2mc"},
                {"spin_funnel:001", "u76lbu"},
                {"spin_funnel:002", "vhr42b"},
                {"spin_funnel:003", "m32fss"},
                {"spin_funnel:005", "a5omia"},
                {"spin_funnel:010", "9au510"},
                {"spin_funnel:020", "h01ev4"},
                {"spin_funnel:050", "gdq0xt"},
                {"spin_funnel:100", "6s2ney"},
                {"store_opened", "hpqrym"},
                {"tier_up:01", "fv0gcx"},
                {"tier_up:02", "8mvzbh"},
                {"tier_up:03", "pzyzk6"},
                {"tier_up:04", "u9yfsn"},
                {"tier_up:05", "iztj0c"},
                {"tier_up:06", "bjmy03"},
                {"tier_up:07", "5uv5hk"},
                {"tier_up:08", "ku5oq9"},
                {"tier_up:09", "u1iys7"},
                {"tier_up:10", "3v0qrk"}
            };

            EditorUtility.SetDirty(this);
        }

        [Button]
        public void SetAdjustTokens_CBN()
        {
            eventTokenDict = new Dictionary_string_string()
            {
                {"apple_connect", "4kdcf1"},
                {"email_connect", "izrpck"},
                {"fb_connect", "6jj41e"},
                {"first_purchase", "cw1hc9"},
                {"item_click", "af7lev"},
                {"level_up:02", "h9a623"},
                {"level_up:03", "ds7yyf"},
                {"level_up:05", "i6d880"},
                {"level_up:10", "se3ht6"},
                {"level_up:20", "sbh83l"},
                {"level_up:30", "5h1eh0"},
                {"level_up:40", "wx067o"},
                {"level_up:50", "ulsgj4"},
                {"level_up:60", "n1q1gg"},
                {"level_up:70", "3dt6no"},
                {"level_up:80", "o90svs"},
                {"level_up:90", "u5tqxx"},
                {"level_up:100", "2anf4n"},
                {"level_up:110", "20grx7"},
                {"level_up:120", "n7wp03"},
                {"level_up:130", "v5jrl2"},
                {"level_up:140", "boj5ci"},
                {"level_up:150", "lks8p0"},
                {"level_up:160", "1g4rdg"},
                {"level_up:170", "el21x0"},
                {"level_up:180", "kaqrb6"},
                {"level_up:190", "hbp7sh"},
                {"level_up:200", "w1kg4w"},
                {"level_up:210", "vydhuf"},
                {"level_up:220", "ej127i"},
                {"level_up:230", "2355fr"},
                {"level_up:240", "l1w7s2"},
                {"level_up:250", "wfhbu3"},
                {"level_up:260", "w3b82a"},
                {"level_up:270", "c9qrp1"},
                {"level_up:280", "xskicg"},
                {"level_up:290", "erpws7"},
                {"level_up:300", "t2hmja"},
                {"lobby", "a0aqeg"},
                {"login", "q1d5s7"},
                {"purchase", "onzzys"},
                {"purchase_1", "t0u33a"},
                {"purchase_2", "lnx9vm"},
                {"purchase_3", "npat9w"},
                {"purchase_4", "wjduz4"},
                {"purchase_5", "hunhxx"},
                {"purchase_6", "rlzcgv"},
                {"slot_enter", "tjwktu"},
                {"spin_funnel:001", "9mi796"},
                {"spin_funnel:002", "ql4rzk"},
                {"spin_funnel:003", "auy7v3"},
                {"spin_funnel:005", "sdmm33"},
                {"spin_funnel:010", "s5j0s1"},
                {"spin_funnel:020", "34m1z2"},
                {"spin_funnel:050", "1hnmte"},
                {"spin_funnel:100", "elpce9"},
                {"store_opened", "aze6i4"},
                {"tier_up:01", "26qydx"},
                {"tier_up:02", "8aujbc"},
                {"tier_up:03", "zdi7si"},
                {"tier_up:04", "ytfml1"},
                {"tier_up:05", "fvsxa9"},
                {"tier_up:06", "429oe9"},
                {"tier_up:07", "oxnx4w"},
                {"tier_up:08", "b2m4sk"},
                {"tier_up:09", "55fde1"},
                {"tier_up:10", "p0k393"}
            };

            EditorUtility.SetDirty(this);
        }

        [Button]
        public void SetAdjustTokens_CSW()
        {
            eventTokenDict = new Dictionary_string_string()
            {
                {"apple_connect", "3wiz7n"},
                {"email_connect", "sw1twb"},
                {"fb_connect", "7v9vh7"},
                {"first_purchase", "b2i9kr"},
                {"item_click", "rjae2q"},
                {"level_up:02", "sy3kj9"},
                {"level_up:03", "5r1i59"},
                {"level_up:05", "f18ty7"},
                {"level_up:10", "iuir4e"},
                {"level_up:20", "b7ogre"},
                {"level_up:30", "flntr0"},
                {"level_up:40", "cms201"},
                {"level_up:50", "5mrvkl"},
                {"level_up:60", "eirl74"},
                {"level_up:70", "boo6it"},
                {"level_up:80", "wrhkrs"},
                {"level_up:90", "w4u6sa"},
                {"level_up:100", "gz0ptb"},
                {"level_up:110", "830x5v"},
                {"level_up:120", "q773uk"},
                {"level_up:130", "so48q1"},
                {"level_up:140", "py1atb"},
                {"level_up:150", "7cl3a4"},
                {"level_up:160", "gwz5fx"},
                {"level_up:170", "qga0wp"},
                {"level_up:180", "6nlt6m"},
                {"level_up:190", "wqrgk1"},
                {"level_up:200", "9v1x3f"},
                {"lobby", "uqsndn"},
                {"login", "3ywojj"},
                {"purchase", "nwe4yg"},
                {"purchase_1", "pdio33"},
                {"purchase_2", "txl8ke"},
                {"purchase_3", "pipea0"},
                {"purchase_4", "ylw8kq"},
                {"purchase_5", "z1famu"},
                {"purchase_6", "i83s3j"},
                {"slot_enter", "5d59iq"},
                {"spin_funnel:001", "fnh1k5"},
                {"spin_funnel:002", "aumhvx"},
                {"spin_funnel:003", "oc1qug"},
                {"spin_funnel:005", "ch0vtt"},
                {"spin_funnel:010", "chpghm"},
                {"spin_funnel:020", "b665cq"},
                {"spin_funnel:050", "7ih6g1"},
                {"spin_funnel:100", "awi87g"},
                {"store_opened", "j3ck4y"},
                {"tier_up:01", "9ghboj"},
                {"tier_up:02", "t9p1ie"},
                {"tier_up:03", "dqk312"},
                {"tier_up:04", "7xkud2"},
                {"tier_up:05", "fsifbb"},
                {"tier_up:06", "vdg2n8"},
                {"tier_up:07", "ivi04s"},
                {"tier_up:08", "73qbq9"},
                {"tier_up:09", "27j27c"},
                {"tier_up:10", "q8yf94"}
            };

            EditorUtility.SetDirty(this);
        }

        [Button]
        public void ImportExtraThumbnails()
        {
            ImportExtraThumbnailList(false);

            EditorUtility.SetDirty(ProductSettings.Instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Button]
        public void ImportExtraAnimationThumbnails()
        {
            ImportExtraAnimationThumbnailList(false);

            EditorUtility.SetDirty(ProductSettings.Instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private GoogleSheet googleSheet;

        [Button]
        public void ImportBuiltInSlotImagePatterns()
        {
            if(string.IsNullOrEmpty(googleSpreadSheetId))
            {
                UnityEngine.Debug.Log(string.Format("{0} file. Insert Google Spread Sheet ID!!!", name));
                return;
            }

            if(string.IsNullOrEmpty(tableName))
            {
                UnityEngine.Debug.Log(string.Format("{0} file. Insert Google Sheet Table Name!!!", name));
                return;
            }

            googleSheet = new GoogleSheet();
            googleSheet.webServiceUrl = SlotMaker.EditorSettings.Instance.googleWebServiceUrl;
            googleSheet.servicePassword = SlotMaker.EditorSettings.Instance.googleWebServicePassword;
            googleSheet.spreadsheetId = googleSpreadSheetId;
            googleSheet.processedResponseCallback.AddListener(Imported);
            googleSheet.GetTable(tableName);
        }

        [Button]
        public void RepairEnvironmentMissMatch()
        {
#if UNITY_EDITOR
            int changedFileCount = 0;
            string strOutput = "";

            UnityEngine.Debug.Log("======Repair file ID miss match.=======");

            for(int i=0; i < copyAssets.Count; ++i)
            {

                if(copyAssets[i].sourceObj == null && copyAssets[i].destObj)
                {
                    UnityEngine.Debug.LogError("You must check null objects");
                }
                else if(copyAssets[i].sourceObj == null)
                {
                    UnityEngine.Debug.LogError( string.Format("Source Object is null. (dest {0})", copyAssets[i].destObj.name) );
                }
                else if(copyAssets[i].destObj == null)
                {
                    UnityEngine.Debug.LogError( string.Format("Dest Object is null. (source {0})", copyAssets[i].sourceObj.name) );
                }
                else
                {
                    var objType = PrefabUtility.GetPrefabType(copyAssets[i].sourceObj);
                    if(PrefabType.None == objType) continue;

                    string sourceObjGUID = "";
                    long sourceObjFileID = 0L;
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(copyAssets[i].sourceObj, out sourceObjGUID, out sourceObjFileID);

                    string destObjGUID = "";
                    long destObjFileID = 0L;
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(copyAssets[i].destObj, out destObjGUID, out destObjFileID);

                    if(sourceObjFileID != destObjFileID)
                    {
                        UnityEngine.Debug.Log( string.Format("{0}({1})\n{2}({3})", copyAssets[i].sourceObj.name, sourceObjFileID, copyAssets[i].destObj.name, destObjFileID) );

                        string sourcePath = AssetDatabase.GUIDToAssetPath(sourceObjGUID);
                        string destPath = AssetDatabase.GUIDToAssetPath(destObjGUID);

                        strOutput += RunRepairScript(sourcePath, sourceObjFileID, destObjFileID);

                        // Re link
                        AssetDatabase.SaveAssets();
                        AssetDatabase.ImportAsset(sourcePath, ImportAssetOptions.ImportRecursive);

                        GameObject sourceObject = (GameObject)AssetDatabase.LoadAssetAtPath(sourcePath, typeof(GameObject));
                        copyAssets[i].sourceObj = sourceObject;

                        ++changedFileCount;
                    }
                }
            }

            if(changedFileCount > 0)
            {
                UnityEngine.Debug.Log( string.Format("Fixed {0} prefab files. You should check (repaire files.txt) file.",changedFileCount) );
                UnityEngine.Debug.Log(strOutput);
                File.WriteAllText(PROJECT_PATH + "/repaire files.txt", strOutput);

                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                UnityEngine.Debug.Log("======Miss match file not found.=======");
            }
#endif
        }

        [Button]
        private void CopyAssetButton()
        {
            string baseFilePath = Application.dataPath + "/Meta/Environment/";
            List<CopyAssetElement> sourceList = new List<CopyAssetElement>();
            List<CopyAssetElement> destList = new List<CopyAssetElement>();
            string[] destDirs = Directory.GetDirectories(baseFilePath + "Original");
            string[] sourceDirs = Directory.GetDirectories(baseFilePath + uniqueName);

            GetFiles(destDirs, ref destList);
            GetFiles(sourceDirs, ref sourceList);
            List<CheckAssetItem> checkAssetItemList = CreateAssetItemList(sourceList, destList);
            EditorUtility.SetDirty(this);

            EnvironmentSettingsWindowsEditor.ShowWindow(this, checkAssetItemList, whiteListCopyAssets);
        }

        // target path prefab file. changed fileid(originalFileID -> changeFileID)
        public string RunRepairScript(string targetPath, long originalFileID, long changeFileID)
        {
            string shFilePath = PROJECT_PATH + "Assets/Meta/Environment/Editor/repair_file_id_miss_match.sh";
            string arguments = string.Format("{0} \'{1}\' \'{2}\' \'{3}\'", shFilePath, targetPath, originalFileID, changeFileID);

            UnityEngine.Debug.Log(arguments);

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "/bin/sh";
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.Arguments = arguments;

            Process p = Process.Start(psi);
            string strOutput = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();

            return strOutput;
        }

        public class EnvironmentGoogleSheetData
        {
            public string title;
        }

        private void Imported(GoogleSheet.QueryType query, List<string> objTypeNames, List<string> jsonData)
        {
            builtInSlotImagePattern.Clear();
            foreach (var json in jsonData)
            {
                var dataList = SlotMaker.Json.SlotSimpleJson.DeserializeObject<List<EnvironmentGoogleSheetData>>(json);
                foreach(var data in dataList)
                {
                    if( string.IsNullOrEmpty(data.title) )
                        UnityEngine.Debug.LogError("You must check environment google sheet datas!");
                    else
                        builtInSlotImagePattern.Add(data.title);
                }
            }
        }

        private void GetFiles(string[] dirs, ref List<CopyAssetElement> objectList)
        {
            if (dirs == null) return;

            foreach (string dirPath in dirs)
            {
                string[] dirsArray = Directory.GetDirectories(dirPath);
                GetFiles(dirsArray, ref objectList);
                string[] files = Directory.GetFiles(dirPath);
                if (files != null)
                {
                    foreach (string file in files)
                    {
                        string extension = Path.GetExtension(file);
                        string fileName = FileUtil.GetProjectRelativePath(file);
                        switch (extension)
                        {
                            case ".prefab":
                            case ".asset":
                            case ".png":
                            case ".wav":
                                UnityEngine.Object temp = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(fileName);
                                CopyAssetElement element = new CopyAssetElement();
                                element.obj = temp;
                                element.FileFullName = fileName;
                                objectList.Add(element);
                                break;
                        }
                    }
                }
            }
        }

        private List<CheckAssetItem> CreateAssetItemList(List<CopyAssetElement> sourceList, List<CopyAssetElement> destList)
        {
            List<CheckAssetItem> itemList = new List<CheckAssetItem>();

            if (destList != null && sourceList != null)
            {
                for (int i = destList.Count - 1; i >= 0; --i)
                {
                    CopyAssetElement sourceItem = sourceList.Find(
                        x =>
                        x.GetFileName().ToLower().Trim() ==
                        string.Format("{0}({1}){2}", Path.GetFileNameWithoutExtension(destList[i].GetFileName()), uniqueName, Path.GetExtension(destList[i].GetFileName())).ToLower().Trim());

                    if (sourceItem != null)
                    {
                        CheckAssetItem item = new CheckAssetItem(true, sourceItem, destList[i]);
                        destList.RemoveAt(i);
                        sourceList.Remove(sourceItem);
                        itemList.Add(item);
                    }
                }
            }
            if (destList != null)
            {
                for (int i = 0; i < destList.Count; ++i)
                    itemList.Add(new CheckAssetItem(false, null, destList[i]));
            }
            if (sourceList != null)
            {
                for (int i = 0; i < sourceList.Count; ++i)
                    itemList.Add(new CheckAssetItem(false, sourceList[i], null));
            }

            return itemList;
        }

        public void ApplyCopyAssetWithEditorWindow(List<CheckAssetItem> checkAssetItemList, List<EnvironmentAssets> _whiteListCopyAssets)
        {
            if (checkAssetItemList != null && checkAssetItemList.Count > 0)
            {
                copyAssets.Clear();
                for (int i = 0; i < checkAssetItemList.Count; ++i)
                {
                    if (checkAssetItemList[i].enabled && checkAssetItemList[i].destElement.obj != null && checkAssetItemList[i].sourceElement.obj != null)
                    {
                        copyAssets.Add(new EnvironmentAssets()
                        {
                            destObj = checkAssetItemList[i].destElement.obj,
                            sourceObj = checkAssetItemList[i].sourceElement.obj
                        });
                    }
                }
            }
            whiteListCopyAssets = _whiteListCopyAssets;
            EditorUtility.SetDirty(this);
        }
    }

    [Serializable]
    public class EnvironmentAssets
    {
        public UnityEngine.Object sourceObj;
        public UnityEngine.Object destObj;
    }

    [Serializable]
    public class CopyAssetElement
    {
        public UnityEngine.Object obj;

        private string fileFullName;
        private string filePath;
        private string fileName;
        public string FileFullName
        {
            set
            {
                fileFullName = value;
                fileName = Path.GetFileName(value);
                filePath = Path.GetDirectoryName(value).Substring("Assets/Meta/Environment/".Length);
            }
        }

        public string GetPath()
        {
            return filePath;
        }

        public string GetFileName()
        {
            return fileName;
        }
    }

    [System.Serializable]
    public class CheckAssetItem
    {
        public bool enabled;
        public CopyAssetElement sourceElement;
        public CopyAssetElement destElement;

        public CheckAssetItem(bool _enabled, CopyAssetElement source, CopyAssetElement dest)
        {
            enabled = _enabled;
            sourceElement = source != null ? source : new CopyAssetElement();
            destElement = dest != null ? dest : new CopyAssetElement();
        }
    }
}
