using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;

namespace BagelCode
{

public class LuckySpinSlotThumbnailLoader : MonoWeakSingleton<LuckySpinSlotThumbnailLoader>
{
    public string bundleName;

    public Dictionary<int, string> thumbnailImages = new Dictionary<int, string>();
    public Dictionary<int, Sprite> symbols = new Dictionary<int, Sprite>();

	private void Awake()
	{
        List<Blackboard> gameInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "/gameInfoList").value;

        bool isError = false;

        for(int i=0; i<gameInfoList.Count; ++i)
        {
            int gameID = BlackboardUtils.FindVariable<int>( gameInfoList[i], "gameId" ).value;

            if(!thumbnailImages.ContainsKey(gameID))
            {
                string gameTitle = BlackboardUtils.FindVariable<string>( gameInfoList[i], "gameTitle" ).value;

                string thumbAssetName = StringTableUtils.GetString(StringTable.StringTableType.Global, "SLOT_THUMBNAIL_SQUARE", out isError, gameTitle);

                thumbnailImages.Add(gameID, thumbAssetName);
            }
        }

        List<int> gameIdReel = BlackboardUtils.GetOrCreateVariable<List<int>>(MainBlackboard.Get(), "gameSpinReel/gameIdReel").value;

        for(int i=0; i<gameIdReel.Count; ++i)
        {
            if(!symbols.ContainsKey(gameIdReel[i]) && thumbnailImages.ContainsKey(gameIdReel[i]))
            {
                symbols.Add(gameIdReel[i], GetSprite(bundleName, thumbnailImages[gameIdReel[i]]));
            }
        }
	}

    private Sprite GetSprite(string bundleName, string assetName)
    {
#if UNITY_EDITOR
        var sprite = AssetBundleManager.LoadAsset<Texture2D>(bundleName, assetName);
        if(sprite != null)
        {
            Rect rec = new Rect(0,0, sprite.width, sprite.height);
            return Sprite.Create(sprite, rec, new Vector2(0,0),1);
        }
        
        return null;
#else

        return AssetBundleManager.LoadAsset<Sprite>(bundleName, assetName);
#endif
    }
}

}
