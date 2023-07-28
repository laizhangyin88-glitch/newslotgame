//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/SVD Tooltip burn"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_NewTex_4("NewTex_4(RGB)", 2D) = "white" { }
PositionUV_X_1("PositionUV_X_1", Range(-2, 2)) = -0.107
PositionUV_Y_1("PositionUV_Y_1", Range(-2, 2)) = 0
ResizeUV_X_4("ResizeUV_X_4", Range(-1, 1)) = 0
ResizeUV_Y_4("ResizeUV_Y_4", Range(-1, 1)) = 0
ResizeUV_ZoomX_4("ResizeUV_ZoomX_4", Range(0.1, 3)) = 1
ResizeUV_ZoomY_4("ResizeUV_ZoomY_4", Range(0.1, 3)) = 0.761
DistortionUV_WaveX_1("DistortionUV_WaveX_1", Range(0, 128)) = 51.136
DistortionUV_WaveY_1("DistortionUV_WaveY_1", Range(0, 128)) = 99.424
DistortionUV_DistanceX_1("DistortionUV_DistanceX_1", Range(0, 1)) = 0.093
DistortionUV_DistanceY_1("DistortionUV_DistanceY_1", Range(0, 1)) = 1
DistortionUV_Speed_1("DistortionUV_Speed_1", Range(-2, 2)) = 5
DistortionUV_WaveX_2("DistortionUV_WaveX_2", Range(0, 128)) = 22.791
DistortionUV_WaveY_2("DistortionUV_WaveY_2", Range(0, 128)) = 98.734
DistortionUV_DistanceX_2("DistortionUV_DistanceX_2", Range(0, 1)) = 0.357
DistortionUV_DistanceY_2("DistortionUV_DistanceY_2", Range(0, 1)) = 0.6
DistortionUV_Speed_2("DistortionUV_Speed_2", Range(-2, 2)) = 2
ResizeUV_X_5("ResizeUV_X_5", Range(-1, 1)) = 0.161
ResizeUV_Y_5("ResizeUV_Y_5", Range(-1, 1)) = 0
ResizeUV_ZoomX_5("ResizeUV_ZoomX_5", Range(0.1, 3)) = 1
ResizeUV_ZoomY_5("ResizeUV_ZoomY_5", Range(0.1, 3)) = 0.9
_NewTex_5("NewTex_5(RGB)", 2D) = "white" { }
_MaskAlpha_Fade_8("_MaskAlpha_Fade_8", Range(0, 1)) = 0
_NewTex_6("NewTex_6(RGB)", 2D) = "white" { }
_ThresholdSmooth_Value_1("_ThresholdSmooth_Value_1", Range(-1, 2)) = -1
_ThresholdSmooth_Smooth_1("_ThresholdSmooth_Smooth_1", Range(0, 1)) = 0.092
_TurnBlackToAlpha_Fade_3("_TurnBlackToAlpha_Fade_3", Range(0, 1)) = 1
_MaskAlpha_Fade_7("_MaskAlpha_Fade_7", Range(0, 1)) = 0
ResizeUV_X_2("ResizeUV_X_2", Range(-1, 1)) = -0.622
ResizeUV_Y_2("ResizeUV_Y_2", Range(-1, 1)) = 0
ResizeUV_ZoomX_2("ResizeUV_ZoomX_2", Range(0.1, 3)) = 3
ResizeUV_ZoomY_2("ResizeUV_ZoomY_2", Range(0.1, 3)) = 1
RotationUV_Rotation_1("RotationUV_Rotation_1", Range(-360, 360)) = -19.285
RotationUV_Rotation_PosX_1("RotationUV_Rotation_PosX_1", Range(-1, 2)) = -0.214
RotationUV_Rotation_PosY_1("RotationUV_Rotation_PosY_1", Range(-1, 2)) =0.614
RotationUV_Rotation_Speed_1("RotationUV_Rotation_Speed_1", Range(-8, 8)) =0
_ColorGradients_Color1_1("_ColorGradients_Color1_1", COLOR) = (1,0.01845203,0,1)
_ColorGradients_Color2_1("_ColorGradients_Color2_1", COLOR) = (1,0.4189905,0,1)
_ColorGradients_Color3_1("_ColorGradients_Color3_1", COLOR) = (1,0.5939968,0,1)
_ColorGradients_Color4_1("_ColorGradients_Color4_1", COLOR) = (1,0.918384,0,1)
_NewTex_7("NewTex_7(RGB)", 2D) = "white" { }
_ThresholdSmooth_Value_2("_ThresholdSmooth_Value_2", Range(-1, 2)) = -1
_ThresholdSmooth_Smooth_2("_ThresholdSmooth_Smooth_2", Range(0, 1)) = 1
_TurnBlackToAlpha_Fade_4("_TurnBlackToAlpha_Fade_4", Range(0, 1)) = 1
_MaskAlpha_Fade_9("_MaskAlpha_Fade_9", Range(0, 1)) = 0
_MaskAlpha_Fade_6("_MaskAlpha_Fade_6", Range(0, 1)) = 0
_OperationBlend_Fade_3("_OperationBlend_Fade_3", Range(0, 1)) = 1
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
_ColorRGBA_Color_1("_ColorRGBA_Color_1", COLOR) = (0.2075472,0.00187777,0,1)
ResizeUV_X_3("ResizeUV_X_3", Range(-1, 1)) = 0.358
ResizeUV_Y_3("ResizeUV_Y_3", Range(-1, 1)) = 0
ResizeUV_ZoomX_3("ResizeUV_ZoomX_3", Range(0.1, 3)) = 1
ResizeUV_ZoomY_3("ResizeUV_ZoomY_3", Range(0.1, 3)) = 0.389
_NewTex_2("NewTex_2(RGB)", 2D) = "white" { }
_MaskAlpha_Fade_3("_MaskAlpha_Fade_3", Range(0, 1)) = 1
_MaskAlpha_Fade_5("_MaskAlpha_Fade_5", Range(0, 1)) = 0
_OperationBlend_Fade_2("_OperationBlend_Fade_2", Range(0, 1)) = 1
ResizeUV_X_1("ResizeUV_X_1", Range(-1, 1)) = -0.36
ResizeUV_Y_1("ResizeUV_Y_1", Range(-1, 1)) = 0
ResizeUV_ZoomX_1("ResizeUV_ZoomX_1", Range(0.1, 3)) = 1.213
ResizeUV_ZoomY_1("ResizeUV_ZoomY_1", Range(0.1, 3)) = 1
_ColorGradients_Color1_2("_ColorGradients_Color1_2", COLOR) = (0,0,0,1)
_ColorGradients_Color2_2("_ColorGradients_Color2_2", COLOR) = (1,1,1,1)
_ColorGradients_Color3_2("_ColorGradients_Color3_2", COLOR) = (1,1,1,1)
_ColorGradients_Color4_2("_ColorGradients_Color4_2", COLOR) = (1,1,1,1)
_TurnBlackToAlpha_Fade_1("_TurnBlackToAlpha_Fade_1", Range(0, 1)) = 1
_NewTex_3("NewTex_3(RGB)", 2D) = "white" { }
_TurnBlackToAlpha_Fade_2("_TurnBlackToAlpha_Fade_2", Range(0, 1)) = 1
_MaskAlpha_Fade_4("_MaskAlpha_Fade_4", Range(0, 1)) = 0
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
sampler2D _NewTex_4;
float PositionUV_X_1;
float PositionUV_Y_1;
float ResizeUV_X_4;
float ResizeUV_Y_4;
float ResizeUV_ZoomX_4;
float ResizeUV_ZoomY_4;
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
float ResizeUV_X_5;
float ResizeUV_Y_5;
float ResizeUV_ZoomX_5;
float ResizeUV_ZoomY_5;
sampler2D _NewTex_5;
float _MaskAlpha_Fade_8;
sampler2D _NewTex_6;
float _ThresholdSmooth_Value_1;
float _ThresholdSmooth_Smooth_1;
float _TurnBlackToAlpha_Fade_3;
float _MaskAlpha_Fade_7;
float ResizeUV_X_2;
float ResizeUV_Y_2;
float ResizeUV_ZoomX_2;
float ResizeUV_ZoomY_2;
float RotationUV_Rotation_1;
float RotationUV_Rotation_PosX_1;
float RotationUV_Rotation_PosY_1;
float RotationUV_Rotation_Speed_1;
float4 _ColorGradients_Color1_1;
float4 _ColorGradients_Color2_1;
float4 _ColorGradients_Color3_1;
float4 _ColorGradients_Color4_1;
sampler2D _NewTex_7;
float _ThresholdSmooth_Value_2;
float _ThresholdSmooth_Smooth_2;
float _TurnBlackToAlpha_Fade_4;
float _MaskAlpha_Fade_9;
float _MaskAlpha_Fade_6;
float _OperationBlend_Fade_3;
sampler2D _NewTex_1;
float4 _ColorRGBA_Color_1;
float ResizeUV_X_3;
float ResizeUV_Y_3;
float ResizeUV_ZoomX_3;
float ResizeUV_ZoomY_3;
sampler2D _NewTex_2;
float _MaskAlpha_Fade_3;
float _MaskAlpha_Fade_5;
float _OperationBlend_Fade_2;
float ResizeUV_X_1;
float ResizeUV_Y_1;
float ResizeUV_ZoomX_1;
float ResizeUV_ZoomY_1;
float4 _ColorGradients_Color1_2;
float4 _ColorGradients_Color2_2;
float4 _ColorGradients_Color3_2;
float4 _ColorGradients_Color4_2;
float _TurnBlackToAlpha_Fade_1;
sampler2D _NewTex_3;
float _TurnBlackToAlpha_Fade_2;
float _MaskAlpha_Fade_4;
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
float4 ColorRGBA(float4 txt, float4 color)
{
txt.rgb += color.rgb;
return txt;
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
float4 Color_Gradients(float4 txt, float2 uv, float4 col1, float4 col2, float4 col3, float4 col4)
{
float4 c1 = lerp(col1, col2, smoothstep(0., 0.33, uv.x));
c1 = lerp(c1, col3, smoothstep(0.33, 0.66, uv.x));
c1 = lerp(c1, col4, smoothstep(0.66, 1, uv.x));
c1.a = txt.a;
return c1;
}

float4 TurnBlackToAlpha(float4 txt, float force, float fade)
{
float3 gs = dot(txt.rgb, float3(1., 1., 1.));
gs=saturate(gs);
return lerp(txt,float4(force*txt.rgb, gs.r), fade);
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
float2 PositionUV(float2 uv, float offsetx, float offsety)
{
uv += float2(offsetx, offsety);
return uv;
}

float4 frag (v2f i) : COLOR
{
float4 NewTex_4 = tex2D(_NewTex_4, i.texcoord);
float2 PositionUV_1 = PositionUV(i.texcoord,PositionUV_X_1,PositionUV_Y_1);
float2 ResizeUV_4 = ResizeUVClamp(PositionUV_1,ResizeUV_X_4,ResizeUV_Y_4,ResizeUV_ZoomX_4,ResizeUV_ZoomY_4);
float2 DistortionUV_1 = DistortionUV(ResizeUV_4,DistortionUV_WaveX_1,DistortionUV_WaveY_1,DistortionUV_DistanceX_1,DistortionUV_DistanceY_1,DistortionUV_Speed_1);
float2 DistortionUV_2 = DistortionUV(DistortionUV_1,DistortionUV_WaveX_2,DistortionUV_WaveY_2,DistortionUV_DistanceX_2,DistortionUV_DistanceY_2,DistortionUV_Speed_2);
float2 ResizeUV_5 = ResizeUVClamp(DistortionUV_2,ResizeUV_X_5,ResizeUV_Y_5,ResizeUV_ZoomX_5,ResizeUV_ZoomY_5);
float4 NewTex_5 = tex2D(_NewTex_5,ResizeUV_5);
float4 MaskAlpha_8=NewTex_4;
MaskAlpha_8.a = lerp(NewTex_5.a * NewTex_4.a, (1 - NewTex_5.a) * NewTex_4.a,_MaskAlpha_Fade_8);
float4 NewTex_6 = tex2D(_NewTex_6, i.texcoord);
float4 _ThresholdSmooth_1 = ThresholdSmooth(NewTex_6,_ThresholdSmooth_Value_1,_ThresholdSmooth_Smooth_1);
float4 TurnBlackToAlpha_3 = TurnBlackToAlpha(_ThresholdSmooth_1,1,_TurnBlackToAlpha_Fade_3);
float4 MaskAlpha_7=MaskAlpha_8;
MaskAlpha_7.a = lerp(TurnBlackToAlpha_3.a * MaskAlpha_8.a, (1 - TurnBlackToAlpha_3.a) * MaskAlpha_8.a,_MaskAlpha_Fade_7);
float2 ResizeUV_2 = ResizeUVClamp(DistortionUV_2,ResizeUV_X_2,ResizeUV_Y_2,ResizeUV_ZoomX_2,ResizeUV_ZoomY_2);
float2 RotationUV_1 = RotationUV(ResizeUV_2,RotationUV_Rotation_1,RotationUV_Rotation_PosX_1,RotationUV_Rotation_PosY_1,RotationUV_Rotation_Speed_1);
float4 _ColorGradients_1 = Color_Gradients(float4(0,0,0,1),RotationUV_1,_ColorGradients_Color1_1,_ColorGradients_Color2_1,_ColorGradients_Color3_1,_ColorGradients_Color4_1);
float4 NewTex_7 = tex2D(_NewTex_7,DistortionUV_2);
float4 _ThresholdSmooth_2 = ThresholdSmooth(NewTex_6,_ThresholdSmooth_Value_2,_ThresholdSmooth_Smooth_2);
float4 TurnBlackToAlpha_4 = TurnBlackToAlpha(_ThresholdSmooth_2,1,_TurnBlackToAlpha_Fade_4);
float4 MaskAlpha_9=NewTex_7;
MaskAlpha_9.a = lerp(TurnBlackToAlpha_4.a * NewTex_7.a, (1 - TurnBlackToAlpha_4.a) * NewTex_7.a,_MaskAlpha_Fade_9);
float4 MaskAlpha_6=_ColorGradients_1;
MaskAlpha_6.a = lerp(MaskAlpha_9.a * _ColorGradients_1.a, (1 - MaskAlpha_9.a) * _ColorGradients_1.a,_MaskAlpha_Fade_6);
float4 OperationBlend_3 = OperationBlend(MaskAlpha_6, MaskAlpha_7, _OperationBlend_Fade_3); 
float4 NewTex_1 = tex2D(_NewTex_1,DistortionUV_2);
float4 ColorRGBA_1 = ColorRGBA(NewTex_1,_ColorRGBA_Color_1);
float2 ResizeUV_3 = ResizeUVClamp(DistortionUV_2,ResizeUV_X_3,ResizeUV_Y_3,ResizeUV_ZoomX_3,ResizeUV_ZoomY_3);
float4 NewTex_2 = tex2D(_NewTex_2,ResizeUV_3);
float4 MaskAlpha_3=ColorRGBA_1;
MaskAlpha_3.a = lerp(NewTex_2.a * ColorRGBA_1.a, (1 - NewTex_2.a) * ColorRGBA_1.a,_MaskAlpha_Fade_3);
MaskAlpha_8.a = lerp(NewTex_5.a * NewTex_4.a, (1 - NewTex_5.a) * NewTex_4.a,_MaskAlpha_Fade_8);
MaskAlpha_7.a = lerp(TurnBlackToAlpha_3.a * MaskAlpha_8.a, (1 - TurnBlackToAlpha_3.a) * MaskAlpha_8.a,_MaskAlpha_Fade_7);
float4 MaskAlpha_5=MaskAlpha_3;
MaskAlpha_5.a = lerp(MaskAlpha_7.a * MaskAlpha_3.a, (1 - MaskAlpha_7.a) * MaskAlpha_3.a,_MaskAlpha_Fade_5);
float4 OperationBlend_2 = OperationBlend(OperationBlend_3, MaskAlpha_5, _OperationBlend_Fade_2); 
float2 ResizeUV_1 = ResizeUVClamp(PositionUV_1,ResizeUV_X_1,ResizeUV_Y_1,ResizeUV_ZoomX_1,ResizeUV_ZoomY_1);
float4 _ColorGradients_2 = Color_Gradients(float4(0,0,0,1),ResizeUV_1,_ColorGradients_Color1_2,_ColorGradients_Color2_2,_ColorGradients_Color3_2,_ColorGradients_Color4_2);
float4 TurnBlackToAlpha_1 = TurnBlackToAlpha(_ColorGradients_2,1,_TurnBlackToAlpha_Fade_1);
float4 NewTex_3 = tex2D(_NewTex_3, i.texcoord);
float4 TurnBlackToAlpha_2 = TurnBlackToAlpha(NewTex_3,1,_TurnBlackToAlpha_Fade_2);
MaskAlpha_9.a = lerp(TurnBlackToAlpha_4.a * NewTex_7.a, (1 - TurnBlackToAlpha_4.a) * NewTex_7.a,_MaskAlpha_Fade_9);
float4 MaskAlpha_4=TurnBlackToAlpha_2;
MaskAlpha_4.a = lerp(MaskAlpha_9.a * TurnBlackToAlpha_2.a, (1 - MaskAlpha_9.a) * TurnBlackToAlpha_2.a,_MaskAlpha_Fade_4);
float4 MaskAlpha_2=TurnBlackToAlpha_1;
MaskAlpha_2.a = lerp(MaskAlpha_4.a * TurnBlackToAlpha_1.a, (1 - MaskAlpha_4.a) * TurnBlackToAlpha_1.a,_MaskAlpha_Fade_2);
float4 MaskAlpha_1=_ColorGradients_1;
MaskAlpha_1.a = lerp(MaskAlpha_2.a * _ColorGradients_1.a, (1 - MaskAlpha_2.a) * _ColorGradients_1.a,_MaskAlpha_Fade_1);
float4 OperationBlend_1 = OperationBlend(OperationBlend_2, MaskAlpha_1, _OperationBlend_Fade_1); 
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
