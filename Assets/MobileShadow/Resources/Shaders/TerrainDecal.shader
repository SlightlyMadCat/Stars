Shader "MobileShadow/TerrainDecal" {
	Properties {
		_MainTex ("Sand", 2D) = "white" {}
		_Mask("Mask Noise", 2D) = "gray" {}
	}
	SubShader {
	Tags {
            "RenderType"="Transparent"
            "Queue" = "Geometry+100"
        }
	Blend SrcAlpha OneMinusSrcAlpha
	ZWrite Off
	Offset -1, -1

		Pass {
			Tags { 
				"LightMode" = "Vertex" 
			 } 
			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag
			
			#pragma target 2.0
 			#pragma multi_compile __ FOG_LINEAR
			#pragma multi_compile __ SHADOWTINT

			#include "UnityCG.cginc"
 
			sampler2D _MainTex; fixed4 _MainTex_ST;
			sampler2D _Mask; fixed4 _Mask_ST;
			uniform sampler2D _MobileShadowTexture;
            uniform float4x4 _MobileShadowMatrix;
            uniform fixed _MobileShadowOpacity;
			fixed4 _MobileShadowColor;

			struct v2f
			{
				half4 pos : SV_POSITION;
				fixed2 uv0 : TEXCOORD0;
				fixed3 uvShadow : TEXCOORD1;
				fixed2 uvMask : TEXCOORD2;
				fixed vMask : TEXCOORD3;
				fixed4 vertexColor : COLOR;
			};

			v2f vert (appdata_full v)
			{
		    	v2f o;
		    	o.pos = UnityObjectToClipPos(v.vertex);
				float2 worldPos = mul (unity_ObjectToWorld, v.vertex).xz;
				o.uv0 = TRANSFORM_TEX(worldPos, _MainTex);
				o.uvMask = TRANSFORM_TEX(worldPos, _Mask);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvShadow = mul(_MobileShadowMatrix, mul(UNITY_MATRIX_M, v.vertex));

				o.vMask = v.color;
				o.vertexColor = 0;
                half3 viewpos = mul (UNITY_MATRIX_MV, v.vertex).xyz;
				half3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal));
				for (int i = 0; i < 4; i++)
				{
					half3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
					half lengthSq = dot(toLight, toLight);

					lengthSq = max(lengthSq, 0.000001);
					toLight *= rsqrt(lengthSq);

					half atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z );

					fixed vertexColor = max (0, dot (viewN, toLight));
					o.vertexColor += unity_LightColor[i] * (vertexColor * atten);
				}
                o.uvShadow.z = min(0.5, o.vertexColor) * 2.0f;
				o.vertexColor += unity_AmbientSky;
				#if defined(FOG_LINEAR)
					UNITY_CALC_FOG_FACTOR(o.pos.z);
					o.vertexColor.a = unityFogFactor;
                #endif
                return o;
			}
 
			fixed4 frag (v2f i) : COLOR
			{
				
                fixed4 shadowMap = tex2D(_MobileShadowTexture, i.uvShadow.xy);
                fixed4 shadow = 1 - saturate(shadowMap.r + shadowMap.g + shadowMap.b) * _MobileShadowOpacity * i.uvShadow.z;

				fixed4 mask = tex2D(_Mask, i.uvMask);
				fixed3 finalColor = tex2D(_MainTex, i.uv0);
				
				finalColor *= i.vertexColor.rgb;

				#ifdef SHADOWTINT
					finalColor *= lerp(_MobileShadowColor, 1, shadow);
				#else
					finalColor *= shadow;
				#endif

				fixed a = min(smoothstep(mask.b-0.3, mask.b + 0.3, 1-i.vMask), 1-i.vMask);

				#if defined(FOG_LINEAR)
                  	finalColor = lerp(unity_FogColor, finalColor, saturate(i.vertexColor.a));
				#endif

				return fixed4(finalColor, a);
			}
			ENDCG
		}
