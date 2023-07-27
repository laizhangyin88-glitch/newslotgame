using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Networking;

using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{

public class FileImageLoader : MonoSingleton<FileImageLoader>
{
    public Sprite LoadTextureToSprite(string path)
    {
        byte[] bytes = FileUtils.Read(path);

        if (bytes != null && bytes.Length > 0)
        {
            return LoadTextureToSprite(bytes);
        }

        return null;
    }

    public Sprite LoadTextureToSprite(byte[] bytes)
    {
        if (bytes != null && bytes.Length > 0)
        {
            Texture2D tex  = new Texture2D(1,1, TextureFormat.RGB24, false);
            tex.LoadImage(bytes);

            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero, 100.0f);
        }

        return null;
    }
}

}
