//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/WDE Flame distortion"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_NewTex_4("NewTex_4(RGB)", 2D) = "white" { }
ResizeUV_X_1("ResizeUV_X_1", Range(-1, 1)) = 0
ResizeUV_Y_1("ResizeUV_Y_1", Range(-1, 1)) = 0
ResizeUV_ZoomX_1("ResizeUV_ZoomX_1", Range(0.1, 3)) = 0.5
ResizeUV_ZoomY_1("ResizeUV_ZoomY_1", Range(0.1, 3)) = 1
AnimatedMouvementUV_X_1("AnimatedMouvementUV_X_1", Range(-1, 1)) = 0
AnimatedMouvementUV_Y_1("AnimatedMouvementUV_Y_1", Range(-1, 1)) = 0.1
AnimatedMouvementUV_Speed_1("AnimatedMouvementUV_Speed_1", Range(-1, 1)) = -0.25
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
ResizeUV_X_2("ResizeUV_X_2", Range(-1, 1)) = 0
ResizeUV_Y_2("ResizeUV_Y_2", Range(-1, 1)) = 0
ResizeUV_ZoomX_2("ResizeUV_ZoomX_2", Range(0.1, 3)) = 1
ResizeUV_ZoomY_2("ResizeUV_ZoomY_2", Range(0.1, 3)) = 1.5
AnimatedMouvementUV_X_2("AnimatedMouvementUV_X_2", Range(-1, 1)) = 0
AnimatedMouvementUV_Y_2("AnimatedMouvementUV_Y_2", Range(-1, 1)) = 0.15
AnimatedMouvementUV_Speed_2("AnimatedMouvementUV_Speed_2", Range(-1, 1)) = -0.5
_NewTex_2("NewTex_2(RGB)", 2D) = "white" { }
_Add_Fade_2("_Add_Fade_2", Range(0, 4)) = 1
_MaskRGBA_Fade_1("_MaskRGBA_Fade_1", Range(0, 1)) = 0
_NewTex_3("NewTex_3(RGB)", 2D) = "white" { }
_FillColor_Color_1("_FillColor_Color_1", COLOR) = (1,1,1,1)
_ThresholdSmooth_Value_1("_ThresholdSmooth_Value_1", Range(-1, 2)) = 0.387
_ThresholdSmooth_Smooth_1("_ThresholdSmooth_Smooth_1", Range(0, 1)) = 0
_MaskRGBA_Fade_2("_MaskRGBA_Fade_2", Range(0, 1)) = 0
_Add_Fade_1("_Add_Fade_1", Range(0, 4)) = 1
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
sampler2D _NewTex_4;
float ResizeUV_X_1;
float ResizeUV_Y_1;
float ResizeUV_ZoomX_1;
float ResizeUV_ZoomY_1;
float AnimatedMouvementUV_X_1;
float AnimatedMouvementUV_Y_1;
float AnimatedMouvementUV_Speed_1;
sampler2D _NewTex_1;
float ResizeUV_X_2;
float ResizeUV_Y_2;
float ResizeUV_ZoomX_2;
float ResizeUV_ZoomY_2;
float AnimatedMouvementUV_X_2;
float AnimatedMouvementUV_Y_2;
float AnimatedMouvementUV_Speed_2;
sampler2D _NewTex_2;
float _Add_Fade_2;
float _MaskRGBA_Fade_1;
sampler2D _NewTex_3;
float4 _FillColor_Color_1;
float _ThresholdSmooth_Value_1;
float _ThresholdSmooth_Smooth_1;
float _MaskRGBA_Fade_2;
float _Add_Fade_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float4 UniColor(float4 txt, float4 color)
{
txt.rgb = lerp(txt.rgb,color.rgb,color.a);
return txt;
}
float4 ThresholdSmooth(float4 txt, float value, float smooth)
{
float l = (txt.x + txt.y + txt.z) * 0.33;
txt.rgb = smoothstep(value, value + smooth, l);
return txt;
}
float2 AnimatedMouvementUV(float2 uv, float offsetx, float offsety, float speed)
{
speed *=_Time*50;
uv += float2(offsetx, offsety)*speed;
uv = fmod(uv,1);
return uv;
}
float2 ResizeUV(float2 uv, float offsetx, float offsety, float zoomx, float zoomy)
{
uv += float2(offsetx, offsety);
uv = fmod(uv * float2(zoomx*zoomx, zoomy*zoomy), 1);
return uv;
}

