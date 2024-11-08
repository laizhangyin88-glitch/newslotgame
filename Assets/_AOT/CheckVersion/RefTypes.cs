using HybridCLR;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class RefTypes
{
    private static List<string> AOTMetaAssemblyFiles { get; } = new List<string>()
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll",
    };

    /// <summary>
    /// com.code-philosophy.hybridclr插件会自动把元数据复制到
    /// {project}/HybridCLRData/AssembliesPostIl2CppStrip/{target}
    /// </summary>
    /// <returns></returns>
    public static IEnumerator LoadMetadataForAOTAssemblies()
    {
        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        /// 
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();

        foreach (var aotDllName in AOTMetaAssemblyFiles)
        {
            string metaDataPath = ApplicationSettings.GetStreamingMetaDataPath(aotDllName);
            metaDataPath += ".bytes";
            UnityWebRequest req = UnityWebRequest.Get(metaDataPath);
            //UnityWebRequest req = UnityWebRequest.Get(Application.streamingAssetsPath + "/Lib/AOT/" + aotDllName);
            yield return req.SendWebRequest();
            if (req.isDone)
            {
                Debug.Log($"加载AOT元数据：{aotDllName}");
                s_assetDatas[aotDllName] = req.downloadHandler.data;
            }
        }

        foreach (var aotDllName in AOTMetaAssemblyFiles)
        {
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(s_assetDatas[aotDllName], mode);
            if (err != LoadImageErrorCode.OK)
                Debug.LogError($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }
}
