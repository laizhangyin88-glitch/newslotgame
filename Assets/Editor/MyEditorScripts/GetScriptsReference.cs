//using System.Collections.Generic;
//using System.IO;
//using UnityEditor;
//using UnityEngine;

//public class GetScriptsReference
//{
//    [MenuItem("Assets/Tools/GetFileReference")]
//    static void GetFileReference()
//    {
//        string target = "";
//        if (Selection.activeObject != null)
//        {
//            target = AssetDatabase.GetAssetPath(Selection.activeObject);
//        }
//        if (string.IsNullOrEmpty(target))
//        {
//            return;
//        }
//        string[] files = Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories);
//        string[] scene = Directory.GetFiles(Application.dataPath, "*unity", SearchOption.AllDirectories);

//        List<Object> fileList = new List<Object>();
//        for (int i = 0; i < files.Length; i++)
//        {
//            string[] source = AssetDatabase.GetDependencies(new string[] { files[i].Replace(Application.dataPath, "Assets") });
//            for (int j = 0; j < source.Length; j++)
//            {
//                if (source[j] == target)
//                {
//                    fileList.Add(AssetDatabase.LoadMainAssetAtPath(files[i].Replace(Application.dataPath, "Assets")));
//                }
//            }
//        }
//        for (int i = 0; i < scene.Length; i++)
//        {
//            string[] source = AssetDatabase.GetDependencies(new string[] { scene[i].Replace(Application.dataPath, "Assets") });
//            for (int j = 0; j < source.Length; j++)
//            {
//                if (source[j] == target)
//                {
//                    fileList.Add(AssetDatabase.LoadMainAssetAtPath(scene[i].Replace(Application.dataPath, "Assets")));
//                }
//            }
//        }
//        Selection.objects = fileList.ToArray();
//    }
//}
