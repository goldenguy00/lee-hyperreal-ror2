// Made with Amplify Shader Editor v1.9.2.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "LeeNotificationBox"
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

        _Mask("Mask", 2D) = "white" {}
        _Pattern("Pattern", 2D) = "white" {}
        _Tiling("Tiling", Vector) = (1,1,0,0)
        _GlowColor("Glow Color", Color) = (0,0.1680822,1,1)
        _PanSpeed("Pan Speed", Vector) = (1,1,0,0)
        _PanSpeedglow("Pan Speed glow", Vector) = (1,1,0,0)
        _glowpanning("glow panning", 2D) = "white" {}
        _TilingGlow("Tiling Glow", Vector) = (4,1,0,0)
        _smoothfadebox("smooth fade box", 2D) = "white" {}
        _Fadevalue("Fade value", Range( 0 , 1)) = 0
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
        	Comp [_StencilComp]
        	Pass [_StencilOp]
        }


        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        
        Pass
        {
            Name "Default"
        CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityShaderVariables.cginc"


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
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4  mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
                
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;

            uniform sampler2D _glowpanning;
            uniform float2 _PanSpeedglow;
            uniform float2 _TilingGlow;
            uniform sampler2D _Mask;
            uniform float4 _Mask_ST;
            uniform float4 _GlowColor;
            uniform sampler2D _Pattern;
            uniform float2 _PanSpeed;
            uniform float2 _Tiling;
            uniform sampler2D _smoothfadebox;
            uniform float4 _smoothfadebox_ST;
            uniform float _Fadevalue;

            
            v2f vert(appdata_t v )
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                

                v.vertex.xyz +=  float3( 0, 0, 0 ) ;

                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                OUT.texcoord = v.texcoord;
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN ) : SV_Target
            {
                //Round up the alpha color coming from the interpolator (to 1.0/256.0 steps)
                //The incoming alpha could have numerical instability, which makes it very sensible to
                //HDR color transparency blend, when it blends with the world's texture.
                const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0/alphaPrecision);
                IN.color.a = round(IN.color.a * alphaPrecision)*invAlphaPrecision;

                float2 temp_cast_0 = (_Time.y).xx;
                float2 panner23 = ( 1.0 * _Time.y * _PanSpeedglow + temp_cast_0);
                float2 texCoord28 = IN.texcoord.xy * _TilingGlow + float2( 0,0 );
                float4 temp_cast_1 = (tex2D( _glowpanning, ( panner23 + texCoord28 ) ).a).xxxx;
                float2 uv_Mask = IN.texcoord.xy * _Mask_ST.xy + _Mask_ST.zw;
                float4 tex2DNode1 = tex2D( _Mask, uv_Mask );
                float4 clampResult35 = clamp( ( ( temp_cast_1 - ( 1.0 - tex2DNode1 ) ) * _GlowColor ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
                float4 temp_output_39_0 = ( clampResult35 * 0.7 );
                float2 temp_cast_2 = (_Time.y).xx;
                float2 panner4 = ( 1.0 * _Time.y * _PanSpeed + temp_cast_2);
                float2 texCoord6 = IN.texcoord.xy * _Tiling + float2( 0,0 );
                float4 tex2DNode2 = tex2D( _Pattern, ( panner4 + texCoord6 ) );
                float4 break84 = tex2DNode2;
                float4 appendResult85 = (float4(break84.r , break84.g , break84.b , 1.0));
                float4 temp_output_1_0_g1 = appendResult85;
                float4 color11 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
                float4 temp_output_2_0_g1 = color11;
                float temp_output_11_0_g1 = distance( temp_output_1_0_g1 , temp_output_2_0_g1 );
                float4 lerpResult21_g1 = lerp( _GlowColor , temp_output_1_0_g1 , saturate( ( ( temp_output_11_0_g1 - 0.5 ) / max( 0.0 , 1E-05 ) ) ));
                float4 ifLocalVar52 = 0;
                if( tex2DNode1.a == 1.0 )
                ifLocalVar52 = ( ( lerpResult21_g1 * 0.75 ) + temp_output_39_0 );
                float4 clampResult60 = clamp( ifLocalVar52 , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
                float4 clampResult71 = clamp( ( temp_output_39_0 + clampResult60 ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
                float2 uv_smoothfadebox = IN.texcoord.xy * _smoothfadebox_ST.xy + _smoothfadebox_ST.zw;
                float4 temp_cast_4 = (( tex2D( _smoothfadebox, uv_smoothfadebox ).a * 1.1 )).xxxx;
                float4 clampResult72 = clamp( ( clampResult71 - temp_cast_4 ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
                

                half4 color = ( ( clampResult72 * 1.3 ) * _Fadevalue );

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                color.rgb *= color.a;

                return color;
            }
        ENDCG
        }
    }
    CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19201
Node;AmplifyShaderEditor.RangedFloatNode;12;-373.6344,-191.3841;Inherit;False;Constant;_Float3;Float 3;3;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;10;-306.9867,-413.8311;Inherit;False;Replace Color;-1;;1;896dccb3016c847439def376a728b869;1,12,0;5;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;394.9135,-716.6918;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;23;-644.2191,-1302.046;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;22;-1097.036,-1519.868;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;35;634.3494,-680.6631;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;37;34.61494,-390.3312;Inherit;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;0;False;0;False;0.75;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;837.1502,-434.5302;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;48.48165,-494.3311;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;40;625.6827,-312.3304;Inherit;False;Constant;_Float1;Float 0;8;0;Create;True;0;0;0;False;0;False;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;-352.1276,-1205.359;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;16;-14.76135,-1022.767;Inherit;True;Property;_glowpanning;glow panning;6;0;Create;True;0;0;0;False;0;False;-1;None;76655c2d5b1071440a314aca9b57432a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;21;-1010.099,-1271.336;Inherit;False;Property;_PanSpeedglow;Pan Speed glow;5;0;Create;True;0;0;0;False;0;False;1,1;-1.5,-1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;28;-676.6635,-1040.548;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;29;-1017.476,-956.6638;Inherit;False;Property;_TilingGlow;Tiling Glow;7;0;Create;True;0;0;0;False;0;False;4,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ConditionalIfNode;52;-179.4254,-37.77588;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;60;80.9411,-2.871917;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;32;280.9449,-435.1886;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;70;320.8229,373.8126;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;498.4484,22.64923;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;71;-137.3054,200.0462;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;72;558.2786,350.8489;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;69;-176.9733,443.3994;Inherit;True;Property;_smoothfadebox;smooth fade box;8;0;Create;True;0;0;0;False;0;False;-1;None;8708766460ec5254c97666443d3041ec;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;903.0175,212.922;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;168.401,563.6338;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-95.05444,671.4734;Inherit;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;0;False;0;False;1.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-820.5705,39.46297;Inherit;True;Property;_Mask;Mask;0;0;Create;True;0;0;0;False;0;False;-1;None;e519f8c0583908c45a1d655384840534;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;79;106.0398,-769.6127;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;80;385.978,-1035.554;Inherit;False;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;75;619.869,132.8905;Inherit;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;0;False;0;False;1.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1299.733,44.02417;Float;False;True;-1;2;ASEMaterialInspector;0;3;LeeNotificationBox;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;3;1;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;True;_ColorMask;False;False;False;False;False;False;False;True;True;0;True;_Stencil;255;True;_StencilReadMask;255;True;_StencilWriteMask;0;True;_StencilComp;0;True;_StencilOp;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;0;True;unity_GUIZTestMode;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;1364.96,219.9835;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;81;1048.731,366.7067;Inherit;False;Property;_Fadevalue;Fade value;9;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;15;-1803.357,-361.6755;Inherit;False;Property;_PanSpeed;Pan Speed;4;0;Create;True;0;0;0;False;0;False;1,1;-0.6,-2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;4;-1499.011,-394.9849;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1437.122,-117.25;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;8;-1630.455,-101.9166;Inherit;False;Property;_Tiling;Tiling;2;0;Create;True;0;0;0;False;0;False;1,1;8,8;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-1245.117,-275.8763;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;5;-1915.053,-632.1428;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;86;-916.2557,-201.7536;Inherit;False;Constant;_Float4;Float 4;10;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1080.333,-481.1663;Inherit;True;Property;_Pattern;Pattern;1;0;Create;True;0;0;0;False;0;False;-1;None;73687fbf2eddffd4b9e8013c726584b9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;84;-818.9225,-467.0869;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-389.9338,-664.5059;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;18;-243.591,-856.0642;Inherit;False;Property;_GlowColor;Glow Color;3;0;Create;True;0;0;0;False;0;False;0,0.1680822,1,1;1,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;11;-762.3202,-744.498;Inherit;False;Constant;_Color3;Color 3;3;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;85;-590.9225,-395.0869;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
WireConnection;10;1;85;0
WireConnection;10;2;11;0
WireConnection;10;3;18;0
WireConnection;10;4;12;0
WireConnection;20;0;80;0
WireConnection;20;1;18;0
WireConnection;23;0;22;2
WireConnection;23;2;21;0
WireConnection;35;0;20;0
WireConnection;39;0;35;0
WireConnection;39;1;40;0
WireConnection;38;0;10;0
WireConnection;38;1;37;0
WireConnection;30;0;23;0
WireConnection;30;1;28;0
WireConnection;16;1;30;0
WireConnection;28;0;29;0
WireConnection;52;0;1;4
WireConnection;52;3;32;0
WireConnection;60;0;52;0
WireConnection;32;0;38;0
WireConnection;32;1;39;0
WireConnection;70;0;71;0
WireConnection;70;1;76;0
WireConnection;42;0;39;0
WireConnection;42;1;60;0
WireConnection;71;0;42;0
WireConnection;72;0;70;0
WireConnection;74;0;72;0
WireConnection;74;1;75;0
WireConnection;76;0;69;4
WireConnection;76;1;77;0
WireConnection;79;0;1;0
WireConnection;80;0;16;4
WireConnection;80;1;79;0
WireConnection;0;0;82;0
WireConnection;82;0;74;0
WireConnection;82;1;81;0
WireConnection;4;0;5;2
WireConnection;4;2;15;0
WireConnection;6;0;8;0
WireConnection;7;0;4;0
WireConnection;7;1;6;0
WireConnection;2;1;7;0
WireConnection;84;0;2;0
WireConnection;83;0;2;0
WireConnection;83;1;18;0
WireConnection;85;0;84;0
WireConnection;85;1;84;1
WireConnection;85;2;84;2
WireConnection;85;3;86;0
ASEEND*/
//CHKSM=606A054287C9880DF8CBD18518114556B0812709