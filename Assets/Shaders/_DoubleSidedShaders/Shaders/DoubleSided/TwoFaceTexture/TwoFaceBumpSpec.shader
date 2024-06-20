// © 2015 Mario Lelas

Shader "DoubleSided/TwoFace/TwoFaceBumpSpec"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess("Shininess", Range(0.03, 1)) = 0.078125
		_MainTex("Face Front (RGB)", 2D) = "white" {}
		_SecTex("Face Back (RGB)", 2D) = "white" {}
		_BumpMap("Normalmap Front", 2D) = "bump" {}
		_BumpMapBack("Normalmap Back", 2D) = "bump" {}
		
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		Cull Off
		LOD 400

		CGPROGRAM
		#pragma surface surf BlinnPhong
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _SecTex;
		sampler2D _BumpMap;
		sampler2D _BumpMapBack;
		fixed4 _Color;
		half _Shininess;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_SecTex;
			float2 uv_BumpMap;
			float face : VFACE;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 frontTex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 backTex = tex2D(_SecTex, IN.uv_SecTex) * _Color;

			float3 normalFront = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			float3 normalBack = UnpackNormal(tex2D(_BumpMapBack, IN.uv_BumpMap));

			float _sign = sign(IN.face);
			float value = (_sign + 1.0f) / 2.0f;

			float4 alb = lerp(backTex, frontTex, value);
			o.Albedo = alb.rgb * _Color.rgb;
			o.Gloss = _Color.a; // alb.a;
			o.Alpha = alb.a * _Color.a;
			o.Specular = _Shininess;
			float3 norm = lerp(normalBack, normalFront, value);
			o.Normal = norm * sign(IN.face);
		}
		ENDCG
	}

		Fallback "DoubleSided/Legacy Shaders/VertexLitCullOff"
}
