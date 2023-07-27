Shader "SlotMaker/Image/Default(SDF)"
{
	Properties
	{
		[PerRenderData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_FaceColor ("Face Color", Color) = (1,1,1,1)
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)

		_Contour("Contour", Range(0, 1)) = 0.5
		_Contour2("Contour 2", Range(0, 1)) = 0.4
		_Smoothing("Smoothing", Range(0, 1)) = 0
		_Smoothing2("Smoothing2", Range(0, 1)) = 0
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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
        Fog { Mode Off }
        ZTest [unity_GUIZTestMode]
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

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
	        #pragma multi_compile __ UNITY_UI_ALPHACLIP

	        struct appdata_ct
	        {
	        	float4 vertex   : POSITION;
	        	float4 color    : COLOR;
	        	float2 texcoord : TEXCOORD0;
	        	UNITY_VERTEX_INPUT_INSTANCE_ID
	        };

	        struct v2f_ct
	        {
	            float4 vertex        : SV_POSITION;
	            fixed4 faceColor     : COLOR;
	            fixed4 outlineColor  : COLOR1;
	            half2  texcoord      : TEXCOORD0;
	            float4 worldPosition : TEXCOORD1;
	            half4  range         : TEXCOORD2;
	            UNITY_VERTEX_OUTPUT_STEREO
	        };

	        sampler2D _MainTex;
	        half4 _FaceColor;
	        half4 _OutlineColor;
	        half _Contour, _Contour2;
			half _Smoothing, _Smoothing2;
	        fixed4 _TextureSampleAdd;

	        v2f_ct vert(appdata_ct IN)
	        {
	            v2f_ct OUT;
	            UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;

				OUT.faceColor = IN.color * _FaceColor;
				OUT.outlineColor = _OutlineColor;

				OUT.range = half4(_Contour - _Smoothing, _Contour + _Smoothing, _Contour2 - _Smoothing2, _Contour2 + _Smoothing2);

				return OUT;
	        }

	        fixed4 frag(v2f_ct IN) : SV_Target
			{
				half distance = tex2D(_MainTex, IN.texcoord).a;
				
				half2 bf;
				bf.x = smoothstep(IN.range.x, IN.range.y, distance);
				bf.y = smoothstep(IN.range.z, IN.range.w, distance);

				half4 color = lerp(IN.outlineColor, IN.faceColor, bf.x);
				color.a = lerp(IN.outlineColor.a, IN.faceColor.a, bf.x) * bf.y;

	            #ifdef UNITY_UI_CLIP_RECT
	            color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
	            #endif

	            #ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

	            return color;
			}
		ENDCG
		}
	}
}
