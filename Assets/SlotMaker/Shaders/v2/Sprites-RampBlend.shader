Shader "SlotMaker/Sprites/RampBlend"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _RampTex ("Ramp Texture", 2D) = "white" {}
        _RampV1 ("Ramp V1", Range(0, 1)) = 0
        _RampV2 ("Ramp V2", Range(0, 1)) = 0
        _BlendFactor ("Blend Factor", Range(0, 1)) = 0
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
                float2 worldPos : TEXCOORD1;
    			UNITY_VERTEX_OUTPUT_STEREO
            };

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

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            sampler2D _RampTex;
            fixed     _RampV1;
            fixed     _RampV2;
            fixed     _BlendFactor;

            fixed4 frag(v2f_ct IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
                fixed3 v1 = tex2D(_RampTex, fixed2(c.r, 1 - _RampV1));
                fixed3 v2 = tex2D(_RampTex, fixed2(c.r, 1 - _RampV2));
                c.rgb = v1 * (1 - _BlendFactor) + v2 * _BlendFactor;
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}
