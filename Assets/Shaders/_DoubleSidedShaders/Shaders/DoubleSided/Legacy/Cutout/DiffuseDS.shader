// © 2015 Mario Lelas
Shader "DoubleSided/Legacy Shaders/Cutout/Diffuse" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}

	SubShader
	 {
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		Cull Off
		LOD 200
	
		CGPROGRAM
		#pragma surface surfDS LambertDS alphatest:_Cutoff
		#pragma target 3.0


		sampler2D _MainTex;
		fixed4 _Color;

		struct Input 
		{
			float2 uv_MainTex;
			float face : VFACE;
		};

		struct Output
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			half Specular;
			fixed Gloss;
			fixed Alpha;
			float face;
		};

		inline fixed4 UnityLambertLightDS (Output s, UnityLight light)
		{
			fixed diff = max (0, dot (s.Normal, light.dir));
			fixed4 c;
			c.rgb = s.Albedo * light.color * diff;
			c.a = s.Alpha;
			return c;
		}

		inline void LightingLambertDS_GI (
			Output s,
			UnityGIInput data,
			inout UnityGI gi)
		{
			gi = UnityGlobalIllumination (data, 1.0, 0.0, s.Normal, false);
		}

		inline fixed4 LightingLambertDS (Output s, UnityGI gi)
		{
			float3 normal = s.Normal * sign(s.face);
			s.Normal = normal;

			fixed4 c;
			c = UnityLambertLightDS (s, gi.light);

#if defined(DIRLIGHTMAP_SEPARATE)
#ifdef LIGHTMAP_ON
					c += UnityLambertLightDS (s, gi.light2);
#endif
#ifdef DYNAMICLIGHTMAP_ON
					c += UnityLambertLightDS (s, gi.light3);
				#endif
#endif

#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
				c.rgb += s.Albedo * gi.indirect.diffuse;
#endif

			return c;
		}

		void surfDS (Input IN, inout Output o)
		 {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.face = IN.face;
		}
		ENDCG
	}

	Fallback "DoubleSided/Legacy Shaders/VertexLitCutoutCullOff"
}
