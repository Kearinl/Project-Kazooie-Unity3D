// © 2015 Mario Lelas

// Simplified Bumped shader. Differences from regular Bumped one:
// - no Main Color
// - Normalmap uses Tiling/Offset of the Base texture
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "DoubleSided/Mobile/Bumped Diffuse"
 {
	Properties
	 {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		Cull Off
		LOD 250

		CGPROGRAM
		#pragma surface surf Lambert noforwardadd
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input 
		{
			float2 uv_MainTex;
			float face : VFACE;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			float3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			o.Normal = normal * sign(IN.face);
		}
		ENDCG  
	}

	FallBack "DoubleSided/Mobile/VertexLitCullOff"
}
