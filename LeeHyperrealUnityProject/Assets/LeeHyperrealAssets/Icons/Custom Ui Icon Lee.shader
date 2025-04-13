// Made with Amplify Shader Editor v1.9.2.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom Ui Icon Lee"
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

        _IconTexture("Icon Texture", 2D) = "white" {}
        _HueShiftDegrees("Hue Shift Degrees", Range( 0 , 360)) = 0
        _BaseHueOffset("Base Hue Offset", Range( -1 , 1)) = 0.6
        _SaturationMultiplier("Saturation Multiplier", Float) = 1
        _LuminanceMultiplier("Luminance Multiplier", Float) = 1
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

            uniform sampler2D _IconTexture;
            uniform float4 _IconTexture_ST;
            uniform float _HueShiftDegrees;
            uniform float _BaseHueOffset;
            uniform float _SaturationMultiplier;
            uniform float _LuminanceMultiplier;
            float3 HSVToRGB( float3 c )
            {
            	float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
            	float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
            	return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
            }
            
            float3 RGBToHSV(float3 c)
            {
            	float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
            	float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
            	float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
            	float d = q.x - min( q.w, q.y );
            	float e = 1.0e-10;
            	return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            
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

                float2 uv_IconTexture = IN.texcoord.xy * _IconTexture_ST.xy + _IconTexture_ST.zw;
                float4 tex2DNode1 = tex2D( _IconTexture, uv_IconTexture );
                float3 hsvTorgb2 = RGBToHSV( tex2DNode1.rgb );
                float clampResult6 = clamp( ( hsvTorgb2.x + ( ( _HueShiftDegrees / 360.0 ) - _BaseHueOffset ) ) , 0.0 , 6.28 );
                float3 hsvTorgb3 = HSVToRGB( float3(clampResult6,( hsvTorgb2.y * _SaturationMultiplier ),( hsvTorgb2.z * _LuminanceMultiplier )) );
                float4 appendResult10 = (float4(hsvTorgb3.x , hsvTorgb3.y , hsvTorgb3.z , tex2DNode1.a));
                

                half4 color = appendResult10;

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
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;439.4,9.533308;Float;False;True;-1;2;ASEMaterialInspector;0;3;Custom Ui Icon Lee;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;3;1;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;True;_ColorMask;False;False;False;False;False;False;False;True;True;0;True;_Stencil;255;True;_StencilReadMask;255;True;_StencilWriteMask;0;True;_StencilComp;0;True;_StencilOp;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;0;True;unity_GUIZTestMode;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.SamplerNode;1;-576.7195,-157.8721;Inherit;True;Property;_IconTexture;Icon Texture;0;0;Create;True;0;0;0;False;0;False;-1;None;c5b77e1b98853334cb6c2dae6e15565a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-376.9662,-310.0785;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;6;-128.1601,-332.3006;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-371.0101,-410.4265;Inherit;False;Constant;_Float1;Float 1;1;0;Create;True;0;0;0;False;0;False;6.28;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;13;-571.9505,-348.9111;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1119.271,-526.287;Inherit;False;Property;_HueShiftDegrees;Hue Shift Degrees;1;0;Create;True;0;0;0;False;0;False;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1090.52,-645.3721;Inherit;False;Constant;_Float3;Float 3;1;0;Create;True;0;0;0;False;0;False;360;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;22;-757.7853,-538.556;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;10;208.0066,101.0739;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RGBToHSVNode;2;-252.6151,-182.1098;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.HSVToRGBNode;3;52.92873,-132.1874;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;24;-431.4714,67.76941;Inherit;False;Property;_SaturationMultiplier;Saturation Multiplier;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-489.8115,158.3513;Inherit;False;Property;_LuminanceMultiplier;Luminance Multiplier;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-139.002,-25.11468;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-149.748,127.6458;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-1002.769,-368.5817;Inherit;False;Property;_BaseHueOffset;Base Hue Offset;2;0;Create;True;0;0;0;False;0;False;0.6;0.06159642;-1;1;0;1;FLOAT;0
WireConnection;0;0;10;0
WireConnection;5;0;2;1
WireConnection;5;1;13;0
WireConnection;6;0;5;0
WireConnection;6;2;7;0
WireConnection;13;0;22;0
WireConnection;13;1;14;0
WireConnection;22;0;4;0
WireConnection;22;1;21;0
WireConnection;10;0;3;1
WireConnection;10;1;3;2
WireConnection;10;2;3;3
WireConnection;10;3;1;4
WireConnection;2;0;1;0
WireConnection;3;0;6;0
WireConnection;3;1;23;0
WireConnection;3;2;26;0
WireConnection;23;0;2;2
WireConnection;23;1;24;0
WireConnection;26;0;2;3
WireConnection;26;1;25;0
ASEEND*/
//CHKSM=E75407AC55521CF192E675143559E42831ED8B59