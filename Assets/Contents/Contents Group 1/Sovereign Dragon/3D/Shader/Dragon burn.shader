//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Dragon burn"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
DistortionUV_WaveX_1("DistortionUV_WaveX_1", Range(0, 128)) = 100
DistortionUV_WaveY_1("DistortionUV_WaveY_1", Range(0, 128)) = 100
DistortionUV_DistanceX_1("DistortionUV_DistanceX_1", Range(0, 1)) = 0.3
DistortionUV_DistanceY_1("DistortionUV_DistanceY_1", Range(0, 1)) = 0.3
DistortionUV_Speed_1("DistortionUV_Speed_1", Range(-2, 2)) = 2
DistortionUV_WaveX_2("DistortionUV_WaveX_2", Range(0, 128)) = 50
DistortionUV_WaveY_2("DistortionUV_WaveY_2", Range(0, 128)) = 50
DistortionUV_DistanceX_2("DistortionUV_DistanceX_2", Range(0, 1)) = 0.5
DistortionUV_DistanceY_2("DistortionUV_DistanceY_2", Range(0, 1)) = 0.5
DistortionUV_Speed_2("DistortionUV_Speed_2", Range(-2, 2)) = 1
_PlasmaFX_Fade_1("_PlasmaFX_Fade_1", Range(0, 1)) = 0.43
_PlasmaFX_Speed_1("_PlasmaFX_Speed_1", Range(0, 1)) = 0
_ThresholdSmooth_Value_1("_ThresholdSmooth_Value_1", Range(-1, 2)) = 0.67
_ThresholdSmooth_Smooth_1("_ThresholdSmooth_Smooth_1", Range(0, 1)) = 0.21
_TintRGBA_Color_1("_TintRGBA_Color_1", COLOR) = (1,0.6079481,0,1)
_NewTex_2("NewTex_2(RGB)", 2D) = "white" { }
_ThresholdSmooth_Value_5("_ThresholdSmooth_Value_5", Range(-1, 2)) = 0
_ThresholdSmooth_Smooth_5("_ThresholdSmooth_Smooth_5", Range(0, 1)) = 0.793
_TurnBlackToAlpha_Fade_2("_TurnBlackToAlpha_Fade_2", Range(0, 1)) = 1
_NewTex_3("NewTex_3(RGB)", 2D) = "white" { }
_ThresholdSmooth_Value_4("_ThresholdSmooth_Value_4", Range(-1, 2)) = 0.19
_ThresholdSmooth_Smooth_4("_ThresholdSmooth_Smooth_4", Range(0, 1)) = 0.366
_TurnBlackToAlpha_Fade_1("_TurnBlackToAlpha_Fade_1", Range(0, 1)) = 1
_MaskAlpha_Fade_3("_MaskAlpha_Fade_3", Range(0, 1)) = 0
_MaskAlpha_Fade_4("_MaskAlpha_Fade_4", Range(0, 1)) = 0
DistortionUV_WaveX_4("DistortionUV_WaveX_4", Range(0, 128)) = 100
DistortionUV_WaveY_4("DistortionUV_WaveY_4", Range(0, 128)) = 200
DistortionUV_DistanceX_4("DistortionUV_DistanceX_4", Range(0, 1)) = 0.1
DistortionUV_DistanceY_4("DistortionUV_DistanceY_4", Range(0, 1)) = 0.25
DistortionUV_Speed_4("DistortionUV_Speed_4", Range(-2, 2)) = 3
DistortionUV_WaveX_3("DistortionUV_WaveX_3", Range(0, 128)) = 100
DistortionUV_WaveY_3("DistortionUV_WaveY_3", Range(0, 128)) = 100
DistortionUV_DistanceX_3("DistortionUV_DistanceX_3", Range(0, 1)) = 0.2
DistortionUV_DistanceY_3("DistortionUV_DistanceY_3", Range(0, 1)) = 0.7
DistortionUV_Speed_3("DistortionUV_Speed_3", Range(-2, 2)) = 1
_PlasmaFX_Fade_2("_PlasmaFX_Fade_2", Range(0, 1)) = 0.291
_PlasmaFX_Speed_2("_PlasmaFX_Speed_2", Range(0, 1)) = 1
_ThresholdSmooth_Value_2("_ThresholdSmooth_Value_2", Range(-1, 2)) = 0.773
_ThresholdSmooth_Smooth_2("_ThresholdSmooth_Smooth_2", Range(0, 1)) = 0.1
_TintRGBA_Color_2("_TintRGBA_Color_2", COLOR) = (1,0.388415,0,1)
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
_ThresholdSmooth_Value_3("_ThresholdSmooth_Value_3", Range(-1, 2)) = 0.821
_ThresholdSmooth_Smooth_3("_ThresholdSmooth_Smooth_3", Range(0, 1)) = 0.634
_TurnBlackToAlpha_Fade_3("_TurnBlackToAlpha_Fade_3", Range(0, 1)) = 1
_MaskAlpha_Fade_2("_MaskAlpha_Fade_2", Range(0, 1)) = 0
_MaskAlpha_Fade_1("_MaskAlpha_Fade_1", Range(0, 1)) = 0
_OperationBlend_Fade_1("_OperationBlend_Fade_1", Range(0, 1)) = 1
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

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
ZWrite Off Blend SrcAlpha One Cull Off 

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

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float _SpriteFade;
float DistortionUV_WaveX_1;
float DistortionUV_WaveY_1;
float DistortionUV_DistanceX_1;
float DistortionUV_DistanceY_1;
float DistortionUV_Speed_1;
float DistortionUV_WaveX_2;
float DistortionUV_WaveY_2;
float DistortionUV_DistanceX_2;
float DistortionUV_DistanceY_2;
float DistortionUV_Speed_2;
float _PlasmaFX_Fade_1;
float _PlasmaFX_Speed_1;
float _ThresholdSmooth_Value_1;
float _ThresholdSmooth_Smooth_1;
float4 _TintRGBA_Color_1;
sampler2D _NewTex_2;
float _ThresholdSmooth_Value_5;
float _ThresholdSmooth_Smooth_5;
float _TurnBlackToAlpha_Fade_2;
sampler2D _NewTex_3;
float _ThresholdSmooth_Value_4;
float _ThresholdSmooth_Smooth_4;
float _TurnBlackToAlpha_Fade_1;
float _MaskAlpha_Fade_3;
float _MaskAlpha_Fade_4;
float DistortionUV_WaveX_4;
float DistortionUV_WaveY_4;
float DistortionUV_DistanceX_4;
float DistortionUV_DistanceY_4;
float DistortionUV_Speed_4;
float DistortionUV_WaveX_3;
float DistortionUV_WaveY_3;
float DistortionUV_DistanceX_3;
float DistortionUV_DistanceY_3;
float DistortionUV_Speed_3;
float _PlasmaFX_Fade_2;
float _PlasmaFX_Speed_2;
float _ThresholdSmooth_Value_2;
float _ThresholdSmooth_Smooth_2;
float4 _TintRGBA_Color_2;
sampler2D _NewTex_1;
float _ThresholdSmooth_Value_3;
float _ThresholdSmooth_Smooth_3;
float _TurnBlackToAlpha_Fade_3;
float _MaskAlpha_Fade_2;
float _MaskAlpha_Fade_1;
float _OperationBlend_Fade_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float2 DistortionUV(float2 p, float WaveX, float WaveY, float DistanceX, float DistanceY, float Speed)
{
Speed *=_Time*100;
p.x= p.x+sin(p.y*WaveX + Speed)*DistanceX*0.05;
p.y= p.y+cos(p.x*WaveY + Speed)*DistanceY*0.05;
return p;
}
float4 TintRGBA(float4 txt, float4 color)
{
float3 tint = dot(txt.rgb, float3(.222, .707, .071));
tint.rgb *= color.rgb;
txt.rgb = lerp(txt.rgb,tint.rgb,color.a);
return txt;
}
inline float RBFXmod(float x,float modu)
{
return x - floor(x * (1.0 / modu)) * modu;
}

