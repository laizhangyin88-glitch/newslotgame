using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
	[CreateAssetMenu(fileName="New Strip Editor", menuName="SlotMaker2/Math/Editor/Strip Editor")]
	public class SymbolStripEditor : ScriptableObject
	{
		[Serializable]
	    public struct Recipe
	    {
	    	[InlineEditor]
	    	public SymbolStrip strip;

	    	[Button]
	    	[PropertyOrder(-3)]
	    	public void Clear()
	    	{
	    		strip.Clear();

	    		EditorUtility.SetDirty(strip);
	    		AssetDatabase.SaveAssets();
	    	}

	    	[HorizontalGroup("Export")]
		    [Button]
		    [PropertyOrder(-2)]
		    public void Import()
		    {
		    	var remap = new Dictionary<string, SymbolEntity>();
		    	int count = strip.symbols.Count;
		    	for (int i = 0; i < count; ++i)
		    	{
		    		remap[strip.symbols[i].name] = strip.symbols[i];
		    	}
		    	
		    	Clear();
		    	
		    	var tokens = EditorGUIUtility.systemCopyBuffer.Split(new char[]{ '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		    	foreach (var token in tokens)
		    	{
		    		SymbolEntity symbolEntity;
		    		if (remap.TryGetValue(token, out symbolEntity))
		    			strip.Add(new OverridenSymbolEntity{ master = symbolEntity });
	    			else
	    				Debug.LogError(string.Format("Could not be found [{0}] symbol", token));
		    	}

		    	EditorUtility.SetDirty(strip);
		    	AssetDatabase.SaveAssets();
		    }

		    [HorizontalGroup("Export")]
		    [Button]
		    [PropertyOrder(-1)]
		    public void Export()
		    {
		    	var sb = new StringBuilder();
		    	int count = strip.Count;
		    	for (int i = 0; i < count; ++i)
		    	{
		    		sb.Append(strip[i].master.name);
		    		sb.Append("\r\n");
		    	}
		    	EditorGUIUtility.systemCopyBuffer = sb.ToString();
		    }
	    }
	    [Space]
	    public List<Recipe> recipes;

	    [Button]
	    [PropertyOrder(-102)]
	    public void Clear()
	    {
	    	foreach (var recipe in recipes)
	    	{
	    		recipe.Clear();
	    	}
	    }

	    [HorizontalGroup("Export")]
	    [Button]
	    [PropertyOrder(-101)]
	    public void Import()
	    {
	    	var remaps = new List<Dictionary<string, SymbolEntity>>();
	    	int recipeCount = recipes.Count;
	    	for (int i = 0; i < recipeCount; ++i)
	    	{
	    		var remap = new Dictionary<string, SymbolEntity>();
	    		var strip = recipes[i].strip;
	    		int count = strip.symbols.Count;
	    		for (int j = 0; j < count; ++j)
	    		{
	    			remap[strip.symbols[j].name] = strip.symbols[j];
	    		}
	    		remaps.Add(remap);

	    		strip.Clear();
	    	}

	    	var lines = EditorGUIUtility.systemCopyBuffer.Split(new char[]{ '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
	    	foreach (var line in lines)
	    	{
	    		var tokens = line.Split(new char[]{ '\t' });
	    		for (int i = 0; i < tokens.Length; ++i)
	    		{
	    			var token = tokens[i];
	    			if (string.IsNullOrEmpty(token))
	    				continue;

    				SymbolEntity symbolEntity;
    				if (remaps[i].TryGetValue(token, out symbolEntity))
    					recipes[i].strip.Add(new OverridenSymbolEntity{ master = symbolEntity });
					else
						Debug.LogError(string.Format("Count not be found [{0}] symbol", token));
	    		}
	    	}

	    	foreach (var recipe in recipes)
	    	{
	    		recipe.strip.AssignIndices();
	    		EditorUtility.SetDirty(recipe.strip);
	    	}
	    	AssetDatabase.SaveAssets();
	    }

	    [HorizontalGroup("Export")]
	    [Button]
	    [PropertyOrder(-100)]
	    public void Export()
	    {
	    	var sb = new StringBuilder();
	    	int recipeCount = recipes.Count;
	    	for (int line = 0; line < 1000; ++line)
	    	{
	    		bool hit = false;
	    		for (int j = 0; j < recipeCount; ++j)
	    		{
	    			var strip = recipes[j].strip;
	    			if (line < strip.Count) 
	    			{
	    				hit = true;
	    				sb.Append(strip[line].master.name);
	    			}
    				if (j < (recipeCount - 1)) sb.Append('\t');
	    		}
	    		if (!hit)
	    			break;
    			sb.Append("\r\n");
	    	}
	    	EditorGUIUtility.systemCopyBuffer = sb.ToString();
	    }

	    [Button]
	    [PropertyOrder(-91)]
	    public void AssignIndices()
	    {
	    	foreach (var recipe in recipes)
	    	{
	    		recipe.strip.AssignIndices();
	    		EditorUtility.SetDirty(recipe.strip);
	    	}
	    	AssetDatabase.SaveAssets();
	    }

	    [Button]
	    [PropertyOrder(-90)]
	    public void AdjustRowOffset()
	    {
	    	foreach (var recipe in recipes)
	    	{
	    		recipe.strip.AdjustRowOffset();
	    		EditorUtility.SetDirty(recipe.strip);
	    	}
	    	AssetDatabase.SaveAssets();
	    }
	}
}