using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using SlotMaker;

namespace BagelCode
{
    public class VideoPlayerManager : MonoBehaviour
    {
        public Transform imageArea;
        public GameObject loadingObj;

        public List<string> permissionExtensions = new List<string>() {".mp4"};

        private VideoPlayer video;
        private AudioSource audio;
        private RenderTexture renderTexture;
        private RawImage image;
        private Vector2 renderTextureSize;
        private VideoAspectRatio renderAspectRatio;

        private bool isInit = false;

        private void Init(Vector2 textureSize, VideoAspectRatio aspectRatio)
        {
            if(isInit) return;
            isInit = true;

            renderTextureSize = textureSize;
            renderAspectRatio = aspectRatio;
            
            video = gameObject.AddComponent<VideoPlayer>();
            video.playOnAwake = false;

            audio = gameObject.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.Pause();

            renderTexture = new RenderTexture((int)renderTextureSize.x, (int)renderTextureSize.y, 16, RenderTextureFormat.ARGB32);
            renderTexture.Create();

            image = imageArea.gameObject.AddComponent<RawImage>();
            image.texture = renderTexture;
        }

        private void OnDestroy()
        {
            if(image != null)
            {
                image.texture = null;
                GameObject.Destroy(image.gameObject);
            }

            if(renderTexture != null)
            {
                renderTexture.Release();
                renderTexture = null;
            }
        }

        public void Play(string url, Vector2 textureSize, VideoAspectRatio aspectRatio)
        {
            if(!string.IsNullOrEmpty(url))
            {
                if(CheckPermissionExtension(url))
                {
                    Init(textureSize, aspectRatio);
                    
                    FileDownloader.Instance.LoadFile(   url, 
                                                        CacheType.FileCache, 
                                                        false,
                                                        VideoFileReady,
                                                        VideoDownloadSuccess,
                                                        null,
                                                        null);
                    if(loadingObj != null)
                        loadingObj.SetActive(true);
                }
                else
                {
                    // Debug.LogError("Not Supported");
                }
            }
        }

        public void VideoFileReady(string url)
        {
            // Ready
        }

        public void VideoDownloadSuccess(FileDownloader.FileDownloadedInfo info)
        {
            Debug.LogError("VideoDownloadSuccess");
            video.renderMode = UnityEngine.Video.VideoRenderMode.RenderTexture;
            video.targetTexture = renderTexture;
            video.targetCameraAlpha = 1f;
            video.source = VideoSource.Url;
            video.url = info.path;
            video.frame = 60;
            video.isLooping = true;
            video.prepareCompleted += VideoReady;
            video.aspectRatio = renderAspectRatio;

            // Set Audio
            video.audioOutputMode = VideoAudioOutputMode.AudioSource;
            video.EnableAudioTrack(0, true);
            video.SetTargetAudioSource(0, audio);

            video.Prepare();
        }

        public void PlayVideo()
        {
            Debug.LogError("PlayVideo");
            if(loadingObj != null)
                loadingObj.SetActive(false);

            if(image != null)
                image.gameObject.SetActive(true);
            video.Play();
        }

        public void VideoReady(VideoPlayer vp)
        {
            Debug.LogError("VideoReady");
            if(vp != video) return;

#if UNITY_ANDROID // Texture one frame bug..
                Invoke("PlayVideo", 0.06f);
#else
                PlayVideo();
#endif
        }

        private bool CheckPermissionExtension(string url)
        {
            try
            {
                string extension = System.IO.Path.GetExtension(url);
                if(!string.IsNullOrEmpty(extension) && permissionExtensions.Contains(extension.ToLower()))
                    return true;
            }
            catch(Exception e)
            {
                return false;
            }

            return false;
        }
    }

}
