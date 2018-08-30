Shader "Custom/DistotionImageEffectShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_VDist("Vertical Distotion", Range(0.01, 2.0)) = 1.5
		_HDist("Horizontal Distotion", Range(0.01, 2.0)) = 1.7
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
			float _VDist;
			float _HDist;

			fixed4 frag(v2f i) : SV_Target
			{
				float2 i_uv = i.uv;
				float2 opt1 = float2(_HDist, _VDist);
				float2 opt2 = float2(2, 2);
				float2 vv = 0.5 / tan(atan(0.5 * opt2) * opt1);
				i_uv = tan(atan((i_uv - 0.5) * opt2) * opt1) * vv + 0.5;
				half4 col = tex2D(_MainTex, i_uv);
				return col;
			}
			ENDCG
		}
	}
}
