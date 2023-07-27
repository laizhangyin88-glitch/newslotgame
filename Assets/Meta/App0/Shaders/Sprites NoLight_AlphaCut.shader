Shader "Sprite/NoLight_AlphaCut"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Cutoff("Alpha cutoff", Range(0.15,0.85)) = 0.4
	}
		SubShader
		{
			Tags {
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			}
			Cull Off

			Pass
			{
				Tags { "LightMode" = "ForwardBase" }
				AlphaToMask On
				ZTest Always

				CGPROGRAM
				#pragma target 2.0
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"
				#include "Lighting.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					half3 normal : NORMAL;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					half3 worldNormal : NORMAL;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _MainTex_TexelSize;
				fixed4 _Color;
				fixed _Cutoff;


				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					return o;
				}

				fixed4 frag(v2f i, fixed facing : VFACE) : SV_Target
				{

					fixed4 col = tex2D(_MainTex, i.uv) * _Color;
					//col.a *= 1 + max(0, (i.uv * _MainTex_TexelSize.zw)) * 0;
					//col.a = (col.a - _Cutoff) / max(fwidth(col.a), 0.0001) + 0.5;
					clip(col.a - _Cutoff);
					return col;
				}
			ENDCG
		}
		}
}
