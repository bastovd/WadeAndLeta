﻿Shader "Waves/surf_Extrusion"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_HeightMap("Height Map", 2D) = "black" {}
		_Amount ("Extrusion Amount", Float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows addshadow vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
		#pragma glsl

        sampler2D _MainTex;
        sampler2D _HeightMap;
        float4 _HeightMap_ST;
		float _Amount;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_DepthTex;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v) {
			float2 hmuv = TRANSFORM_TEX(v.texcoord.xy, _HeightMap);
			fixed height = tex2Dlod(_HeightMap, float4(hmuv.xy, 0, 0)).r;
			v.vertex.xyz += v.normal * height * _Amount;
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
