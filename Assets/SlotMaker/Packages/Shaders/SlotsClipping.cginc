#ifndef SLOTS_CLIPPING_INCLUDED
#define SLOTS_CLIPPING_INCLUDED
 
struct slots_appdata_t
{
    float4 vertex   : POSITION;
    float4 color    : COLOR;
    float2 texcoord : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct slots_v2f
{
    float4 vertex   : SV_POSITION;
    fixed4 color    : COLOR;
    float2 texcoord : TEXCOORD0;
#if defined(_CLIP_DEPTH1) || defined(_CLIP_DEPTH2) || defined(_CLIP_DEPTH3)
    float2 worldPos0 : TEXCOORD1;
#if defined(_CLIP_DEPTH2) || defined(_CLIP_DEPTH3)
    float2 worldPos1 : TEXCOORD2;
#if defined(_CLIP_DEPTH3)
    float2 worldPos2 : TEXCOORD3;
#endif
#endif
#endif
    UNITY_VERTEX_OUTPUT_STEREO
};

#if defined(_CLIP_DEPTH1) || defined(_CLIP_DEPTH2) || defined(_CLIP_DEPTH3)
    float4x4 _ClipMatrix0;
    float4 _ClipArgs0;
#if defined(_ENABLE_TEXTURE_MASK)
    sampler2D _TextureMask;
#endif
#if defined(_CLIP_DEPTH2) || defined(_CLIP_DEPTH3)
    float4x4 _ClipMatrix1;
    float4 _ClipArgs1;
#if defined(_CLIP_DEPTH3)
    float4x4 _ClipMatrix2;
    float4 _ClipArgs2;
#endif
#endif
#endif

#if defined(_CLIP_NONE)
    #define ClippingVert(IN, OUT)
    #define ClippingFrag(IN) 1
#else
void ClippingVert(inout slots_appdata_t IN, inout slots_v2f OUT)
{
#if defined(_CLIP_DEPTH1) || defined(_CLIP_DEPTH2) || defined(_CLIP_DEPTH3)
    OUT.worldPos0 = _ClipArgs0.xy - mul(unity_ObjectToWorld, IN.vertex).xy;
    OUT.worldPos0 = mul(_ClipMatrix0, float4(OUT.worldPos0, 0, 0)).xy;
#if defined(_ENABLE_TEXTURE_MASK)
    OUT.worldPos0 = OUT.worldPos0 * -0.5 + float2(0.5, 0.5);
#endif
#if defined(_CLIP_DEPTH2) || defined(_CLIP_DEPTH3)
    OUT.worldPos1 = _ClipArgs1.xy - mul(unity_ObjectToWorld, IN.vertex).xy;
    OUT.worldPos1 = mul(_ClipMatrix1, float4(OUT.worldPos1, 0, 0)).xy;
#if defined(_CLIP_DEPTH3)
    OUT.worldPos2 = _ClipArgs2.xy - mul(unity_ObjectToWorld, IN.vertex).xy;
    OUT.worldPos2 = mul(_ClipMatrix2, float4(OUT.worldPos2, 0, 0)).xy;
#endif
#endif
#endif   
}

fixed ClippingFrag(inout slots_v2f IN)
{
    fixed2 factor = fixed2(1, 1);
    fixed fade = 1;
#if defined(_CLIP_DEPTH1) || defined(_CLIP_DEPTH2) || defined(_CLIP_DEPTH3)
#if !defined(_ENABLE_TEXTURE_MASK)
    factor = (float2(1, 1) - abs(IN.worldPos0)) * _ClipArgs0.zw;
    fade = min(factor.x, factor.y);
#else
    if (IN.worldPos0.x < 0 || IN.worldPos0.y < 0 || IN.worldPos0.x > 1 || IN.worldPos0.y > 1)
    {
        fade = 0;
    }
    else
    {
        fade = tex2D(_TextureMask, IN.worldPos0).a;
        fade = saturate((fade - _ClipArgs0.z) * _ClipArgs0.w);
    }
#endif
#if defined(_CLIP_DEPTH2) || defined(_CLIP_DEPTH3)
    factor = (float2(1, 1) - abs(IN.worldPos1)) * _ClipArgs1.zw;
    fade = min(fade, min(factor.x, factor.y));
#if defined(_CLIP_DEPTH3)
    factor = (float2(1, 1) - abs(IN.worldPos2)) * _ClipArgs2.zw;
    fade = min(fade, min(factor.x, factor.y));
#endif
#endif
#endif
    return saturate(fade);
}
#endif
 
#endif // SLOTS_CLIPPING_INCLUDED