//////////////////////////////////////////////
/// 2DxFX v3 - by VETASOFT 2018 //
//////////////////////////////////////////////


//////////////////////////////////////////////

Shader "CVSTA/UI/Distortion"
{
    Properties
    {
    [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
    _Color ("Tint", Color) = (1,1,1,1)

    DistortionUV_WaveX_1("DistortionUV_WaveX_1", Range(0, 128)) = 10
    DistortionUV_WaveY_1("DistortionUV_WaveY_1", Range(0, 128)) = 10
    DistortionUV_DistanceX_1("DistortionUV_DistanceX_1", Range(0, 1)) = 0.3
    DistortionUV_DistanceY_1("DistortionUV_DistanceY_1", Range(0, 1)) = 0.3
    DistortionUV_Speed_1("DistortionUV_Speed_1", Range(-2, 4)) = 1
    _Effect_Fade("_Effect_Fade", Range(0, 1)) = 1
    _SpriteFade("SpriteFade", Range(0, 1)) = 1.0

    // required for UI.Mask
    [HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
    [HideInInspector]_Stencil("Stencil ID", Float) = 0
    [HideInInspector]_StencilOp("Stencil Operation", Float) = 0
    [HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
    [HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
    [HideInInspector]_ColorMask("Color Mask", Float) = 15

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
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]


    // required for UI.Mask
    Stencil
    {
    Ref [_Stencil]
    Comp [_StencilComp]
    Pass [_StencilOp]
    ReadMask [_StencilReadMask]
    WriteMask [_StencilWriteMask]
    }

    Pass
    {
        Name "Default"
    CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma fragmentoption ARB_precision_hint_fastest
        #pragma target 2.0

        #include "UnityCG.cginc"
        #include "UnityUI.cginc"

        #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
        #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

        struct appdata_t
        {
            float4 vertex   : POSITION;
            float4 color    : COLOR;
            float2 texcoord : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID            
        };

        struct v2f
        {
            float4 vertex   : SV_POSITION;
            fixed4 color    : COLOR;
            float2 texcoord  : TEXCOORD0;
            float4 worldPosition : TEXCOORD1;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        sampler2D _MainTex;
        float _SpriteFade;
        float DistortionUV_WaveX_1;
        float DistortionUV_WaveY_1;
        float DistortionUV_DistanceX_1;
        float DistortionUV_DistanceY_1;
        float DistortionUV_Speed_1;
        float _Effect_Fade;
        float4 _ClipRect;
        fixed4 _Color;
        float4 _MainTex_ST;

        v2f vert(appdata_t v)
        {
            v2f OUT;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
            OUT.worldPosition = v.vertex;
            OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

            OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

            OUT.color = v.color * _Color;
            return OUT;
        }


        float2 DistortionUV(float2 p, float WaveX, float WaveY, float DistanceX, float DistanceY, float Speed)
        {
            Speed *=_Time*100;
            p.x= p.x+sin(p.y*WaveX + Speed)*DistanceX*0.05;
            p.y= p.y+cos(p.x*WaveY + Speed)*DistanceY*0.05;
            
            return p;
        }

        float4 frag (v2f i) : COLOR
        {
            float2 DistortionUV_1 = DistortionUV(i.texcoord,DistortionUV_WaveX_1,DistortionUV_WaveY_1,DistortionUV_DistanceX_1,DistortionUV_DistanceY_1,DistortionUV_Speed_1);
            i.texcoord = lerp(i.texcoord,DistortionUV_1,_Effect_Fade);

            float4 _MainTex_1 = tex2D(_MainTex,i.texcoord);
            float4 FinalResult = _MainTex_1;

            FinalResult.rgb *= i.color.rgb;
            FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;

            #ifdef UNITY_UI_CLIP_RECT
            FinalResult.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
            #endif

            #ifdef UNITY_UI_ALPHACLIP
            clip (FinalResult.a - 0.001);
            #endif

            return FinalResult;
        }

    ENDCG
    }
    }
    Fallback "Sprites/Default"
}