float3 RBFXrainbow(float t)
{
t= RBFXmod(t,1.0);
float tx = t * 8;
float r = clamp(tx - 4.0, 0.0, 1.0) + clamp(2.0 - tx, 0.0, 1.0);
float g = tx < 2.0 ? clamp(tx, 0.0, 1.0) : clamp(4.0 - tx, 0.0, 1.0);
float b = tx < 4.0 ? clamp(tx - 2.0, 0.0, 1.0) : clamp(6.0 - tx, 0.0, 1.0);
return float3(r, g, b);
}

float4 Plasma(float4 txt, float2 uv, float _Fade, float speed)
{
float _TimeX=_Time.y * speed;
float a = 1.1 + _TimeX * 2.25;
float b = 0.5 + _TimeX * 1.77;
float c = 8.4 + _TimeX * 1.58;
float d = 610 + _TimeX * 2.03;
float x1 = 2.0 * uv.x;
float n = sin(a + x1) + sin(b - x1) + sin(c + 2.0 * uv.y) + sin(d + 5.0 * uv.y);
n = RBFXmod(((5.0 + n) / 5.0), 1.0);
float4 nx=txt;
n += nx.r * 0.2 + nx.g * 0.4 + nx.b * 0.2;
float4 ret=float4(RBFXrainbow(n),txt.a);
return lerp(txt,ret,_Fade);
}
float4 ThresholdSmooth(float4 txt, float value, float smooth)
{
float l = (txt.x + txt.y + txt.z) * 0.33;
txt.rgb = smoothstep(value, value + smooth, l);
return txt;
}
float4 OperationBlend(float4 origin, float4 overlay, float blend)
{
float4 o = origin; 
o.a = overlay.a + origin.a * (1 - overlay.a);
o.rgb = (overlay.rgb * overlay.a + origin.rgb * origin.a * (1 - overlay.a)) * (o.a+0.0000001);
o.a = saturate(o.a);
o = lerp(origin, o, blend);
return o;
}

