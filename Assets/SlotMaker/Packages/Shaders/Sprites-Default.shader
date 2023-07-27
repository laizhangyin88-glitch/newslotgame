// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "SlotMaker2/Sprites/Default"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Blending)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("SrcBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("DstBlend", Int) = 10

        [Header(Stencil)]
        _Stencil ("Stencil ID", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Int) = 8
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilOp ("Stencil Operation", Int) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        [Header(Clipping)]
        [KeywordEnum(None, Depth1, Depth2, Depth3)] _Clip ("Clipping Depth", Float) = 0
        [Toggle(_ENABLE_TEXTURE_MASK)] _EnableTextureMask ("Enable Texture Mask", Float) = 0

        [Header(Extra)]
        _ColorMask ("Color Mask", Float) = 15
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode ("Culling Mode", Int) = 0

        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull [_CullMode]
        Lighting Off
        ZWrite Off
        Blend [_SrcBlend] [_DstBlend]
        ColorMask [_ColorMask]

        Pass
        {
        CGPROGRAM
            #pragma vertex SlotsSpriteVert
            #pragma fragment SlotsSpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #pragma multi_compile __ _ENABLE_TEXTURE_MASK
            #pragma multi_compile _CLIP_NONE _CLIP_DEPTH1 _CLIP_DEPTH2 _CLIP_DEPTH3
            #include "UnitySprites.cginc"
            #include "SlotsClipping.cginc"

            slots_v2f SlotsSpriteVert(slots_appdata_t IN)
            {
                slots_v2f OUT;

                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;
                ClippingVert(IN, OUT);

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 SlotsSpriteFrag(slots_v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
                c.a *= ClippingFrag(IN);
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}
