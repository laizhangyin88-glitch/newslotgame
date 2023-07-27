//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Fireball Shader Green Updated"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_Color ("Tint", Color) = (1, 1, 1, 1)
AnimatedZoomUV_AnimatedZoomUV_Zoom_1("AnimatedZoomUV_AnimatedZoomUV_Zoom_1", Range(0.2, 4)) = 1.066
AnimatedZoomUV_AnimatedZoomUV_PosX_1("AnimatedZoomUV_AnimatedZoomUV_PosX_1", Range(-1, 2)) = 0.5
AnimatedZoomUV_AnimatedZoomUV_PosY_1("AnimatedZoomUV_AnimatedZoomUV_PosY_1", Range(-1, 2)) = 0.5
AnimatedZoomUV_AnimatedZoomUV_Intensity_1("AnimatedZoomUV_AnimatedZoomUV_Intensity_1", Range(0, 4)) = 0.462
AnimatedZoomUV_AnimatedZoomUV_Speed_1("AnimatedZoomUV_AnimatedZoomUV_Speed_1", Range(-10, 10)) = 1.904
AnimatedMouvementUV_X_1("AnimatedMouvementUV_X_1", Range(-1, 1)) = 0
AnimatedMouvementUV_Y_1("AnimatedMouvementUV_Y_1", Range(-1, 1)) = 0
AnimatedMouvementUV_Speed_1("AnimatedMouvementUV_Speed_1", Range(-1, 1)) = 0.1
DistortionUV_WaveX_1("DistortionUV_WaveX_1", Range(0, 128)) = 44.788
DistortionUV_WaveY_1("DistortionUV_WaveY_1", Range(0, 128)) = 47.166
DistortionUV_DistanceX_1("DistortionUV_DistanceX_1", Range(0, 1)) = 0.228
DistortionUV_DistanceY_1("DistortionUV_DistanceY_1", Range(0, 1)) = 0.228
DistortionUV_Speed_1("DistortionUV_Speed_1", Range(-2, 2)) = 1
_SourceNewTex_1("_SourceNewTex_1(RGB)", 2D) = "white" { }
_TurnTransparent_Speed_1("_TurnTransparent_Speed_1", Range(-8, 8)) = -2.825
_AlphaIntensity_Fade_1("_AlphaIntensity_Fade_1", Range(0, 6)) = 0.656
_CircleFade_PosX_1("_CircleFade_PosX_1", Range(-1, 2)) = 0.5
_CircleFade_PosY_1("_CircleFade_PosY_1", Range(-1, 2)) = 0.5
_CircleFade_Size_1("_CircleFade_Size_1", Range(-1, 1)) = 0.3
_CircleFade_Dist_1("_CircleFade_Dist_1", Range(0, 1)) = 0.2
_NewTex_2("NewTex_2(RGB)", 2D) = "white" { }
_SourceNewTex_2("_SourceNewTex_2(RGB)", 2D) = "white" { }
_ShadowLight_Precision_1("_ShadowLight_Precision_1(Deprecated)", Range(1, 32)) = 12.383
_ShadowLight_Size_1("_ShadowLight_Size_1", Range(0, 16)) = 1.631
_ShadowLight_Color_1("_ShadowLight_Color_1", COLOR) = (1,0,0,1)
_ShadowLight_Intensity_1("_ShadowLight_Intensity_1", Range(0, 4)) = 2.187
_ShadowLight_PosX_1("_ShadowLight_PosX_1", Range(-1, 1)) = 0
_ShadowLight_PosY_1("_ShadowLight_PosY_1", Range(-1, 1)) = 0
_ShadowLight_NoSprite_1("_ShadowLight_NoSprite_1", Range(0, 1)) = 0.661
_HdrCreate_Value_1("_HdrCreate_Value_1", Range(0, 4)) = 2.956
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
_Mask2RGBA_Fade_1("_Mask2RGBA_Fade_1", Range(0, 1)) = 0
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
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off 

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
float AnimatedZoomUV_AnimatedZoomUV_Zoom_1;
float AnimatedZoomUV_AnimatedZoomUV_PosX_1;
float AnimatedZoomUV_AnimatedZoomUV_PosY_1;
float AnimatedZoomUV_AnimatedZoomUV_Intensity_1;
float AnimatedZoomUV_AnimatedZoomUV_Speed_1;
float AnimatedMouvementUV_X_1;
float AnimatedMouvementUV_Y_1;
float AnimatedMouvementUV_Speed_1;
float DistortionUV_WaveX_1;
float DistortionUV_WaveY_1;
float DistortionUV_DistanceX_1;
float DistortionUV_DistanceY_1;
float DistortionUV_Speed_1;
sampler2D _SourceNewTex_1;
float _TurnTransparent_Speed_1;
float _AlphaIntensity_Fade_1;
float _CircleFade_PosX_1;
float _CircleFade_PosY_1;
float _CircleFade_Size_1;
float _CircleFade_Dist_1;
sampler2D _NewTex_2;
sampler2D _SourceNewTex_2;
float _ShadowLight_Precision_1;
float _ShadowLight_Size_1;
float4 _ShadowLight_Color_1;
float _ShadowLight_Intensity_1;
float _ShadowLight_PosX_1;
float _ShadowLight_PosY_1;
float _ShadowLight_NoSprite_1;
float _HdrCreate_Value_1;
sampler2D _NewTex_1;
float _Mask2RGBA_Fade_1;
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
float4 Circle_Fade(float4 txt, float2 uv, float posX, float posY, float Size, float Smooth)
{
float2 center = float2(posX, posY);
float dist = 1.0 - smoothstep(Size, Size + Smooth, length(center - uv));
txt.a *= dist;
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
float2 AnimatedMouvementUV(float2 uv, float offsetx, float offsety, float speed)
{
speed *=_Time*50;
uv += float2(offsetx, offsety)*speed;
uv = fmod(uv,1);
return uv;
}
float4 ColorTurnTransparent(float2 uv, sampler2D txt, float speed)
{
float4 txt1=tex2D(txt,uv);
float2 tuv = uv;
uv *= 2.5;
float time = (_Time/4)*speed;
float a = time * 50;
float n = sin(a + 2.0 * uv.x) + sin(a - 2.0 * uv.x) + sin(a + 2.0 * uv.y) + sin(a + 5.0 * uv.y);
n = fmod(((5.0 + n) / 5.0), 1.0);
n += tex2D(txt, tuv).r * 0.61 + tex2D(txt, tuv).g * 0.4 + tex2D(txt, tuv).b * 0.2;
n=fmod(n,1.0);
float tx = n * 6.0;
float r = clamp(tx - 2.0, 0.0, 1.0) + clamp(2.0 - tx, 0.0, 1.0);
float4 sortie=float4(1.0, 1.0, 1.0,r);
sortie.rgb=1-sortie.a;
sortie.a*=txt1.a*r;
return sortie; 
}
float4 HdrCreate(float4 txt,float value)
{
if (txt.r>0.98) txt.r=2;
if (txt.g>0.98) txt.g=2;
if (txt.b>0.98) txt.b=2;
return lerp(saturate(txt),txt, value);
}
float2 AnimatedZoomUV(float2 uv, float zoom, float posx, float posy, float radius, float speed)
{
float2 center = float2(posx, posy);
uv -= center;
zoom -= radius * 0.1;
zoom += sin(_Time * speed * 20) * 0.1 * radius;
uv = uv * zoom;
uv += center;
return uv;
}

float4 AlphaIntensity(float4 txt,float fade)
{
if (txt.a < 1) txt.a = txt.a*fade;
return txt;
}

float4 ShadowLight(sampler2D source, float2 uv, float precision, float size, float4 color, float intensity, float posx, float posy,float fade)
{
const int samples = 3;
const int samples2 = 1;
float4 ret = float4(0, 0, 0, 0);
float count = 0;
for (int iy = -samples2; iy < samples2; iy++)
{
for (int ix = -samples2; ix < samples2; ix++)
{
float2 uv2 = float2(ix, iy);
uv2 /= samples;
uv2 *= size*0.1;
uv2 += float2(-posx,posy);
uv2 = saturate(uv+uv2);
ret += tex2D(source, uv2);
count++;
}
}
ret = lerp(float4(0, 0, 0, 0), ret / count, intensity);
ret.rgb = color.rgb;
float4 m = ret;
float4 b = tex2D(source, uv);
ret = lerp(ret, b, b.a);
ret = lerp(m,ret,fade);
return ret;
}
float4 frag (v2f i) : COLOR
{
float2 AnimatedZoomUV_1 = AnimatedZoomUV(i.texcoord,AnimatedZoomUV_AnimatedZoomUV_Zoom_1,AnimatedZoomUV_AnimatedZoomUV_PosX_1,AnimatedZoomUV_AnimatedZoomUV_PosY_1,AnimatedZoomUV_AnimatedZoomUV_Intensity_1,AnimatedZoomUV_AnimatedZoomUV_Speed_1);
float2 AnimatedMouvementUV_1 = AnimatedMouvementUV(AnimatedZoomUV_1,AnimatedMouvementUV_X_1,AnimatedMouvementUV_Y_1,AnimatedMouvementUV_Speed_1);
float2 DistortionUV_1 = DistortionUV(AnimatedMouvementUV_1,DistortionUV_WaveX_1,DistortionUV_WaveY_1,DistortionUV_DistanceX_1,DistortionUV_DistanceY_1,DistortionUV_Speed_1);
float4 _TurnTransparent_1 = ColorTurnTransparent(DistortionUV_1,_SourceNewTex_1,_TurnTransparent_Speed_1);
float4 AlphaIntensity_1 = AlphaIntensity(_TurnTransparent_1,_AlphaIntensity_Fade_1);
float4 _CircleFade_1 = Circle_Fade(AlphaIntensity_1,i.texcoord,_CircleFade_PosX_1,_CircleFade_PosY_1,_CircleFade_Size_1,_CircleFade_Dist_1);
float4 NewTex_2 = tex2D(_NewTex_2,AnimatedZoomUV_1);
float4 _ShadowLight_1 = ShadowLight(_SourceNewTex_2,DistortionUV_1,_ShadowLight_Precision_1,_ShadowLight_Size_1,_ShadowLight_Color_1,_ShadowLight_Intensity_1,_ShadowLight_PosX_1,_ShadowLight_PosY_1,_ShadowLight_NoSprite_1);
float4 HdrCreate_1 = HdrCreate(_ShadowLight_1,_HdrCreate_Value_1);
float4 NewTex_1 = tex2D(_NewTex_1, i.texcoord);
float4 Mask2RGBA_1 = lerp(NewTex_2,HdrCreate_1, lerp(NewTex_1.r, 1 - NewTex_1.r ,_Mask2RGBA_Fade_1));
float4 OperationBlend_1 = OperationBlend(_CircleFade_1, Mask2RGBA_1, _OperationBlend_Fade_1); 
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