float4 TurnBlackToAlpha(float4 txt, float force, float fade)
{
float3 gs = dot(txt.rgb, float3(1., 1., 1.));
gs=saturate(gs);
return lerp(txt,float4(force*txt.rgb, gs.r), fade);
}

float4 frag (v2f i) : COLOR
{
float2 DistortionUV_1 = DistortionUV(i.texcoord,DistortionUV_WaveX_1,DistortionUV_WaveY_1,DistortionUV_DistanceX_1,DistortionUV_DistanceY_1,DistortionUV_Speed_1);
float2 DistortionUV_2 = DistortionUV(DistortionUV_1,DistortionUV_WaveX_2,DistortionUV_WaveY_2,DistortionUV_DistanceX_2,DistortionUV_DistanceY_2,DistortionUV_Speed_2);
float4 _PlasmaFX_1 = Plasma(float4(1,1,1,1),DistortionUV_2,_PlasmaFX_Fade_1,_PlasmaFX_Speed_1);
float4 _ThresholdSmooth_1 = ThresholdSmooth(_PlasmaFX_1,_ThresholdSmooth_Value_1,_ThresholdSmooth_Smooth_1);
float4 TintRGBA_1 = TintRGBA(_ThresholdSmooth_1,_TintRGBA_Color_1);
float4 NewTex_2 = tex2D(_NewTex_2, i.texcoord);
float4 _ThresholdSmooth_5 = ThresholdSmooth(NewTex_2,_ThresholdSmooth_Value_5,_ThresholdSmooth_Smooth_5);
float4 TurnBlackToAlpha_2 = TurnBlackToAlpha(_ThresholdSmooth_5,1,_TurnBlackToAlpha_Fade_2);
float4 NewTex_3 = tex2D(_NewTex_3, i.texcoord);
float4 _ThresholdSmooth_4 = ThresholdSmooth(NewTex_3,_ThresholdSmooth_Value_4,_ThresholdSmooth_Smooth_4);
float4 TurnBlackToAlpha_1 = TurnBlackToAlpha(_ThresholdSmooth_4,1,_TurnBlackToAlpha_Fade_1);
float4 MaskAlpha_3=TurnBlackToAlpha_2;
MaskAlpha_3.a = lerp(TurnBlackToAlpha_1.a * TurnBlackToAlpha_2.a, (1 - TurnBlackToAlpha_1.a) * TurnBlackToAlpha_2.a,_MaskAlpha_Fade_3);
float4 MaskAlpha_4=TintRGBA_1;
MaskAlpha_4.a = lerp(MaskAlpha_3.a * TintRGBA_1.a, (1 - MaskAlpha_3.a) * TintRGBA_1.a,_MaskAlpha_Fade_4);
float2 DistortionUV_4 = DistortionUV(i.texcoord,DistortionUV_WaveX_4,DistortionUV_WaveY_4,DistortionUV_DistanceX_4,DistortionUV_DistanceY_4,DistortionUV_Speed_4);
float2 DistortionUV_3 = DistortionUV(DistortionUV_4,DistortionUV_WaveX_3,DistortionUV_WaveY_3,DistortionUV_DistanceX_3,DistortionUV_DistanceY_3,DistortionUV_Speed_3);
float4 _PlasmaFX_2 = Plasma(float4(1,1,1,1),DistortionUV_3,_PlasmaFX_Fade_2,_PlasmaFX_Speed_2);
float4 _ThresholdSmooth_2 = ThresholdSmooth(_PlasmaFX_2,_ThresholdSmooth_Value_2,_ThresholdSmooth_Smooth_2);
float4 TintRGBA_2 = TintRGBA(_ThresholdSmooth_2,_TintRGBA_Color_2);
float4 NewTex_1 = tex2D(_NewTex_1, i.texcoord);
float4 _ThresholdSmooth_3 = ThresholdSmooth(NewTex_1,_ThresholdSmooth_Value_3,_ThresholdSmooth_Smooth_3);
float4 TurnBlackToAlpha_3 = TurnBlackToAlpha(_ThresholdSmooth_3,1,_TurnBlackToAlpha_Fade_3);
float4 MaskAlpha_2=TintRGBA_2;
MaskAlpha_2.a = lerp(TurnBlackToAlpha_3.a * TintRGBA_2.a, (1 - TurnBlackToAlpha_3.a) * TintRGBA_2.a,_MaskAlpha_Fade_2);
float4 MaskAlpha_1=MaskAlpha_2;
MaskAlpha_1.a = lerp(TurnBlackToAlpha_1.a * MaskAlpha_2.a, (1 - TurnBlackToAlpha_1.a) * MaskAlpha_2.a,_MaskAlpha_Fade_1);
float4 OperationBlend_1 = OperationBlend(MaskAlpha_4, MaskAlpha_1, _OperationBlend_Fade_1); 
float4 FinalResult = OperationBlend_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
