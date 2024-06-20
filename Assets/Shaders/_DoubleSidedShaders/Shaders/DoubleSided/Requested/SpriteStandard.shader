// © 2015 Mario Lelas
Shader "DoubleSided/Other/SpriteStandard"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}

	_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
		[Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0

		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}

	[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

	_Parallax("Height Scale", Range(0.005, 0.08)) = 0.02
		_ParallaxMap("Height Map", 2D) = "black" {}

	_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

	_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

	_DetailMask("Detail Mask", 2D) = "white" {}

	_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
	_DetailNormalMapScale("Scale", Float) = 1.0
		_DetailNormalMap("Normal Map", 2D) = "bump" {}

	[HideInInspector]
	_Flip("Flip", Int) = 1

	[Enum(UV0,0,UV1,1)] _UVSec("UV Set for secondary textures", Float) = 0


		// Blending state
		[HideInInspector] _Mode("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
	}

		CGINCLUDE
#define UNITY_SETUP_BRDF_INPUT MetallicSetup
		ENDCG

		SubShader
	{
		Tags{ "RenderType" = "Opaque" "PerformanceChecks" = "False" }
		LOD 300

		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
	{
		Name "FORWARD"
		Tags{ "LightMode" = "ForwardBase" }

		Blend[_SrcBlend][_DstBlend]
		ZWrite[_ZWrite]
		Cull Off

		CGPROGRAM
#pragma target 3.0

		// -------------------------------------

#pragma shader_feature _NORMALMAP
#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma shader_feature _EMISSION
#pragma shader_feature _METALLICGLOSSMAP
#pragma shader_feature ___ _DETAIL_MULX2
#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
#pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
#pragma shader_feature _PARALLAXMAP

#pragma multi_compile_fwdbase
#pragma multi_compile_fog

#pragma vertex vertBase
#pragma fragment fragBaseDS


		int _Flip;

#ifndef UNITY_STANDARD_CORE_FORWARD_INCLUDED
#define UNITY_STANDARD_CORE_FORWARD_INCLUDED

#if defined(UNITY_NO_FULL_STANDARD_SHADER)
#	define UNITY_STANDARD_SIMPLE 1
#endif

#include "UnityStandardConfig.cginc"

#if UNITY_STANDARD_SIMPLE
#include "UnityStandardCore.cginc"
#include "UnityStandardCoreForwardSimple.cginc"
	VertexOutputBaseSimple vertBase(VertexInput v) { return vertForwardBaseSimple(v); }


#ifndef UNITY_STANDARD_CORE_FORWARD_SIMPLE_INCLUDED
#define UNITY_STANDARD_CORE_FORWARD_SIMPLE_INCLUDED
#if defined(UNITY_NO_FULL_STANDARD_SHADER)
#	define UNITY_STANDARD_SIMPLE 1
#endif

	half4 fragForwardBaseSimpleInternalDS(VertexOutputBaseSimple i,
		in float face)
	{
		FragmentCommonData s = FragmentSetupSimple(i);

		float _sign = sign(face);
		float3 normal = s.normalWorld;
		normal.y *= _sign;
		normal.xyz *= _sign * _Flip;
		s.normalWorld = normal;


		UnityLight mainLight = MainLightSimple(i, s);

		half atten = SHADOW_ATTENUATION(i);

		half occlusion = Occlusion(i.tex.xy);
		half rl = dot(REFLECTVEC_FOR_SPECULAR(i, s), LightDirForSpecular(i, mainLight));

		UnityGI gi = FragmentGI(s, occlusion, i.ambientOrLightmapUV, atten, mainLight);
		half3 attenuatedLightColor = gi.light.color * mainLight.ndotl;

		half3 c = BRDF3_Indirect(s.diffColor, s.specColor, gi.indirect, PerVertexGrazingTerm(i, s), PerVertexFresnelTerm(i));
		c += BRDF3DirectSimple(s.diffColor, s.specColor, s.oneMinusRoughness, rl) * attenuatedLightColor;
		c += UNITY_BRDF_GI(s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, occlusion, gi);
		c += Emission(i.tex.xy);

		UNITY_APPLY_FOG(i.fogCoord, c);

		return OutputForward(half4(c, 1), s.alpha);
	}

	half4 fragBaseDS(VertexOutputBaseSimple i, in float face : VFACE) : SV_Target{ return fragForwardBaseSimpleInternalDS(i, face); }

#endif // UNITY_STANDARD_CORE_FORWARD_INCLUDED


#else
#include "UnityStandardCore.cginc"
	VertexOutputForwardBase vertBase(VertexInput v) { return vertForwardBase(v); }


	half4 fragForwardBaseInternalDS(VertexOutputForwardBase i, in float face)
	{
		FRAGMENT_SETUP(s)

		float _sign = sign(face);
		float3 normal = s.normalWorld;
		normal.y *= _sign;
		normal.xyz *= _sign * _Flip;
		s.normalWorld = normal;


#if UNITY_OPTIMIZE_TEXCUBELOD
		s.reflUVW = i.reflUVW;
#endif

		UnityLight mainLight = MainLight(/*normal*/);
		half atten = SHADOW_ATTENUATION(i);


		half occlusion = Occlusion(i.tex.xy);
		UnityGI gi = FragmentGI(s, occlusion, i.ambientOrLightmapUV, atten, mainLight);

		half4 c = UNITY_BRDF_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, (-s.eyeVec), gi.light, gi.indirect);
		c.rgb += UNITY_BRDF_GI(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, (-s.eyeVec), occlusion, gi);
		c.rgb += Emission(i.tex.xy);

		UNITY_APPLY_FOG(i.fogCoord, c.rgb);
		return OutputForward(c, s.alpha);
	}

	half4 fragBaseDS(VertexOutputForwardBase i, in float face : VFACE) : SV_Target{ return fragForwardBaseInternalDS(i, face); }

#endif

#endif // UNITY_STANDARD_CORE_FORWARD_INCLUDED


		ENDCG
	}
		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
		Pass
	{
		Name "FORWARD_DELTA"
		Tags{ "LightMode" = "ForwardAdd" }
		Blend[_SrcBlend] One
		Fog{ Color(0,0,0,0) } // in additive pass fog should be black
		ZWrite Off
		ZTest LEqual
		Cull Off
		CGPROGRAM
#pragma target 3.0

		// -------------------------------------


#pragma shader_feature _NORMALMAP
#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma shader_feature _METALLICGLOSSMAP
#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
#pragma shader_feature ___ _DETAIL_MULX2
#pragma shader_feature _PARALLAXMAP

#pragma multi_compile_fwdadd_fullshadows
#pragma multi_compile_fog


#pragma vertex vertAdd
#pragma fragment fragAddDS

			int _Flip;

#ifndef UNITY_STANDARD_CORE_FORWARD_INCLUDED
#define UNITY_STANDARD_CORE_FORWARD_INCLUDED

#if defined(UNITY_NO_FULL_STANDARD_SHADER)
#	define UNITY_STANDARD_SIMPLE 1
#endif

#include "UnityStandardConfig.cginc"

#if UNITY_STANDARD_SIMPLE
#include "UnityStandardCore.cginc"
#include "UnityStandardCoreForwardSimple.cginc"
		VertexOutputForwardAddSimple vertAdd(VertexInput v) { return vertForwardAddSimple(v); }


#ifndef UNITY_STANDARD_CORE_FORWARD_SIMPLE_INCLUDED
#define UNITY_STANDARD_CORE_FORWARD_SIMPLE_INCLUDED
#if defined(UNITY_NO_FULL_STANDARD_SHADER)
#	define UNITY_STANDARD_SIMPLE 1
#endif

		half4 fragForwardAddSimpleInternalDS(VertexOutputForwardAddSimple i)
		{
			FragmentCommonData s = FragmentSetupSimpleAdd(i);

			float _sign = sign(face);
			float3 normal = s.normalWorld;
			normal.y *= _sign;
			normal.xyz *= _sign * _Flip;
			s.normalWorld = normal;

			half3 c = BRDF3DirectSimple(s.diffColor, s.specColor, s.oneMinusRoughness, dot(REFLECTVEC_FOR_SPECULAR(i, s), i.lightDir));

#if SPECULAR_HIGHLIGHTS // else diffColor has premultiplied light color
			c *= _LightColor0.rgb;
#endif

			c *= LIGHT_ATTENUATION(i) * LambertTerm(LightSpaceNormal(i, s), i.lightDir);

			UNITY_APPLY_FOG_COLOR(i.fogCoord, c.rgb, half4(0, 0, 0, 0)); // fog towards black in additive pass
			return OutputForward(half4(c, 1), s.alpha);
		}

		half4 fragAddDS(VertexOutputForwardAddSimple i, in float face : VFACE) : SV_Target{ return fragForwardAddSimpleInternalDS(i, face); }

#endif // UNITY_STANDARD_CORE_FORWARD_INCLUDED


#else
#include "UnityStandardCore.cginc"
		VertexOutputForwardAdd vertAdd(VertexInput v) { return vertForwardAdd(v); }


		half4 fragForwardAddInternalDS(VertexOutputForwardAdd i, float face)
		{
			FRAGMENT_SETUP_FWDADD(s)

			float _sign = sign(face);
			float3 normal = s.normalWorld;
			normal.y *= _sign;
			normal.xyz *= _sign * _Flip;
			s.normalWorld = normal;

			//UnityLight light = AdditiveLight(s.normalWorld, IN_LIGHTDIR_FWDADD(i), LIGHT_ATTENUATION(i));
			//UnityIndirect noIndirect = ZeroIndirect();
			//half4 c = UNITY_BRDF_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, light, noIndirect);
			//UNITY_APPLY_FOG_COLOR(i.fogCoord, c.rgb, half4(0, 0, 0, 0)); // fog towards black in additive pass
			//return OutputForward(c, s.alpha);

			UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld)
			UnityLight light = AdditiveLight(IN_LIGHTDIR_FWDADD(i), atten);
			UnityIndirect noIndirect = ZeroIndirect();
			half4 c = UNITY_BRDF_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, light, noIndirect);
			UNITY_APPLY_FOG_COLOR(i.fogCoord, c.rgb, half4(0, 0, 0, 0)); // fog towards black in additive pass
			return OutputForward(c, s.alpha);
		}

		half4 fragAddDS(VertexOutputForwardAdd i, in float face : VFACE) : SV_Target{ return fragForwardAddInternalDS(i, face); }

