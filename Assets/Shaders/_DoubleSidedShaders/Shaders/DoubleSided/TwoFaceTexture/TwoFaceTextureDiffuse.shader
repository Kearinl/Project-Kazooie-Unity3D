// © 2015 Mario Lelas




Shader "DoubleSided/TwoFace/TwoFaceTextureDiffuse" 
{
	Properties
	 {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Face Front (RGB)", 2D) = "white" {}
		_SecTex("Face Back (RGB)", 2D) = "white" {}
	}
	SubShader
	 {
		Tags { "RenderType"="Opaque" }
		Cull Off
		LOD 200

		CGPROGRAM
		#pragma surface surfDS LambertDS
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _SecTex;
		fixed4 _Color;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_SecTex;
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
			fixed4 frontTex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 backTex = tex2D(_SecTex, IN.uv_SecTex) * _Color;

			float _sign = sign(IN.face);
			float value = (_sign + 1.0f) / 2.0f;
			float4 alb = lerp(backTex, frontTex, value);

			o.Albedo = alb.rgb;
			o.Alpha = alb.a;
			o.face = IN.face;
		}
		ENDCG
	}

	Fallback "DoubleSided/Legacy Shaders/VertexLitCullOff"
}
