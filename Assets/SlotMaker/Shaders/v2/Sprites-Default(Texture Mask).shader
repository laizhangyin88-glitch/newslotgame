// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "SlotMaker/Sprites/Default(Texture Mask)"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _ClipTex ("Clip Texture", 2D) = "white" {}
        _Cutoff ("Cutoff", Range(-1,1)) = -1
        _CutoffSharpness ("Cutoff Sharpness", Range(0.001,1)) = 1
        _ClipPivot ("Clip Pivot", Vector) = (0,0,0,0)
        _ClipRange ("Clip Range", Vector) = (0,0,1,1)
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
 
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
 
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
     
            struct appdata_ct
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            }; 
 
            struct v2f_ct
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
                float2 clipUV   : TEXCOORD1;
    			UNITY_VERTEX_OUTPUT_STEREO
            };
     
            float4 _ClipPivot;
            float4x4 _ClipRotation;
            float4 _ClipRange;
 
            v2f_ct vert(appdata_ct IN)
            {
                v2f_ct OUT;

                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

            #ifdef UNITY_INSTANCING_ENABLED
                IN.vertex.xy *= _Flip;
            #endif

                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;

                OUT.clipUV = _ClipPivot.xy - mul(unity_ObjectToWorld, IN.vertex).xy;
                OUT.clipUV = (mul(_ClipRotation, float4(OUT.clipUV, 0, 0)).xy * _ClipRange.zw + _ClipRange.xy) * -0.5 + float2(0.5, 0.5);

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }
 
            sampler2D _ClipTex;
            fixed     _Cutoff;
            fixed     _CutoffSharpness;
 
            fixed4 frag(v2f_ct IN) : SV_Target
            {
                fixed factor = 0;
                if (IN.clipUV.x < 0 || IN.clipUV.y < 0 || IN.clipUV.x > 1 || IN.clipUV.y > 1)
                {
                }
                else 
                {
                    factor = tex2D(_ClipTex, IN.clipUV).a;
                    factor = saturate((factor - _Cutoff) * _CutoffSharpness);
                }

                fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
                c.a *= factor;
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}
 
