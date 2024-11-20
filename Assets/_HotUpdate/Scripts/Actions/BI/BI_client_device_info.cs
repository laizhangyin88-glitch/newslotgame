using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_device_info : ActionTask<Blackboard>
{
    protected override void OnExecute()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["device_model"]       = SystemInfo.deviceModel;
        customData["device_type"]        = SystemInfo.deviceType.ToString();
        customData["os_version"]         = SystemInfo.operatingSystem;
        customData["cpu_count"]          = SystemInfo.processorCount;
        customData["cpu_frequency"]      = SystemInfo.processorFrequency;
        customData["cpu_type"]           = SystemInfo.processorType;
        customData["memory"]             = SystemInfo.systemMemorySize;
        customData["gpu_type"]           = SystemInfo.graphicsDeviceType.ToString();
        customData["gpu_vendor"]         = SystemInfo.graphicsDeviceVendor;
        customData["gpu_version"]        = SystemInfo.graphicsDeviceVersion;
        customData["gpu_memory"]         = SystemInfo.graphicsMemorySize;
        customData["gpu_multi_threaded"] = SystemInfo.graphicsMultiThreaded;
        customData["gpu_shader_level"]   = SystemInfo.graphicsShaderLevel;
        // customData["image_effect"]       = SystemInfo.supportsImageEffects;
        customData["screen_width"]       = Screen.width;
        customData["screen_height"]      = Screen.height;
        customData["device_push_setting"] = BlackboardQueryUtils.GetDevicePushSetting();
        Analytics.CustomEvent("client_device_info", customData);

        EndAction();
    }
}

}
