Shader "Volumentric/VolumetricShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Radius("Radius", Float) = 0.5
		[Space]
		[Space]
		_Color("Color", Color) = (1,1,1,1)
		_ShadowColor("ShadowColor", Color) = (1,1,1,1)
		_SpecularPower("Specular Power", Float) = 0
		_Gloss("Gloss", Float) = 0
		[Space]
		[Space]
		_SMinK("Smooth min k", Int) = 16
		[Space]
		[Space]
		[PowerSlider(4)] _FresnelPow("Fresnel Power", Range(0.25, 4)) = 1
		[Space]
		[Space]
		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		_FresnelBias("Fresnel Bias", Float) = 0
		_FresnelScale("Fresnel Scale", Float) = 1
		_FresnelPower("Fresnel Power", Float) = 1
    }
    SubShader
    {
		// No culling
		//Cull Off
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //// make fog work
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
				float4 pos : SV_POSITION; // Clip space
				float3 wPos : TEXCOORD2; // World position
				half3 worldNormal : TEXCOORD3; // World normal
				fixed4 diff : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			uniform int _Count;
			uniform float4 _Centers[100]; // max number is 100
			uniform float _Radii[100];
			float _Radius;
			// lighting
			float4 _Color;
			float4 _ShadowColor;
			float _SpecularPower;
			float _Gloss;
			// smooth
			float _SMinK;
			// other effects
			float _FresnelPow;
			fixed4 _FresnelColor;
			fixed _FresnelBias;
			fixed _FresnelScale;
			fixed _FresnelPower;

			#define STEPS 128
			#define MIN_DISTANCE 0.01

			/// SHAPE BLENDING
			float sdf_blend(float d1, float d2, float a)
			{
				return a * d1 + (1 - a) * d2;
			}
			// ----------------


			/// SHAPES
			// sphere
			float sdf_sphere(float3 p, float3 c, float r)
			{
				return distance(p, c) - r;
			}

			// box
			float vmax(float3 v)
			{
				return max(max(v.x, v.y), v.z);
			}

			float sdf_boxcheap(float3 p, float3 c, float3 s)
			{
				return vmax(abs(p - c) - s);
			}
			/// --------------

			float sdf_smin(float3 p, float k = 16)
			{
				float res = 0;
				for (int i = 0; i < _Count; i++) {
					res += exp(-_SMinK * sdf_sphere(p, _Centers[i].xyz, _Radii[i]));
				}
				return -log(max(0.0001, res)) / _SMinK;
			}

			float map(float3 p)
			{
				return sdf_smin(p);
			}

			float blend(float3 p) {
				return sdf_blend
				(
					sdf_sphere(p, 0, 2),
					sdf_boxcheap(p, 0, 1.5),
					(_SinTime[3] + 1.) / 2.
				);
			}

			/// LIGHTING
			#include "Lighting.cginc"


			float3 normal(float3 p)
			{
				const float eps = 0.01;

				return normalize
				(float3
					(map(p + float3(eps, 0, 0)) - map(p - float3(eps, 0, 0)),
						map(p + float3(0, eps, 0)) - map(p - float3(0, eps, 0)),
						map(p + float3(0, 0, eps)) - map(p - float3(0, 0, eps))
						)
				);
			}

			fixed4 simpleLambert(fixed3 normal, fixed3 worldNormal, fixed3 worldPos) {
				fixed3 lightDir = _WorldSpaceLightPos0.xyz;	// Light direction
				//lightDir *= (_WorldSpaceLightPos0.w – 0.5) * 2.0;
				fixed3 lightCol = _LightColor0.rgb;		// Light color

				// Specular not working
				float3 viewDirection = normalize(worldPos - _WorldSpaceCameraPos);
				fixed3 h = (lightDir - viewDirection) / 2.; // viewDirection missing
				fixed s = pow(dot(normal, h), _SpecularPower) * _Gloss;

				// normal
				fixed NdotL = max(dot(normal, lightDir), 0);
				float3 diff = NdotL * lightCol;
				fixed4 c;
				c.rgb = _Color * diff;// +s; // specular not working

				// ambient DOESN'T WORK
				c.rgb += ShadeSH9(half4(normal, 1));
				//c.rgb = lerp(_Color * lightCol + s, _ShadowColor + s, 1 - NdotL);// *(1 - NdotL) * _ShadowColor;

				// fresnel
				fixed fresnel = _FresnelBias + _FresnelScale * pow(1 + dot(viewDirection, normal), _FresnelPower);
				c.rgb = lerp(c.rgb, _FresnelColor, fresnel);

				c.a = 1;
				return c;
			}

			fixed4 renderSurface(float3 p, float3 worldNormal, float3 worldPos)
			{
				float3 n = normal(p);
				return simpleLambert(n, worldNormal, worldPos);
			}
			///

			fixed4 raymarch(float3 position, float3 direction, float3 worldNormal, float3 worldPos)
			{
				for (int i = 0; i < STEPS; i++)
				{
					float distance = map(position);
					if (distance < MIN_DISTANCE)
						return renderSurface(position, worldNormal, worldPos);

					position += distance * direction;
				}
				return fixed4(1, 1, 1, 0);
			}

            v2f vert (appdata v)
            {
                v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				half3 wNormal = UnityObjectToWorldNormal(v.normal);
				o.worldNormal = wNormal;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				//o.diff = float4(1, 1, 1, 1);
				//o.diff.rgb += ShadeSH9(half4(wNormal, 1));
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				//float3 worldPosition = i.wPos;
				float3 viewDirection = normalize(i.wPos - _WorldSpaceCameraPos);
				fixed4 color = raymarch(i.wPos, viewDirection, i.worldNormal, i.wPos);
				color.rgb *= tex2D(_MainTex, i.uv).rgb;

				//color *= i.diff;

				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, color);
				return color;
            }
            ENDCG
        }
    }
}
