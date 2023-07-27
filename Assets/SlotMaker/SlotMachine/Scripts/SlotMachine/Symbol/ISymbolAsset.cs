using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace SlotMaker
{
    public interface ISymbolGraph
    {
        Graph Graph { get; set; }
    }

    public interface ISymbolBehaviour
    {
        SymbolBehaviour Behaviour { get; set; }
    }

    public interface ISymbolPrefab
    {
        List<GameObject> Prefabs { get; }
    }

    public interface ISymbolSprite
    {
        List<Sprite> Sprites { get; }
    }

    public interface ISymbolMaterial
    {
        List<Material> Materials { get; }
    }
}
