using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public interface IMetaStreamingPlayer
    {
        void Init(float volume, bool isLoop);
        void SetURL(string url);
        string GetURL();
        void Clear();
        void Load();
        void Play();
        void Stop();
        bool IsLoaded();
    }
}

