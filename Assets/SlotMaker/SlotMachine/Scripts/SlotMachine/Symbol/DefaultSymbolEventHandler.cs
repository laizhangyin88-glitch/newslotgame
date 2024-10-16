using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace SlotMaker
{
    public class DefaultSymbolEventHandler : SymbolEventHandler
    {
        [Serializable]
        public class SymbolRendererList
        {
#if UNITY_EDITOR
            [TitleGroup("Symbol Info")][ReadOnly][PreviewField(Alignment = ObjectFieldAlignment.Left)] public Texture texture;
            [ReadOnly] public int index;
            [ReadOnly] public string name;
#endif
            [TableList(AlwaysExpanded = true)]
            public List<SymbolRenderer> value;
        }

        [Serializable]
        public class SymbolRenderer
        {
            public SpriteRenderer spriteRenderer;
            public int spriteIndex;
            [CustomValueDrawer("SortingLayerDrawer")]
            public int sortingLayer = 2;
            public int sortingOrder;

            public void SetActive(bool value)
            {
                this.spriteRenderer.gameObject.SetActive(value);
            }

            public void Update(BaseSymbol symbol)
            {
                this.spriteRenderer.sprite = symbol.symbolAssets.GetSprite(symbol.symbolIndex, this.spriteIndex);
                this.spriteRenderer.sortingLayerID = SortingLayer.layers[this.sortingLayer].id;
                this.spriteRenderer.sortingOrder = this.sortingOrder;
            }

            private int SortingLayerDrawer(int value, GUIContent label)
            {
#if UNITY_EDITOR
                int valueIndex = SortingLayer.layers.ToList().FindIndex((layer) => layer.value == value);
                string[] displayString = SortingLayer.layers.ToList().Select((sortingLayer) => sortingLayer.name).ToArray();
                return EditorGUILayout.Popup(label, valueIndex, displayString);
#else
                return value;
#endif
            }
        }

        [LabelText("Entry Preset")]
        public List<SymbolRendererList> symbolPresets = new List<SymbolRendererList>();

        public override void Clear(BaseSymbol symbol)
        {
            base.Clear(symbol);

            foreach (var renderer in symbolPresets[symbol.symbolIndex].value) { renderer.SetActive(false); }
        }

        public override void Change(BaseSymbol symbol)
        {
            foreach (var preset in symbolPresets)
            {
                foreach (var renderer in preset.value) { renderer.SetActive(false); }
            }
            base.Clear(symbol);
        }

        public override void Apply(BaseSymbol symbol)
        {
            foreach (var renderer in symbolPresets[symbol.symbolIndex].value)
            {
                renderer.Update(symbol);
                renderer.SetActive(true);
            }
            base.Apply(symbol);
        }

#if UNITY_EDITOR
        [Button("Initialize Preset")]
        public void InitializeSymbolPreset(GameObject customData)
        {
            behaviours.Clear();
            symbolPresets.Clear();

            SymbolAssets symbolAssets = customData.GetComponent<SymbolAssets>();
            for (int i = 0; i < symbolAssets.Count; i++)
            {
                SymbolBehaviour original = symbolAssets.GetSymbolBehaviour(i);
                if (original)
                    InstantiateBehaviour(original);

                symbolPresets.Add(
                    new SymbolRendererList()
                    {
                        texture = symbolAssets.GetSpriteCount(i) == 0 ? null : symbolAssets.GetSprite(i, 0)?.texture,
                        index = i,
                        name = symbolAssets.GetSymbolBehaviour(i).name,
                        value = new List<SymbolRenderer>() { new SymbolRenderer() },
                    }
                );
            }
        }
#endif
    }
}
