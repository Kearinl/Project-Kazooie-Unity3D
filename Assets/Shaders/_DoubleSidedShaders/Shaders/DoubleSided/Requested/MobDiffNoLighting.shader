// © 2015 Mario Lelas

// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "DoubleSided/Mobile/DiffuseNoLighting"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}

	SubShader
	{
			Tags{ "RenderType" = "Opaque" }
			Cull Off
			LOD 150

			CGPROGRAM


#pragma surface surfDS LambertDS  noforwardadd
#pragma target 3.0



			sampler2D _MainTex;
			fixed4 _Color;
			float _Cutoff;

			struct Input
			{
				float2 uv_MainTex;
			};

			struct Output
			{
				fixed3 Albedo;
				fixed3 Normal;
				fixed3 Emission;
				fixed Alpha;
			};

			inline fixed4 UnityLambertLightDS(Output s, UnityLight light)
			{
				fixed4 c;
				c.rgb = s.Albedo;
				c.a = s.Alpha;
				return c;
			}

			inline void LightingLambertDS_GI(
				Output s,
				UnityGIInput data,
				inout UnityGI gi)
			{
				gi.light.color = half3(0, 0, 0);
				gi.light.dir = half3(0, 0, 1);
				gi.indirect.diffuse = half3(0, 0, 0);
				gi.indirect.specular = half3(0, 0, 0);
			}

			inline fixed4 LightingLambertDS(Output s, UnityGI gi)
			{

				return fixed4(s.Albedo.rgb, s.Alpha);
			}


			void surfDS(Input IN,  inout Output o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

				clip(c.a - _Cutoff);

				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}

			ENDCG

	}
	Fallback "DoubleSided/Other/VertexLitCutoutCullOff"
}
