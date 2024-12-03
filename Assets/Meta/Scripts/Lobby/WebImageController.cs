using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;

#if DEV
using TMPro;
#endif

namespace BagelCode
{

public class WebImageController : MonoBehaviour
{
    public GameObject loadingObject;
    public GameObject backgroundObject;
    public ContextElement imageContextElement;

    private IContextImage _imageElement;
    private IContextImage imageElement
    {
        get
        {
            if( _imageElement == null )
            {
                if( imageContextElement == null ) return null;
                _imageElement = imageContextElement as IContextImage;
            }

            return _imageElement;
        }
    }

    [SerializeField] private string imageURL;
    private int retriedCount = 0;

    private const long DEFAULT_WAIT_INTERVAL = 200;
    private const long MAX_WAIT_INTERVAL = 5000;

    private bool isDownloaded = false;
    private bool useBackground = false;

#if DEV
    private TextMeshProUGUI assetNameText = null;

    public void SetDebugText(string text)
    {
        if( assetNameText == null )
        {
            var go = new GameObject();
            go.transform.parent = transform;
            go.transform.localPosition = new Vector3(10f, 0f, 0f);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            go.name = "Asset Name";
            assetNameText = go.AddComponent<TextMeshProUGUI>();
            assetNameText.fontSize = 34;
            assetNameText.color = new Color(0, 255, 255);
        }

        assetNameText.text = text+"(Web)";
        if(isDownloaded)
            assetNameText.gameObject.SetActive(false);
    }
#endif

    public void SetWebImage(string url, bool useDefaultIcon)
    {
        StopAllCoroutines();

        retriedCount = 0;
        imageURL = url;
        isDownloaded = false;
        useBackground = useDefaultIcon;

        RequestDownloadImage(imageURL);
    }

    private void OnEnable()
    {
        if(isDownloaded)
        {
            ActiveLoading(false);
            ActiveImage(true);

            if(imageContextElement != null)
            {
                var image = imageContextElement as ContextImage;
                if(image != null && image.image.sprite == null)
                {
                    SetWebImage(imageURL, useBackground);
                }
            }
        }
        else
        {
            SetWebImage(imageURL, useBackground);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void ImageDownloaded(Sprite img)
    {
        try
        {
            if(gameObject == null || !gameObject.activeInHierarchy) return;

            isDownloaded = true;

            if( imageElement != null )
                imageElement.SetSprite(img);

            ActiveLoading(false);
            ActiveImage(true);
#if DEV
            if( assetNameText != null )
                assetNameText.gameObject.SetActive(false);
#endif
        }
        catch(Exception e)
        {
#if DEV
            if (ApplicationSettings.LogTest())
                Debug.LogError( "LoadImageDownloaded Exception." );
#endif
        }

    }

    private void ImageDownloadFailed(WebImageDownloader.WebImageDownloadError error)
    {
        try
        {
            if(gameObject == null || !gameObject.activeInHierarchy) return;
#if DEV
            if( assetNameText != null )
                assetNameText.gameObject.SetActive(true);
#endif
                StartCoroutine(RetryImageDownload());
        }
        catch(Exception e)
        {
#if DEV
            if (ApplicationSettings.LogTest())
                Debug.LogError( "ImageDownloadFailed Exception." );
#endif
        }
    }

    private void ActiveLoading(bool isActive)
    {
        if( loadingObject != null )
            loadingObject.SetActive(isActive);
    }

    private void ActiveImage(bool isActive)
    {
        if( backgroundObject != null )
            backgroundObject.SetActive(useBackground && !isActive );

        if( imageContextElement != null)
            imageContextElement.gameObject.SetActive(isActive);
    }

    private IEnumerator RetryImageDownload()
    {
        ++retriedCount;
        long waitTimeMs = GetWaitTimeMs();
        yield return new WaitForSeconds((float)waitTimeMs / 1000.0f);

        RequestDownloadImage(imageURL);
    }

    private void RequestDownloadImage(string imageURL)
    {
        ActiveImage(false);

        if( string.IsNullOrEmpty(imageURL) ) return;
        if( imageElement == null ) return;

        ActiveLoading(true);

        // Set Hash
        imageElement.SetHash(imageURL.GetHashCode().ToString());

        WebImageDownloader.Instance.LoadWebImage(
            imageURL,
            CacheType.FileCache,
            false,
            null,
            delegate(Sprite img)
            {
                if (img != null)
                {
                    if(imageElement == null || imageContextElement == null) return;
                    if (imageElement.CheckHash(imageURL.GetHashCode().ToString()))
                    {
                        ImageDownloaded(img);
                    }
                }
            },
            null,
            ImageDownloadFailed
        );
    }

    private long GetWaitTimeMs()
    {
        if( retriedCount < 1 ) retriedCount = 1;

        long waitTime = (long)(Math.Pow(2, retriedCount - 1) * DEFAULT_WAIT_INTERVAL);
        if( waitTime > MAX_WAIT_INTERVAL )
            waitTime = MAX_WAIT_INTERVAL;
        return waitTime;
    }
}

}

