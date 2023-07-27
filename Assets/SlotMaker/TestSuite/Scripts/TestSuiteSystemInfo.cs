using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlotMaker.TestSuite
{
    public class TestSuiteSystemInfo : MonoBehaviour
    {
        public TextMeshProUGUI logText;
        public TextMeshProUGUI screenText;

#if DEV
        private FrameTiming[] frameTimings = new FrameTiming[3];

        private float maxResolutionWidthScale = 1.0f;
        private float maxResolutionHeightScale = 1.0f;
        private float minResolutionWidthScale = 0.5f;
        private float minResolutionHeightScale = 0.5f;
        private float scaleWidthIncrement = 0.1f;
        private float scaleHeightIncrement = 0.1f;

        private float widthScale = 1.0f;
        private float heightScale = 1.0f;

        private uint frameCount = 0;

        const uint kNumFrameTimings = 2;

        double gpuFrameTime;
        double cpuFrameTime;

        private void Awake()
        {
            var sb = new StringBuilder();
            sb.Append("Device Model: " + SystemInfo.deviceModel);
            sb.Append("\nDevice Name: " + SystemInfo.deviceName);
            sb.Append("\nDevice Type: " + SystemInfo.deviceType);
            sb.Append("\nOS: " + SystemInfo.operatingSystem);
            sb.Append("\nOS Family: " + SystemInfo.operatingSystemFamily);
            sb.Append("\nCPU Count: " + SystemInfo.processorCount);
            sb.Append("\nCPU Frequency: " + SystemInfo.processorFrequency);
            sb.Append("\nCPU Type: " + SystemInfo.processorType);
            sb.Append("\nMemory: " + SystemInfo.systemMemorySize);
            sb.Append("\nGPU Type: " + SystemInfo.graphicsDeviceType);
            sb.Append("\nGPU Vendor: " + SystemInfo.graphicsDeviceVendor);
            sb.Append("\nGPU Version: " + SystemInfo.graphicsDeviceVersion);
            sb.Append("\nGPU Memory: " + SystemInfo.graphicsMemorySize);
            sb.Append("\nGPU Multi-Threaded: " + SystemInfo.graphicsMultiThreaded);
            sb.Append("\nGPU Shader Level: " + SystemInfo.graphicsShaderLevel);
            logText.text = sb.ToString();
        }

        private void Update()
        {
            DetermineResolution();

            int rezWidth = (int)Mathf.Ceil(ScalableBufferManager.widthScaleFactor * Screen.currentResolution.width);
            int rezHeight = (int)Mathf.Ceil(ScalableBufferManager.heightScaleFactor * Screen.currentResolution.height);
            screenText.text = string.Format("Scale: {0:F3}x{1:F3}\nResolution: {2}x{3}\nScaleFactor: {4:F3}x{5:F3}\nGPU: {6:F3}\nCPU: {7:F3}", 
                widthScale, heightScale, rezWidth, rezHeight, 
                ScalableBufferManager.widthScaleFactor,
                ScalableBufferManager.heightScaleFactor,
                gpuFrameTime,
                cpuFrameTime);
        }

        private void DetermineResolution()
        {
            ++frameCount;
            if (frameCount <= kNumFrameTimings)
                return;

            FrameTimingManager.CaptureFrameTimings();
            FrameTimingManager.GetLatestTimings(kNumFrameTimings, frameTimings);
            if (frameTimings.Length < kNumFrameTimings)
                return;

            gpuFrameTime = (double)frameTimings[0].gpuFrameTime;
            cpuFrameTime = (double)frameTimings[0].cpuFrameTime;
        }
#endif

        public void Decrease()
        {
#if DEV
            widthScale = Mathf.Max(minResolutionWidthScale, widthScale - scaleWidthIncrement);
            heightScale = Mathf.Max(minResolutionHeightScale, heightScale - scaleHeightIncrement);

            ScalableBufferManager.ResizeBuffers(widthScale, heightScale);
#endif
        }

        public void Increase()
        {
#if DEV
            widthScale = Mathf.Min(maxResolutionWidthScale, widthScale + scaleWidthIncrement);
            heightScale = Mathf.Min(maxResolutionHeightScale, heightScale + scaleHeightIncrement);

            ScalableBufferManager.ResizeBuffers(widthScale, heightScale);
#endif
        }

        public void Close()
        {
            GameObject.Destroy(gameObject);
        }
    }
}
