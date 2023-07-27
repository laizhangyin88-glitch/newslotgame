using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BagelCode.EpicAlbum
{
    [CreateAssetMenu(fileName="New EpicAlbumAssets", menuName="Meta/ScriptableObject/EpicAlbumAssets")]
    public class EpicAlbumAssets : ScriptableObject
    {
        [PreviewField(75, ObjectFieldAlignment.Center)]
        public List<Sprite> categoryIconAssets;
    }
}