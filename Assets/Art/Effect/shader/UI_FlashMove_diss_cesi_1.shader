// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "XuanFu/Particles/UI_FlashMove_diss_cesi_01"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_AddTex_02("AddTex_02", 2D) = "black" {}
		_AddTex("AddTex", 2D) = "black" {}
		[HDR]_AddColor("AddColor", Color) = (1,1,1,1)
		_DissolveTex("DissolveTex", 2D) = "white" {}
		_noise_diss_TEX("noise_diss_TEX", 2D) = "white" {}
		_NoiseTex("NoiseTex", 2D) = "white" {}
		_diss_Speed("diss_Speed", Vector) = (0,-0.33,0,0)
		_Te_Speed("Te_Speed", Vector) = (0,0,0,0)
		_Te_Speed_02("Te_Speed_02", Vector) = (0,0,0,0)
		_noise_diss_speed("noise_diss_speed", Vector) = (0,0,0,0)
		_Noise_Speed("Noise_Speed", Vector) = (0,0,0,0)
		_NoiseScale("NoiseScale", Range( 0 , 0.5)) = 0
		_noise_diss_INT("noise_diss_INT", Range( 0 , 0.5)) = 0
		_diss("diss", Float) = 0.88
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}


		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_COLOR

			
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
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform sampler2D _AddTex;
			uniform float2 _Te_Speed;
			uniform float4 _AddTex_ST;
			uniform sampler2D _NoiseTex;
			uniform float2 _Noise_Speed;
			uniform float4 _NoiseTex_ST;
			uniform float _NoiseScale;
			uniform sampler2D _AddTex_02;
			uniform float2 _Te_Speed_02;
			uniform float4 _AddTex_02_ST;
			uniform float4 _AddColor;
			uniform sampler2D _TextureSample1;
			uniform float4 _TextureSample1_ST;
			uniform sampler2D _DissolveTex;
			uniform float2 _diss_Speed;
			uniform float4 _DissolveTex_ST;
			uniform sampler2D _noise_diss_TEX;
			uniform float2 _noise_diss_speed;
			uniform float4 _noise_diss_TEX_ST;
			uniform float _noise_diss_INT;
			uniform float _diss;

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				
				
				OUT.worldPosition.xyz +=  float3( 0, 0, 0 ) ;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 tex2DNode3 = tex2D( _MainTex, uv_MainTex );
				float2 appendResult44 = (float2(_Te_Speed.x , _Te_Speed.y));
				float2 uv_AddTex = IN.texcoord.xy * _AddTex_ST.xy + _AddTex_ST.zw;
				float2 panner46 = ( 1.0 * _Time.y * appendResult44 + uv_AddTex);
				float2 appendResult20 = (float2(_Noise_Speed.x , _Noise_Speed.y));
				float2 uv_NoiseTex = IN.texcoord.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
				float2 panner17 = ( 1.0 * _Time.y * appendResult20 + uv_NoiseTex);
				float2 temp_cast_0 = (tex2D( _NoiseTex, panner17 ).r).xx;
				float2 lerpResult13 = lerp( panner46 , temp_cast_0 , _NoiseScale);
				float4 tex2DNode5 = tex2D( _AddTex, lerpResult13 );
				float2 appendResult108 = (float2(_Te_Speed_02.x , _Te_Speed_02.y));
				float2 uv_AddTex_02 = IN.texcoord.xy * _AddTex_02_ST.xy + _AddTex_02_ST.zw;
				float2 panner109 = ( 1.0 * _Time.y * appendResult108 + uv_AddTex_02);
				float4 tex2DNode110 = tex2D( _AddTex_02, panner109 );
				float temp_output_114_0 = ( ( tex2DNode110.r * tex2DNode110.g ) * 5.0 );
				float4 temp_output_74_0 = ( ( tex2DNode3 + ( ( tex2DNode5.r * temp_output_114_0 ) * _AddColor ) ) * ( tex2DNode3.a * tex2DNode5.a ) );
				float2 uv_TextureSample1 = IN.texcoord.xy * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
				float4 tex2DNode89 = tex2D( _TextureSample1, uv_TextureSample1 );
				float2 appendResult86 = (float2(_diss_Speed.x , _diss_Speed.y));
				float2 uv_DissolveTex = IN.texcoord.xy * _DissolveTex_ST.xy + _DissolveTex_ST.zw;
				float2 panner88 = ( 1.0 * _Time.y * appendResult86 + uv_DissolveTex);
				float2 appendResult101 = (float2(_noise_diss_speed.x , _noise_diss_speed.y));
				float2 uv_noise_diss_TEX = IN.texcoord.xy * _noise_diss_TEX_ST.xy + _noise_diss_TEX_ST.zw;
				float2 panner102 = ( 1.0 * _Time.y * appendResult101 + uv_noise_diss_TEX);
				float2 temp_cast_1 = (tex2D( _noise_diss_TEX, panner102 ).r).xx;
				float2 lerpResult98 = lerp( panner88 , temp_cast_1 , _noise_diss_INT);
				float temp_output_81_0 = saturate( ( ( ( ( tex2DNode89.r + tex2DNode89.g ) + tex2D( _DissolveTex, lerpResult98 ).r ) * temp_output_114_0 ) + -1.5 + ( _diss * -2.0 ) ) );
				
				half4 color = ( temp_output_74_0 * (0.0 + (IN.color.a - 0.0) * (1.0 - 0.0) / (1.0 - 0.0)) * temp_output_81_0 );
				
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
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18900
14;111;1906;762;1996.994;-280.9491;1;True;False
Node;AmplifyShaderEditor.Vector2Node;106;-1378.783,1353.466;Inherit;False;Property;_Te_Speed_02;Te_Speed_02;8;0;Create;True;0;0;0;False;0;False;0,0;0.08,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;23;-1603.313,724.1324;Inherit;False;Property;_Noise_Speed;Noise_Speed;10;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;99;-1296.439,2505.912;Inherit;False;Property;_noise_diss_speed;noise_diss_speed;9;0;Create;True;0;0;0;False;0;False;0,0;0.2,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;20;-1325.681,724.2952;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;107;-1412.697,1147.438;Inherit;False;0;110;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;108;-1101.151,1354.628;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;22;-1637.227,516.1053;Inherit;False;0;14;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;100;-1330.353,2295.629;Inherit;False;0;104;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;85;-961.908,1994.605;Inherit;False;Property;_diss_Speed;diss_Speed;6;0;Create;True;0;0;0;False;0;False;0,-0.33;0.3,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;43;-1286.255,240.6417;Inherit;False;Property;_Te_Speed;Te_Speed;7;0;Create;True;0;0;0;False;0;False;0,0;0.1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;101;-1018.807,2503.819;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;109;-828.2631,1225.29;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;44;-1008.624,241.8045;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;45;-1320.169,33.61456;Inherit;False;0;5;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;87;-994.522,1791.37;Inherit;False;0;78;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;86;-684.2767,1995.767;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;102;-745.92,2375.48;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;17;-1052.794,594.9564;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;46;-735.7367,112.4657;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;110;-651.2339,992.1692;Inherit;True;Property;_AddTex_02;AddTex_02;0;0;Create;True;0;0;0;False;0;False;-1;None;a4c5b3f4c91d33740a2039b9ce2a8da2;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;24;-616.7639,755.5856;Inherit;False;Property;_NoiseScale;NoiseScale;11;0;Create;True;0;0;0;False;0;False;0;0.2927662;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-760.6134,553.9263;Inherit;True;Property;_NoiseTex;NoiseTex;5;0;Create;True;0;0;0;False;0;False;-1;None;a188eec5752586b4bb2b4ecd664d213f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;88;-403.804,1860.74;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-413.97,2624.203;Inherit;False;Property;_noise_diss_INT;noise_diss_INT;12;0;Create;True;0;0;0;False;0;False;0;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;104;-452.7391,2334.45;Inherit;True;Property;_noise_diss_TEX;noise_diss_TEX;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;13;-253.3666,447.9828;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;98;131.374,2187.939;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;-305.405,966.9171;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;89;259.1497,1106.662;Inherit;True;Property;_TextureSample1;Texture Sample 1;14;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;115;-179.5353,1230.758;Inherit;False;Constant;_Float1;Float 1;15;0;Create;True;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-27.88186,970.6545;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-48.6001,249.7999;Inherit;True;Property;_AddTex;AddTex;1;0;Create;True;0;0;0;False;0;False;-1;None;60fd76baf107d444b93379ed03b1c29f;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;97;600.4631,900.885;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;78;241.9982,1700.539;Inherit;True;Property;_DissolveTex;DissolveTex;3;0;Create;True;0;0;0;False;0;False;-1;None;6808ec11fccd7c549b8ee4482e958f15;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;25;556.4227,654.0134;Inherit;False;Property;_AddColor;AddColor;2;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;0.1297291,0.1297291,0.4369823,0.3882353;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;2;-176.6,-193.3999;Inherit;True;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;288.7445,674.6017;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;92;1029.429,1073.635;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;961.342,1879.364;Inherit;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;0;False;0;False;-2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;872.5397,1682.978;Inherit;False;Property;_diss;diss;13;0;Create;True;0;0;0;False;0;False;0.88;-1.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;77;1133.134,1542.385;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;-1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;1203.342,1758.365;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;36;1273.563,-422.9608;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;859.3416,366.8616;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;117;1337.866,1208.951;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;100.7477,-196.2075;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;564.6168,30.06769;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;80;1398.439,1478.323;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;935.6282,-143.5271;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;35;1484.187,-383.3228;Inherit;False;FLOAT;1;0;FLOAT;0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SaturateNode;81;1651.342,1523.366;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;42;1631.985,-373.0878;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;1408.658,75.26865;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;637.8048,1231.548;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;1985.771,-127.066;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;2087.292,391.8074;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;642.8347,-436.6754;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;2371.664,-58.12025;Float;False;True;-1;2;ASEMaterialInspector;0;4;XuanFu/Particles/UI_FlashMove_diss_cesi_01;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;True;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;True;0;True;-9;False;False;False;False;False;False;False;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;0;True;-11;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;20;0;23;1
WireConnection;20;1;23;2
WireConnection;108;0;106;1
WireConnection;108;1;106;2
WireConnection;101;0;99;1
WireConnection;101;1;99;2
WireConnection;109;0;107;0
WireConnection;109;2;108;0
WireConnection;44;0;43;1
WireConnection;44;1;43;2
WireConnection;86;0;85;1
WireConnection;86;1;85;2
WireConnection;102;0;100;0
WireConnection;102;2;101;0
WireConnection;17;0;22;0
WireConnection;17;2;20;0
WireConnection;46;0;45;0
WireConnection;46;2;44;0
WireConnection;110;1;109;0
WireConnection;14;1;17;0
WireConnection;88;0;87;0
WireConnection;88;2;86;0
WireConnection;104;1;102;0
WireConnection;13;0;46;0
WireConnection;13;1;14;1
WireConnection;13;2;24;0
WireConnection;98;0;88;0
WireConnection;98;1;104;1
WireConnection;98;2;103;0
WireConnection;116;0;110;1
WireConnection;116;1;110;2
WireConnection;114;0;116;0
WireConnection;114;1;115;0
WireConnection;5;1;13;0
WireConnection;97;0;89;1
WireConnection;97;1;89;2
WireConnection;78;1;98;0
WireConnection;111;0;5;1
WireConnection;111;1;114;0
WireConnection;92;0;97;0
WireConnection;92;1;78;1
WireConnection;79;0;84;0
WireConnection;79;1;76;0
WireConnection;52;0;111;0
WireConnection;52;1;25;0
WireConnection;117;0;92;0
WireConnection;117;1;114;0
WireConnection;3;0;2;0
WireConnection;75;0;3;4
WireConnection;75;1;5;4
WireConnection;80;0;117;0
WireConnection;80;1;77;0
WireConnection;80;2;79;0
WireConnection;72;0;3;0
WireConnection;72;1;52;0
WireConnection;35;0;36;4
WireConnection;81;0;80;0
WireConnection;42;0;35;0
WireConnection;74;0;72;0
WireConnection;74;1;75;0
WireConnection;105;0;89;1
WireConnection;105;1;89;2
WireConnection;70;0;74;0
WireConnection;70;1;42;0
WireConnection;70;2;81;0
WireConnection;82;0;74;0
WireConnection;82;1;81;0
WireConnection;73;0;3;3
WireConnection;73;1;5;4
WireConnection;0;0;70;0
ASEEND*/
//CHKSM=39928FD1D3264C9C70B8A1D79D25FA7DD87ADDA9