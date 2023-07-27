//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Rainbow"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_NewTex_2("NewTex_2(RGB)", 2D) = "white" { }
_NewTex_4("NewTex_4(RGB)", 2D) = "white" { }
LiquidUV_WaveX_1("LiquidUV_WaveX_1", Range(0, 2)) = 1.528
LiquidUV_WaveY_1("LiquidUV_WaveY_1", Range(0, 2)) = 0.337
LiquidUV_DistanceX_1("LiquidUV_DistanceX_1", Range(0, 1)) = 0.395
LiquidUV_DistanceY_1("LiquidUV_DistanceY_1", Range(0, 1)) = 0.04
LiquidUV_Speed_1("LiquidUV_Speed_1", Range(-2, 2)) = 0.246
_NewTex_5("NewTex_5(RGB)", 2D) = "white" { }
_Brightness_Fade_1("_Brightness_Fade_1", Range(0, 1)) = 0
_SourceNewTex_1("_SourceNewTex_1(RGB)", 2D) = "white" { }
_NewTex_3("NewTex_3(RGB)", 2D) = "white" { }
AnimatedInfiniteZoomUV_Zoom_1("AnimatedInfiniteZoomUV_Zoom_1", Range(-1, 4)) = 0.929
AnimatedInfiniteZoomUV_PosX_1("AnimatedInfiniteZoomUV_PosX_1", Range(-1, 2)) = -0.007
AnimatedInfiniteZoomUV_PosY_1("AnimatedInfiniteZoomUV_PosY_1", Range(-1, 2)) = -0.006
AnimatedInfiniteZoomUV_Intensity_1("AnimatedInfiniteZoomUV_Intensity_1", Range(0, 4)) = 0.9
AnimatedInfiniteZoomUV_Speed_1("AnimatedInfiniteZoomUV_Speed_1", Range(-10, 10)) = 3.5
_NewTex_6("NewTex_6(RGB)", 2D) = "white" { }
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
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
sampler2D _NewTex_2;
sampler2D _NewTex_4;
float LiquidUV_WaveX_1;
float LiquidUV_WaveY_1;
float LiquidUV_DistanceX_1;
float LiquidUV_DistanceY_1;
float LiquidUV_Speed_1;
sampler2D _NewTex_5;
float _Brightness_Fade_1;
sampler2D _SourceNewTex_1;
sampler2D _NewTex_3;
float AnimatedInfiniteZoomUV_Zoom_1;
float AnimatedInfiniteZoomUV_PosX_1;
float AnimatedInfiniteZoomUV_PosY_1;
float AnimatedInfiniteZoomUV_Intensity_1;
float AnimatedInfiniteZoomUV_Speed_1;
sampler2D _NewTex_6;
sampler2D _NewTex_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float4 Brightness(float4 txt, float value)
{
txt.rgb += value;
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
float4 OperationBlendMask(float4 origin, float4 overlay, float4 mask, float blend)
{
float4 o = origin; 
origin.rgb = overlay.a * overlay.rgb + origin.a * (1 - overlay.a) * origin.rgb;
origin.a = overlay.a + origin.a * (1 - overlay.a);
origin.a *= mask;
origin = lerp(o, origin,blend);
return origin;
}
float2 HumanBreathExUV(float2 uv, sampler2D smp, float intensity, float speed, float sideintensity)
{
float t = _Time * 15 * speed;
float i = intensity * 0.01;
float si = sideintensity * 0.01;
float val = (sin(t * 3.1415));
float val2 = exp(-sin(t * 3.1415));
float org = val * i - i / 2;
float4 n = tex2D(smp, uv+org);
uv.y = lerp(uv.y, uv.y+org, n.r);
n = tex2D(smp, uv);
uv.x = lerp(uv.x, uv.x + val2 * si, n.g);
uv.x = lerp(uv.x, uv.x - val2 * si, n.b);
return uv;
}
float2 AnimatedInfiniteZoomUV(float2 uv, float zoom2, float posx, float posy, float radius, float speed)
{
uv+=float2(posx,posy);
float2 muv = uv;
float atans = (atan2(uv.x - 0.5, uv.y - 0.5) + 3.1415) / (3.1415 * 2.);
float time = _Time * speed*10;
uv -= 0.5;
 uv *= (1. / pow(4., frac(time / 2.)));
uv += 0.5;
float2 tri = abs(1. - (uv * 2.));
 float zoom = min(pow(2., floor(-log2(tri.x))), pow(2., floor(-log2(tri.y))));
 float zoom_id = log2(zoom) + 1.;
 float div = ((pow(2., ((-zoom_id) - 1.)) * ((-2.) + pow(2., zoom_id))));
 float2 uv2 = (((uv) - (div)) * zoom);
 uv2 = lerp(muv * radius, uv2 * radius, zoom2);
 return uv2;
}
float2 LiquidUV(float2 p, float WaveX, float WaveY, float DistanceX, float DistanceY, float Speed)
{ Speed *= _Time * 100;
float x = sin(p.y * 4 * WaveX + Speed);
float y = cos(p.x * 4 * WaveY + Speed);
x += sin(p.x)*0.1;
y += cos(p.y)*0.1;
x *= y;
y *= x;
x *= y + WaveY*8;
y *= x + WaveX*8;
p.x = p.x + x * DistanceX * 0.015;
p.y = p.y + y * DistanceY * 0.015;

return p;
}
float4 frag (v2f i) : COLOR
{
float4 NewTex_2 = tex2D(_NewTex_2, i.texcoord);
float4 NewTex_4 = tex2D(_NewTex_4, i.texcoord);
float2 LiquidUV_1 = LiquidUV(i.texcoord,LiquidUV_WaveX_1,LiquidUV_WaveY_1,LiquidUV_DistanceX_1,LiquidUV_DistanceY_1,LiquidUV_Speed_1);
float4 NewTex_5 = tex2D(_NewTex_5,LiquidUV_1);
float4 Brightness_1 = Brightness(NewTex_5,_Brightness_Fade_1);
float4 SourceRGBA_1 = tex2D(_SourceNewTex_1, LiquidUV_1);
float4 NewTex_3 = tex2D(_NewTex_3, i.texcoord);
float4 OperationBlendMask_2 = OperationBlendMask(SourceRGBA_1, Brightness_1, NewTex_3, 1); 
float4 OperationBlend_1 = OperationBlend(NewTex_4, OperationBlendMask_2, 1); 
float2 AnimatedInfiniteZoomUV_1 = AnimatedInfiniteZoomUV(i.texcoord,AnimatedInfiniteZoomUV_Zoom_1,AnimatedInfiniteZoomUV_PosX_1,AnimatedInfiniteZoomUV_PosY_1,AnimatedInfiniteZoomUV_Intensity_1,AnimatedInfiniteZoomUV_Speed_1);
float4 NewTex_6 = tex2D(_NewTex_6,AnimatedInfiniteZoomUV_1);
float4 OperationBlend_3 = OperationBlend(OperationBlend_1, NewTex_6, 1); 
float4 NewTex_1 = tex2D(_NewTex_1, i.texcoord);
float4 OperationBlendMask_1 = OperationBlendMask(OperationBlend_3, NewTex_2, NewTex_1, 1); 
float4 OperationBlend_2 = OperationBlend(NewTex_2, OperationBlendMask_1, 1); 
float4 FinalResult = OperationBlend_2;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
