using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using System.Timers;
using BagelCode;

namespace SlotMaker
{
    [AddComponentMenu("SlotMaker/Sound/Manager")]
    public class GSManager : MonoWeakSingleton<GSManager>
    {
        public AudioMixer masterMixer;
        public AudioMixer musicMixer;
        public AudioMixer sfxMixer;

        // Meta
        public AudioMixer metaMusicMixer;
        //

        public Blackboard mixerGroups;
        public Blackboard snapshots;
        public Blackboard easeCurves;
        public List<Blackboard> handlers;
        public ObjectPool pool; // 每个pool的子对象有GSSource组件
         
        public float clipLifeTime = 60f;
        public const float volumeOfMute = -40;


        private bool isRun = false;
        private Queue<Action> taskQueue = new Queue<Action>();
        private void Update()
        {
            if (!isRun)
            {
                isRun = true;
                while (taskQueue.Count > 0)
                {
                    var task = taskQueue.Dequeue();
                    task.Invoke();
                }
                isRun = false;
            }
        }


        protected void Awake()
        {
            MusicVolume = (float)PlayerPrefs.GetFloat("MUTE_MUSIC", 1);
            SfxVolume = (float)PlayerPrefs.GetFloat("MUTE_SFX", 1);

            StartCoroutine(ClearUnusedClipsCoroutine());

            ReturnUnuseGSSToPool();

            MessageDispatcher.Register("OnFgBg", tmp_FgBg);
        }

        protected override void OnDestroy()
        {
            MessageDispatcher.UnRegister("OnFgBg", tmp_FgBg);

            if (clearUnuseTimer != null)
            {
                clearUnuseTimer.Stop();
                clearUnuseTimer.Dispose();
                clearUnuseTimer = null;
            }

            base.OnDestroy();
        }


        /// <summary>切换前后台时，重新创建AudioListener</summary>
        private void tmp_FgBg(ParadoxNotion.EventData eventData)
        {
            if (eventData.name == "FgBg")
            {

                /* GameObject mainFSMObject = SlotMaker.ScreenCapture.Instance.gameObject;
                 MetaStreamingSoundController streamingSoundController = null;
                 if (mainFSMObject == null)
                 {
                     streamingSoundController = mainFSMObject.GetComponentInChildren<MetaStreamingSoundController>();
                 }*/

                GameObject taggedObjects = GameObject.Find("Main FSM");
                MetaStreamingSoundController streamingSoundController = taggedObjects.transform.Find("Sound View").GetComponent<MetaStreamingSoundController>();

                if (streamingSoundController == null)
                {
                    Debug.LogError("streamingSoundController is null !");
                }


                Debug.Log($"@GSManager 收到前后台切换信息：isFg = {eventData.value}");
                if ((bool)(eventData.value ?? false) == true)  //前台
                {
                    long nowTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    long Dtime = 0;
                    if (time != -1)
                    {
                        Dtime = nowTime - time;
                        time = -1;
                    }
                    //if (Dtime > 1000) { }

                    if (clearAllTimer != null)
                    {
                        clearAllTimer.Stop();
                        clearAllTimer.Dispose();
                        clearAllTimer = null;
                    }
                    clearAllTimer = new System.Timers.Timer(10000);
                    clearAllTimer.AutoReset = false; // 是否重复执行
                    clearAllTimer.Elapsed += (object sender, ElapsedEventArgs e) => {
                        taskQueue.Enqueue(() =>
                        {
                            StartCoroutine(ResetAudioListener());
                            Debug.Log($"@【FgBg】 复位声音池");

                            for (var i = 0; i < pool.transform.childCount; i++)
                            {
                                GSSource gss = pool.transform.GetChild(i).GetComponent<GSSource>();
                                if(gss.Handler != null)
                                    gss.Clear();
                            }
                            pool.ResetPool();

                            streamingSoundController?.FgBgReLoadBGM();
                        });
                    };
                    clearAllTimer.Start();


                    Debug.Log($"@GSManager 前后台切换时间差 = {Dtime}");
                }
                else  //后台
                {
                    //streamingSoundController?.FgBgSetBGMState();
                    time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }
            }
        }


#if UNITY_EDITOR

