using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace SlotMaker
{
    public class SymbolAssets : MonoBehaviour
    {
        public List<BaseSymbolAsset> assets;
        public List<ObjectPool> pools;

        public int assetGroupId;

        public int Count { get { return assets.Count; } }

        public int GetSpriteCount(int groupId)
        {
            var asset = assets[groupId] as ISymbolSprite;
            return asset?.Sprites.Count ?? 0;
        }

        public Sprite GetSprite(int groupId, int spriteId)
        {
            var asset = assets[groupId] as ISymbolSprite;
            return asset?.Sprites[spriteId];
        }

        public Material GetMaterial(int groupId, int materialId)
        {
            var asset = assets[groupId] as ISymbolMaterial;
            return asset?.Materials[materialId];
        }

        public GameObject GetPrefab(int groupId, int prefabId)
        {
            var asset = assets[groupId] as ISymbolPrefab;
            return asset?.Prefabs[prefabId];
        }

        public GameObject GetGameObject(int groupId, int prefabId)
        {
            var prefab = GetPrefab(groupId, prefabId);
            if (!prefab)
                return null;

            var go = Instantiate(prefab) as GameObject;
            go.name = prefab.name;
            return go;
        }

        public GameObject GetPooledObject(int poolId)
        {
            return pools[poolId].GetObject().gameObject;
        }

        public Graph GetGraph(int groupId)
        {
            var asset = assets[groupId] as ISymbolGraph;
            return asset?.Graph;
        }

        public SymbolBehaviour GetSymbolBehaviour(int groupId)
        {
            var asset = assets[groupId] as ISymbolBehaviour;
            return asset?.Behaviour;
        }
    }
}
