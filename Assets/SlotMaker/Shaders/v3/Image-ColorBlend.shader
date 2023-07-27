Shader "SlotMaker/Image/ColorBlend"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _BlendTex ("Blend Texture", 2D) = "white" {}
        _BlendColor ("Blend Color", Color) = (1,1,1,1)
        _BlendWeight ("Blend Weight", Range(0,1)) = 0

        [Header(Blending)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("SrcBlend", Int) = 5.0 // SrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("DstBlend", Int) = 10.0 // OneMinusSrcAlpha

        [Header(Stencil)]
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        [Header(Rendering)]
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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

        Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
        ZTest [unity_GUIZTestMode]
        Blend [_SrcBlend] [_DstBlend]
        ColorMask [_ColorMask]

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_ct
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f_ct
            {
                float4 vertex        : SV_POSITION;
                fixed4 color         : COLOR;
                float4 texcoord      : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BlendTex;
            float4 _BlendTex_ST;
            fixed4 _Color;
            fixed4 _BlendColor;
            fixed _BlendWeight;
            float4 _ClipRect;
            fixed4 _TextureSampleAdd;

            v2f_ct vert(appdata_ct IN)
            {
                v2f_ct OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord.xy = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.texcoord.zw = TRANSFORM_TEX(IN.texcoord, _BlendTex);
                
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f_ct IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord.xy) + _TextureSampleAdd) * IN.color;

                half weight = lerp(0, tex2D(_BlendTex, IN.texcoord.zw).a, _BlendWeight);
                color.rgb = lerp(color.rgb, _BlendColor.rgb, weight);

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
        ENDCG
        }
    }
}
