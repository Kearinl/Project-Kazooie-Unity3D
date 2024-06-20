// © 2016 Mario Lelas

Shader "DoubleSided/Other/MobileBumpTileBoth" 
	{
		Properties
		{
			_MainTex("Base (RGB)", 2D) = "white" {}
			_BumpMap("Normalmap", 2D) = "bump" {}
		}
			SubShader
		{
			Tags{ "RenderType" = "Opaque" }
			Cull Off
			LOD 250

			CGPROGRAM
			#pragma surface surf Lambert noforwardadd 
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _BumpMap;


			struct Input
			{
				float2 uv_MainTex : TEXCOORD0;
				float2 uv_BumpMap : TEXCOORD1;
				float face : VFACE;
			};

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
				float3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap.xy));
				float _sign = sign(IN.face);

				o.Albedo = tex.rgb;
				o.Alpha = tex.a;
				o.Normal = normal * _sign;
			}
			ENDCG
		}
		FallBack "DoubleSided/Mobile/VertexLitCullOff"
	}
