Shader "Waves/TriGradient"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ColorA ("Color A", Color) = (1,1,1,1)
		_ColorB ("Color B", Color) = (1,1,1,1)
		_ColorC ("Color C", Color) = (1,1,1,1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

			float4 _ColorA;
			float4 _ColorB;
			float4 _ColorC;

            fixed4 frag (v2f i) : SV_Target
            {
				float4 emissive = lerp(_ColorB.rgba, _ColorC.rgba, (i.uv.y - 0.5) / 0.5) * step(1.0 - 0.5, i.uv.y);
				emissive += lerp(_ColorA.rgba, _ColorB.rgba, (i.uv.y - 0.0) / 0.5) * step(i.uv.y, 0.5);
                
				return emissive;
            }
            ENDCG
        }
    }
}
