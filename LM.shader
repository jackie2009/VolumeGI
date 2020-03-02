// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PengLu/Unlit/TextureLM" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_SHLightingScale("LightProbe influence scale",float) = 1
}
 
SubShader {
	Tags { "Queue"="Geometry""LightMode"="ForwardBase""RenderType"="Opaque"   }
	LOD 100
	
	Pass {  
		CGPROGRAM
 
 #pragma target 3.5
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
 
		
 
			struct v2f {
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				half2 texcoord : TEXCOORD0;
				float3  worldPos : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};
// uniform float4 allLPData[800]; 
       StructuredBuffer <float4> allLPData;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _SHLightingScale;
			   sampler3D _VolumeGITex;
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				 
				o.worldPos = mul(UNITY_MATRIX_M, v.vertex);
	            o.normal=UnityObjectToWorldNormal(v.normal);
			
							 
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				 
				 fixed4 col = tex3D(_VolumeGITex, i.worldPos/10);
				
				return   (max(0,i.normal.z*col.r)+max(0,i.normal.z*col.b)+max(0,-i.normal.x*col.g)+max(0,-i.normal.x*col.a)) *_SHLightingScale;
			}
		ENDCG
	}
}
}