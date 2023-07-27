using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Diagnostics;

namespace BagelCode
{
    [CreateAssetMenu(fileName="EnvironmentSettingsHelper", menuName="Meta/Environment/EnvironmentSettingsHelper")]
    public class EnvironmentSettingsHelper : ScriptableObject
    {
        private static string PROJECT_PATH => Application.dataPath.Replace("Assets", "");

        public Object sourceFolder;
        public string sourceProjectName;
        public string copyProjectName;

        [Button]
        public void Copy()
        {
            if(sourceFolder == null)
            {
                UnityEngine.Debug.LogError("Insert sourceFolder");
                return;
            }

            if(string.IsNullOrEmpty(sourceProjectName))
            {
                UnityEngine.Debug.LogError("Insert sourceProjectName");
                return;
            }

            if(string.IsNullOrEmpty(copyProjectName))
            {
                UnityEngine.Debug.LogError("Insert copyProjectName");
                return;
            }

            string srcPath = AssetDatabase.GetAssetPath(sourceFolder);
            string dstPath = srcPath.Replace(sourceFolder.name, "") + copyProjectName;
            if (AssetDatabase.IsValidFolder(dstPath))
            {
                UnityEngine.Debug.LogError( string.Format("Exist dstPath. change copyProjectName"));
                return;
            }

            UnityEngine.Debug.Log(string.Format( "Copy Assets) {0} -> {1}", srcPath, dstPath));
            if (!AssetDatabase.IsValidFolder(srcPath))
            {
                UnityEngine.Debug.LogError(srcPath + "is not folder");
                return;
            }

            AssetDatabase.StartAssetEditing();

            FileUtil.CopyFileOrDirectory(srcPath, dstPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            AssetDatabase.StopAssetEditing();
        }

        [Button]
        public void Migration()
        {
            if(sourceFolder == null)
            {
                UnityEngine.Debug.LogError("Insert sourceFolder");
                return;
            }

            if(string.IsNullOrEmpty(sourceProjectName))
            {
                UnityEngine.Debug.LogError("Insert sourceProjectName");
                return;
            }

            if(string.IsNullOrEmpty(copyProjectName))
            {
                UnityEngine.Debug.LogError("Insert copyProjectName");
                return;
            }

            string srcPath = AssetDatabase.GetAssetPath(sourceFolder);
            string dstPath = srcPath.Replace(sourceFolder.name, "") + copyProjectName;

            if (!AssetDatabase.IsValidFolder(srcPath) || !AssetDatabase.IsValidFolder(dstPath))
            {
                UnityEngine.Debug.LogError(srcPath + "is not folder");
                return;
            }
            UnityEngine.Debug.Log(string.Format( "Migration GUID) {0} -> {1}", srcPath, dstPath));

            AssetDatabase.StartAssetEditing();

            string copyFullPath = PROJECT_PATH + srcPath;
            string shFilePath = PROJECT_PATH + "/environment_migration.sh";
            string arg1 = srcPath;
            string arg2 = dstPath;
            string arg3 = sourceProjectName;
            string arg4 = copyProjectName;
            string arguments = string.Format("{0} {1} {2} {3} {4}", shFilePath, arg1, arg2, arg3, arg4);

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
            UnityEngine.Debug.Log(strOutput);
            File.WriteAllText(PROJECT_PATH + "/migration_logs.txt", strOutput);

            AssetDatabase.StopAssetEditing();

            UnityEngine.Debug.Log("Done");
        }

        // [Button]
        // public void RepairEnvironmentMissMatch()
        // {
        //     string environmentSettingsFilePath = PROJECT_PATH + "/Assets/Meta/Environments/SLOTS3/EnvironmentSettings.asset";
        // }
    }
}