Pass {
			Tags { 
				"LightMode" = "VertexLM" 
			 } 
			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag
			
			#pragma target 2.0
 			#pragma multi_compile __ FOG_LINEAR
			#pragma multi_compile __ SHADOWTINT

			#include "UnityCG.cginc"
 
			sampler2D _MainTex; fixed4 _MainTex_ST;
			sampler2D _Mask; fixed4 _Mask_ST;
			uniform sampler2D _MobileShadowTexture;
            uniform float4x4 _MobileShadowMatrix;
            uniform fixed _MobileShadowOpacity;
			fixed4 _MobileShadowColor;

			struct v2f
			{
				half4 pos : SV_POSITION;
				fixed2 uv0 : TEXCOORD0;
				fixed3 uvShadow : TEXCOORD1;
				fixed2 uvMask : TEXCOORD2;
				fixed vMask : TEXCOORD3;
				fixed4 vertexColor : COLOR;
				fixed2 lmap : TEXCOORD4;
			};

			v2f vert (appdata_full v)
			{
		    	v2f o;
		    	o.pos = UnityObjectToClipPos(v.vertex);
				float2 worldPos = mul (unity_ObjectToWorld, v.vertex).xz;
				o.uv0 = TRANSFORM_TEX(worldPos, _MainTex);
				o.uvMask = TRANSFORM_TEX(worldPos, _Mask);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvShadow = mul(_MobileShadowMatrix, mul(UNITY_MATRIX_M, v.vertex));

				o.vMask = v.color;
				o.vertexColor = 0;
                half3 viewpos = mul (UNITY_MATRIX_MV, v.vertex).xyz;
				half3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal));
				for (int i = 0; i < 4; i++)
				{
					half3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
					half lengthSq = dot(toLight, toLight);

					lengthSq = max(lengthSq, 0.000001);
					toLight *= rsqrt(lengthSq);

					half atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z );

					fixed vertexColor = max (0, dot (viewN, toLight));
					o.vertexColor += unity_LightColor[i] * (vertexColor * atten);
				}
                o.uvShadow.z = min(0.5, o.vertexColor) * 2.0f;
				o.vertexColor += unity_AmbientSky;

				o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

				#if defined(FOG_LINEAR)
					UNITY_CALC_FOG_FACTOR(o.pos.z);
					o.vertexColor.a = unityFogFactor;
                #endif
                return o;
			}
 
			fixed4 frag (v2f i) : COLOR
			{
				
                fixed4 shadowMap = tex2D(_MobileShadowTexture, i.uvShadow.xy);
                fixed4 shadow = 1 - saturate(shadowMap.r + shadowMap.g + shadowMap.b) * _MobileShadowOpacity * i.uvShadow.z;

				fixed4 mask = tex2D(_Mask, i.uvMask);
				fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				fixed3 finalColor = tex2D(_MainTex, i.uv0) * lm;
				
				finalColor *= i.vertexColor.rgb;

				#ifdef SHADOWTINT
					finalColor *= lerp(_MobileShadowColor, 1, shadow);
				#else
					finalColor *= shadow;
				#endif

				fixed a = min(smoothstep(mask.b-0.3, mask.b + 0.3, 1-i.vMask), 1-i.vMask);

				#if defined(FOG_LINEAR)
                  	finalColor = lerp(unity_FogColor, finalColor, saturate(i.vertexColor.a));
				#endif

				return fixed4(finalColor, a);
			}
			ENDCG
		}
Pass {
			Tags { 
				"LightMode" = "VertexLMRGBM" 
			 } 
			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag
			
			#pragma target 2.0
 			#pragma multi_compile __ FOG_LINEAR
			#pragma multi_compile __ SHADOWTINT

			#include "UnityCG.cginc"
 
			sampler2D _MainTex; fixed4 _MainTex_ST;
			sampler2D _Mask; fixed4 _Mask_ST;
			uniform sampler2D _MobileShadowTexture;
            uniform float4x4 _MobileShadowMatrix;
            uniform fixed _MobileShadowOpacity;
			fixed4 _MobileShadowColor;

			struct v2f
			{
				half4 pos : SV_POSITION;
				fixed2 uv0 : TEXCOORD0;
				fixed3 uvShadow : TEXCOORD1;
				fixed2 uvMask : TEXCOORD2;
				fixed vMask : TEXCOORD3;
				fixed4 vertexColor : COLOR;
				fixed2 lmap : TEXCOORD4;
			};

			v2f vert (appdata_full v)
			{
		    	v2f o;
		    	o.pos = UnityObjectToClipPos(v.vertex);
				float2 worldPos = mul (unity_ObjectToWorld, v.vertex).xz;
				o.uv0 = TRANSFORM_TEX(worldPos, _MainTex);
				o.uvMask = TRANSFORM_TEX(worldPos, _Mask);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvShadow = mul(_MobileShadowMatrix, mul(UNITY_MATRIX_M, v.vertex));

				o.vMask = v.color;
				o.vertexColor = 0;
                half3 viewpos = mul (UNITY_MATRIX_MV, v.vertex).xyz;
				half3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal));
				for (int i = 0; i < 4; i++)
				{
					half3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
					half lengthSq = dot(toLight, toLight);

					lengthSq = max(lengthSq, 0.000001);
					toLight *= rsqrt(lengthSq);

					half atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z );

					fixed vertexColor = max (0, dot (viewN, toLight));
					o.vertexColor += unity_LightColor[i] * (vertexColor * atten);
				}
                o.uvShadow.z = min(0.5, o.vertexColor) * 2.0f;
				o.vertexColor += unity_AmbientSky;

				o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

				#if defined(FOG_LINEAR)
					UNITY_CALC_FOG_FACTOR(o.pos.z);
					o.vertexColor.a = unityFogFactor;
                #endif
                return o;
			}
 
			fixed4 frag (v2f i) : COLOR
			{
				
                fixed4 shadowMap = tex2D(_MobileShadowTexture, i.uvShadow.xy);
                fixed4 shadow = 1 - saturate(shadowMap.r + shadowMap.g + shadowMap.b) * _MobileShadowOpacity * i.uvShadow.z;

				fixed4 mask = tex2D(_Mask, i.uvMask);

				fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				fixed3 finalColor = tex2D(_MainTex, i.uv0) * lm;
				
				finalColor *= i.vertexColor.rgb;

				#ifdef SHADOWTINT
					finalColor *= lerp(_MobileShadowColor, 1, shadow);
				#else
					finalColor *= shadow;
				#endif

				fixed a = min(smoothstep(mask.b-0.3, mask.b + 0.3, 1-i.vMask), 1-i.vMask);

				#if defined(FOG_LINEAR)
                  	finalColor = lerp(unity_FogColor, finalColor, saturate(i.vertexColor.a));
				#endif

				return fixed4(finalColor, a);
			}
			ENDCG
		}

	}
}