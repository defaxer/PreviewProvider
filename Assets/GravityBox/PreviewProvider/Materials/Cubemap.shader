﻿Shader "PreviewGenerator/Cubemap" 
{
	Properties
	{
		_Cubemap("Cubemap(RGB)", CUBE) = "" { }
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		Pass 
		{
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f 
			{
				float4 pos : SV_POSITION;
				float3 cubenormal : TEXCOORD1;
			};

			samplerCUBE _Cubemap;
			
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 worldNrm = mul(unity_ObjectToWorld, v.normal).xyz;
				float3 worldView = normalize(worldPos - _WorldSpaceCameraPos);

				o.cubenormal = reflect(worldView, worldNrm);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return texCUBE(_Cubemap, i.cubenormal);
			}
			ENDCG
		}
	}
	Fallback "Unlit/Texture"
}