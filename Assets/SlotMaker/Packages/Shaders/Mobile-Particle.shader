// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "SlotMaker2/Particles/Default"
{
    Properties
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Blending)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("SrcBlend", Int) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("DstBlend", Int) = 10

        [Header(Clipping)]
        [KeywordEnum(None, Depth1, Depth2, Depth3)] _Clip ("Clipping Depth", Float) = 0
        [Toggle(_ENABLE_TEXTURE_MASK)] _EnableTextureMask ("Enable Texture Mask", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend [_SrcBlend] [_DstBlend]

        Pass
        {
        CGPROGRAM
            #pragma vertex SlotsParticleVert
            #pragma fragment SlotsParticleFrag
            #pragma target 2.0
            #pragma multi_compile __ _ENABLE_TEXTURE_MASK
            #pragma multi_compile _CLIP_NONE _CLIP_DEPTH1 _CLIP_DEPTH2 _CLIP_DEPTH3
            #include "UnityCG.cginc"
            #include "SlotsClipping.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            slots_v2f SlotsParticleVert(slots_appdata_t IN)
            {
                slots_v2f OUT;

                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;
                ClippingVert(IN, OUT);

                return OUT;
            }

            fixed4 SlotsParticleFrag(slots_v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                c.a *= ClippingFrag(IN);
                return c;
            }
        ENDCG
        }
    }
}
