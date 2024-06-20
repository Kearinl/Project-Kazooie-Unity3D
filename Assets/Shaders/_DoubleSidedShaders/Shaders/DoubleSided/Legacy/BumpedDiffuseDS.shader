// © 2015 Mario Lelas
Shader "DoubleSided/Legacy Shaders/Bumped DiffuseDS" 
{
	Properties
	 {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		Cull Off
		LOD 300

		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _Color;

		struct Input
		 {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float face : VFACE;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			float3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Normal = normal * sign(IN.face);
		}
		ENDCG  
	}

	FallBack "DoubleSided/Legacy Shaders/VertexLitCullOff"
}