float2 ResizeUVClamp(float2 uv, float offsetx, float offsety, float zoomx, float zoomy)
{
uv += float2(offsetx, offsety);
uv = fmod(clamp(uv * float2(zoomx*zoomx, zoomy*zoomy), 0.0001, 0.9999), 1);
return uv;
}
float4 frag (v2f i) : COLOR
{
float4 NewTex_4 = tex2D(_NewTex_4, i.texcoord);
float2 ResizeUV_1 = ResizeUV(i.texcoord,ResizeUV_X_1,ResizeUV_Y_1,ResizeUV_ZoomX_1,ResizeUV_ZoomY_1);
float2 AnimatedMouvementUV_1 = AnimatedMouvementUV(ResizeUV_1,AnimatedMouvementUV_X_1,AnimatedMouvementUV_Y_1,AnimatedMouvementUV_Speed_1);
float4 NewTex_1 = tex2D(_NewTex_1,AnimatedMouvementUV_1);
float4 _ThresholdSmooth_3 = ThresholdSmooth(NewTex_1,0.5,0.9);
float2 ResizeUV_2 = ResizeUV(i.texcoord,ResizeUV_X_2,ResizeUV_Y_2,ResizeUV_ZoomX_2,ResizeUV_ZoomY_2);
float2 AnimatedMouvementUV_2 = AnimatedMouvementUV(ResizeUV_2,AnimatedMouvementUV_X_2,AnimatedMouvementUV_Y_2,AnimatedMouvementUV_Speed_2);
float4 NewTex_2 = tex2D(_NewTex_2,AnimatedMouvementUV_2);
float4 _ThresholdSmooth_2 = ThresholdSmooth(NewTex_2,0.3,0.9);
_ThresholdSmooth_3 = lerp(_ThresholdSmooth_3,_ThresholdSmooth_3*_ThresholdSmooth_3.a + _ThresholdSmooth_2*_ThresholdSmooth_2.a,_Add_Fade_2);
float4 MaskRGBA_1=NewTex_4;
MaskRGBA_1.a = lerp(_ThresholdSmooth_3.r * NewTex_4.a, (1 - _ThresholdSmooth_3.r) * NewTex_4.a,_MaskRGBA_Fade_1);
float4 NewTex_3 = tex2D(_NewTex_3, i.texcoord);
float4 FillColor_1 = UniColor(NewTex_3,_FillColor_Color_1);
_ThresholdSmooth_3 = lerp(_ThresholdSmooth_3,_ThresholdSmooth_3*_ThresholdSmooth_3.a + _ThresholdSmooth_2*_ThresholdSmooth_2.a,_Add_Fade_2);
float4 _ThresholdSmooth_1 = ThresholdSmooth(_ThresholdSmooth_3,_ThresholdSmooth_Value_1,_ThresholdSmooth_Smooth_1);
float4 MaskRGBA_2=FillColor_1;
MaskRGBA_2.a = lerp(_ThresholdSmooth_1.r * FillColor_1.a, (1 - _ThresholdSmooth_1.r) * FillColor_1.a,_MaskRGBA_Fade_2);
MaskRGBA_1 = lerp(MaskRGBA_1,MaskRGBA_1*MaskRGBA_1.a + MaskRGBA_2*MaskRGBA_2.a,_Add_Fade_1 * MaskRGBA_2.a);
float4 FinalResult = MaskRGBA_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
