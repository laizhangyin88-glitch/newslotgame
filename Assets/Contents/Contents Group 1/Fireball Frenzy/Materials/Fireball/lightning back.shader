//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/lightning back"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_Color ("Tint", Color) = (1,1,1,1)
[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
_ShadowLight_Precision_1("_ShadowLight_Precision_1(Deprecated)", Range(1, 32)) = 1
_ShadowLight_Size_1("_ShadowLight_Size_1", Range(0, 16)) = 3.236
_ShadowLight_Color_1("_ShadowLight_Color_1", COLOR) = (0,0.7,1,1)
_ShadowLight_Intensity_1("_ShadowLight_Intensity_1", Range(0, 4)) = 1.793
_ShadowLight_PosX_1("_ShadowLight_PosX_1", Range(-1, 1)) = 0
_ShadowLight_PosY_1("_ShadowLight_PosY_1", Range(-1, 1)) = 0
_ShadowLight_NoSprite_1("_ShadowLight_NoSprite_1", Range(0, 1)) = 0.993
FishEyeUV_Size_1("FishEyeUV_Size_1", Range(0, 0.5)) = 0.244
ZoomUV_Zoom_1("ZoomUV_Zoom_1", Range(0.2, 4)) = 1.628
ZoomUV_PosX_1("ZoomUV_PosX_1", Range(-3, 3)) = 0.5
ZoomUV_PosY_1("ZoomUV_PosY_1", Range(-3, 3)) =0.5
KaleidoscopeUV_PosX_1("KaleidoscopeUV_PosX_1",  Range(-2, 2)) = 0.5
KaleidoscopeUV_PosY_1("KaleidoscopeUV_PosY_1",  Range(-2, 2)) = 0.5
KaleidoscopeUV_Number_1("KaleidoscopeUV_Number_1", Range(0, 6)) = 1
_GenerateLightning_PosX_1("_GenerateLightning_PosX_1", Range(-2, 2)) = 0.5
_GenerateLightning_PosY_1("_GenerateLightning_PosY_1", Range(-2, 2)) = 0.5
_GenerateLightning_Size_1("_GenerateLightning_Size_1", Range( 1, 8)) = 2.6
_GenerateLightning_Number_1("_GenerateLightning_Number_1", Range(2, 16)) = 7.406
_GenerateLightning_Speed_1("_GenerateLightning_Speed_1", Range( 0, 8)) = 1
_Add_Fade_1("_Add_Fade_1", Range(0, 4)) = 1
_CircleFade_PosX_1("_CircleFade_PosX_1", Range(-1, 2)) = 0.5
_CircleFade_PosY_1("_CircleFade_PosY_1", Range(-1, 2)) = 0.5
_CircleFade_Size_1("_CircleFade_Size_1", Range(-1, 1)) = 0.207
_CircleFade_Dist_1("_CircleFade_Dist_1", Range(0, 1)) = 0.428
_PremadeGradients_Offset_1("_PremadeGradients_Offset_1", Range(-1, 1)) =0
_PremadeGradients_Fade_1("_PremadeGradients_Fade_1", Range(0, 1)) =1
_PremadeGradients_Speed_1("_PremadeGradients_Speed_1", Range(-2, 2)) =0
_HdrCreate_Value_1("_HdrCreate_Value_1", Range(0, 4)) = 1
_OperationBlend_Fade_1("_OperationBlend_Fade_1", Range(0, 1)) = 1
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

}

