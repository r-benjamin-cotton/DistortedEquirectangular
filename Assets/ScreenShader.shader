Shader "Custom/ScreenShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_VDist("Vertical Distotion", Range(0.01, 2.0)) = 1.5
		_HDist("Horizontal Distotion", Range(0.01, 2.0)) = 1.7
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _VDist;
			float _HDist;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
#if 0
				float2 opt1 = float2(_HDist, _VDist);
				float2 opt2 = tan(0.785398 * opt1) * 2.0;
				o.uv = tan(atan((o.uv - 0.5) * opt2) / opt1) * 0.5 + 0.5;
#endif
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				float2 i_uv = i.uv;
#if 1
				float2 opt1 = float2(_HDist, _VDist);
				float2 opt2 = tan(0.785398 * opt1) * 2.0;
				i_uv = tan(atan((i_uv - 0.5) * opt2) / opt1) * 0.5 + 0.5;
#endif
				half4 col = tex2D(_MainTex, i_uv);
				return col;
			}
			ENDCG
		}
	}
}
