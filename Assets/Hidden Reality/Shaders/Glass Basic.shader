Shader "4liceD/Glass Basic"
{
	Properties
	{
		_MainColor("MainColor", Color) = (0.3959684,0,1,0)
		_SideColor("SideColor", Color) = (1,0.7317086,0,0)
		_Gradient("Gradient", Range( 0 , 1)) = 0.5
		_Smoothness("Smoothness", Range( 0 , 1)) = 1
		_Opacity("Opacity", Range( 0 , 1)) = 0.85
		_Boost("Boost", Range( 0 , 10)) = 2
		_Normal("Normal", 2D) = "bump" {}
		_Smudges("Smudges", 2D) = "white" {}
		_SmudgesSmoothness("SmudgesSmoothness", Range( 0 , 1)) = 0.8
		[Enum(UnityEngine.Rendering.CullMode)]_Culling("Culling", Int) = 2
		_FallbackReflection("FallbackReflection", CUBE) = "black" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull [_Culling]
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
			float2 uv_texcoord;
			float3 worldRefl;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform int _Culling;
		uniform float _Opacity;
		uniform float4 _MainColor;
		uniform float _Gradient;
		uniform float4 _SideColor;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _Smoothness;
		uniform samplerCUBE _FallbackReflection;
		uniform float _Boost;
		uniform sampler2D _Smudges;
		uniform float4 _Smudges_ST;
		uniform float _SmudgesSmoothness;


		float IfReflectionprobeexistsflipped1_g5(  )
		{
			int w = 0; 
			int h = 0; 
			int res = 0;
			#ifndef SHADER_TARGET_SURFACE_ANALYSIS
			unity_SpecCube0.GetDimensions(w, h); 
			#endif
			if (h <= 4) res = 1;
			return res;
		}


		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult89 = dot( ase_worldNormal , ase_worldViewDir );
			float temp_output_91_0 = ( 1.0 - saturate( dotResult89 ) );
			float FresnalOpacity100 = pow( temp_output_91_0 , 4.0 );
			float clampResult103 = clamp( ( FresnalOpacity100 + _Opacity ) , 0.0 , 1.0 );
			float temp_output_10_0 = distance( i.uv_texcoord , float2( 0.5,0.5 ) );
			float4 temp_output_16_0 = ( ( 1.0 - temp_output_10_0 ) * _MainColor * _Gradient );
			float4 SideColor32 = _SideColor;
			float4 temp_output_17_0 = ( temp_output_10_0 * SideColor32 * ( 1.0 - _Gradient ) );
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 tex2DNode8 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float3 indirectNormal6 = normalize( WorldNormalVector( i , tex2DNode8 ) );
			float Smoothness73 = _Smoothness;
			Unity_GlossyEnvironmentData g6 = UnityGlossyEnvironmentSetup( Smoothness73, data.worldViewDir, indirectNormal6, float3(0,0,0));
			float3 indirectSpecular6 = UnityGI_IndirectSpecular( data, 1.0, indirectNormal6, g6 );
			float localIfReflectionprobeexistsflipped1_g5 = IfReflectionprobeexistsflipped1_g5();
			float3 ase_worldReflection = WorldReflectionVector( i, float3( 0, 0, 1 ) );
			float4 FallbackReflection106 = ( localIfReflectionprobeexistsflipped1_g5 * texCUBE( _FallbackReflection, ase_worldReflection ) );
			float4 Reflection18 = ( float4( indirectSpecular6 , 0.0 ) + FallbackReflection106 );
			float3 desaturateInitialColor63 = Reflection18.rgb;
			float desaturateDot63 = dot( desaturateInitialColor63, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar63 = lerp( desaturateInitialColor63, desaturateDot63.xxx, 1.0 );
			float2 uv_Smudges = i.uv_texcoord * _Smudges_ST.xy + _Smudges_ST.zw;
			float4 tex2DNode75 = tex2D( _Smudges, uv_Smudges );
			float temp_output_93_0 = ( 1.0 - pow( temp_output_91_0 , 0.1 ) );
			float3 indirectNormal96 = normalize( WorldNormalVector( i , tex2DNode8 ) );
			Unity_GlossyEnvironmentData g96 = UnityGlossyEnvironmentSetup( _SmudgesSmoothness, data.worldViewDir, indirectNormal96, float3(0,0,0));
			float3 indirectSpecular96 = UnityGI_IndirectSpecular( data, 1.0, indirectNormal96, g96 );
			float3 ReflectionSmudges94 = indirectSpecular96;
			float4 Fresnel35 = ( float4( ( tex2DNode75.r * temp_output_93_0 * ReflectionSmudges94 ) , 0.0 ) + ( tex2DNode75.r * temp_output_93_0 * FallbackReflection106 ) );
			c.rgb = ( ( ( temp_output_16_0 + temp_output_17_0 ) * float4( desaturateVar63 , 0.0 ) * _Boost ) + Fresnel35 ).rgb;
			c.a = clampResult103;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.worldRefl = -worldViewDir;
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}