SubShader
{
Tags
{
"Queue" = "Transparent"
"IgnoreProjector" = "True"
"RenderType" = "Transparent"
"PreviewType" = "Plane"
"CanUseSpriteAtlas" = "True"

}

Cull Off
Lighting Off
ZWrite Off
Blend SrcAlpha OneMinusSrcAlpha


CGPROGRAM

#pragma surface surf Lambert vertex:vert  nolightmap nodynlightmap keepalpha noinstancing
#pragma multi_compile _ PIXELSNAP_ON
#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
#include "UnitySprites.cginc"
struct Input
{
float2 uv_MainTex;
float4 color;
};

float _SpriteFade;
float _ShadowLight_Precision_1;
float _ShadowLight_Size_1;
float4 _ShadowLight_Color_1;
float _ShadowLight_Intensity_1;
float _ShadowLight_PosX_1;
float _ShadowLight_PosY_1;
float _ShadowLight_NoSprite_1;
float FishEyeUV_Size_1;
float ZoomUV_Zoom_1;
float ZoomUV_PosX_1;
float ZoomUV_PosY_1;
float KaleidoscopeUV_PosX_1;
float KaleidoscopeUV_PosY_1;
float KaleidoscopeUV_Number_1;
float _GenerateLightning_PosX_1;
float _GenerateLightning_PosY_1;
float _GenerateLightning_Size_1;
float _GenerateLightning_Number_1;
float _GenerateLightning_Speed_1;
float _Add_Fade_1;
float _CircleFade_PosX_1;
float _CircleFade_PosY_1;
float _CircleFade_Size_1;
float _CircleFade_Dist_1;
float _PremadeGradients_Offset_1;
float _PremadeGradients_Fade_1;
float _PremadeGradients_Speed_1;
float _HdrCreate_Value_1;
float _OperationBlend_Fade_1;

void vert(inout appdata_full v, out Input o)
{
v.vertex.xy *= _Flip.xy;
#if defined(PIXELSNAP_ON)
v.vertex = UnityPixelSnap (v.vertex);
#endif
UNITY_INITIALIZE_OUTPUT(Input, o);
o.color = v.color * _Color * _RendererColor;
}


float2 RotationUV(float2 uv, float rot, float posx, float posy, float speed)
{
rot=rot+(_Time*speed*360);
uv = uv - float2(posx, posy);
float angle = rot * 0.01744444;
float sinX = sin(angle);
float cosX = cos(angle);
float2x2 rotationMatrix = float2x2(cosX, -sinX, sinX, cosX);
uv = mul(uv, rotationMatrix) + float2(posx, posy);
return uv;
}
float2 ZoomUV(float2 uv, float zoom, float posx, float posy)
{
float2 center = float2(posx, posy);
uv -= center;
uv = uv * zoom;
uv += center;
return uv;
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
float4 Color_PreGradients(float4 rgba, float4 a, float4 b, float4 c, float4 d, float offset, float fade, float speed)
{
float gray = (rgba.r + rgba.g + rgba.b) / 3;
gray += offset+(speed*_Time*20);
float4 result = a + b * cos(6.28318 * (c * gray + d));
result.a = rgba.a;
result.rgb = lerp(rgba.rgb, result.rgb, fade);
return result;
}
float Lightning_Hash(float2 p)
{
float3 p2 = float3(p.xy, 1.0);
return frac(sin(dot(p2, float3(37.1, 61.7, 12.4)))*3758.5453123);
}

float Lightning_noise(in float2 p)
{
float2 i = floor(p);
float2 f = frac(p);
f *= f * (1.5 - .5*f);
return lerp(lerp(Lightning_Hash(i + float2(0., 0.)), Lightning_Hash(i + float2(1., 0.)), f.x),
lerp(Lightning_Hash(i + float2(0., 1.)), Lightning_Hash(i + float2(1., 1.)), f.x),
f.y);
}

float Lightning_fbm(float2 p)
{
float v = 0.0;
v += Lightning_noise(p*1.0)*.5;
v += Lightning_noise(p*2.)*.25;
v += Lightning_noise(p*4.)*.125;
v += Lightning_noise(p*8.)*.0625;
return v;
}

float4 Generate_Lightning(float2 uv, float2 uvx, float posx, float posy, float size, float number, float speed, float black)
{
uv -= float2(posx, posy);
uv *= size;
uv -= float2(posx, posy);
float rot = (uv.x*uvx.x + uv.y*uvx.y);
float time = _Time * 20 * speed;
float4 r = float4(0, 0, 0, 0);
for (int i = 1; i < number; ++i)
{
float t = abs(.750 / ((rot + Lightning_fbm(uv + (time*5.75) / float(i)))*65.));
r += t *0.5;
}
r = saturate(r);
r.a = saturate(r.r + black);
return r;

}
float2 KaleidoscopeUV(float2 uv, float posx, float posy, float number)
{
uv = uv - float2(posx, posy);
float r = length(uv);
float a = abs(atan2(uv.y, uv.x));
float sides = number;
float tau = 3.1416;
a = fmod(a, tau / sides);
a = abs(a - tau / sides / 2.);
uv = r * float2(cos(a), sin(a));
return uv;
}
float2 FishEyeUV(float2 uv, float size)
{
float2 m = float2(0.5, 0.5);
float2 d = uv - m;
float r = sqrt(dot(d, d));
float power = (2.0 * 3.141592 / (2.0 * sqrt(dot(m, m)))) * (size+0.001);
float bind = sqrt(dot(m, m));
uv = m + normalize(d) * tan(r * power) * bind / tan(bind * power);
return uv;
}
float4 HdrCreate(float4 txt,float value)
{
if (txt.r>0.98) txt.r=2;
if (txt.g>0.98) txt.g=2;
if (txt.b>0.98) txt.b=2;
return lerp(saturate(txt),txt, value);
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
void surf(Input i, inout SurfaceOutput o)
{
float4 _ShadowLight_1 = ShadowLight(_MainTex,i.uv_MainTex,_ShadowLight_Precision_1,_ShadowLight_Size_1,_ShadowLight_Color_1,_ShadowLight_Intensity_1,_ShadowLight_PosX_1,_ShadowLight_PosY_1,_ShadowLight_NoSprite_1);
float2 FishEyeUV_1 = FishEyeUV(i.uv_MainTex,FishEyeUV_Size_1);
float2 ZoomUV_1 = ZoomUV(FishEyeUV_1,ZoomUV_Zoom_1,ZoomUV_PosX_1,ZoomUV_PosY_1);
float2 KaleidoscopeUV_1 = KaleidoscopeUV(ZoomUV_1,KaleidoscopeUV_PosX_1,KaleidoscopeUV_PosY_1,KaleidoscopeUV_Number_1);
float4 _GenerateLightning_1 = Generate_Lightning(KaleidoscopeUV_1,float2(0,1),_GenerateLightning_PosX_1,_GenerateLightning_PosY_1,_GenerateLightning_Size_1,_GenerateLightning_Number_1,_GenerateLightning_Speed_1,0);
float2 RotationUV_1 = RotationUV(KaleidoscopeUV_1,90,0.5,0.5,0);
float4 _GenerateLightning_2 = Generate_Lightning(RotationUV_1,float2(0,1),0.5,0.5,2.622,6.82,1,0);
_GenerateLightning_1 = lerp(_GenerateLightning_1,_GenerateLightning_1*_GenerateLightning_1.a + _GenerateLightning_2*_GenerateLightning_2.a,_Add_Fade_1 * _GenerateLightning_2.a);
float4 _CircleFade_1 = Circle_Fade(_GenerateLightning_1,i.uv_MainTex,_CircleFade_PosX_1,_CircleFade_PosY_1,_CircleFade_Size_1,_CircleFade_Dist_1);
float4 _PremadeGradients_1 = Color_PreGradients(_CircleFade_1,float4(0.55,0.55,0.55,1),float4(0.8,0.8,0.8,1),float4(0.29,0.29,0.29,1),float4(0.54,0.59,0.6900001,1),_PremadeGradients_Offset_1,_PremadeGradients_Fade_1,_PremadeGradients_Speed_1);
float4 HdrCreate_1 = HdrCreate(_PremadeGradients_1,_HdrCreate_Value_1);
float4 OperationBlend_1 = OperationBlend(_ShadowLight_1, HdrCreate_1, _OperationBlend_Fade_1); 
OperationBlend_1 = saturate(OperationBlend_1); 
float4 FinalResult = OperationBlend_1;
o.Albedo = FinalResult.rgb* i.color.rgb;
o.Alpha = FinalResult.a * _SpriteFade * i.color.a;
clip(o.Alpha - 0.05);
}

ENDCG
}
Fallback "Sprites /Default"
}
