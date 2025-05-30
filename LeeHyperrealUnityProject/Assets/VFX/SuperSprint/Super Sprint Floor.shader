// Made with Amplify Shader Editor v1.9.2.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Super Sprint"
{
	Properties
	{
		_ExtraBright("Extra Bright", 2D) = "white" {}
		_VertexMove("Vertex Move", 2D) = "white" {}
		_VertexMove1("Vertex Move", 2D) = "white" {}
		_VertexStrength("Vertex Strength", Float) = 0.1
		_MainColor("Main Color", Color) = (0,0,0,0)
		_flooralpha("floor alpha", 2D) = "white" {}
		_zoom("zoom", Vector) = (1,1,0,0)
		_offsetalpha("offset alpha", Vector) = (1,1,0,0)
		_opacity("opacity", Float) = 7.33
		_emitstrength("emit strength", Float) = 1
		_YVertexMoveOfsset("Y Vertex Move Ofsset", Float) = 0
		_Main_Color_Mult("Main_Color_Mult", Range( 0 , 50)) = 1
		_Panspeed("Pan speed", Vector) = (0,0,0,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform sampler2D _VertexMove;
		uniform float _VertexStrength;
		uniform float _YVertexMoveOfsset;
		uniform float _Main_Color_Mult;
		uniform float4 _MainColor;
		uniform sampler2D _ExtraBright;
		uniform float2 _Panspeed;
		uniform float2 _zoom;
		uniform float2 _offsetalpha;
		uniform sampler2D _VertexMove1;
		uniform float _emitstrength;
		uniform sampler2D _flooralpha;
		uniform float _opacity;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float temp_output_10_0 = ( _Time.y * 0.1 );
			float4 appendResult11 = (float4(temp_output_10_0 , ( temp_output_10_0 * 0.25 ) , 0.0 , 0.0));
			float2 uv_TexCoord8 = v.texcoord.xy + appendResult11.xy;
			float4 tex2DNode2 = tex2Dlod( _VertexMove, float4( uv_TexCoord8, 0, 0.0) );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float4 appendResult51 = (float4(0.0 , ( ( (tex2DNode2).g * ( ase_vertex3Pos / ase_worldPos ).y * _VertexStrength ) - _YVertexMoveOfsset ) , 0.0 , 0.0));
			v.vertex.xyz += appendResult51.xyz;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord21 = i.uv_texcoord * _zoom + _offsetalpha;
			float2 panner61 = ( 1.0 * _Time.y * _Panspeed + uv_TexCoord21);
			float4 temp_output_7_0 = ( ( _Main_Color_Mult * _MainColor ) * tex2D( _ExtraBright, panner61 ) );
			float temp_output_33_0 = ( _Time.y * 0.2 );
			float4 appendResult34 = (float4(temp_output_33_0 , ( temp_output_33_0 * 0.15 ) , 0.0 , 0.0));
			float2 uv_TexCoord31 = i.uv_texcoord + appendResult34.xy;
			o.Emission = ( ( temp_output_7_0 - ( tex2D( _VertexMove1, uv_TexCoord31 ) * 1.0 ) ) * _emitstrength ).rgb;
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float clampResult67 = clamp( ( (0.0 + (tex2D( _flooralpha, panner61 ).a - 0.0) * (_opacity - 0.0) / (1.0 - 0.0)) - tex2D( _TextureSample0, uv_TextureSample0 ).a ) , 0.0 , 1.0 );
			o.Alpha = clampResult67;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19201
Node;AmplifyShaderEditor.RangedFloatNode;24;-485.4374,134.2176;Inherit;False;Property;_opacity;opacity;8;0;Create;True;0;0;0;False;0;False;7.33;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-388.481,-143.4438;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-412.481,-28.44379;Inherit;False;Constant;_Float2;Float 2;9;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-1085.123,347.3301;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TimeNode;9;-1752.703,480.3719;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-1476.366,404.294;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;11;-1142.3,515.6494;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-1267.058,679.59;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1609.375,724.9573;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-1405.223,848.6854;Inherit;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;0;False;0;False;0.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-877.7,212.2;Inherit;True;Property;_VertexMove;Vertex Move;1;0;Create;True;0;0;0;False;0;False;-1;None;d7b84a1217c42e04faee3ba4b13cacab;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;31;-143.0187,827.764;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TimeNode;32;-810.5986,960.8058;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-534.2616,884.7279;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;34;-200.1957,996.0833;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-324.9536,1160.024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-667.2706,1205.391;Inherit;False;Constant;_Float3;Float 0;2;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-463.1187,1329.119;Inherit;False;Constant;_Float4;Float 1;2;0;Create;True;0;0;0;False;0;False;0.15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;38;64.40436,692.6339;Inherit;True;Property;_VertexMove1;Vertex Move;2;0;Create;True;0;0;0;False;0;False;-1;None;d7b84a1217c42e04faee3ba4b13cacab;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;30;-170.9348,-182.5178;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;413.4346,484.0148;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;42;476.4346,694.0148;Inherit;False;Constant;_Float5;Float 5;10;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;26;-515.2374,282.1177;Inherit;False;False;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-973,-270;Inherit;True;Property;_ExtraBright;Extra Bright;0;0;Create;True;0;0;0;False;0;False;-1;None;b38ff4e4130c0e04fb5a79198fd1e4b8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;15;-783.0601,-38.3841;Inherit;True;Property;_flooralpha;floor alpha;5;0;Create;True;0;0;0;False;0;False;-1;None;d24d4a0b43b0f174c8f5e0005b393b2b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;43.23999,-75.07547;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PosVertexDataNode;27;-819.9371,418.0377;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-530.275,607.7213;Inherit;False;Property;_VertexStrength;Vertex Strength;3;0;Create;True;0;0;0;False;0;False;0.1;0.06;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;48;-549.0471,449.6516;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;49;-412.1674,404.8117;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;51;-203.308,507.4714;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldPosInputsNode;46;-830.9266,574.4512;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-296.9801,323.26;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;52;-75.77856,338.8389;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-202.7786,252.8389;Inherit;False;Property;_YVertexMoveOfsset;Y Vertex Move Ofsset;10;0;Create;True;0;0;0;False;0;False;0;0.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-286.8926,-288.3397;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-406.5802,-508.8504;Inherit;False;Property;_Main_Color_Mult;Main_Color_Mult;11;0;Create;True;0;0;0;False;0;False;1;11;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;6;-714.404,-458.5961;Inherit;False;Property;_MainColor;Main Color;4;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0.3039951,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-153.4009,-439.6049;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;23;-1524.671,125.8604;Inherit;False;Property;_offsetalpha;offset alpha;7;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-1239.445,-11.91458;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.5,0.5;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;22;-1483.326,-67.13968;Inherit;False;Property;_zoom;zoom;6;0;Create;True;0;0;0;False;0;False;1,1;1,4;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;61;-1275.231,-319.1933;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;62;-1556.117,-291.7222;Inherit;False;Property;_Panspeed;Pan speed;12;0;Create;True;0;0;0;False;0;False;0,0;0,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;39;-41.19919,-253.7269;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-148.0446,-84.393;Inherit;False;Property;_emitstrength;emit strength;9;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;66;-84.47761,125.4877;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;25;-242.3371,30.21764;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;67;56.1222,200.2211;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;63;-587.3614,87.51025;Inherit;True;Property;_TextureSample0;Texture Sample 0;13;0;Create;True;0;0;0;False;0;False;-1;None;5866e485b65be1c45b60c8191df0e3ed;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;71;325.8569,-112.9032;Inherit;False;Constant;_Float6;Float 6;14;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;252.7591,26.39736;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Super Sprint;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;5;False;;10;False;;1;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;28;0;2;0
WireConnection;28;1;29;0
WireConnection;8;1;11;0
WireConnection;10;0;9;2
WireConnection;10;1;13;0
WireConnection;11;0;10;0
WireConnection;11;1;12;0
WireConnection;12;0;10;0
WireConnection;12;1;14;0
WireConnection;2;1;8;0
WireConnection;31;1;34;0
WireConnection;33;0;32;2
WireConnection;33;1;36;0
WireConnection;34;0;33;0
WireConnection;34;1;35;0
WireConnection;35;0;33;0
WireConnection;35;1;37;0
WireConnection;38;1;31;0
WireConnection;30;0;7;0
WireConnection;30;1;28;0
WireConnection;41;0;38;0
WireConnection;41;1;42;0
WireConnection;26;0;2;0
WireConnection;1;1;61;0
WireConnection;15;1;61;0
WireConnection;44;0;39;0
WireConnection;44;1;43;0
WireConnection;48;0;27;0
WireConnection;48;1;46;0
WireConnection;49;0;48;0
WireConnection;51;1;52;0
WireConnection;4;0;26;0
WireConnection;4;1;49;1
WireConnection;4;2;5;0
WireConnection;52;0;4;0
WireConnection;52;1;53;0
WireConnection;7;0;55;0
WireConnection;7;1;1;0
WireConnection;55;0;54;0
WireConnection;55;1;6;0
WireConnection;21;0;22;0
WireConnection;21;1;23;0
WireConnection;61;0;21;0
WireConnection;61;2;62;0
WireConnection;39;0;7;0
WireConnection;39;1;41;0
WireConnection;66;0;25;0
WireConnection;66;1;63;4
WireConnection;25;0;15;4
WireConnection;25;4;24;0
WireConnection;67;0;66;0
WireConnection;0;2;44;0
WireConnection;0;9;67;0
WireConnection;0;11;51;0
ASEEND*/
//CHKSM=F729E9DB25405297F3E330A49C3A51D532D814E9