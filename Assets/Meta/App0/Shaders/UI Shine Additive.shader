// Writen by Martin Nerurkar ( www.sharkbombs.com ).
// Based on Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "CVSTA/UI/Shine"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

		[Header(Shine Time Edit)]
		[Space(10)]
		[Toggle] _SHINE_REVERSE("Reverse", float) = 0
		[Toggle] _SHINE_SMOOTH("SmoothStep", float) = 0
		[Toggle] _SHINE_CUBIC("Cubic", float) = 0

		[Header(Atlas Size Calculate)]
		[Space(10)]
		[Toggle] _Atlas("ON/OFF", float) = 0


		[Header(UVTex)]
		[Space(10)]
		_UVTex("UV Texture", 2D) = "white" {}
		_UVColor("UV Color", Color) = (1,1,1,1)

		[Header(Shine Edit)]
		[Space(10)]
		[Toggle] _Active("ON/OFF", float) = 0
		_ShineFreq("Shine Frequency", Float) = 1
		_ShinePause("Shine Pause", Float) = 1
		_ShineWidth("Shine Width", Float) = 0.02
		_ShineFade("Shine Fade", Float) = 0.2
		
		[Header(Stencil)]
		[Space(10)]
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_AtlasInSpriteVector ("Atlas In Sprite Vector", Vector) = (0, 0, 0, 0)

		_ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
		CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "UIShineShader.cginc"

			#pragma multi_compile _ACTIVE_OFF _ACTIVE_ON
			#pragma multi_compile _ATLAS_OFF _ATLAS_ON
			#pragma multi_compile _SHINE_REVERSE_OFF _SHINE_REVERSE_ON
			#pragma multi_compile _SHINE_SMOOTH_OFF _SHINE_SMOOTH_ON
			#pragma multi_compile _SHINE_CUBIC_OFF _SHINE_CUBIC_ON
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				fixed time      : COLOR1;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;

			float4 _ClipRect;
			float4 _MainTex_ST;

			fixed _ShineFreq;
			fixed _ShinePause;
			fixed _ShineWidth;
			fixed _ShineFade;

			v2f vert(appdata_t v)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = v.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
				OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				OUT.color = v.color;

				OUT.time = getShineTime(_ShineFreq, _ShinePause, _Time.y, _ShineWidth, _ShineFade);

				return OUT;
			}

			fixed4 _TextureSampleAdd;
			
			//토글 관련
			fixed _SHINE_REVERSE;
			fixed _SHINE_SMOOTH;
			fixed _SHINE_CUBIC;
			fixed _Active;
			fixed _Atlas;

			float2 uv;
			sampler2D _UVTex;
			float4 _UVTex_TexelSize;
			fixed4 _UVColor;
			float4 _AtlasInSpriteVector;

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

				#ifdef UNITY_UI_CLIP_RECT
				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				#endif

				//아틀라스 사이즈 계산 꺼주는 기능
				#ifdef _ATLAS_ON
				uv = (IN.texcoord - _AtlasInSpriteVector.xy) / (_AtlasInSpriteVector.zw  - _AtlasInSpriteVector.xy);

				#else
					uv = IN.texcoord;
				#endif
				
				//ACTIVE ON/OFF
				#ifdef _ACTIVE_ON
					color.rgb += getShineUV(IN.time, _ShineWidth, _ShineFade, (tex2D(_UVTex, uv).rb)) * _UVColor;
				#endif

				#ifdef UNITY_UI_ALPHACLIP
				clip(color.a - 0.001);
				#endif

				return color;
			}

            ENDCG
        }
    }
	Fallback "UI/Default"
}
