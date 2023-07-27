using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BagelCode.Scratcher
{
    [CreateAssetMenu(fileName="New CollectingGameChestAssets", menuName="Meta/ScriptableObject/CollectingGameChestAssets")]
    public class CollectingGameChestAssets : ScriptableObject
    {
        [PreviewField(75, ObjectFieldAlignment.Center)]
        public List<Sprite> assets;
    }
}