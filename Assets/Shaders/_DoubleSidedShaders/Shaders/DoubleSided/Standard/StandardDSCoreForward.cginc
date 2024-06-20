
#ifndef UNITY_STANDARD_CORE_FORWARD_INCLUDED
#define UNITY_STANDARD_CORE_FORWARD_INCLUDED

#if defined(UNITY_NO_FULL_STANDARD_SHADER)
#   define UNITY_STANDARD_SIMPLE 1
#endif

#include "UnityStandardConfig.cginc"

#if UNITY_STANDARD_SIMPLE

#include "UnityStandardCoreForwardSimple.cginc"

half4 fragForwardBaseSimpleInternalDS(VertexOutputBaseSimple i, in float face : VFACE)
{
	UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);

	FragmentCommonData s = FragmentSetupSimple(i);

	float _sign = sign(face);
	/// flip direction of normal based on sign of face
	float3 normal = s.normalWorld * _sign;
	s.normalWorld = normal;

	UnityLight mainLight = MainLightSimple(i, s);

#if !defined(LIGHTMAP_ON) && defined(_NORMALMAP)
	half ndotl = saturate(dot(s.tangentSpaceNormal, i.tangentSpaceLightDir));
#else
	half ndotl = saturate(dot(s.normalWorld, mainLight.dir));
#endif

	//we can't have worldpos here (not enough interpolator on SM 2.0) so no shadow fade in that case.
	half shadowMaskAttenuation = UnitySampleBakedOcclusion(i.ambientOrLightmapUV, 0);
	half realtimeShadowAttenuation = SHADOW_ATTENUATION(i);
	half atten = UnityMixRealtimeAndBakedShadows(realtimeShadowAttenuation, shadowMaskAttenuation, 0);

	half occlusion = Occlusion(i.tex.xy);
	half rl = dot(REFLECTVEC_FOR_SPECULAR(i, s), LightDirForSpecular(i, mainLight));

	UnityGI gi = FragmentGI(s, occlusion, i.ambientOrLightmapUV, atten, mainLight);
	half3 attenuatedLightColor = gi.light.color * ndotl;

	half3 c = BRDF3_Indirect(s.diffColor, s.specColor, gi.indirect, PerVertexGrazingTerm(i, s), PerVertexFresnelTerm(i));
	c += BRDF3DirectSimple(s.diffColor, s.specColor, s.smoothness, rl) * attenuatedLightColor;
	c += Emission(i.tex.xy);

	UNITY_APPLY_FOG(i.fogCoord, c);

	return OutputForward(half4(c, 1), s.alpha);
}

half4 fragForwardAddSimpleInternalDS(VertexOutputForwardAddSimple i, in float face : VFACE)
{
	UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);

	FragmentCommonData s = FragmentSetupSimpleAdd(i);

	float _sign = sign(face);
	/// flip direction of normal based on sign of face
	float3 normal = s.normalWorld * _sign;
	s.normalWorld = normal;

	half3 c = BRDF3DirectSimple(s.diffColor, s.specColor, s.smoothness, dot(REFLECTVEC_FOR_SPECULAR(i, s), i.lightDir));

#if SPECULAR_HIGHLIGHTS // else diffColor has premultiplied light color
	c *= _LightColor0.rgb;
#endif

	UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld)
	c *= atten * saturate(dot(LightSpaceNormal(i, s), i.lightDir));

	UNITY_APPLY_FOG_COLOR(i.fogCoord, c.rgb, half4(0, 0, 0, 0)); // fog towards black in additive pass
	return OutputForward(half4(c, 1), s.alpha);
}

VertexOutputBaseSimple vertBase(VertexInput v) { return vertForwardBaseSimple(v); }
VertexOutputForwardAddSimple vertAdd(VertexInput v) { return vertForwardAddSimple(v); }
half4 fragBaseDS(VertexOutputBaseSimple i,in float face : VFACE) : SV_Target{ return fragForwardBaseSimpleInternalDS(i, face); }
half4 fragAddDS(VertexOutputForwardAddSimple i, in float face : VFACE) : SV_Target{ return fragForwardAddSimpleInternalDS(i, face); }

#else

#include "UnityStandardCore.cginc"

half4 fragForwardBaseInternalDS(VertexOutputForwardBase i, in float face : VFACE)
{
	UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);

	FRAGMENT_SETUP(s)

	float _sign = sign(face);
	/// flip direction of normal based on sign of face
	float3 normal = s.normalWorld * _sign;
	s.normalWorld = normal;

	UNITY_SETUP_INSTANCE_ID(i);
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

	UnityLight mainLight = MainLight();
	UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);

	half occlusion = Occlusion(i.tex.xy);
	UnityGI gi = FragmentGI(s, occlusion, i.ambientOrLightmapUV, atten, mainLight);

	half4 c = UNITY_BRDF_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);
	c.rgb += Emission(i.tex.xy);

	UNITY_EXTRACT_FOG_FROM_EYE_VEC(i);
	UNITY_APPLY_FOG(_unity_fogCoord, c.rgb);
	return OutputForward(c, s.alpha);

}
half4 fragForwardAddInternalDS(VertexOutputForwardAdd i, in float face : VFACE)
{
	UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);

	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

	FRAGMENT_SETUP_FWDADD(s)

	float _sign = sign(face);
	/// flip direction of normal based on sign of face
	float3 normal = s.normalWorld * _sign;
	s.normalWorld = normal;

	UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld)
	UnityLight light = AdditiveLight(IN_LIGHTDIR_FWDADD(i), atten);
	UnityIndirect noIndirect = ZeroIndirect();

	half4 c = UNITY_BRDF_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, light, noIndirect);

	UNITY_EXTRACT_FOG_FROM_EYE_VEC(i);
	UNITY_APPLY_FOG_COLOR(_unity_fogCoord, c.rgb, half4(0, 0, 0, 0)); // fog towards black in additive pass
	return OutputForward(c, s.alpha);

}

VertexOutputForwardBase vertBase(VertexInput v) { return vertForwardBase(v); }
VertexOutputForwardAdd vertAdd(VertexInput v) { return vertForwardAdd(v); }
half4 fragBaseDS(VertexOutputForwardBase i, in float face : VFACE) : SV_Target{ return fragForwardBaseInternalDS(i, face); }
half4 fragAddDS(VertexOutputForwardAdd i, in float face : VFACE) : SV_Target{ return fragForwardAddInternalDS(i, face); }

#endif

#endif // UNITY_STANDARD_CORE_FORWARD_INCLUDED