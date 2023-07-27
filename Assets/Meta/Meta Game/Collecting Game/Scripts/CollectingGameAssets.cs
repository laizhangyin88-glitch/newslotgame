using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BagelCode.Scratcher
{
    [CreateAssetMenu(fileName="New CollectingGameAssets", menuName="Meta/ScriptableObject/CollectingGameAssets")]
    public class CollectingGameAssets : ScriptableObject
    {
        [PreviewField(75, ObjectFieldAlignment.Center)]
        public List<Sprite> starAssets;
        public List<ItemAssets> itemAssets;
    }
    
    [Serializable]
    public class ItemAssets
    {
        [PreviewField(75, ObjectFieldAlignment.Center)]
        public List<Sprite> assets;
    }

}