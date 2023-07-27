Shader "SlotMaker/Sprites/Default(SDF)"
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

		[Enum(CullMode)] _Cull ("Cull Mode", Float) = 1
		
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
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

		Cull [_Cull]
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

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
	            half4  range         : TEXCOORD1;
	            UNITY_VERTEX_OUTPUT_STEREO
	        };

	        half4 _FaceColor;
	        half4 _OutlineColor;
	        half _Contour, _Contour2;
			half _Smoothing, _Smoothing2;

	        v2f_ct vert(appdata_ct IN)
	        {
	            v2f_ct OUT;
	            UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

			#ifdef UNITY_INSTANCING_ENABLED
                IN.vertex.xy *= _Flip;
            #endif

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
			#ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
            #endif
				
				OUT.texcoord = IN.texcoord;

				OUT.faceColor = IN.color * _FaceColor * _RendererColor;
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

	            return color;
			}
		ENDCG
		}
	}
}
