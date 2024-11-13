using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Rendering
{
	public static class ShaderUtils
	{
		public static readonly string[] KEYWORD_CLIPPING_DEPTH = new string[]{ "_CLIP_NONE", "_CLIP_DEPTH1", "_CLIP_DEPTH2", "_CLIP_DEPTH3" };
	    public static readonly string KEYWORD_CLIPPING_ENABLE_TEXTURE_MASK = "_ENABLE_TEXTURE_MASK";
	    public static readonly string KEYWORD_ENABLE_VERTEX_SCALING = "_ENABLE_VERTEX_SCALING";

	    public static int ID_COLOR;
	    public static int ID_TEXTURE_MASK;

	    public static int[] ID_CLIPPING_MATRIX = new int[3];
	    public static int[] ID_CLIPPING_ARGS = new int[3];

	    private static bool isInitialized = false;

	    static ShaderUtils()
	    {
	    	GetShaderPropertyIDs();
	    }

	    private static void GetShaderPropertyIDs()
	    {
	    	if (!isInitialized)
	    	{
	    		isInitialized = true;

	    		ID_COLOR = Shader.PropertyToID("_Color");
	    		ID_TEXTURE_MASK = Shader.PropertyToID("_TextureMask");

	    		ID_CLIPPING_MATRIX[0] = Shader.PropertyToID("_ClipMatrix0");
	    		ID_CLIPPING_MATRIX[1] = Shader.PropertyToID("_ClipMatrix1");
	    		ID_CLIPPING_MATRIX[2] = Shader.PropertyToID("_ClipMatrix2");

	    		ID_CLIPPING_ARGS[0] = Shader.PropertyToID("_ClipArgs0");
	    		ID_CLIPPING_ARGS[1] = Shader.PropertyToID("_ClipArgs1");
	    		ID_CLIPPING_ARGS[2] = Shader.PropertyToID("_ClipArgs2");
	    	}
	    }

	    public static void SetKeywords(Material material, string[] keywords, int index)
	    {
	    	for (int i = 0; i < keywords.Length; ++i)
	    	{
	    		SetKeyword(material, keywords[i], i == index);
	    	}
	    }

	    public static void SetKeyword(Material material, string keyword, bool enable)
	    {
	    	if (enable)
	    	{
	    		if (!material.IsKeywordEnabled(keyword))
	    			material.EnableKeyword(keyword);
	    	}
	    	else
	    	{
	    		if (material.IsKeywordEnabled(keyword))
	    			material.DisableKeyword(keyword);
	    	}
	    }
	}
}