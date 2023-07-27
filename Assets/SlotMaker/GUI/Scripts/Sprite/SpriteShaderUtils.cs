using UnityEngine;

namespace SlotMaker
{
    public static class SpriteShaderUtils
    {
        public static int COLOR_PROPERTY_ID;
        public static int UVS_PROPERTY_ID;
        public static int CENTER_OFFSET_PROPERTY_ID;
    	public static int[] CLIP_PIVOT_PROPERTY_ID = new int[4];
    	public static int[] CLIP_RANGE_PROPERTY_ID = new int[4];
        public static int[] CLIP_ROTATATION_PROPERTY_ID = new int[4];
    	public static int[] CLIP_ARGS_PROPERTY_ID = new int[4];
    	public static int[] CLIP_TEX_PROPERTY_ID = new int[4];
    	public static int[] CLIP_CUTOFF_PROPERTY_ID = new int[4];
    	public static int[] CLIP_CUTOFF_SHARPNESS_PROPERTY_ID = new int[4];
    	public static int[] SCALE_PROPERTY_ID = new int[4];

        static SpriteShaderUtils()
        {
            GetShaderPropertyIDs();
        }

        public static void GetShaderPropertyIDs()
    	{
            COLOR_PROPERTY_ID = Shader.PropertyToID("_Color");
            UVS_PROPERTY_ID = Shader.PropertyToID("_UVs");
            CENTER_OFFSET_PROPERTY_ID = Shader.PropertyToID("_CenterOffset");

            for (int i = 0; i < 4; ++i)
            {
                string postFix = "";
                if (i > 0) postFix = i.ToString();

                CLIP_PIVOT_PROPERTY_ID[i] = Shader.PropertyToID("_ClipPivot" + postFix);
                CLIP_RANGE_PROPERTY_ID[i] = Shader.PropertyToID("_ClipRange" + postFix);
                CLIP_ROTATATION_PROPERTY_ID[i] = Shader.PropertyToID("_ClipRotation" + postFix);
                CLIP_ARGS_PROPERTY_ID[i] = Shader.PropertyToID("_ClipArgs" + postFix);
                CLIP_TEX_PROPERTY_ID[i] = Shader.PropertyToID("_ClipTex" + postFix);
                CLIP_CUTOFF_PROPERTY_ID[i] = Shader.PropertyToID("_Cutoff" + postFix);
                CLIP_CUTOFF_SHARPNESS_PROPERTY_ID[i] = Shader.PropertyToID("_CutoffSharpness" + postFix);
                SCALE_PROPERTY_ID[i] = Shader.PropertyToID("_Scale" + postFix);
                
            }
    	}
    }
}
