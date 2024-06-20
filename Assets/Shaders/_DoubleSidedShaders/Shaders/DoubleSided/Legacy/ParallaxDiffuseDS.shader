// © 2015 Mario Lelas
Shader "DoubleSided/Legacy Shaders/Parallax DiffuseDS" 
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_Parallax ("Height", Range (0.005, 0.08)) = 0.02
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_ParallaxMap ("Heightmap (A)", 2D) = "black" {}
	}

	CGINCLUDE
	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _ParallaxMap;
	fixed4 _Color;
	float _Parallax;

	struct Input 
	{
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float3 viewDir;
		float face : VFACE;
	};


	void surfDS (Input IN, inout SurfaceOutput o) 
	{
		half h = tex2D (_ParallaxMap, IN.uv_BumpMap).w;
		float2 offset = ParallaxOffset (h, _Parallax, IN.viewDir * sign(IN.face));
		IN.uv_MainTex += offset;
		IN.uv_BumpMap += offset;
	
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
		float3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
		o.Normal = normal * sign(IN.face);
	}
	ENDCG



	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		Cull Off
		LOD 500

		CGPROGRAM
		#pragma surface surfDS Lambert
		#pragma target 3.0
		ENDCG

		
	}

	SubShader
	 {
		Tags { "RenderType"="Opaque" }
		Cull Off
		LOD 500

		CGPROGRAM
		#pragma surface surfDS Lambert nodynlightmap
		#pragma target 3.0
		ENDCG
	}

	FallBack "DoubleSided/Legacy Shaders/VertexLitCullOff"
}

