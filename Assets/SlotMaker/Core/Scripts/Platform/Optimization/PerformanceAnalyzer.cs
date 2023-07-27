using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class PerformanceAnalyzer : MonoWeakSingleton<PerformanceAnalyzer>
    {
    	public FPSCounter sample;

    	[Serializable]
    	public class FPSCounter
    	{
    		public float frequency = 0.5f;

    		public int[] samples = new int[12];
    		public int sampleCount;

    		public int firstFrameCount;
    		public float firstTime;

    		public int lastFrameCount;
    		public float lastTime;

    		public int beginFrameCount;
    		public float beginTime;

    		public int endFrameCount;
    		public float endTime;

    		public int lower1FPS;
    		public int lower10FPS;

    		public void Reset()
    		{
    			for (int i = 0; i < 12; ++i)
    				samples[i] = 0;
    			sampleCount = 0;
    		}

    		public void FirstSample()
    		{
    			firstFrameCount = Time.frameCount;
    			firstTime = Time.realtimeSinceStartup;
    		}

    		public void BeginSample()
    		{
    			beginFrameCount = Time.frameCount;
    			beginTime = Time.realtimeSinceStartup;
    		}

    		public void EndSample()
    		{
    			endFrameCount = Time.frameCount;
    			endTime = Time.realtimeSinceStartup;

                int fps = currentFPS;
    			int index = Mathf.Clamp(fps / 5, 0, 11);
    			++samples[index];
    			++sampleCount;
    		}

            public bool SampleReady()
            {
                return timeSpan > 0f;
            }

    		public void LastSample()
    		{
    			lastFrameCount = Time.frameCount;
    			lastTime = Time.realtimeSinceStartup;

    			lower1FPS  = -1;
    			lower10FPS = -1;

    			int lower1 = sampleCount / 100;
    			int lower10 = sampleCount / 10;
    			int acc = 0;
    			for (int i = 0; i < 12; ++i)
    			{
    				if (samples[i] > 0)
    				{
    					acc += samples[i];
    					if (lower1FPS < 0 && acc >= lower1)
    						lower1FPS = i * 5 + 5;
    					if (lower10FPS < 0 && acc >= lower10)
    						lower10FPS = i * 5 + 5;
    				}
    			}
    		}

    		public int totalFrameCount { get { return lastFrameCount - firstFrameCount; } }
    		public int frameCount { get { return endFrameCount - beginFrameCount; } }

    		public float totalTimeSpan { get { return lastTime - firstTime; } }
    		public float timeSpan { get { return endTime - beginTime; } }

    		public int averageFPS { get { return Mathf.RoundToInt(totalFrameCount / totalTimeSpan); } }
    		public int currentFPS { get { return Mathf.RoundToInt(frameCount / timeSpan); } }
    	}

    	public void BeginSample()
    	{
    		sample.Reset();
    		sample.FirstSample();
    		sample.BeginSample();

    		StartCoroutine(Sampling());
    	}

    	public void EndSample()
    	{
    		StopAllCoroutines();

            if (sample.SampleReady())
                sample.EndSample();
            
    		sample.LastSample();
    	}

    	private IEnumerator Sampling()
    	{
    		while (true)
    		{
    			yield return new WaitForSeconds(sample.frequency);

    			sample.EndSample();
    			sample.BeginSample();
    		}
    	}

        public struct TimeSample
        {
            public string category;
            public string key;
            public float value;

            public TimeSample(string category, string key)
            {
                this.category = category;
                this.key = key;
                this.value = 0f;
            }

            public void BeginSample()
            {
                value = Time.realtimeSinceStartup;
            }

            public void EndSample()
            {
                value = Time.realtimeSinceStartup - value;
                PerformanceAnalyzer.Instance.timeSamples.Add(this);
            }
        }
        public List<TimeSample> timeSamples = new List<TimeSample>();
    }
}