        List<string> _testHandlerIds = new List<string>();
        [Button]
        public void test_showGSHandleUseInfo()
        {
            for (int j = 0; j< _testHandlerIds.Count; j++) { 
                for (int i = 0; i < handlers.Count; ++i)
                {
                    var variable = handlers[i].GetVariable<GSHandler>(_testHandlerIds[j]);
                    if (variable != null && variable.value.UseCount > 0)
                        Debug.LogWarning($"BB Sound Key = {_testHandlerIds[j]} ;  UseCount = {variable.value.UseCount}");
                }
            }
        }
#endif

        [Button]
        public void test_FgBgResetPool()
        {

            /* GameObject mainFSMObject = SlotMaker.ScreenCapture.Instance.gameObject;
             MetaStreamingSoundController streamingSoundController = null;
             if (mainFSMObject == null)
             {
                 streamingSoundController = mainFSMObject.GetComponentInChildren<MetaStreamingSoundController>();
             }*/


            GameObject taggedObjects = GameObject.Find("Main FSM");
            MetaStreamingSoundController streamingSoundController = taggedObjects.transform.Find("Sound View").GetComponent<MetaStreamingSoundController>();

            if (streamingSoundController == null)
            {
                Debug.LogError("streamingSoundController is null !");
            }
            //streamingSoundController?.FgBgSetBGMState();

            taskQueue.Enqueue(() =>
            {
                StartCoroutine(ResetAudioListener());
                Debug.Log($"@【FgBg】 复位声音池");

                for (var i = 0; i < pool.transform.childCount; i++)
                {
                    GSSource gss = pool.transform.GetChild(i).GetComponent<GSSource>();
                    if (gss.Handler != null)
                        gss.Clear();
                }
                pool.ResetPool();

                streamingSoundController?.FgBgReLoadBGM();
            });

        }


        private long time = -1;
        private Timer clearUnuseTimer = null;
        private Timer clearAllTimer = null;
        private bool isClearUnuseGSS = true;


        [Button]
        private void temp_ChangeAutoClearGSS()
        {
            isClearUnuseGSS = !isClearUnuseGSS;
            ReturnUnuseGSSToPool();
        }


        /// <summary>定时删除不用的音效资源</summary>
        private void ReturnUnuseGSSToPool()
        {
            if (clearUnuseTimer != null)
            {
                clearUnuseTimer.Stop();
                clearUnuseTimer.Dispose();
                clearUnuseTimer = null;
            }
            if (isClearUnuseGSS)
            {
                Debug.Log("@enable Clear Unuse GSS");
                clearUnuseTimer = new System.Timers.Timer(15000);
                clearUnuseTimer.AutoReset = true; // 是否重复执行
                clearUnuseTimer.Elapsed += (object sender, ElapsedEventArgs e) => {
                    taskQueue.Enqueue(() =>
                    {
                        tmp_ClearUnuseGSSource();
                    });
                };
                this.clearUnuseTimer.Start();
            }
            else
            {
                Debug.Log("@disable Clear Unuse GSS");
            }
        }


        [Button]
        public void tmp_ClearUnuseGSSource()
        {
            //Debug.Log("@Clear Unuse GSS");
            long nowTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            //List<GSSource> gsss= new List<GSSource>();

            Dictionary<string, List<GSSource>> dicGass = new Dictionary<string, List<GSSource>>();
            for (var i = 0; i < pool.transform.childCount; i++)
            {
                GSSource gss = pool.transform.GetChild(i).GetComponent<GSSource>();
                if (gss.gameObject.active == true)
                {
                    string key = $"{gss.Handler.clip.bundleName}-{gss.Handler.clip.assetName}";
                    //Debug.Log(key);
                    if (!dicGass.ContainsKey(key))
                    {
                        dicGass.Add(key, new List<GSSource>());
                    }
                    dicGass[key].Add(gss);
                }
            }
            foreach (var kv in dicGass)
            {
                if (kv.Value.Count > 1)
                {
                    kv.Value.Sort((a, b) => (int)(a.startUseTime - b.startUseTime));  //越大排越后

                    //Debug.Log($"{kv.Key} 0 - {kv.Value[0].startUseTime}");

                    for (int i = 1; i < kv.Value.Count; i++)
                    {
                        GSSource gss = kv.Value[i];
                        if (nowTime - gss.startUseTime > 8000)
                        {
                            //Debug.Log($"@clear: {kv.Key} {i} - {kv.Value[i].startUseTime}");
                            gss.Clear();
                        }
                    }
                }
            }
        }

