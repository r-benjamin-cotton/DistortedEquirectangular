Shader "Custom/SoftenImageEffectShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader
	{
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

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			fixed4 frag(v2f i) : SV_Target
			{
				float k = 0.2;
				float2 uv00 = i.uv + float2(-k, -k) * _MainTex_TexelSize.xy;
				float2 uv01 = i.uv + float2(+k, -k) * _MainTex_TexelSize.xy;
				float2 uv10 = i.uv + float2(-k, +k) * _MainTex_TexelSize.xy;
				float2 uv11 = i.uv + float2(+k, +k) * _MainTex_TexelSize.xy;
				half4 c00 = tex2D(_MainTex, uv00);
				half4 c01 = tex2D(_MainTex, uv01);
				half4 c10 = tex2D(_MainTex, uv10);
				half4 c11 = tex2D(_MainTex, uv11);
				half4 col = (c00 + c01 + c10 + c11) * 0.25;
				return col;
			}
			ENDCG
		}
	}
}
