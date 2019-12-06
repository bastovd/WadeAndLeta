Shader "Waves/surf_PointDistort"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_Amount ("Extrusion Amount", Float) = 0.5
		_DistanceMultiplier ("Distance Mult", Float) = 5.0
		_DistancePower ("Distance Power", Float) = 2.0
    }
    SubShader
    {
        //Tags { "RenderType"="Opaque" } // !!! ENABLE LATER
		Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
		//#pragma surface surf Standard fullforwardshadows addshadow vertex:vert // !!! ENABLE LATER
		#pragma surface surf Standard fullforwardshadows vertex:vert alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
		#pragma glsl

        sampler2D _MainTex;
		float _Amount;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// incoming
		float _DistanceMultiplier;
		float _DistancePower;
		uniform float3 _CollisionPoint;

        struct Input
        {
            float2 uv_MainTex;
			float3 worldPos;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v) {
			//v.vertex.xyz += v.normal * _Amount;
			v.vertex = mul(UNITY_MATRIX_MV, v.vertex);
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float distanceToPopper = length(IN.worldPos - _CollisionPoint);

			// Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			float dist = saturate(distanceToPopper * _DistanceMultiplier);
			float power = pow(dist, _DistancePower);
			o.Albedo = c.rgb * dist;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

			o.Alpha = c.a * power;
			//o.Alpha *= distanceToPopper * _DistanceMultiplier;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