        IEnumerator ResetAudioListener()
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("MainCamera");
            foreach (GameObject obj in taggedObjects)
            {
                Debug.Log($"@【FgBg】 重置 {obj.name} 的AudioListener组件");
                // 对每个对象执行操作
                AudioListener al = obj.GetComponent<AudioListener>();
                if (al != null)
                {
                    Destroy(al);
                }
                yield return new WaitUntil(() => obj.GetComponent<AudioListener>() == null);
                Debug.Log($"@【FgBg】 完成重置 {obj.name} 的AudioListener组件");
                obj.AddComponent<AudioListener>();
            }
        }

        public float MusicVolume
        {
            get
            {
                float vol;
                musicMixer.GetFloat("musicVol", out vol);
                if (metaMusicMixer != null)
                    metaMusicMixer.GetFloat("musicVol", out vol);
                return 1f - vol / volumeOfMute;
            }

            set
            {
                //Debug.LogError($"MusicVolume = {value}");
                musicMixer.SetFloat("musicVol", (1f - value) * volumeOfMute);
                if (metaMusicMixer != null)
                    metaMusicMixer.SetFloat("musicVol", (1f - value) * volumeOfMute); 
            }
        }

        public float SfxVolume
        {
            get
            {
                float vol;
                sfxMixer.GetFloat("sfxVol", out vol);
                return 1f - vol / volumeOfMute;
            }

            set
            {
                //Debug.LogError($"sfxVol = {value}");
                sfxMixer.SetFloat("sfxVol", (1f - value) * volumeOfMute);
            }
        }

        public AudioMixerGroup GetAudioMixerGroup(GSMixerGroup output)
        {
            return mixerGroups.GetValue<AudioMixerGroup>(output.ToString());
        }

        public AudioMixerSnapshot GetAudioMixerSnapshot(string snapshotName)
        {
            return snapshots.GetValue<AudioMixerSnapshot>(snapshotName);
        }

        public AnimationCurve GetEaseCurve(GSEaseType easeType)
        {
            return easeCurves.GetValue<AnimationCurve>(easeType.ToString());
        }

        /// <summary>从声音池里获取一个新的GSSource对象</summary>
        public GSSource GetSource()
        {
            GSSource gss = pool.GetObject().GetComponent<GSSource>();
            gss.startUseTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return gss;
        }

        public IGSHandler GetHandler(string handlerId)
        {
            for (int i = 0; i < handlers.Count; ++i)
            {
                var variable = handlers[i].GetVariable<GSHandler>(handlerId);
#if UNITY_EDITOR
                if (variable != null &&!_testHandlerIds.Contains(handlerId))
                    _testHandlerIds.Add(handlerId);
#endif
                if (variable != null)
                    return variable.value;
            }

            Debug.LogWarning(string.Format("Do not found GSHander({0})!", handlerId));

            return new GSNullHandler();
        }

        public void ClearUnusedClips(float cachingTime = float.MinValue)
        {
            for (int i = 0; i < handlers.Count; ++i)
            {
                var variables = handlers[i].variables;
                foreach (var pair in variables)
                {
                    var handler = (GSHandler)(pair.Value.value);
                    var clip = handler.clip;

                    if (clip.IsUnUsedPtr(cachingTime))
                        clip.Clear();
                }
            }
        }

        private IEnumerator ClearUnusedClipsCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(clipLifeTime);

                ClearUnusedClips(clipLifeTime);

                yield return Resources.UnloadUnusedAssets();
            }
        }
    }
}
