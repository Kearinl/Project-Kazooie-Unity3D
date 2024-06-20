// © 2017 Mario Lelas
Shader "DoubleSided/TwoFace/TwoFaceVertexLitCutoutCullOff" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_SpecColor("Spec Color", Color) = (1,1,1,0)
		_Emission("Emissive Color", Color) = (0,0,0,0)
		_Shininess("Shininess", Range(0.1, 1)) = 0.7
	}

		SubShader
	{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 100

		// Non-lightmapped
		Pass{
		Tags{ "LightMode" = "Vertex" }
		Alphatest Greater[_Cutoff]
		AlphaToMask True
		ColorMask RGB
		Material{
		Diffuse[_Color]
		Ambient[_Color]
		Shininess[_Shininess]
		Specular[_SpecColor]
		Emission[_Emission]
	}
		Lighting On
		SeparateSpecular On
		SetTexture[_MainTex]{
		Combine texture * primary DOUBLE, texture * primary
	}
	}

		// Lightmapped, encoded as dLDR
		Pass{
		Tags{ "LightMode" = "VertexLM" }
		Alphatest Greater[_Cutoff]
		AlphaToMask True
		ColorMask RGB

		BindChannels{
		Bind "Vertex", vertex
		Bind "normal", normal
		Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
		Bind "texcoord", texcoord1 // main uses 1st uv
	}
		SetTexture[unity_Lightmap]{
		matrix[unity_LightmapMatrix]
		constantColor[_Color]
		combine texture * constant
	}
		SetTexture[_MainTex]{
		combine texture * previous DOUBLE, texture * primary
	}
	}

		// Lightmapped, encoded as RGBM
		Pass
	{
		Tags{ "LightMode" = "VertexLMRGBM" }
		Alphatest Greater[_Cutoff]
		AlphaToMask True
		ColorMask RGB

		BindChannels
	{
		Bind "Vertex", vertex
		Bind "normal", normal
		Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
		Bind "texcoord1", texcoord1 // unused
		Bind "texcoord", texcoord2 // main uses 1st uv
	}

		SetTexture[unity_Lightmap]
	{
		matrix[unity_LightmapMatrix]
		combine texture * texture alpha DOUBLE
	}

		SetTexture[unity_Lightmap]
	{
		constantColor[_Color]
		combine previous * constant
	}
		SetTexture[_MainTex]
	{
		combine texture * previous QUAD, texture * primary
	}
	}

		// Pass to render object as a shadow caster
		Pass
	{
		Name "Caster"
		Tags{ "LightMode" = "ShadowCaster" }
		Cull Off


		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_shadowcaster
#include "UnityCG.cginc"

		struct v2f
	{
		V2F_SHADOW_CASTER;
		float2  uv : TEXCOORD1;
	};

	uniform float4 _MainTex_ST;

	v2f vert(appdata_base v)
	{
		v2f o;
		TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}

	sampler2D _MainTex;
	sampler2D _SecTex;
	float _CutoffFront;
	float _CutoffBack;
	uniform fixed4 _Color;

	float4 frag(v2f i,float face : VFACE) : SV_Target
	{
		fixed4 frontTex = tex2D(_MainTex, i.uv);
	fixed4 backTex = tex2D(_SecTex, i.uv);
	float _sign = sign(face);
	if (_sign < 0)
		clip(backTex.a*_Color.a - _CutoffBack);
	else
		clip(frontTex.a*_Color.a - _CutoffFront);

	SHADOW_CASTER_FRAGMENT(i)
	}
		ENDCG
	}
	}

}
