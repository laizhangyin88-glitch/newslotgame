//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Gold shiny Diffuse mask"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_TurnGold_Speed_1("_TurnGold_Speed_1", Range(-8, 8)) = 3.886
_TintRGBA_Color_1("_TintRGBA_Color_1", COLOR) = (1,0.7848822,0,1)
_HdrCreate_Value_1("_HdrCreate_Value_1", Range(0, 4)) = 0.832
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
_MaskRGBA_Fade_1("_MaskRGBA_Fade_1", Range(0, 1)) = 0
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
float _TurnGold_Speed_1;
float4 _TintRGBA_Color_1;
float _HdrCreate_Value_1;
sampler2D _NewTex_1;
float _MaskRGBA_Fade_1;
float _OperationBlend_Fade_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float4 TintRGBA(float4 txt, float4 color)
{
float3 tint = dot(txt.rgb, float3(.222, .707, .071));
tint.rgb *= color.rgb;
txt.rgb = lerp(txt.rgb,tint.rgb,color.a);
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
float4 ColorTurnGold(float2 uv, sampler2D txt, float speed)
{
float4 txt1=tex2D(txt,uv);
float lum = dot(txt1.rgb, float3 (0.2126, 0.2152, 0.4722));
float3 metal = float3(lum,lum,lum);
metal.r = lum * pow(1.46*lum, 4.0);
metal.g = lum * pow(1.46*lum, 4.0);
metal.b = lum * pow(0.86*lum, 4.0);
float2 tuv = uv;
uv *= 2.5;
float time = (_Time/4)*speed;
float a = time * 50;
float n = sin(a + 2.0 * uv.x) + sin(a - 2.0 * uv.x) + sin(a + 2.0 * uv.y) + sin(a + 5.0 * uv.y);
n = fmod(((5.0 + n) / 5.0), 1.0);
n += tex2D(txt, tuv).r * 0.21 + tex2D(txt, tuv).g * 0.4 + tex2D(txt, tuv).b * 0.2;
n=fmod(n,1.0);
float tx = n * 6.0;
float r = clamp(tx - 2.0, 0.0, 1.0) + clamp(2.0 - tx, 0.0, 1.0);
float4 sortie=float4(1.0, 1.0, 1.0,r);
sortie.rgb=metal.rgb+(1-sortie.a);
sortie.rgb=sortie.rgb/2+dot(sortie.rgb, float3 (0.1126, 0.4552, 0.1722));
sortie.rgb-=float3(0.0,0.1,0.45);
sortie.rg+=0.025;
sortie.a=txt1.a;
return sortie; 
}
float4 HdrCreate(float4 txt,float value)
{
if (txt.r>0.98) txt.r=2;
if (txt.g>0.98) txt.g=2;
if (txt.b>0.98) txt.b=2;
return lerp(saturate(txt),txt, value);
}
float4 frag (v2f i) : COLOR
{
float4 _TurnGold_1 = ColorTurnGold(i.texcoord,_MainTex,_TurnGold_Speed_1);
float4 TintRGBA_1 = TintRGBA(_TurnGold_1,_TintRGBA_Color_1);
float4 HdrCreate_1 = HdrCreate(TintRGBA_1,_HdrCreate_Value_1);
float4 NewTex_1 = tex2D(_NewTex_1, i.texcoord);
float4 MaskRGBA_1=HdrCreate_1;
MaskRGBA_1.a = lerp(NewTex_1.r * HdrCreate_1.a, (1 - NewTex_1.r) * HdrCreate_1.a,_MaskRGBA_Fade_1);
float4 _MainTex_1 = tex2D(_MainTex, i.texcoord);
float4 OperationBlend_1 = OperationBlend(_MainTex_1, MaskRGBA_1, _OperationBlend_Fade_1); 
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