#endif

#endif // UNITY_STANDARD_CORE_FORWARD_INCLUDED



		ENDCG
	}
		// ------------------------------------------------------------------
		//  Shadow rendering pass
		Pass
	{
		Name "ShadowCaster"
		Tags{ "LightMode" = "ShadowCaster" }

		ZWrite On ZTest LEqual
		Cull Off
		CGPROGRAM
#pragma target 3.0

		// -------------------------------------


#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma multi_compile_shadowcaster

#pragma vertex vertShadowCaster
#pragma fragment fragShadowCaster

#include "UnityStandardShadow.cginc"

		ENDCG
	}
		// ------------------------------------------------------------------
		//  Deferred pass
		Pass
	{
		Name "DEFERRED"
		Tags{ "LightMode" = "Deferred" }
		Cull Off
		CGPROGRAM
#pragma target 3.0
#pragma exclude_renderers nomrt


		// -------------------------------------

#pragma shader_feature _NORMALMAP
#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma shader_feature _EMISSION
#pragma shader_feature _METALLICGLOSSMAP
#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
#pragma shader_feature ___ _DETAIL_MULX2
#pragma shader_feature _PARALLAXMAP

#pragma multi_compile ___ UNITY_HDR_ON
#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
#pragma multi_compile ___ DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON

#pragma vertex vertDeferred
#pragma fragment fragDeferredDS

#include "UnityStandardCore.cginc"

		int _Flip;

		void fragDeferredDS(
			VertexOutputDeferred i,
			in float face : VFACE,
			out half4 outGBuffer0 : SV_Target0,
			out half4 outGBuffer1 : SV_Target1,
			out half4 outGBuffer2 : SV_Target2,
			out half4 outEmission : SV_Target3          // RT3: emission (rgb), --unused-- (a)
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
			, out half4 outShadowMask : SV_Target4       // RT4: shadowmask (rgba)
#endif
		)
	{
#if (SHADER_TARGET < 30)
			outGBuffer0 = 1;
			outGBuffer1 = 1;
			outGBuffer2 = 0;
			outEmission = 0;
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
			outShadowMask = 1;
#endif
			return;
#endif

		FRAGMENT_SETUP(s)

		float _sign = sign(face);
		float3 normal = s.normalWorld;
		normal.y *= _sign;
		normal.xyz *= _sign * _Flip;
		s.normalWorld = normal;

		// no analytic lights in this pass
		UnityLight dummyLight = DummyLight();
		half atten = 1;

		// only GI
		half occlusion = Occlusion(i.tex.xy);
#if UNITY_ENABLE_REFLECTION_BUFFERS
		bool sampleReflectionsInDeferred = false;
#else
		bool sampleReflectionsInDeferred = true;
#endif

		UnityGI gi = FragmentGI(s, occlusion, i.ambientOrLightmapUV, atten, dummyLight, sampleReflectionsInDeferred);

		half3 emissiveColor = UNITY_BRDF_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect).rgb;

#ifdef _EMISSION
		emissiveColor += Emission(i.tex.xy);
#endif

#ifndef UNITY_HDR_ON
		emissiveColor.rgb = exp2(-emissiveColor.rgb);
#endif

		UnityStandardData data;
		data.diffuseColor = s.diffColor;
		data.occlusion = occlusion;
		data.specularColor = s.specColor;
		data.smoothness = s.smoothness;
		data.normalWorld = s.normalWorld;

		UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

		// Emissive lighting buffer
		outEmission = half4(emissiveColor, 1);

		// Baked direct lighting occlusion if any
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
		outShadowMask = UnityGetRawBakedOcclusions(i.ambientOrLightmapUV.xy, IN_WORLDPOS(i));
#endif
	}

	ENDCG
	}

		// ------------------------------------------------------------------
		// Extracts information for lightmapping, GI (emission, albedo, ...)
		// This pass it not used during regular rendering.
		Pass
	{
		Name "META"
		Tags{ "LightMode" = "Meta" }

		Cull Off

		CGPROGRAM
#pragma vertex vert_meta
#pragma fragment frag_meta

#pragma shader_feature _EMISSION
#pragma shader_feature _METALLICGLOSSMAP
#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature ___ _DETAIL_MULX2

#include "UnityStandardMeta.cginc"
		ENDCG
	}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" "PerformanceChecks" = "False" }
		LOD 150


		// ------------------------------------------------------------------
		// Extracts information for lightmapping, GI (emission, albedo, ...)
		// This pass it not used during regular rendering.
		Pass
	{
		Name "META"
		Tags{ "LightMode" = "Meta" }

		Cull Off

		CGPROGRAM
#pragma vertex vert_meta
#pragma fragment frag_meta

#pragma shader_feature _EMISSION
#pragma shader_feature _METALLICGLOSSMAP
#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature ___ _DETAIL_MULX2

#include "UnityStandardMeta.cginc"
		ENDCG
	}
	}

		FallBack "VertexLit"
		CustomEditor "StandardShaderGUI"
}
