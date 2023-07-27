// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SlotMaker/Utility/Line Default"
{
	Properties
	{
		_MainColor ("Main Color", Color) = (1,1,1,1)
		_TintColor ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"RenderType" = "Transparent"
			"Queue"="Transparent"
		}

		Pass
		{
			Cull 	 Off
			Lighting Off
			ZWrite   Off
			Blend    SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
	        #pragma fragment frag

			#include "UnityCG.cginc"

			fixed4    _MainColor;
			fixed4    _TintColor;
			sampler2D _MainTex;

			struct appdata_ct
	        {
	            float4 vertex   : POSITION;
	            float2 texcoord : TEXCOORD0;
	            float4 color    : COLOR;
	        };

	        struct v2f_ct
	        {
	            float4 vertex   : POSITION;
	            float2 texcoord : TEXCOORD0;
	            float4 color    : COLOR;
	        };

			v2f_ct vert (appdata_ct v)
	        {
	            v2f_ct o;
	            o.vertex   = UnityObjectToClipPos(v.vertex);
	            o.texcoord = v.texcoord;
	            o.color    = v.color;
	            return o;
	        }

			half4 frag (v2f_ct IN) : COLOR
	        {
	        	return tex2D(_MainTex, IN.texcoord) * IN.color * _TintColor * _MainColor;
	        }
			ENDCG
		}
    }
}
