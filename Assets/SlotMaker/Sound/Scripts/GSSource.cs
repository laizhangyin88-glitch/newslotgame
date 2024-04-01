using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace SlotMaker
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PooledObject))]
    public class GSSource : MonoBehaviour
    {
        private AudioSource _source;
        private AudioSource source
        {
            get
            {
                if (_source == null)
                    _source = GetComponent<AudioSource>();

                return _source;
            }
        }

        public GSHandler Handler { get; set; }

        private float maxVolume;

        public long startUseTime = 0;
        public bool IsPlaying
        {
            get
            {
                return source.isPlaying;
            }
        }

        public bool IsFading
        {
            get
            {
                return coroutine != null;
            }
        }

        public float Volume
        {
            get
            {
                return source.volume / maxVolume;
            }

            set
            {
                source.volume = value * maxVolume;
            }
        }

        public bool Mute
        {
            get
            {
                return source.mute;
            }

            set
            {
                source.mute = value;
            }
        }

        public float Time
        {
            get
            {
                return source.time;
            }

            set
            {
                source.time = value;
            }
        }

        private IEnumerator coroutine;

        public void Initialize(GSHandler handler)
        {
            Handler = handler;


            //source属性赋值
            source.clip = Handler.clip.Load(); //资源包里加载clip
            source.loop = Handler.loop;
            source.volume = Handler.volume;
            maxVolume = Handler.volume;
            source.mute = Handler.Mute;
            source.outputAudioMixerGroup = Handler.outputAudioMixerGroup;

            //方法绑定
            Handler.onVolumeChanged += OnVolumeChanged;
            Handler.onMute += OnMute;
            Handler.onStop += Stop;
            Handler.onPause += Pause;
            Handler.onUnPause += UnPause;
            Handler.onClear += Clear;
        }

        public void Play()
        {
            Play(Handler.delay);
        }

        public void Play(float delay)
        {
            if (delay > 0f)
                source.PlayDelayed(delay);
            else
                source.Play();

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            if (Handler != null && Handler.fadeIn != null && Handler.fadeIn.time > 0f)
            {
                coroutine = FadeInCo();
                StartCoroutine(coroutine);
            }
            else
            {
                Volume = 1.0f;
            }
        }

        private void OnVolumeChanged(float vol)
        {
            Volume = vol;
        }

        private void OnMute(bool mute)
        {
            Mute = mute;
        }

        public void Stop()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            if (Handler != null && Handler.fadeOut != null && Handler.fadeOut.time > 0f)
            {
                coroutine = FadeOutCo();
                StartCoroutine(coroutine);
            }
            else
            {
                source.Stop();
            }
        }

        public void Pause()
        {
            source.Pause();
        }

        public void UnPause()
        {
            source.UnPause();
        }

        /// <summary>停止播放，解除和GSHandler的绑定，丢回预设池</summary>
        public void Clear()
        {
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (source != null && source.isPlaying == true) { }
                source.Stop();

            if (coroutine != null)
                StopCoroutine(coroutine);

            Handler.onVolumeChanged -= OnVolumeChanged;
            Handler.onMute -= OnMute;
            Handler.onStop -= Stop;
            Handler.onPause -= Pause;
            Handler.onUnPause -= UnPause;
            Handler.onClear -= Clear;

            Handler.clip.UnLoad();
            Handler.OnDestroySource(this);
            Handler = null;

            if (source != null )
                 source.clip = null;

            coroutine = null;

            GetComponent<PooledObject>().ReturnToPool();
        }


        private void LateUpdate()
        {
            if ((Handler == null) ||
                ((Handler.autoRelease) && !source.isPlaying))
            {
                ReturnToPool();
            }
        }

        private IEnumerator FadeInCo()
        {
            var curve = GSManager.Instance.GetEaseCurve(Handler.fadeIn.easeType);
            float startVolume = Volume;
            float volumeRange = 1f - startVolume;
            var startTime = UnityEngine.Time.time;
            var animationTime = Handler.fadeIn.time;

            while (true)
            {
                float deltaTime = UnityEngine.Time.time - startTime;
                Volume = curve.Evaluate(deltaTime / animationTime) * volumeRange + startVolume;
                if (deltaTime >= animationTime)
                    break;

                yield return null;
            }
            coroutine = null;
        }

        private IEnumerator FadeOutCo()
        {
            var curve = GSManager.Instance.GetEaseCurve(Handler.fadeOut.easeType);
            float startVolume = Volume;
            float volumeRange = startVolume;
            var startTime = UnityEngine.Time.time;
            var animationTime = Handler.fadeOut.time;

            while (true)
            {
                float deltaTime = UnityEngine.Time.time - startTime;
                Volume = startVolume - (curve.Evaluate(deltaTime / animationTime) * volumeRange);
                if (deltaTime >= animationTime)
                    break;

                yield return null;
            }

            source.Stop();
            coroutine = null;
        }
    }
}
