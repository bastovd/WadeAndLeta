Shader "Unlit/FresnelTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		//_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		[PowerSlider(4)] _FresnelExponent("Fresnel Exponent", Range(0.25, 4)) = 1

		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		_FresnelBias("Fresnel Bias", Float) = 0
		_FresnelScale("Fresnel Scale", Float) = 1
		_FresnelPower("Fresnel Power", Float) = 1
    }
    SubShader
    {
        Tags { 
			"Queue" = "Geometry"
			"IgnoreProjector" = "True"
			"RenderType" = "Opaque"
		}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
				float3 wPos : TEXCOORD2; // World position
				half3 worldNormal : TEXCOORD3; // World normal

				float fresnel : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _Color;
			// test
			float _FresnelExponent;

			fixed4 _FresnelColor;
			fixed _FresnelBias;
			fixed _FresnelScale;
			fixed _FresnelPower;

            v2f vert (appdata v)
            {
                //v2f o;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                //return o;

				//v2f o;
				//o.vertex = UnityObjectToClipPos(v.vertex);
				//o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				//o.worldNormal = UnityObjectToWorldNormal(v.normal);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o, o.vertex);

				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				float3 i = normalize(ObjSpaceViewDir(v.pos));
				o.fresnel = _FresnelBias + _FresnelScale * pow(1 + dot(i, v.normal), _FresnelPower);
				return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
    //            // sample the texture
    //            fixed4 col = tex2D(_MainTex, i.uv);

				//float3 viewDirection = normalize(i.wPos - _WorldSpaceCameraPos);
				////get the dot product between the normal and the view direction
				//float fresnel = dot(i.worldNormal, viewDirection);
				////invert the fresnel so the big values are on the outside
				//fresnel = saturate(1 - fresnel);
				////raise the fresnel value to the exponents power to be able to adjust it
				//fresnel = pow(fresnel, _FresnelExponent);
				////combine the fresnel value with a color
				//float3 fresnelColor = fresnel * _FresnelColor;

				//col.rgb = lerp(col.rgb, fresnelColor, fresnel);
				////col.rgb += fresnelColor;
    //            // apply fog
    //            //UNITY_APPLY_FOG(i.fogCoord, col);
    //            return col;

				fixed4 c = tex2D(_MainTex, i.uv) * _Color;
				return lerp(c, _FresnelColor, 1 - i.fresnel);
            }
            ENDCG
        }
    }
}
