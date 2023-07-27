// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "SlotMaker/Sprites/Default(Soft Clip) 1"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _ClipPivot ("Clip Pivot", Vector) = (0,0,0,0)
        _ClipRange ("Clip Range", Vector) = (0,0,1,1)
        _ClipArgs ("Clip Arguments", Vector) = (1000,1000,0,1)
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
                float2 worldPos1 : TEXCOORD2;
    			UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _ClipPivot;
            float4x4 _ClipRotation;
            float4 _ClipRange;
            float4 _ClipArgs;
            float4 _ClipPivot1;
            float4x4 _ClipRotation1;
            float4 _ClipRange1;
            float4 _ClipArgs1;
 
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

                OUT.worldPos = _ClipPivot.xy - mul(unity_ObjectToWorld, IN.vertex).xy;
                OUT.worldPos = mul(_ClipRotation, float4(OUT.worldPos, 0, 0)).xy * _ClipRange.zw + _ClipRange.xy;
                OUT.worldPos1 = _ClipPivot1.xy - mul(unity_ObjectToWorld, IN.vertex).xy;
                OUT.worldPos1 = mul(_ClipRotation1, float4(OUT.worldPos1, 0, 0)).xy * _ClipRange1.zw + _ClipRange1.xy;

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }
 
            fixed4 frag(v2f_ct IN) : SV_Target
            {
                // Softness factor
                fixed2 factor = (float2(1, 1) - abs(IN.worldPos)) * _ClipArgs.xy;
                fixed fade = min(factor.x, factor.y);

                factor = (float2(1, 1) - abs(IN.worldPos1)) * _ClipArgs1.xy;
                fade = min(fade, min(factor.x, factor.y));

                fade = clamp(fade, 0, 1);

                // Sample the texture
                fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
                c.a *= fade;
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}
 
