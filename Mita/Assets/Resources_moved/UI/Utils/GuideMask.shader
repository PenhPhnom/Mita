// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "UI/GuideMask"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
		[Enum(Circle,0,Rect,1,CorerRect,2)]_MaskType("MaskType", Float) = 0
		_Origin("Origin", Vector) = (32.13,56.7,20,0.5)
		_Corner("Corner", Float) = 0

	}

		SubShader
		{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }

			Stencil
			{
				Ref[_Stencil]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
				CompFront[_StencilComp]
				PassFront[_StencilOp]
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
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask[_ColorMask]


			Pass
			{
				Name "Default"
			CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0

				#include "UnityCG.cginc"
				#include "UnityUI.cginc"

			//#pragma multi_compile __ UNITY_UI_CLIP_RECT
			//#pragma multi_compile __ UNITY_UI_ALPHACLIP

			#pragma shader_feature __ UNITY_UI_CLIP_RECT
			#pragma shader_feature __ UNITY_UI_ALPHACLIP

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
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord2 : TEXCOORD2;
			};

			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform float _MaskType;
			uniform float4 _Origin;
			uniform float _Corner;


			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				OUT.ase_texcoord2 = IN.vertex;

				OUT.worldPosition.xyz += float3(0, 0, 0);
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;

				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float4 break128 = (IN.color * _Color);
				float3 appendResult32 = (float3(break128.r , break128.g , break128.b));
				float3 Color16 = appendResult32;
				float MaskType15 = _MaskType;
				float4 RectXYZW11 = _Origin;
				float4 break83 = RectXYZW11;
				float Corner68 = _Corner;
				float2 temp_cast_0 = (Corner68).xx;
				float4 appendResult81 = (float4((Corner68 + (RectXYZW11).xy) , ((RectXYZW11).zw - temp_cast_0)));
				float4 newInner82 = appendResult81;
				float4 break85 = newInner82;
				float4 appendResult86 = (float4(break83.x , break85.y , break83.z , break85.w));
				float4 temp_output_3_0_g6 = appendResult86;
				float2 appendResult14 = (float2(IN.ase_texcoord2.xyz.xy));
				float2 pos55 = appendResult14;
				float2 temp_output_2_0_g6 = pos55;
				float2 break11_g6 = (step((temp_output_3_0_g6).xy , temp_output_2_0_g6) * step(temp_output_2_0_g6 , (temp_output_3_0_g6).zw));
				float temp_output_12_0_g6 = (break11_g6.x * break11_g6.y);
				float4 appendResult87 = (float4(break85.x , break83.y , break85.z , break83.w));
				float4 temp_output_3_0_g7 = appendResult87;
				float2 temp_output_2_0_g7 = pos55;
				float2 break11_g7 = (step((temp_output_3_0_g7).xy , temp_output_2_0_g7) * step(temp_output_2_0_g7 , (temp_output_3_0_g7).zw));
				float temp_output_12_0_g7 = (break11_g7.x * break11_g7.y);
				float2 appendResult100 = (float2(break85.x , break85.y));
				float2 appendResult101 = (float2(break85.x , break85.w));
				float2 appendResult102 = (float2(break85.z , break85.y));
				float2 appendResult103 = (float2(break85.z , break85.w));
				float clampResult122 = clamp((1.0 - ((min(min(distance(appendResult100 , pos55) , distance(appendResult101 , pos55)) , min(distance(appendResult102 , pos55) , distance(pos55 , appendResult103))) - (Corner68 - 1.5)) * (1.0 / 1.5))) , 0.0 , 1.0);
				float clampResult95 = clamp((temp_output_12_0_g6 + temp_output_12_0_g7 + clampResult122) , 0.0 , 1.0);
				float CrossRect96 = (1.0 - clampResult95);
				float alpha31 = break128.a;
				float4 temp_output_3_0_g8 = RectXYZW11;
				float2 temp_output_2_0_g8 = pos55;
				float2 break11_g8 = (step((temp_output_3_0_g8).xy , temp_output_2_0_g8) * step(temp_output_2_0_g8 , (temp_output_3_0_g8).zw));
				float temp_output_12_0_g8 = (break11_g8.x * break11_g8.y);
				float RectAlpha59 = (1.0 - temp_output_12_0_g8);
				float2 appendResult8 = (float2(_Origin.x , _Origin.y));
				float2 center10 = appendResult8;
				float Radius9 = _Origin.z;
				float width21 = _Origin.w;
				float clampResult29 = clamp(((distance(pos55 , center10) - Radius9) / width21) , 0.0 , 1.0);
				float CircleAlpha30 = clampResult29;
				float temp_output_33_0 = (alpha31 * CircleAlpha30);
				float ifLocalVar63 = 0;
				UNITY_BRANCH
				if (MaskType15 <= 0.0)
				ifLocalVar63 = temp_output_33_0;
				else
				ifLocalVar63 = (RectAlpha59 * alpha31);
				float ifLocalVar118 = 0;
				UNITY_BRANCH
				if (MaskType15 <= 1.0)
				ifLocalVar118 = ifLocalVar63;
				else
				ifLocalVar118 = (CrossRect96 * alpha31);
				float4 appendResult36 = (float4(Color16 , ifLocalVar118));

				half4 color = appendResult36;

				#ifdef UNITY_UI_CLIP_RECT
				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				#endif

				#ifdef UNITY_UI_ALPHACLIP
				clip(color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
		}
			CustomEditor "ASEMaterialInspector"
}