Shader "SlotMaker/Sprites/Default(Distortion)"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _ClipPivot ("Clip Pivot", Vector) = (0,0,0,0)
        _ClipRange ("Clip Range", Vector) = (0,0,1,1)
        _UVs ("UVs", Vector) = (0,0,1,1)
        _CenterOffset ("Distortion Center Offset", Float) = 0
        _ClipArgs ("Clip Arguments", Vector) = (1,1,1,1) // Softness, Depness, Range, Smoothness
        [Header(Scale)]
        [Toggle(_ENABLE_VERTEX_SCALING)] _Scale ("Scale", Vector) = (1,1,1,1)
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
            #pragma target 3.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #pragma multi_compile _ _ENABLE_VERTEX_SCALING
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
     
            float4 _ClipPivot;
            float4x4 _ClipRotation;
            float4 _ClipRange;
            float4 _ClipArgs;
            float4 _Scale;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _UVs)
                UNITY_DEFINE_INSTANCED_PROP(fixed, _CenterOffset)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f_ct vert(appdata_ct IN)
            {
                v2f_ct OUT;

                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;

                OUT.worldPos = _ClipPivot.xy - mul(unity_ObjectToWorld, IN.vertex).xy;
                OUT.worldPos = mul(_ClipRotation, float4(OUT.worldPos, 0, 0)).xy * _ClipRange.zw + _ClipRange.xy;

                fixed deepness = _ClipArgs.y;
                fixed range = max(0.1, _ClipArgs.z);

                fixed factor = abs(OUT.worldPos.y) - 1;
                fixed normalizedSteepness = saturate(factor / range);
                fixed interpolatedSteepness = pow(normalizedSteepness, 2);
                fixed distortion = interpolatedSteepness * deepness * sign(OUT.worldPos.y);

                IN.vertex.y += distortion;

                #ifdef UNITY_INSTANCING_ENABLED
                    IN.vertex.xy *= _Flip;
                #endif

                OUT.vertex = UnityObjectToClipPos(IN.vertex);

                #if defined(_ENABLE_VERTEX_SCALING)
                    OUT.vertex.xy *= _Scale.xy;
                #endif

                #ifdef PIXELSNAP_ON
                    OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 frag(v2f_ct IN) : SV_Target
            {
                fixed4 uvs = UNITY_ACCESS_INSTANCED_PROP(Props, _UVs);
                fixed distortionCenterOffset = UNITY_ACCESS_INSTANCED_PROP(Props, _CenterOffset);

                fixed smoothness = max(0.1, _ClipArgs.w);
                fixed range = max(0.1, _ClipArgs.z);

                fixed factor = abs(IN.worldPos.y) - 1;
                fixed normalizedSteepness = saturate(factor / range);
                fixed interpolatedSteepness = pow(normalizedSteepness, smoothness);

                fixed width = uvs.z - uvs.x;
                fixed center = uvs.x + width * 0.5 + distortionCenterOffset;

                fixed distortion = sign(IN.texcoord.x - center) * interpolatedSteepness * abs(IN.worldPos.x) * _ClipRange.z;
                IN.texcoord.x += distortion;

                // Softness factor
                fixed2 softnessFactor = float2(_Scale.z, _Scale.w) - float2(IN.worldPos.y, IN.worldPos.y * -1) * _ClipArgs.x;
                fixed fade = clamp(min(softnessFactor.x, softnessFactor.y), 0, 1);

                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                c.a *= fade;

                // not rendering other textures in atlas
                if (IN.texcoord.x < uvs.x || IN.texcoord.x > uvs.z || IN.texcoord.y < uvs.y || IN.texcoord.y > uvs.w)
                    c.a = 0;

                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
    SubShader
    {
        // Fallback Shader. This does all the distortion on the vertex shader instead of the fragment shader.
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
            #pragma multi_compile _ _ENABLE_VERTEX_SCALING
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
                float param : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };
     
            float4 _ClipPivot;
            float4x4 _ClipRotation;
            float4 _ClipRange;
            float4 _ClipArgs;
            float4 _Scale;

            v2f_ct vert(appdata_ct IN)
            {
                v2f_ct OUT;

                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;

                OUT.worldPos = _ClipPivot.xy - mul(unity_ObjectToWorld, IN.vertex).xy;
                OUT.worldPos = mul(_ClipRotation, float4(OUT.worldPos, 0, 0)).xy * _ClipRange.zw + _ClipRange.xy;

                fixed deepness = _ClipArgs.y;
                fixed range = max(0.1, _ClipArgs.z);

                fixed factor = abs(OUT.worldPos.y) - 1;
                fixed normalizedSteepness = saturate(factor / range);
                fixed interpolatedSteepness = pow(normalizedSteepness, 2);
                fixed distortion = interpolatedSteepness * deepness * sign(OUT.worldPos.y);

                IN.vertex.y += distortion;

                #ifdef UNITY_INSTANCING_ENABLED
                    IN.vertex.xy *= _Flip;
                #endif

                OUT.vertex = UnityObjectToClipPos(IN.vertex);

                #if defined(_ENABLE_VERTEX_SCALING)
                    OUT.vertex.xy *= _Scale.xy;
                #endif

                #ifdef PIXELSNAP_ON
                    OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 frag(v2f_ct IN) : SV_Target
            {
                // Softness factor
                fixed2 factor = float2(_Scale.z, _Scale.w) - float2(IN.worldPos.y, IN.worldPos.y * -1) * _ClipArgs.x;
                fixed fade = clamp(min(factor.x, factor.y), 0, 1);

                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                c.a *= fade;
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}
 

