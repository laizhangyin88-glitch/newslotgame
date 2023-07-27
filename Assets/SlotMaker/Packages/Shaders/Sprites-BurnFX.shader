Shader "Sprites/FX/Burn"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(FX)]
        _DissolveTex ("Dissolve Texture", 2D) = "white" {}
        _Edge ("Edge", Range(0.01, 0.5)) = 0.01
        [Toggle(RANDOM_DISSOLVE)] _UseRandomDissolve ("World Position Random", Float) = 1

        [Header(Edge Color)]
        [Toggle(EDGE_COLOR)] _UseEdgeColor ("Edge Color?", Float) = 1
        [HideIfDisabled(EDGE_COLOR)] [NoScaleOffset] _EdgeAroundRamp ("Edge Ramp", 2D) = "white" {}
        [HideIfDisabled(EDGE_COLOR)] _EdgeAround ("Edge Color Range", Range(0, 0.5)) = 0
        [HideIfDisabled(EDGE_COLOR)] _EdgeAroundPower ("Edge Color Power", Range(1, 5)) = 1
        [HideIfDisabled(EDGE_COLOR)] _EdgeAroundHDR ("Edge Color HDR", Range(1, 3)) = 1
        [HideIfDisabled(EDGE_COLOR)] _EdgeDistortion ("Edge Distortion", Range(0, 1)) = 0
        [HideIfDisabled(EDGE_COLOR)] _EdgeAvoid ("Edge Avoid", Range(0, 1)) = 0.15

        [Header(Rendering)]
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode ("Culling Mode", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("SrcBlend", Int) = 1.0
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("DstBlend", Int) = 10.0

        [Header(SpritesDefault)]
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

        Cull [_CullMode]
        Lighting Off
        ZWrite Off
        Blend [_SrcBlend] [_DstBlend]

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma shader_feature RANDOM_DISSOLVE
            #pragma shader_feature EDGE_COLOR

            #include "UnitySprites.cginc"

            struct _appdata
            {
                float4 vertex    : POSITION;
                fixed4 color     : COLOR;
                float2 texcoord  : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct _v2f
            {
                float4 vertex    : SV_POSITION;
                fixed4 color     : COLOR;
                float2 uv        : TEXCOORD0;
                float2 uv2       : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _DissolveTex;
            float4 _DissolveTex_ST;
            fixed _Edge;
            fixed _DissolveSize;

            #ifdef EDGE_COLOR
                sampler2D _EdgeAroundRamp;
                fixed _EdgeAround;
                float _EdgeAroundPower;
                float _EdgeAroundHDR;
                fixed _EdgeDistortion;
                fixed _EdgeAvoid;
            #endif

            _v2f vert(_appdata IN)
            {
                _v2f OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                OUT.color = IN.color * _Color * _RendererColor;
                OUT.uv = IN.texcoord;
                #ifdef RANDOM_DISSOLVE
                // World Pos UV
                OUT.uv2 = TRANSFORM_TEX(mul(unity_ObjectToWorld, IN.vertex), _DissolveTex);
                #else
                OUT.uv2 = TRANSFORM_TEX(IN.texcoord, _DissolveTex);
                #endif
                
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 frag(_v2f IN) : SV_Target
            {
                fixed4 c;
                fixed alpha;

                fixed progress = 1 - IN.color.a;
                if (progress < 0.01)
                {
                    c = tex2D(_MainTex, IN.uv) * IN.color;
                    alpha = 1;
                }
                else
                {
                    // Edge
                    fixed dissolve = tex2D(_DissolveTex, IN.uv2).r;
                    fixed edge = lerp(dissolve - _Edge, dissolve + _Edge, progress);
                    alpha = smoothstep(progress - _Edge, progress + _Edge, edge);
                    
                #ifdef EDGE_COLOR
                    // Edge Around Factor
                    fixed edgearound = lerp(dissolve - _EdgeAround, dissolve + _EdgeAround, progress);
                    edgearound = smoothstep(progress - _EdgeAround, progress + _EdgeAround, edgearound);
                    edgearound = pow(edgearound, _EdgeAroundPower);

                    // Edge Around Distortion
                    fixed distort = edgearound * _EdgeAvoid * alpha;
                    float2 uv = lerp(IN.uv, IN.uv + distort - _EdgeAvoid, progress * _EdgeDistortion);
                    
                    c = tex2D(_MainTex, uv);
                    c.rgb *= IN.color.rgb;

                    // Edge Arround Color
                    fixed3 ca = tex2D(_EdgeAroundRamp, fixed2(1 - edgearound, 0)).rgb;
                    ca = (c.rgb + ca) * ca * _EdgeAroundHDR;
                    c.rgb = lerp(ca, c.rgb, edgearound);
                #else
                    c = tex2D(_MainTex, IN.uv) * IN.color;
                #endif
                }

                c.a *= alpha;
                c.rgb *= c.a;

                return c;
            }
        ENDCG
        }
    }

    Fallback "Sprites/Default"
}
